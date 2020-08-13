using System;
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

        public SirDailyRecord(DateTime date)
        {
            Date = date;
        }

        public DateTime Date { get; set; }

        public void InitializeWithStates(MultiStateModel<SirAgent> multiStateModel)
        {
            // Do nothing : states are hardcoded
        }

        public void RecordTransition(SirAgent agent, Transition<SirAgent> transition)
        {
            if (transition.Description == "S_to_E") TransitSToE++;
            if (transition.Description == "E_to_I") TransitEToI++;
            if (transition.Description == "I_to_R") TransitIToR++;
        }

        public void RecordState(SirAgent agent)
        {
            if (agent.CurrentState.Name == "S") S++;
            if (agent.CurrentState.Name == "E") E++;
            if (agent.CurrentState.Name == "I") I++;
            if (agent.CurrentState.Name == "R") R++;
        }

        public string CsvHeading => "S,E,I,R,N,S_to_E,E_to_I,I_to_R";

        public int N => S + E + I + R;

        public string CsvString(int iterationNumber, int day)
        {
            return $"{iterationNumber},{day},{Date},{S},{E},{I},{R},{N},{TransitSToE},{TransitEToI},{TransitIToR}";
        }
    }
}