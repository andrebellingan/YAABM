using System;
using System.Collections.Generic;
using Serilog;
using Yaabm.generic.Random;

namespace Yaabm.generic
{
    public abstract class Simulation<TAgent, TMultiStateModel, TLocalArea, TPopulationDynamics, TSimulation>
        where TAgent : Agent<TAgent>
        where TMultiStateModel : MultiStateModel<TAgent>
        where TLocalArea : LocalArea<TAgent>
        where TPopulationDynamics : PopulationDynamics<TAgent>
        where TSimulation : Simulation<TAgent, TMultiStateModel, TLocalArea, TPopulationDynamics, TSimulation>
    {
        public delegate void DayEventDelegate(int day, DateTime date);

        private readonly Dictionary<int, List<IIntervention>> _interventions =
            new Dictionary<int, List<IIntervention>>();

        private bool _hasBeenRun;
        private readonly List<Tuple<TAgent, Transition<TAgent>>> _transitionsToApply = new List<Tuple<TAgent, Transition<TAgent>>>();

        private readonly IDictionary<string, TLocalArea> _localContexts = new Dictionary<string, TLocalArea>();
        private readonly IDictionary<string, GeographicArea<TAgent>> _allContexts = new Dictionary<string, GeographicArea<TAgent>>();

        public IEnumerable<TLocalArea> LocalAreas => _localContexts.Values;

        // this should be set to true in cases where the transitions are based on probabilities.
        // In models where the time the agent will leave a state is determined when the state is entered it can be set to false (but this should be decided on a case-by-case basis
        // Obviously the shuffling process adds to the CPU overhead
        private readonly bool _shuffleTransitions;

        /// <summary>
        /// Construct a new instance of the Simulation class
        /// </summary>
        /// <param name="startDate">The starting date of the simulation</param>
        /// <param name="iterationNo">The iteration of this simulation</param>
        /// <param name="seed">the seed value for the random generator</param>
        /// <param name="shuffleTransitions">Sets whether the order in which state transitions are tested is shuffled randomly</param>
        protected Simulation(DateTime startDate, int iterationNo, int seed, bool shuffleTransitions)
        {
            IterationNo = iterationNo;
            StartDate = startDate;
            RandomProvider = new DefaultRandom(seed);
            _shuffleTransitions = shuffleTransitions;

            RootContext = new Region<TAgent>("root", "root", "Global"); // This is fine because it is only a grouped context - not an instance of TG
            _allContexts.Add(RootContext.Name, RootContext);
        }

        protected Region<TAgent> AddRegion(RegionSpec regionSpec)
        {
            return AddRegion(regionSpec.ParentRegionName, regionSpec.Name, regionSpec.FullName, regionSpec.RegionType);
        }

        protected Region<TAgent> AddRegion(string parentRegionName, string name, string fullname, string regionType)
        {
            if (string.IsNullOrEmpty(parentRegionName)) throw new ArgumentException("Cannot add region without parent region's name. Did you mean to use \"root\"?");
            if (!_allContexts.ContainsKey(parentRegionName)) throw new ArgumentOutOfRangeException($"The given parent region {parentRegionName} has not been defined");
            if (!(_allContexts[parentRegionName] is Region<TAgent> parentRegion)) throw new ArgumentNullException($"Cannot add context as child to {parentRegionName}");

            var newRegion = new Region<TAgent>(name, fullname, regionType);
            parentRegion.AddChild(newRegion);
            _allContexts.Add(newRegion.Name, newRegion);
            return newRegion;
        }

        protected void AddLocalArea(string parentContextName, TLocalArea newContext)
        {
            if (!(_allContexts[parentContextName] is Region<TAgent> parentContext)) throw new ArgumentNullException($"Cannot add context as child to {parentContextName}");

            parentContext.AddChild(newContext);
            _allContexts.Add(newContext.Name, newContext);
            _localContexts.Add(newContext.Name, newContext);
        }

        public TMultiStateModel MultiStateModel { get; protected set; }

        public IRandomProvider RandomProvider { get; }

        public TPopulationDynamics PopulationDynamics { get; protected set; }

        public int IterationNo { get; }

        public Region<TAgent> RootContext { get; }

        public int Day { get; private set; }

        public DateTime StartDate { get; }

        public DateTime Date => StartDate.AddDays(Day);

        public DayEventDelegate OnDayCompleted { get; set; }

        private void Iterate(int numberOfDays)
        {
#if !DEBUG
            try
            {
#endif
                for (var d = 0; d < numberOfDays; d++)
                { 
                    Log.Verbose($"Simulation {IterationNo} iterating day {d} days");
                    IterateOneDay();
                }
#if !DEBUG
        }
            catch (NotImplementedException ex)
            {
                Log.Error(ex, "Part of the simulation is not implemented!", ex.TargetSite);
                throw;
            }
            catch (Exception x)
            {
                Log.Error(x, $"Something went wrong with simulation {IterationNo}\n{x.Message}");
                throw;
            }
#endif
        }

        public SimulationResults<TAgent, TMultiStateModel> SimulationResults { get; private set; }

        private void IterateOneDay()
        {
            var populationToSimulate = PopulationDynamics.EnumeratePopulation();

            ApplyInterventions();
            TakeCensus(populationToSimulate);

            UpdateContexts(RootContext);

            IterateAgentBehaviour(populationToSimulate);

            _transitionsToApply.Clear();
            SimulateInfections();
            IterateWithinHostMultiStateModel(populationToSimulate);
            foreach (var transit in _transitionsToApply)
                ApplyTransitionToPerson(transit.Item1, transit.Item2);

            DayCompleted();
            Day++;
        }

        private void UpdateContexts(GeographicArea<TAgent> context)
        {
            if (context is Region<TAgent> asGroup)
            {
                foreach (var child in asGroup.ChildContexts) UpdateContexts(child.Value);
                return;
            }

            var asLocal = (TLocalArea) context;
            asLocal.Day = Day;
            asLocal.Date = Date;
            UpdateLocalContext(asLocal);
        }

        protected virtual void UpdateLocalContext(TLocalArea asLocal)
        {
            // By default nothing is updated
        }

        private void IterateWithinHostMultiStateModel(IEnumerable<TAgent> populationToSimulate)
        {
            foreach (var agent in populationToSimulate)
                ModelMultiStateModelOnAgent(agent);
        }

        private void ModelMultiStateModelOnAgent(TAgent agent)
        {
            if (agent.TransitionReserved) return; // This agent already had it's one transition for the day
            var transition = MultiStateModel.DetermineWithinHostStateTransitions(agent, RandomProvider, _shuffleTransitions);

            if (transition == null) return;
            agent.TransitionReserved = true;

            _transitionsToApply.Add(new Tuple<TAgent, Transition<TAgent>>(agent, transition));
        }

        private void SimulateInfections()
        {
            foreach (var agent in PopulationDynamics.GetInfectiousAgents())
            {
                var encounters = PopulationDynamics.GetEncounters(agent, RandomProvider);

                foreach (var contact in encounters)
                {
                    if (contact.Agent == agent) continue; // can't infect yourself
                    if (contact.Agent.TransitionReserved) continue;
                    if (!contact.Agent.CurrentState.IsSusceptible) continue;

                    var transition = MultiStateModel.DetermineAgentInteractionTransition(agent, contact, RandomProvider);

                    if (transition == null) continue;

                    contact.Agent.TransitionReserved = true;

                    _transitionsToApply.Add(new Tuple<TAgent, Transition<TAgent>>(contact.Agent, transition));
                }
            }
        }

        private void ApplyTransitionToPerson(TAgent agent, Transition<TAgent> transition)
        {
            MoveAgentToState(agent, transition.Destination, RandomProvider);

            SimulationResults.RecordTransition(agent, transition, Day);
        }

        protected void MoveAgentToState(TAgent agent, ModelState<TAgent> destinationState, IRandomProvider random)
        {
            destinationState.StateEntered(agent, random);
            agent.SetCurrentState(destinationState, Day);
        }

        private static void IterateAgentBehaviour(IEnumerable<TAgent> population)
        {
            foreach (var agent in population) agent.Behave();
        }

        private void TakeCensus(IEnumerable<TAgent> population)
        {
            foreach (var agent in population)
                CapturePersonState(agent);
        }

        private void CapturePersonState(TAgent agent)
        {
            SimulationResults.RecordState(agent, Day);
        }

        private void ApplyInterventions()
        {
            if (!_interventions.ContainsKey(Day)) return;

            var dailyInterventions = _interventions[Day];
            foreach (var intervention in dailyInterventions)
            {
                intervention.Apply(this);
                Log.Verbose($"Simulation {IterationNo}: Applied intervention on day {Day}: {intervention.Description}");
            }
        }

        private void DayCompleted()
        {
            OnDayCompleted?.Invoke(Day, Date);
        }

        public bool Run(int numberOfDays)
        {
            Log.Verbose($"Simulation {IterationNo} preparing environment");
            PrepareSimulation(numberOfDays);

            if (_hasBeenRun)
            {
                throw new InvalidOperationException("The simulation has already been run");
            }
            SimulationResults = new SimulationResults<TAgent, TMultiStateModel>(MultiStateModel, numberOfDays, StartDate, IterationNo, GenerateDailyRecordInstance);

            Iterate(numberOfDays);
            _hasBeenRun = true;

            Log.Verbose($"Simulation {IterationNo} finished running");

            return _hasBeenRun;
        }

        protected abstract void PrepareSimulation(in int numberOfDays);

        protected abstract IDailyRecord<TAgent> GenerateDailyRecordInstance(int day, DateTime date);

        public void AddIntervention(IIntervention intervention)
        {
            var dayOfIntervention = intervention.DayOfIntervention;
            if (!_interventions.ContainsKey(dayOfIntervention))
                _interventions.Add(dayOfIntervention, new List<IIntervention>());

            _interventions[intervention.DayOfIntervention].Add(intervention);
        }

        internal void AddInterventions(IEnumerable<IIntervention> interventions)
        {
            foreach (var intervention in interventions)
            {
                AddIntervention(intervention);
            }
        }

        public TAgent AddAgent(ModelState<TAgent> initialState, TLocalArea context)
        {
            var newAgent = PopulationDynamics.CreateAgent();
            newAgent.HomeArea = context;
            MoveAgentToState(newAgent, initialState, RandomProvider);
            return newAgent;
        }

        public TLocalArea GetContextByName(string contextName)
        {
            return _localContexts[contextName];
        }
    }
}