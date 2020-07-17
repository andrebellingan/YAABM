namespace Yaabm.generic
{
    public interface IDailyRecord<TAgent> where TAgent : Agent<TAgent>
    {
        void InitializeWithStates(MultiStateModel<TAgent> multiStateModel);

        void RecordTransition(TAgent agent, Transition<TAgent> transition);

        void RecordState(TAgent agent);

        string CsvHeading { get; }

        string CsvString(int iterationNo, int day);
    }
}