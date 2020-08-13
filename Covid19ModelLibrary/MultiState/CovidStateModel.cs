using System;
using Covid19ModelLibrary.Scenarios;
using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    public class CovidStateModel : MultiStateModel<Human>
    {
        public ModelState<Human> S { get; private set; }
        public ModelState<Human> E { get; private set; }
        public ModelState<Human> A { get; private set; }
        public ModelState<Human> Ar { get; private set; }
        public ModelState<Human> M { get; private set; }
        public ModelState<Human> Mr { get; private set; }
        public ModelState<Human> Sv { get; private set; }
        public ModelState<Human> Ih { get; private set; }
        public ModelState<Human> Ic { get; private set; }
        public ModelState<Human> Hr { get; private set; }
        public ModelState<Human> Hc { get; private set; }
        public ModelState<Human> Hd { get; private set; }
        public ModelState<Human> Cr { get; private set; }
        public ModelState<Human> Cd { get; private set; }
        public ModelState<Human> Rh { get; private set; }
        public ModelState<Human> Rc { get; private set; }
        public ModelState<Human> Dh { get; private set; }
        public ModelState<Human> Dc { get; private set; }

        public override ModelState<Human> DefaultState => S;

        public CovidStateModel(DiseaseParameters diseaseParameters)
        {
            Parameters = diseaseParameters;
            //TODO: process parameters?

            SetupStates();

            SetInfectionTransition(new Infection(this));

            // Within host transitions
            CreateConditionalTransition(E, A, (agent) => agent.Symptoms == Symptoms.Asymptomatic && agent.NumberOfDaysInCurrentState >= agent.IncubationPeriod);
        }

        public DiseaseParameters Parameters { get; }

        private void SetupStates()
        {
            S = CreateModelState("S", false, true);
            E = CreateModelState("E", false, false, PersonExposed);
            // asymptomatic
            A = CreateModelState("A", true, false, IncubationEnds);
            Ar = CreateModelState("Ar", false, false);
            // Mild
            M = CreateModelState("M", true, false, IncubationEnds);
            Mr = CreateModelState("Mr", false, false);
            // Severe
            Sv = CreateModelState("Sv", true, false, IncubationEnds);
            // Isolated
            Ih = CreateModelState("Ih", false, false, PersonIsolated); // no longer spreading (page 7 of August ASSA report)
            Ic = CreateModelState("Ic", false, false, PersonIsolated);
            // Hospitalized
            Hr = CreateModelState("Hr", true, false, PersonHospitalized);
            Hc = CreateModelState("Hc", true, false, PersonHospitalized);
            Hd = CreateModelState("Hd", true, false, PersonHospitalized);
            // In ICU
            Cr = CreateModelState("Cr", true, false, PersonCritical);
            Cd = CreateModelState("Cd", true, false, PersonCritical);
            // Recovered
            Rh = CreateModelState("Rh", false, false);
            Rc = CreateModelState("Rc", false, false);
            // Dead
            Dh = CreateModelState("Dh", false, false);
            Dc = CreateModelState("Dc", false, false);
        }

        private void PersonCritical(Human agent, IRandomProvider random)
        {
            throw new NotImplementedException();
        }

        private void PersonHospitalized(Human agent, IRandomProvider random)
        {
            throw new NotImplementedException();
        }

        private void PersonIsolated(Human agent, IRandomProvider random)
        {
            throw new NotImplementedException();
        }

        private void IncubationEnds(Human agent, IRandomProvider random)
        {
            throw new NotImplementedException();
        }

        private void PersonExposed(Human agent, IRandomProvider random)
        {
            throw new NotImplementedException();
        }
    }
}
