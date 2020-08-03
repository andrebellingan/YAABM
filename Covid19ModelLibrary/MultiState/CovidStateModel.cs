using System;
using Yaabm.generic;

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

            SetInfectionTransition(new CovidInfection(this));
            
            AddTransition(new TrEndIncubationPeriod(this));
            AddTransition(new TrRecover(this));
        }

        private void WhenPersonRecovers(Human agent, IRandomProvider random)
        {
            agent.IsInfectious = false;
            agent.Ward.HospitalSystem.DischargePatient(agent);
        }

        private void WhenIncubationEnds(Human agent, IRandomProvider random)
        {
            throw new NotImplementedException(nameof(WhenIncubationEnds));
        }

        private void WhenPersonExposed(Human agent, IRandomProvider random)
        {
            throw new NotImplementedException(nameof(WhenPersonExposed));
        }

        public ModelState<Human> S { get; }
        public ModelState<Human> E { get; }
        public ModelState<Human> I { get; }
        public ModelState<Human> R { get; }
        public override ModelState<Human> DefaultState => S;
    }
}
