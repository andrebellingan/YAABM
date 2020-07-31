using System;
using Yaabm.generic;
using Yaabm.generic.Random;

namespace Covid19ModelLibrary.MultiState
{
    public class CovidStateModel : MultiStateModel<Human>
    {
        public CovidStateModel()
        {
            S = CreateModelState("S");
            E = CreateModelState("E", WhenPersonExposed);
            I = CreateModelState("I", WhenIncubationEnds);
            R = CreateModelState("R", WhenPersonRecovers);
            Ih = CreateModelState("IH", WhenPersonAdmitted);
            Qih = CreateModelState("QIH", WhenPersonQueuesForHospital);
            Icu = CreateModelState("Icu", WhenPersonEntersIcu);
            QIcu = CreateModelState("QICU", WhenPersonQueuesForIcu);
            RIcu = CreateModelState("RICU", WhenPersonExitsIcu);
            D = CreateModelState("D", WhenPersonDies);

            SetInfectionTransition(new CovidInfection(this));
            
            AddTransition(new TrEndIncubationPeriod(this));
            AddTransition(new TrRecoverOutOfHospital(this));
            AddTransition(new TrAdmitToHospital(this));
            AddTransition(new TrQueueForHospital(this));
            AddTransition(new TrFinallyAdmittedToHospital(this));
            AddTransition(new TrDieWaitingForHospital(this));
            AddTransition(new TrAdmitToIcu(this));
            AddTransition(new TrRecoverInHospital(this));
            AddTransition(new TrDieInHospital(this));
            AddTransition(new TrMovedToIcu(this));
            AddTransition(new TrDieInIcu(this));
            AddTransition(new TrExitIcu(this));
            AddTransition(new TrDischargeAfterIcu(this));
        }

        private void WhenPersonDies(Human agent, IRandomProvider random)
        {
            agent.IsInfectious = false;
            agent.IsAlive = false;
            agent.Ward.HospitalSystem.DischargePatient(agent);
        }

        private void WhenPersonExitsIcu(Human agent, IRandomProvider random)
        {
            var avgDays = agent.Ward.GetMeanDaysRecoveringAfterIcu(agent);
            agent.DaysRecoveringAfterIcu = random.SamplePoisson(avgDays);
        }

        private void WhenPersonQueuesForIcu(Human agent, IRandomProvider random)
        {
            throw new NotImplementedException();
        }

        private void WhenPersonEntersIcu(Human agent, IRandomProvider random)
        {
            var icuOutcomeWeights = agent.Ward.GetIntensiveCareOutcomeWeights(agent);
            agent.IntensiveCareOutcome = RandomChooser<IntensiveCareOutcome>.RandomChoice(icuOutcomeWeights, random);

            var avgTimeInIcu = agent.Ward.GetAvgTimeInIcu(agent);
            agent.DaysInIcu = random.SamplePoisson(avgTimeInIcu);
        }

        private void WhenPersonQueuesForHospital(Human agent, IRandomProvider random)
        {
            var lambda = agent.Ward.MeanSurvivalWaitingForHospital;
            agent.DaysCanSurviveWithoutHospital = random.SamplePoisson(lambda);
        }

        private void WhenPersonAdmitted(Human agent, IRandomProvider random)
        {
            agent.Ward.HospitalSystem.AdmitPatient(agent);
            var outcomeWeights = agent.Ward.GetHospitalOutcomeWeights(agent);
            agent.HospitalOutcome = RandomChooser<HospitalOutcome>.RandomChoice(outcomeWeights, random);

            var averageDaysInHospital = agent.Ward.GetAverageHospitalStay(agent.HospitalOutcome);
            agent.DaysInHospital = random.SamplePoisson(averageDaysInHospital);
        }

        private void WhenPersonRecovers(Human agent, IRandomProvider random)
        {
            agent.IsInfectious = false;
            agent.Symptoms = DiseaseSymptoms.None;
            agent.Hospitalization = Hospitalization.None;
            agent.Ward.HospitalSystem.DischargePatient(agent);
        }

        private void WhenIncubationEnds(Human agent, IRandomProvider random)
        {
            agent.IsInfectious = true;
            var symptomProportions = agent.Ward.GetSymptomWeights(agent);
            agent.Symptoms = RandomChooser<DiseaseSymptoms>.RandomChoice(symptomProportions, random);
            var hospitalizationWeights = agent.Ward.GetHospitalizationWeights(agent);

            agent.Hospitalization = RandomChooser<Hospitalization>.RandomChoice(hospitalizationWeights, random);

            var averageDays = agent.Ward.GetMeanInfectedTime(agent);
            agent.DaysInInfectedState = random.SamplePoisson(averageDays);
        }

        private void WhenPersonExposed(Human agent, IRandomProvider random)
        {
            agent.IncubationDays = random.SamplePoisson(agent.Ward.MeanIncubationPeriod);
            agent.Symptoms = DiseaseSymptoms.Incubating;
        }

        public ModelState<Human> S { get; }
        public ModelState<Human> E { get; }
        public ModelState<Human> I { get; }
        public ModelState<Human> R { get; }
        public ModelState<Human> Ih { get; }
        public ModelState<Human> Qih { get; }
        public ModelState<Human> Icu { get; }
        public ModelState<Human> QIcu { get; }
        public ModelState<Human> RIcu { get; }
        public ModelState<Human> D { get; }
    }
}
