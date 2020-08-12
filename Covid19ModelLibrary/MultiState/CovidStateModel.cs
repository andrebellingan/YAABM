using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    public class CovidStateModel : MultiStateModel<Human>
    {
        public CovidStateModel()
        {
            S = CreateModelState("S", false, true);
            E = CreateModelState("E", false, false);
            I = CreateModelState("I", true, false);
            R = CreateModelState("R", false, false);

            SetInfectionTransition(new CovidInfection(this));
            
            AddTransition(new TrEndIncubationPeriod(this));
            AddTransition(new TrRecover(this));
        }

        public ModelState<Human> S { get; }
        public ModelState<Human> E { get; }
        public ModelState<Human> I { get; }
        public ModelState<Human> R { get; }
        public override ModelState<Human> DefaultState => S;
    }
}
