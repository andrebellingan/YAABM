using System;
using System.Collections.Generic;
using Serilog;
using Yaabm.generic.Random;

namespace Yaabm.generic
{
    public abstract class Simulation<TAgent, TMultiStateModel, TLocalArea, TPopulationDynamics, TSimulation>
        where TAgent : Agent<TAgent>
        where TMultiStateModel : MultiStateModel<TAgent>, new()
        where TLocalArea : LocalArea<TAgent>             //TODO: Make this a simple construction method as well and remove direct access to children
        where TPopulationDynamics : PopulationDynamics<TAgent>, new()
        where TSimulation : Simulation<TAgent, TMultiStateModel, TLocalArea, TPopulationDynamics, TSimulation>
    {
        public delegate void DayEventDelegate(int day, DateTime date);

        private readonly Dictionary<int, List<IIntervention>> _interventions =
            new Dictionary<int, List<IIntervention>>();

        private bool _hasBeenRun;
        private readonly List<Tuple<TAgent, Transition<TAgent>>> _transitionsToApply = new List<Tuple<TAgent, Transition<TAgent>>>();
        private readonly List<Tuple<TAgent, Transition<TAgent>>> _infectionsToApply = new List<Tuple<TAgent, Transition<TAgent>>>();
        private readonly Dictionary<int, TAgent> _infectedAgents = new Dictionary<int, TAgent>();

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
            MultiStateModel = new TMultiStateModel();
            PopulationDynamics = new TPopulationDynamics();
            PopulationDynamics.Initialize(MultiStateModel);
            StartDate = startDate;
            RandomProvider = new DefaultRandom(seed);
            _shuffleTransitions = shuffleTransitions; 

            RootContext = new Region<TAgent>("root", "root", "Global"); // This is fine because it is only a grouped context - not an instance of TG
            _allContexts.Add(RootContext.Name, RootContext);
        }

        protected void AddRegion(RegionSpec regionSpec)
        {
            AddRegion(regionSpec.ParentRegionName, regionSpec.Name, regionSpec.FullName, regionSpec.RegionType);
        }

        protected void AddRegion(string parentRegionName, string name, string fullname, string regionType)
        {
            if (string.IsNullOrEmpty(parentRegionName)) throw new ArgumentException("Cannot add region without parent region's name. Did you mean to use \"root\"?");
            if (!_allContexts.ContainsKey(parentRegionName)) throw new ArgumentOutOfRangeException($"The given parent region {parentRegionName} has not been defined");
            if (!(_allContexts[parentRegionName] is Region<TAgent> parentRegion)) throw new ArgumentNullException($"Cannot add context as child to {parentRegionName}");

            var newRegion = new Region<TAgent>(name, fullname, regionType);
            parentRegion.AddChild(newRegion);
            _allContexts.Add(newRegion.Name, newRegion);
        }

        protected void AddLocalArea(string parentContextName, TLocalArea newContext)
        {
            if (!(_allContexts[parentContextName] is Region<TAgent> parentContext)) throw new ArgumentNullException($"Cannot add context as child to {parentContextName}");

            parentContext.AddChild(newContext);
            _allContexts.Add(newContext.Name, newContext);
            _localContexts.Add(newContext.Name, newContext);
        }

        public TMultiStateModel MultiStateModel { get; }

        public IRandomProvider RandomProvider { get; }

        public TPopulationDynamics PopulationDynamics { get; }

        public int IterationNo { get; }

        public Region<TAgent> RootContext { get; }

        public int Day { get; private set; }

        public DateTime StartDate { get; }

        public DateTime Date => StartDate.AddDays(Day);

        public DayEventDelegate OnDayCompleted { get; set; }

        private void Iterate(int numberOfDays)
        {
            try
            {
                for (var d = 0; d < numberOfDays; d++)
                {
                    IterateOneDay();
                }
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
        }

        public SimulationResults<TAgent, TMultiStateModel> SimulationResults { get; private set; }

        private void IterateOneDay()
        {
            var populationToSimulate = PopulationDynamics.EnumeratePopulation(RandomProvider, false);

            ApplyInterventions();
            TakeCensus(populationToSimulate);

            UpdateContexts(RootContext);

            IterateAgentBehaviour(populationToSimulate);

            SimulateInfections();
            IterateWithinHostMultiStateModel(populationToSimulate);

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

        protected abstract void UpdateLocalContext(TLocalArea asLocal);

        private void IterateWithinHostMultiStateModel(TAgent[] populationToSimulate)
        {
            _transitionsToApply.Clear();

            // Do this single threaded
            foreach (var agent in populationToSimulate)
                ModelMultiStateModelOnAgent(agent);

            foreach (var transit in _transitionsToApply)
                ApplyTransitionToPerson(transit);
        }

        private void ModelMultiStateModelOnAgent(TAgent agent)
        {
            if (_infectedAgents.ContainsKey(agent.Id)) return; // This agent already had it's one transition for the day
            var transition = MultiStateModel.DetermineWithinHostStateTransitions(agent, RandomProvider, _shuffleTransitions);

            if (transition == null) return;

            _transitionsToApply.Add(new Tuple<TAgent, Transition<TAgent>>(agent, transition));
        }

        private void SimulateInfections()
        {
            _infectionsToApply.Clear();
            _infectedAgents.Clear();

            foreach (var agent in PopulationDynamics.GetInfectiousAgents())
            {
                var contacts = PopulationDynamics.GetContacts(agent, RandomProvider);

                foreach (var contact in contacts)
                {
                    if (contact == agent) continue; // can't infect yourself
                    if (!MultiStateModel.InfectionTransition.IsSusceptible(contact)) continue;

                    var transition = MultiStateModel.DetermineAgentInteractionTransition(agent, contact, RandomProvider);

                    if (transition == null) continue;

                    _infectionsToApply.Add(new Tuple<TAgent, Transition<TAgent>>(contact, transition));
                }
            }

            foreach (var transit in _infectionsToApply)
            {
                _infectedAgents.Add(transit.Item1.Id, transit.Item1);
                ApplyTransitionToPerson(transit);
            }
        }

        private void ApplyTransitionToPerson(Tuple<TAgent, Transition<TAgent>> transitionInfo)
        {
            var (agent, transition) = transitionInfo;
            MoveAgentToState(agent, transition.Destination, RandomProvider);

            SimulationResults.RecordTransition(agent, transition, Day);
        }

        private void MoveAgentToState(TAgent agent, ModelState<TAgent> destinationState, IRandomProvider random)
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
            foreach (var intervention in dailyInterventions) intervention.Apply(this);
        }

        private void DayCompleted()
        {
            OnDayCompleted?.Invoke(Day, Date);
        }

        public bool Run(int numberOfDays)
        {
            if (_hasBeenRun)
            {
                throw new InvalidOperationException("The simulation has already been run");
            }
            SimulationResults = new SimulationResults<TAgent, TMultiStateModel>(MultiStateModel, numberOfDays, StartDate, IterationNo, GenerateDailyRecordInstance);

            Iterate(numberOfDays);
            _hasBeenRun = true;

            return _hasBeenRun;
        }

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
            var newAgent = PopulationDynamics.CreateAgent(initialState, context.Day);
            newAgent.Context = context;
            return newAgent;
        }

        public TLocalArea GetContextByName(string contextName)
        {
            return _localContexts[contextName];
        }

        public void ApplyChangeToAllContexts(Action<TLocalArea> action)
        {
            foreach (var context in _localContexts)
            {
                action(context.Value);
            }
        }
    }
}