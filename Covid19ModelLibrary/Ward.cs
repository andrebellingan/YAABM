using System;
using System.Collections.Generic;
using System.Data;
using Covid19ModelLibrary.Geography;
using Covid19ModelLibrary.Initialization;
using Covid19ModelLibrary.Population;
using Covid19ModelLibrary.Scenarios;
using Serilog;
using Yaabm.generic;

namespace Covid19ModelLibrary
{
    public class Ward : LocalArea<Human>
    {
        public Ward(WardRecord ward, string areaType, CovidPopulation environment, CovidSimulation simulation) 
            : base(ward.WardId.ToString(), ward.WardId.ToString(), areaType, environment)
        {
            WardRecord = ward;
            WardId = ward.WardId;
            OwnerSimulation = simulation;
        }

        public CovidSimulation OwnerSimulation { get; }

        public int WardId { get; set; }

        public BasicHospitalSystem HospitalSystem => LocalResourceSystem as BasicHospitalSystem;

        public WardRecord WardRecord { get; }
        public DiseaseParameters DiseaseParameters => OwnerSimulation.DiseaseParameters;

        public void GeneratePopulation(in double scalingFactor, WardAgeDistribution ageDistribution, IRandomProvider random)
        {
            Residents.Clear();

            var agentsInThisWard = WardRecord.CorrectedPopulation(scalingFactor);
            var ages = ageDistribution.Sample(agentsInThisWard, random);

            for (var i = 0; i < agentsInThisWard; i++)
            {
                var newAgent = OwnerSimulation.AddAgent(OwnerSimulation.MultiStateModel.S, this);
                newAgent.AgeBand = ages[i];
                newAgent.HomeArea = this;
            }
        }

        public List<int> SampleHouseholdSizes(WardHouseSizeDistribution householdDistribution, IRandomProvider random)
        {
            var sizes = new List<int>();
            var cumulativeSizes = 0;
            while (cumulativeSizes < Residents.Count)
            {
                var maxRemainingSize = Math.Min(Residents.Count - cumulativeSizes, householdDistribution.MaximumSize);
                var randomSize = householdDistribution.Sample(maxRemainingSize, random.RandomSource);
                sizes.Add(randomSize);
                cumulativeSizes += randomSize;
            }

            return sizes;
        }

        public void GenerateHouseholds(WardHouseSizeDistribution householdDistribution, ContactMatrix homeContactMatrix, IRandomProvider random)
        {
            var houseSizes = SampleHouseholdSizes(householdDistribution, random);
            var houseHolds = AllocatePopulationToHouseholds(houseSizes, homeContactMatrix, random);

            var asCovidPop = (CovidPopulation) Environment;
            foreach (var house in houseHolds)
            {
                house.AddContacts(asCovidPop);
            }
        }

        private IEnumerable<HouseHold> AllocatePopulationToHouseholds(List<int> houseSizes, ContactMatrix homeContactMatrix, IRandomProvider random)
        {
            var households = new List<HouseHold>();

            var householdHeads = SelectHouseholdHeads(houseSizes.Count, random);
            var notHouseholdHeads = SelectNotHouseHoldHeads(householdHeads);

            for (var i = 0; i < householdHeads.Count; i++)
            {
                var headAgent = householdHeads[i];

                var newHousehold = new HouseHold(headAgent, i);
                households.Add(newHousehold);
                headAgent.HouseHold = newHousehold;

                var familySize = houseSizes[i];
                if (familySize > 1)
                {
                    // pick n other agents based on probability given their age
                    var p = homeContactMatrix.ContactWeightedAgentList(notHouseholdHeads, headAgent);

                    var familyMembers = CovidPopulation.SampleWeightedAgents(p, familySize - 1, random);
                    foreach (var family in familyMembers)
                    {
                        notHouseholdHeads.Remove(family);
                        if (newHousehold.Members.Contains(family)) throw new DuplicateNameException("Already part of family");
                        newHousehold.Members.Add(family);
                        family.HouseHold = newHousehold;
                    }

                    familyMembers.Add(headAgent);
                }
            }

            return households;
        }

        private HashSet<Human> SelectNotHouseHoldHeads(ICollection<Human> householdHeads)
        {
            var result = new HashSet<Human>();

            foreach (var agent in Residents)
            {
                if (householdHeads.Contains(agent)) continue;
                result.Add(agent);
            }

            return result;
        }

        private List<Human> SelectHouseholdHeads(in int numberOfHouseholds, IRandomProvider random)
        {
            var heads = random.RandomSelect(Residents.Count, numberOfHouseholds);
            var result = new List<Human>();
            foreach (var i in heads)
            {
                result.Add(Residents[i]);
            }

            return result;
        }

    }
}
