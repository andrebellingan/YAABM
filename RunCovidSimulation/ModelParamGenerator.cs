using System;
using Covid19ModelLibrary;
using Yaabm.generic;

namespace RunCovidSimulation
{
    /*
     *
     * This class is used to generate an initial copy of the scenario file in the correct format.
     *
     * WARNING: These parameter values are not calibrated and do not represent any official scenario
     *
     */


    internal static class ModelParamGenerator
    {
        internal static object GetModelParameters(int numberOfAgents, int noOfDays)
        {
            var modelParams = new CovidModelParameters
            {
                ScenarioName = "Baseline",
                NumberOfAgentsToSimulate = numberOfAgents,
                DaysToProject = noOfDays,
                StartDate = new DateTime(2020, 02, 06),
                PopulationFile = "./Data/ProvincePopulation.txt",
                BetaParam = 1.219512195,
                RelativeBetaAsymptomatic = 0.65,
                HospitalBedsPerThousand = 2.8,
                IncubationPeriod = 5.1,
                MeanSurvivalWaitingForHospital = 2.5,
                MeanDaysRecoveringAfterIcu = 0.000001, // ignoring this for now because we are using the other value for MeanTimeInIcuIfRecovers but it must be non-zero for Poisson
                SymptomWeights = GenerateSymptomWeights(),
                HospitalOutcomeWeights = GenerateHospitalOutcomeWeights(),
                SevereHospitalizationWeights = GenerateSeverHospitalizationWeights(),
                MeanInfectiousTimeAsymptomatic = 2.05,
                MeanInfectiousTimeMild = 2.05,
                MeanInfectiousTimeSevere = 6.0,
                IcuOutcomeWeights = GenerateIcuOutcomeWeights(),
                MeanTimeInIcuIfDies = 13.0,
                MeanTimeInIcuIfRecovers = 16.0,
                MeanTimeInHospitalIfIcu = 2.6,
                MeanTimeInHospitalIfRecover = 8.0,
                MeanTimeInHospitalIfDie = 8.0,
                SusceptibilityFactor = 1.0,
                Interventions = GenerateInterventions()
            };

            return modelParams;
        }

        private static InterventionList GenerateInterventions()
        {
            var interventions = new InterventionList()
            {
                new InterventionSpec()
                {
                    DayToApply = 30,
                    InterventionName = "RunCovidSimulation.ImportCaseToProvince, Covid19ModelLibrary",
                }
            };

            interventions[0].Parameters.Add(new InterventionParam()
            {
                Name="NumberOfCases",
                TypeName = "System.Int32",
                Value = "1"
            });

            return interventions;
        }

        private static AgeIcuOutcomeWeights GenerateIcuOutcomeWeights()
        {
            var icuOutcomeWeights = new AgeIcuOutcomeWeights()
            {
                {
                    AgeBand.Age00To04,
                    new IcuOutcomeWeights()
                    {
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Die, 0.0077),
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Recover, 0.9923)
                    }
                },
                {
                    AgeBand.Age05To09,
                    new IcuOutcomeWeights()
                    {
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Die, 0.0077),
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Recover, 0.9923)
                    }
                },
                {
                    AgeBand.Age10To14,
                    new IcuOutcomeWeights()
                    {
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Die, 0.0294),
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Recover, 0.9706)
                    }
                },
                {
                    AgeBand.Age15To19,
                    new IcuOutcomeWeights()
                    {
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Die, 0.0294),
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Recover, 0.9706)
                    }
                },
                {
                    AgeBand.Age20To24,
                    new IcuOutcomeWeights()
                    {
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Die, 0.287),
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Recover, 0.713)
                    }
                },
                {
                    AgeBand.Age25To29,
                    new IcuOutcomeWeights()
                    {
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Die, 0.287),
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Recover, 0.713)
                    }
                },
                {
                    AgeBand.Age30To34,
                    new IcuOutcomeWeights()
                    {
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Die, 0.378),
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Recover, 0.622)
                    }
                },
                {
                    AgeBand.Age35To39,
                    new IcuOutcomeWeights()
                    {
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Die, 0.378),
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Recover, 0.622)
                    }
                },
                {
                    AgeBand.Age40To44,
                    new IcuOutcomeWeights()
                    {
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Die, 0.413),
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Recover, 0.587)
                    }
                },
                {
                    AgeBand.Age45To49,
                    new IcuOutcomeWeights()
                    {
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Die, 0.413),
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Recover, 0.587)
                    }
                },
                {
                    AgeBand.Age50To54,
                    new IcuOutcomeWeights()
                    {
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Die, 0.455),
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Recover, 0.545)
                    }
                },
                {
                    AgeBand.Age55To59,
                    new IcuOutcomeWeights()
                    {
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Die, 0.455),
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Recover, 0.545)
                    }
                },
                {
                    AgeBand.Age60To64,
                    new IcuOutcomeWeights()
                    {
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Die, 0.462),
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Recover, 0.538)
                    }
                },
                {
                    AgeBand.Age65To69,
                    new IcuOutcomeWeights()
                    {
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Die, 0.462),
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Recover, 0.538)
                    }
                },
                {
                    AgeBand.Age70To74,
                    new IcuOutcomeWeights()
                    {
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Die, 0.469),
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Recover, 0.531)
                    }
                },
                {
                    AgeBand.Age75To79,
                    new IcuOutcomeWeights()
                    {
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Die, 0.469),
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Recover, 0.531)
                    }
                },
                {
                    AgeBand.Age80Plus,
                    new IcuOutcomeWeights()
                    {
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Die, 0.497),
                        new WeightedChoice<IntensiveCareOutcome>(IntensiveCareOutcome.Recover, 0.503)
                    }
                },
            };

            return icuOutcomeWeights;
        }

        private static SevereHospitalizationWeights GenerateSeverHospitalizationWeights()
        {
            return new SevereHospitalizationWeights()
            {
                new WeightedChoice<Hospitalization>(Hospitalization.Hospital, 0.8875),
                new WeightedChoice<Hospitalization>(Hospitalization.IntensiveCare, 1 - 0.8875)
            };
        }

        private static AgeHospitalOutcomeWeights GenerateHospitalOutcomeWeights()
        {
            var hOutcomeWeights = new AgeHospitalOutcomeWeights()
            {
                {
                    AgeBand.Age00To04,
                    new HospitalOutcomeWeights()
                    {
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Die, 0.0077),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.MoveToIntensiveCare, 0.144242424),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Recover, 0.848057576)
                    }
                },
                {
                    AgeBand.Age05To09,
                    new HospitalOutcomeWeights()
                    {
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Die, 0.0077),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.MoveToIntensiveCare, 0.144242424),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Recover, 0.848057576)
                    }
                },
                {
                    AgeBand.Age10To14,
                    new HospitalOutcomeWeights()
                    {
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Die, 0.0294),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.MoveToIntensiveCare, 0.144242424),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Recover, 0.826357576)
                    }
                },
                {
                    AgeBand.Age15To19,
                    new HospitalOutcomeWeights()
                    {
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Die, 0.0294),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.MoveToIntensiveCare, 0.144242424),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Recover, 0.826357576)
                    }
                },
                {
                    AgeBand.Age20To24,
                    new HospitalOutcomeWeights()
                    {
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Die, 0.0315),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.MoveToIntensiveCare, 0.144242424),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Recover, 0.824257576)
                    }
                },
                {
                    AgeBand.Age25To29,
                    new HospitalOutcomeWeights()
                    {
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Die, 0.0315),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.MoveToIntensiveCare, 0.144242424),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Recover, 0.824257576)
                    }
                },
                {
                    AgeBand.Age30To34,
                    new HospitalOutcomeWeights()
                    {
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Die, 0.0672),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.MoveToIntensiveCare, 0.144242424),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Recover, 0.788557576)
                    }
                },
                {
                    AgeBand.Age35To39,
                    new HospitalOutcomeWeights()
                    {
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Die, 0.0672),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.MoveToIntensiveCare, 0.144242424),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Recover, 0.788557576)
                    }
                },
                {
                    AgeBand.Age40To44,
                    new HospitalOutcomeWeights()
                    {
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Die, 0.0672),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.MoveToIntensiveCare, 0.144242424),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Recover, 0.788557576)
                    }
                },
                {
                    AgeBand.Age45To49,
                    new HospitalOutcomeWeights()
                    {
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Die, 0.0672),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.MoveToIntensiveCare, 0.144242424),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Recover, 0.788557576)
                    }
                },
                {
                    AgeBand.Age50To54,
                    new HospitalOutcomeWeights()
                    {
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Die, 0.1715),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.MoveToIntensiveCare, 0.144242424),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Recover, 0.684257576)
                    }
                },
                {
                    AgeBand.Age55To59,
                    new HospitalOutcomeWeights()
                    {
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Die, 0.1715),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.MoveToIntensiveCare, 0.144242424),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Recover, 0.684257576)
                    }
                },
                {
                    AgeBand.Age60To64,
                    new HospitalOutcomeWeights()
                    {
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Die, 0.2856),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.MoveToIntensiveCare, 0.144242424),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Recover, 0.570157576)
                    }
                },
                {
                    AgeBand.Age65To69,
                    new HospitalOutcomeWeights()
                    {
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Die, 0.2856),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.MoveToIntensiveCare, 0.144242424),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Recover, 0.570157576)
                    }
                },
                {
                    AgeBand.Age70To74,
                    new HospitalOutcomeWeights()
                    {
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Die, 0.3136),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.MoveToIntensiveCare, 0.144242424),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Recover, 0.542157576)
                    }
                },
                {
                    AgeBand.Age75To79,
                    new HospitalOutcomeWeights()
                    {
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Die, 0.3136),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.MoveToIntensiveCare, 0.144242424),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Recover, 0.542157576)
                    }
                },
                {
                    AgeBand.Age80Plus,
                    new HospitalOutcomeWeights()
                    {
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Die, 0.3682),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.MoveToIntensiveCare, 0.144242424),
                        new WeightedChoice<HospitalOutcome>(HospitalOutcome.Recover, 0.487557576)
                    }
                }
            };

            return hOutcomeWeights;
        }

        private static AgeSymptomWeights GenerateSymptomWeights()
        {
            var weights = new AgeSymptomWeights()
            {
                {
                    AgeBand.Age00To04,
                    new SymptomWeightsList()
                    {
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Asymptomatic, 0.5),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Mild, 0.4995),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Severe, 0.0005)
                    }
                },
                {
                    AgeBand.Age05To09,
                    new SymptomWeightsList()
                    {
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Asymptomatic, 0.5),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Mild, 0.4995),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Severe, 0.0005)
                    }
                },
                {
                    AgeBand.Age10To14,
                    new SymptomWeightsList()
                    {
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Asymptomatic, 0.5),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Mild, 0.4985),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Severe, 0.0015)
                    }
                },
                {
                    AgeBand.Age15To19,
                    new SymptomWeightsList()
                    {
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Asymptomatic, 0.5),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Mild, 0.4985),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Severe, 0.0015)
                    }
                },
                {
                    AgeBand.Age20To24,
                    new SymptomWeightsList()
                    {
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Asymptomatic, 0.5),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Mild, 0.494),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Severe, 0.006)
                    }
                },
                {
                    AgeBand.Age25To29,
                    new SymptomWeightsList()
                    {
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Asymptomatic, 0.5),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Mild, 0.494),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Severe, 0.006)
                    }
                },
                {
                    AgeBand.Age30To34,
                    new SymptomWeightsList()
                    {
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Asymptomatic, 0.5),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Mild, 0.484),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Severe, 0.016)
                    }
                },
                {
                    AgeBand.Age35To39,
                    new SymptomWeightsList()
                    {
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Asymptomatic, 0.5),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Mild, 0.484),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Severe, 0.016)
                    }
                },
                {
                    AgeBand.Age40To44,
                    new SymptomWeightsList()
                    {
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Asymptomatic, 0.5),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Mild, 0.4755),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Severe, 0.0245)
                    }
                },
                {
                    AgeBand.Age45To49,
                    new SymptomWeightsList()
                    {
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Asymptomatic, 0.5),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Mild, 0.4755),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Severe, 0.0245)
                    }
                },
                {
                    AgeBand.Age50To54,
                    new SymptomWeightsList()
                    {
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Asymptomatic, 0.5),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Mild, 0.449),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Severe, 0.051)
                    }
                },
                {
                    AgeBand.Age55To59,
                    new SymptomWeightsList()
                    {
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Asymptomatic, 0.5),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Mild, 0.449),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Severe, 0.051)
                    }
                },
                {
                    AgeBand.Age60To64,
                    new SymptomWeightsList()
                    {
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Asymptomatic, 0.5),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Mild, 0.417),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Severe, 0.083)
                    }
                },
                {
                    AgeBand.Age65To69,
                    new SymptomWeightsList()
                    {
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Asymptomatic, 0.5),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Mild, 0.417),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Severe, 0.083)
                    }
                },
                {
                    AgeBand.Age70To74,
                    new SymptomWeightsList()
                    {
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Asymptomatic, 0.5),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Mild, 0.3785),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Severe, 0.1215)
                    }
                },
                {
                    AgeBand.Age75To79,
                    new SymptomWeightsList()
                    {
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Asymptomatic, 0.5),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Mild, 0.3785),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Severe, 0.1215)
                    }
                },
                {
                    AgeBand.Age80Plus,
                    new SymptomWeightsList()
                    {
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Asymptomatic, 0.5),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Mild, 0.3635),
                        new WeightedChoice<DiseaseSymptoms>(DiseaseSymptoms.Severe, 0.1365)
                    }
                }
            };

            return weights;
        }
    }
}
