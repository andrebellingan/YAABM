﻿using System;
using System.Collections.Generic;
using System.Data;
using Covid19ModelLibrary.Geography;
using Covid19ModelLibrary.Initialization;
using Covid19ModelLibrary.Population;
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
            CovidEnvironment = environment;
        }

        public CovidPopulation CovidEnvironment { get; set; }

        public int LockDownLevel { get; set; } = -1;

        public CovidSimulation OwnerSimulation { get; }

        public int WardId { get; set; }

        public WardRecord WardRecord { get; }

        public void GeneratePopulation(in double scalingFactor, WardAgeDistribution ageDistribution, IRandomProvider random)
        {
            ResidentIds.Clear();

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
            while (cumulativeSizes < ResidentIds.Count)
            {
                var maxRemainingSize = Math.Min(ResidentIds.Count - cumulativeSizes, householdDistribution.MaximumSize);
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
                var headAgentId = householdHeads[i];
                var headAgent = Environment.AgentById(headAgentId);

                var newHousehold = new HouseHold(headAgentId, i);
                households.Add(newHousehold);
                headAgent.HouseHold = newHousehold;

                var familySize = houseSizes[i];
                if (familySize > 1)
                {
                    // pick n other agents based on probability given their age
                    var p = homeContactMatrix.ContactWeightedAgentList(notHouseholdHeads, CovidEnvironment, headAgent);

                    var familyMembers = CovidPopulation.SampleWeightedAgents(notHouseholdHeads, p, familySize - 1, random);
                    foreach (var family in familyMembers)
                    {
                        notHouseholdHeads.Remove(family);
                        if (newHousehold.Members.Contains(family)) throw new DuplicateNameException("Already part of family");
                        newHousehold.Members.Add(family);
                        var houseMate = CovidEnvironment.AgentById(family);
                        houseMate.HouseHold = newHousehold;
                    }

                    familyMembers.Add(headAgentId);
                }
            }

            return households;
        }

        private List<int> SelectNotHouseHoldHeads(ICollection<int> householdHeads)
        {
            var result = new List<int>();

            foreach (var agent in ResidentIds)
            {
                if (householdHeads.Contains(agent)) continue;
                result.Add(agent);
            }

            return result;
        }

        private List<int> SelectHouseholdHeads(in int numberOfHouseholds, IRandomProvider random)
        {
            var heads = random.RandomSelect(ResidentIds.Count, numberOfHouseholds);
            var result = new List<int>();
            foreach (var i in heads)
            {
                result.Add(ResidentIds[i]);
            }

            return result;
        }

        public GeographicArea<Human> AreaAtLevel(GeoLevel level)
        {
            if (level == GeoLevel.Ward) return this;
            var nextLevel = this.Parent;
            while (nextLevel.Parent != null)
            {
                var nextType = (GeoLevel) nextLevel.Tag;
                if (nextType == level) return nextLevel;
                nextLevel = nextLevel.Parent;
            }

            return null; // didn't find at the parent's level
        }

        public GeographicArea<Human> OutputRegion { get; private set; }

        public void SetOutputRegion(GeoLevel outputDetail)
        {
            OutputRegion = AreaAtLevel(outputDetail);
        }
    }
}
