using Yaabm.generic;

namespace TestSirModel.Model
{
    public class SirDailyRecord : IDailyRecord<SirAgent>
    {
        public int S { get; private set; }
        public int E { get; private set; }
        public int I { get; private set; }
        public int R { get; private set; }

        public int TransitSToE { get; private set; }
        public int TransitEToI { get; private set; }
        public int TransitIToR { get; private set; }

        public void InitializeWithStates(MultiStateModel<SirAgent> multiStateModel)
        {
            // Do nothing : states are hardcoded
        }

        public void RecordTransition(SirAgent agent, Transition<SirAgent> transition)
        {
            if (transition.Description == "S->E") TransitSToE++;
            if (transition.Description == "E->I") TransitEToI++;
            if (transition.Description == "I->R") TransitIToR++;
        }

        public void RecordState(SirAgent agent)
        {
            if (agent.CurrentState.Name == "S") S++;
            if (agent.CurrentState.Name == "E") E++;
            if (agent.CurrentState.Name == "I") I++;
            if (agent.CurrentState.Name == "R") R++;
        }

        public string CsvHeading => "S,E,I,R,N,S->E,E->I,I->R";

        public int N => S + E + I + R;

        public string CsvString(int iterationNumber, int day)
        {
            return $"{iterationNumber},{day},{S},{E},{I},{R},{N},{TransitSToE},{TransitEToI},{TransitIToR}";
        }
    }
}