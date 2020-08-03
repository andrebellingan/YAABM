using System;
using Covid19ModelLibrary.Initialization;
using Covid19ModelLibrary.Population;
using Yaabm.generic;

namespace Covid19ModelLibrary
{
    public class Ward : LocalArea<Human>
    {
        public Ward(int wardId, CovidInitializationInfo modelParams, CovidPopulation populationDynamics) 
            : base(wardId.ToString(), wardId.ToString(), "Province", populationDynamics)
        {
            throw new NotImplementedException();
        }

        public BasicHospitalSystem HospitalSystem => LocalResourceSystem as BasicHospitalSystem;
    }
}
