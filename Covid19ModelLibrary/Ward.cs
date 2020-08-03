using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Covid19ModelLibrary.Geography;
using Covid19ModelLibrary.Initialization;
using Covid19ModelLibrary.Population;
using Serilog;
using Yaabm.generic;
using Yaabm.generic.Random;

namespace Covid19ModelLibrary
{
    public class Ward : LocalArea<Human>
    {
        public Ward(WardRecord ward, string areaType, CovidPopulation populationDynamics) 
            : base(ward.WardId.ToString(), ward.WardId.ToString(), areaType, populationDynamics)
        {
            WardRecord = ward;
            WardId = ward.WardId;
        }

        public int WardId { get; set; }

        public BasicHospitalSystem HospitalSystem => LocalResourceSystem as BasicHospitalSystem;

        public WardRecord WardRecord { get; }

        public void GeneratePopulation(in double scalingFactor, WardAgeDistribution ageDistribution, IRandomProvider random)
        {
            Population.Clear();

            var agentsInThisWard = WardRecord.CorrectedPopulation(scalingFactor);
            var ages = ageDistribution.Sample(agentsInThisWard, random);

            for (var i = 0; i < agentsInThisWard; i++)
            {
                var newAgent = PopulationDynamics.CreateAgent(0);
                newAgent.AgeBand = ages[i];
                newAgent.Context = this;
                Population.Add(newAgent);
            }
        }

        public List<int> SampleHouseholdSizes(WardHouseSizeDistribution householdDistribution, IRandomProvider random)
        {
            Log.Verbose($"Generating households for ward {WardId} with population size {Population.Count}");

            var sizes = new List<int>();
            var cumulativeSizes = 0;
            while (cumulativeSizes < Population.Count)
            {
                var maxRemainingSize = Math.Min(Population.Count - cumulativeSizes, householdDistribution.MaximumSize);
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

            var asCovidPop = (CovidPopulation) PopulationDynamics;
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


                headAgent.HouseholdNumber = i;
                var newHousehold = new HouseHold(headAgent, headAgent.HouseholdNumber);
                households.Add(newHousehold);
                var familySize = houseSizes[i];
                if (familySize > 1)
                {
                    // pick n other agents based on probability given their age
                    var p = ContactWeightedNonHouseholdMembers(homeContactMatrix, notHouseholdHeads, familySize, headAgent);

                    var familyMembers = SelectFamilyMembers(p, familySize - 1, random.RandomSource);
                    SetHouseholdNumber(familyMembers, i);
                    foreach (var family in familyMembers)
                    {
                        notHouseholdHeads.Remove(family);
                        if (newHousehold.Members.Contains(family)) throw new DuplicateNameException("Already part of family");
                        newHousehold.Members.Add(family);
                    }

                    familyMembers.Add(headAgent);
                }
            }

            return households;
        }

        private List<Human> SelectFamilyMembers(List<Tuple<Human, double>> candidates, int familySize, Random random)
        {
            var weights = new List<WeightedItem<Human>>();

            foreach (var c in candidates)
            {
                weights.Add(new WeightedItem<Human>(c.Item1, c.Item2));
            }

            var selected = WeightedSampler<Human>.PickMultipleItems(weights, familySize, random);
            return selected;
        }

        private void SetHouseholdNumber(List<Human> familyMembers, int i)
        {
            foreach (var dude in familyMembers)
            {
                dude.HouseholdNumber = i;
            }
        }

        private Dictionary<Human, double> NormalizeP(Dictionary<Human, double> p)
        {
            var pTotal = p.Values.Sum();
            var result = new Dictionary<Human, double>();

            foreach (var pair in p)
            {
                result.Add(pair.Key, pair.Value / pTotal);
            }

            return result;
        }

        private static List<Tuple<Human, double>> ContactWeightedNonHouseholdMembers(ContactMatrix householdContactMatrix, HashSet<Human> notHouseholdHeads, int familySize, Human householdHead)
        {
            var weights = new List<Tuple<Human, double>>();

            var contracts = householdContactMatrix.Contacts(householdHead.AgeBand);

            var totalWeight = 0d;
            foreach (var other in notHouseholdHeads)
            {
                var weight = contracts[other.AgeBand];
                totalWeight += weight;
                weights.Add(new Tuple<Human, double>(other, weight));
            }

            var result = new List<Tuple<Human, double>>();

            foreach (var t in weights)
            {
                result.Add(new Tuple<Human, double>(t.Item1, t.Item2/totalWeight));
            }

            return result;
        }

        private HashSet<Human> SelectNotHouseHoldHeads(List<Human> householdHeads)
        {
            var result = new HashSet<Human>();

            foreach (var agent in Population)
            {
                if (householdHeads.Contains(agent)) continue;
                result.Add(agent);
            }

            return result;
        }

        private List<Human> SelectHouseholdHeads(in int numberOfHouseholds, IRandomProvider random)
        {
            var heads = random.RandomSelect(Population.Count, numberOfHouseholds);
            var result = new List<Human>();
            foreach (var i in heads)
            {
                result.Add(Population[i]);
            }

            return result;

        }
    }
}
