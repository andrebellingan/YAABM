using System;
using System.Collections.Generic;
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
            //TODO: Initialize
        }

        public void RecordTransition(Human agent, Transition<Human> transition)
        {
            //TODO: Record transition
        }

        public void RecordState(Human agent)
        {
            //TODO: Record state
        }

        public string CsvHeading => "NOT IMPLEMENTED!!!";
   
        public string CsvString(int iterationNo, int day)
        {
            return "not implemented";
        }
    }
}