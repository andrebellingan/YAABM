using System;
using System.Collections.Generic;
using Yaabm.generic;

namespace Covid19ModelLibrary
{
    public class BasicHospitalSystem : ILocalResourceSystem<Human>
    {
        private readonly Dictionary<int, Human> _admittedPatients = new Dictionary<int, Human>();

        private readonly Dictionary<int, Human> _reservedPatients = new Dictionary<int, Human>();

        public BasicHospitalSystem(double bedsPerThousand)
        {
            BedsPerThousand = bedsPerThousand;
        }

        public double BedsPerThousand { get; set; }

        public void IterateOneDay(LocalArea<Human> localContext)
        {
            //TODO: Implement hospital system
        }

        public int ReservedBeds => _reservedPatients.Count;

        public bool HospitalBedIsAvailable(Human agent, bool reserveBedIfTrue)
        {
            if (OccupiedBeds + ReservedBeds >= NumberOfBeds)
            {
                return false;
            }

            if (reserveBedIfTrue && !_reservedPatients.ContainsKey(agent.Id))
            {
                _reservedPatients.Add(agent.Id, agent); // had already requested a bed
            }

            return true;
        }

        public bool IcuBedIsAvailable()
        {
            //TODO: Implement Icu bed limitations
            return true;
        }

        public void SetPopulation(double livesPerProvince)
        {
            NumberOfBeds = Convert.ToInt32(Math.Round(livesPerProvince * BedsPerThousand / 1000, 0));
        }

        public int NumberOfBeds { get; set; }

        public int OccupiedBeds => _admittedPatients.Count;

        public void AdmitPatient(Human patient)
        {
            if (OccupiedBeds > NumberOfBeds) throw new InvalidOperationException("Can't put patient in bed that doesn't exist!");

            if (!_reservedPatients.ContainsKey(patient.Id))
            {
                throw new InvalidOperationException("This patient doesn't have a reserved hospital bed");
            }
            _reservedPatients.Remove(patient.Id);
            _admittedPatients.Add(patient.Id, patient);
        }

        public void DischargePatient(Human patient)
        {
            if (_admittedPatients.ContainsKey(patient.Id))
            {
                _admittedPatients.Remove(patient.Id);
            }
        }
    }
}