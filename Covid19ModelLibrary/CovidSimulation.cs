using System;
using Covid19ModelLibrary.MultiState;
using Covid19ModelLibrary.Population;
using Yaabm.generic;

namespace Covid19ModelLibrary
{
    public class CovidSimulation : Simulation<Human, CovidStateModel, CovidContext, CovidPopulation, CovidSimulation>
    {
        public CovidSimulation(int seed, int iterationNo, CovidModelParameters parameters) : base(parameters.StartDate, iterationNo, seed, false)
        {
            InitializePopulation(parameters);
        }

        private void InitializePopulation(CovidModelParameters parameters)
        {
            foreach (var province in (Province[]) Enum.GetValues(typeof(Province)))
            {
                var provinceHospitalSystem = new BasicHospitalSystem(parameters.HospitalBedsPerThousand);
                var provinceContext = new CovidContext(provinceHospitalSystem, province, RootContext, parameters.Clone());

                AddLocalContext("root", provinceContext);
            }

            var populationPyramid = PopulationPyramid.LoadFromFile(parameters.PopulationFile);

            populationPyramid.GeneratePopulation(parameters.NumberOfAgentsToSimulate, this);
            PopulationDynamics.GenerateContacts(parameters.ContactMatrix);
        }

        protected override void UpdateLocalContext(CovidContext asLocal)
        {
            /*
             * THIS CODE IS VERY, VERY WRONG
             *
             * For a start it can result in probabilities > 1, which is impossible
             * It is also causing the number of infections to spike hard.
             * I suspect this is mainly because the concept of a beta parameter doesn't translate to an agent based model
             */

            var currentDayResults = (CovidRecord) SimulationResults.Result(Day);

            var susceptibleTotal = currentDayResults.GetSusceptibleTotal(asLocal.Province);
            var infectiousTotal = currentDayResults.GetInfectiousTotal(asLocal.Province);

            var iAsymptomatic = PopulationDynamics.GetInfectiousBySymptoms(DiseaseSymptoms.Asymptomatic, asLocal.Province);
            var iMild = PopulationDynamics.GetInfectiousBySymptoms(DiseaseSymptoms.Mild, asLocal.Province);
            var iSevere = PopulationDynamics.GetInfectiousBySymptoms(DiseaseSymptoms.Severe, asLocal.Province);

            var infectiousStrength = asLocal.RelativeBetaAsymptomatic * iAsymptomatic + iMild + iSevere;

            var n = susceptibleTotal + infectiousTotal;
            asLocal.ProbabilityOfInfection = Math.Min(asLocal.BetaParam * asLocal.RelativeLockDownBeta * infectiousStrength / n, 1d); // probability cannot be greater than 1
        }

        protected override IDailyRecord<Human> GenerateDailyRecordInstance(int day, DateTime date)
        {
            return new CovidRecord(date);
        }
    }
}
