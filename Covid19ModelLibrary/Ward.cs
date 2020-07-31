using System;
using System.Collections.Generic;
using System.ComponentModel;
using Yaabm.generic;

namespace Covid19ModelLibrary
{
    public enum Province
    {
        EasternCape = 0,
        FreeState = 1,
        // ReSharper disable once IdentifierTypo
        Gauteng = 2,
        // ReSharper disable once IdentifierTypo
        KwazuluNatal = 3,
        Limpopo = 4,
        // ReSharper disable once IdentifierTypo
        Mpumalanga = 5,
        NorthWest = 6,
        NorthernCape = 7,
        WesternCape = 8
    }

    public class Ward : LocalArea<Human>
    {
        public Ward(int wardId, CovidModelParameters parameters) 
            : base(wardId.ToString(), wardId.ToString(), "Province")
        {
            ModelParameters = parameters;
        }

        public CovidModelParameters ModelParameters { get; }

        private readonly List<WeightedChoice<Hospitalization>> _noHospitalization
            = new List<WeightedChoice<Hospitalization>>()
            {
                new WeightedChoice<Hospitalization>(Hospitalization.None, 1.0) // Specifically nobody in Asymptomatic or Mild gets hospitalized
            };

        public Province Province { get; }

        public BasicHospitalSystem HospitalSystem => LocalResourceSystem as BasicHospitalSystem;

        public double MeanIncubationPeriod => ModelParameters.IncubationPeriod;

        public double ProbabilityOfInfection { get; set; }

        public double BetaParam => ModelParameters.BetaParam;

        public double RelativeBetaAsymptomatic => ModelParameters.RelativeBetaAsymptomatic;

        public double MeanSurvivalWaitingForHospital => ModelParameters.MeanSurvivalWaitingForHospital;

        public double RelativeLockDownBeta { get; set; } = 1.0d;

        public double GetMeanDaysRecoveringAfterIcu(Human agent)
        {
            return ModelParameters.MeanDaysRecoveringAfterIcu;
        }

        public List<WeightedChoice<IntensiveCareOutcome>> GetIntensiveCareOutcomeWeights(Human agent)
        {
            return ModelParameters.IcuOutcomeWeights[agent.AgeBand];
        }

        public double GetAvgTimeInIcu(Human agent)
        {
            switch (agent.IntensiveCareOutcome)
            {
                case IntensiveCareOutcome.Recover:
                    return ModelParameters.MeanTimeInIcuIfRecovers;
                case IntensiveCareOutcome.Die:
                    return ModelParameters.MeanTimeInIcuIfDies;
                default:
                    throw new InvalidOperationException($"Unhandled ICU outcome value = {agent.IntensiveCareOutcome}");
            }
        }

        public List<WeightedChoice<HospitalOutcome>> GetHospitalOutcomeWeights(Human agent)
        {
            return ModelParameters.HospitalOutcomeWeights[agent.AgeBand];
        }

        public double GetAverageHospitalStay(HospitalOutcome hospitalOutcome)
        {
            switch (hospitalOutcome)
            {
                case HospitalOutcome.Recover:
                    return ModelParameters.MeanTimeInHospitalIfRecover;
                case HospitalOutcome.Die:
                    return ModelParameters.MeanTimeInHospitalIfDie;
                case HospitalOutcome.MoveToIntensiveCare:
                    return ModelParameters.MeanTimeInHospitalIfIcu;
                default:
                    throw new InvalidEnumArgumentException(nameof(hospitalOutcome), (int)hospitalOutcome,
                        typeof(HospitalOutcome));
            }
        }

        public List<WeightedChoice<DiseaseSymptoms>> GetSymptomWeights(Human agent)
        {
            return ModelParameters.SymptomWeights[agent.AgeBand];
        }

        public List<WeightedChoice<Hospitalization>> GetHospitalizationWeights(Human agent)
        {
            if (agent.Symptoms == DiseaseSymptoms.Asymptomatic || agent.Symptoms == DiseaseSymptoms.Mild)
                return _noHospitalization;

            return ModelParameters.SevereHospitalizationWeights;
        }

        public double GetMeanInfectedTime(Human agent)
        {
            switch (agent.Symptoms)
            {
                case DiseaseSymptoms.Asymptomatic:
                    return ModelParameters.MeanInfectiousTimeAsymptomatic;
                case DiseaseSymptoms.Mild:
                    return ModelParameters.MeanInfectiousTimeMild;
                case DiseaseSymptoms.Severe:
                    return ModelParameters.MeanInfectiousTimeSevere;
                default:
                    throw new InvalidOperationException($"The symptoms should not be {agent.Symptoms} when person is infected");
            }
        }

        public double SusceptibilityFactor(Human susceptibleAgent)
        {
            return ModelParameters.SusceptibilityFactor;
        }
    }
}
