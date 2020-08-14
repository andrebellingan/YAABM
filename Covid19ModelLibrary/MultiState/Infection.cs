using System;
using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    public class Infection : InfectionTransition<Human>
    {
        private readonly Func<Human, double> _infectiousness;

        public Infection(CovidStateModel stateModel, Func<Human, double> infectiousness) : base("S_to_E", stateModel.S, stateModel.E)
        {
            _infectiousness = infectiousness;
        }

        public override bool InfectionOccurs(Human carrierAgent, Encounter<Human> encounter, IRandomProvider randomProvider)
        {
            var p = _infectiousness(carrierAgent);
            return randomProvider.Chance(p);
        }

    }
}
