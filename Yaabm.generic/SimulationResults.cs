using System;
using System.Collections.Generic;
using System.Text;

namespace Yaabm.generic
{
    public class SimulationResults<TAgent, TModel> 
        where TAgent : Agent<TAgent>
        where TModel : MultiStateModel<TAgent>
    {
        private readonly Dictionary<int, IDailyRecord<TAgent>> _dailyRecords = new Dictionary<int, IDailyRecord<TAgent>>();

        protected TModel MultiStateModel { get; }

        private readonly Func<int, DateTime, IDailyRecord<TAgent>> _dailyRecordGeneratorFunc;

        public SimulationResults(TModel multiStateModel, int numberOfDays, DateTime startDate, int iterationNumber, Func<int, DateTime, IDailyRecord<TAgent> > dailyRecordGeneratorFunc)
        {
            MultiStateModel = multiStateModel;
            IterationNumber = iterationNumber;
            _dailyRecordGeneratorFunc = dailyRecordGeneratorFunc;

            _dailyRecords.Clear();
            for (var d = 0; d < numberOfDays; d++)
            {
                var newRecord = dailyRecordGeneratorFunc(d, startDate.AddDays(d));
                newRecord.InitializeWithStates(MultiStateModel);
                _dailyRecords.Add(d, newRecord);
            }
        }

        public int IterationNumber { get; }

        public void RecordTransition(TAgent agent, Transition<TAgent> transition, int day)
        {
            _dailyRecords[day].RecordTransition(agent, transition);
        }

        public void RecordState(TAgent agent, int day)
        {
            _dailyRecords[day].RecordState(agent);
        }

        public string CsvHeading()
        {
            var dummyRecord = _dailyRecordGeneratorFunc(0, DateTime.Now);
            dummyRecord.InitializeWithStates(MultiStateModel);
            return $"iteration,day,date,{dummyRecord.CsvHeading}";
        }

        public string CsvString()
        {
            var stringBuilder = new StringBuilder();

            foreach (var recordPair in _dailyRecords)
            {
                stringBuilder.Append($"{recordPair.Value.CsvString(IterationNumber, recordPair.Key)}\n");
            }

            return stringBuilder.ToString();
        }

        public IDailyRecord<TAgent> Result(int day)
        {
            return _dailyRecords[day];
        }
    }
}