using System;
using System.Collections.Generic;
using Serilog;
using Yaabm.generic;

namespace Covid19ModelLibrary
{
    public class CovidRecord : IDailyRecord<Human>
    {
        public CovidRecord(DateTime date)
        {
            Date = date;
        }

        public DateTime Date { get; set; }

        private readonly Dictionary<ModelState<Human>, int[]> _stateCounts = new Dictionary<ModelState<Human>, int[]>();

        private readonly Dictionary<Transition<Human>, int[]> _transitionCounts = new Dictionary<Transition<Human>, int[]>();

        public void InitializeWithStates(MultiStateModel<Human> multiStateModel)
        {
            Log.Error("CovidRecord. InitializeWithStates() not implemented");
        }

        public void RecordTransition(Human agent, Transition<Human> transition)
        {
            Log.Verbose("CovidRecord.RecordTransition() not implemented");
        }

        public void RecordState(Human agent)
        {
            Log.Verbose("CovidRecord.RecordState() not implemented");
        }

        public string CsvHeading => "NOT IMPLEMENTED!!!";
   
        public string CsvString(int iterationNo, int day)
        {
            Log.Verbose("CovidRecord.CsvString() not implemented");
            return "not implemented";
        }
    }
}