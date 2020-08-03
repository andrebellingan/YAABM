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
            throw new NotImplementedException();
        }

        public void RecordTransition(Human agent, Transition<Human> transition)
        {
            throw new NotImplementedException();
        }

        public void RecordState(Human agent)
        {
            throw new NotImplementedException();
        }

        public string CsvHeading => throw new NotImplementedException();
   
        public string CsvString(int iterationNo, int day)
        {
            throw new NotImplementedException();
        }
    }
}