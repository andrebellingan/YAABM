﻿using System.Xml.Serialization;
using Yaabm.generic;

namespace Covid19ModelLibrary
{
    public enum AgeBand
    {
        Age00To09 = 0,
        Age10To19 = 1,
        Age20To29 = 2,
        Age30To39 = 3,
        Age40To49 = 4,
        Age50To59 = 5,
        Age60To69 = 6,
        Age70To79 = 7,
        Age80Plus = 8
    }


    public class Human : Agent<Human>
    {
        public Human(int id) : base(id) 
        {}

        public AgeBand AgeBand { get; set; } = AgeBand.Age00To09;

        [XmlAttribute(AttributeName = "AgeBand")]
        public string AgeBandAsString => AgeBand.ToString();

        [XmlAttribute] public int WardId => Ward.WardId;

        public bool IsInfectious { get; set; }

        public int? IncubationDays { get; set; }


        public Ward Ward { get; private set; }

        private LocalArea<Human> _localContext;

        public override LocalArea<Human> Context
        {
            get => _localContext;
            set
            {
                _localContext = value;
                Ward = value as Ward;
            }
        }

        public int HouseholdNumber { get; set; }
    }
}
 