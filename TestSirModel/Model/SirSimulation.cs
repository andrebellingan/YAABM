using System;
using Yaabm.generic;

namespace TestSirModel.Model
{
    public class SirSimulation : Simulation<SirAgent, SirStateModel, SirContext, SirEnvironment, SirSimulation>
    {
        private readonly int _popSizeS;
        private readonly int _popSizeE;
        private readonly int _popSizeI;
        private readonly int _popSizeR;

        private readonly double _beta;
        private readonly double _gamma;
        private readonly double _sigma;

        public SirSimulation(int seed, int popSizeS, int popSizeE, int popSizeI, int popSizeR, double beta, double gamma, double sigma, int iterationNo) : base( DateTime.Today, iterationNo, seed, true)
        {
            _popSizeS = popSizeS;
            _popSizeE = popSizeE;
            _popSizeI = popSizeI;
            _popSizeR = popSizeR;
            _beta = beta;
            _gamma = gamma;
            _sigma = sigma;
            MultiStateModel = new SirStateModel();
        }

        private void InitializeSirSimulation(SirStateModel multiStateModel)
        {
            // Setup the homogeneous population etc.
            var context = new SirContext("Universe", 
                PopulationDynamics)
            {
                LocalResourceSystem = new EmptyResourceSystem()
            };
            context.SetDiseaseParameters(_beta, _gamma, _sigma);
            AddLocalArea("root", context);

            GenerateStatePopulation(context, multiStateModel.S, _popSizeS);
            GenerateStatePopulation(context, multiStateModel.E, _popSizeE);
            GenerateStatePopulation(context, multiStateModel.I, _popSizeI);
            GenerateStatePopulation(context, multiStateModel.R, _popSizeR);
        }

        private void GenerateStatePopulation(SirContext context, ModelState<SirAgent> state, int numberOfLives)
        {
            for (var s = 0; s < numberOfLives; s++)
            {
                AddAgent(state, context);
            }
        }

        protected override void UpdateLocalContext(SirContext localContext)
        {
            var todayCensus = (SirDailyRecord) SimulationResults.Result(Day);

            localContext.SusceptibleTotal = todayCensus.S;
            localContext.ExposedTotal = todayCensus.E;
            localContext.InfectiousTotal = todayCensus.I;
            localContext.ResistantTotal = todayCensus.R;
            localContext.N = todayCensus.N;

            localContext.ProbabilityOfInfection = localContext.BetaParam * localContext.InfectiousTotal / localContext.N;
        }

        protected override void PrepareSimulation(in int numberOfDays)
        {
            InitializeSirSimulation(MultiStateModel);
        }

        protected override IDailyRecord<SirAgent> GenerateDailyRecordInstance(int day, DateTime date)
        {
            return new SirDailyRecord(Date);
        }
    }
}
