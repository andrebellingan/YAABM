using System.ComponentModel;
using Covid19ModelLibrary.Scenarios;
using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    public class CovidStateModel : MultiStateModel<Human>
    {
        public ModelState<Human> S { get; private set; }
        public ModelState<Human> E { get; private set; }
        public ModelState<Human> A { get; private set; }
        public ModelState<Human> Ar { get; private set; }
        public ModelState<Human> M { get; private set; }
        public ModelState<Human> Mr { get; private set; }
        public ModelState<Human> Sv { get; private set; }
        public ModelState<Human> Ih { get; private set; }
        public ModelState<Human> Ic { get; private set; }
        public ModelState<Human> Hr { get; private set; }
        public ModelState<Human> Hc { get; private set; }
        public ModelState<Human> Hd { get; private set; }
        public ModelState<Human> Cr { get; private set; }
        public ModelState<Human> Cd { get; private set; }
        public ModelState<Human> Rh { get; private set; }
        public ModelState<Human> Rc { get; private set; }
        public ModelState<Human> Dh { get; private set; }
        public ModelState<Human> Dc { get; private set; }

        public override ModelState<Human> DefaultState => S;

        public CovidStateModel(DiseaseParameters diseaseParameters)
        {
            Parameters = diseaseParameters;
            //TODO: process parameters?

            SetupStates();

            SetInfectionTransition(new Infection(this, Infectiousness));

            // Incubation transitions
            CreateConditionalTransition(E, A, (agent) => agent.Symptoms == Symptoms.Asymptomatic && agent.NumberOfDaysInCurrentState >= agent.IncubationPeriod);
            CreateConditionalTransition(E, M, (agent) => agent.Symptoms == Symptoms.Mild && agent.NumberOfDaysInCurrentState >= agent.IncubationPeriod);
            CreateConditionalTransition(E, Sv, (agent) => agent.Symptoms == Symptoms.Severe && agent.NumberOfDaysInCurrentState >= agent.IncubationPeriod);

            // Severe to isolation
            CreateConditionalTransition(Sv, Ih, agent => agent.SevereToHospital && agent.NumberOfDaysInCurrentState > agent.DaysInfectious);
            CreateConditionalTransition(Sv, Ic, agent => !agent.SevereToHospital && agent.NumberOfDaysInCurrentState > agent.DaysInfectious);

            // Admitted to hospital
            CreateConditionalTransition(Ih, Hr, agent => agent.HospitalOutcome == HospitalOutcome.Recover && agent.NumberOfDaysInCurrentState >= agent.DaysInHospital);
            CreateConditionalTransition(Ih, Hc, agent => agent.HospitalOutcome == HospitalOutcome.IntensiveCare && agent.NumberOfDaysInCurrentState >= agent.DaysInHospital);
            CreateConditionalTransition(Ih, Hd, agent => agent.HospitalOutcome == HospitalOutcome.Die && agent.NumberOfDaysInCurrentState >= agent.DaysInHospital);

            // Moved to intensive care
            CreateConditionalTransition(Ic, Cr, agent => !agent.DiesInIcu && agent.NumberOfDaysInCurrentState >= agent.DaysIsolated);
            CreateConditionalTransition(Ic, Cd, agent => agent.DiesInIcu && agent.NumberOfDaysInCurrentState >= agent.DaysIsolated);
            CreateConditionalTransition(Hc, Cr, agent => !agent.DiesInIcu && agent.NumberOfDaysInCurrentState >= agent.DaysInHospital);
            CreateConditionalTransition(Hc, Cd, agent => agent.DiesInIcu && agent.NumberOfDaysInCurrentState >= agent.DaysInHospital);

            // Recovery transitions
            CreateConditionalTransition(A, Ar, (agent) => agent.NumberOfDaysInCurrentState >= agent.DaysInfectious);
            CreateConditionalTransition(M, Mr, (agent) => agent.NumberOfDaysInCurrentState >= agent.DaysInfectious);
            CreateConditionalTransition(Hr, Rh, agent => agent.NumberOfDaysInCurrentState >= agent.DaysInHospital);
            CreateConditionalTransition(Cr, Rc, agent => agent.NumberOfDaysInCurrentState >= agent.DaysInIntensiveCare);

            // Deaths
            CreateConditionalTransition(Hd, Dh, agent => agent.NumberOfDaysInCurrentState >= agent.DaysInHospital);
            CreateConditionalTransition(Cd, Dc, agent => agent.NumberOfDaysInCurrentState >= agent.DaysInIntensiveCare);
        }

        public DiseaseParameters Parameters { get; }

        private double Infectiousness(Human agent)
        {
            var symptomsFactor = agent.Symptoms == Symptoms.Asymptomatic ? Parameters.RelativeBetaAsymptomatic : 1.0;
            var lockdownFactor = Parameters.GetLockDownFactor(agent.Ward.LockDownLevel);
            return Parameters.ProbInfection * symptomsFactor * lockdownFactor;
        }

        private void SetupStates()
        {
            S = CreateModelState("S", false, true);
            E = CreateModelState("E", false, false, PersonExposed);
            // asymptomatic
            A = CreateModelState("A", true, false);
            Ar = CreateModelState("Ar", false, false);
            // Mild
            M = CreateModelState("M", true, false);
            Mr = CreateModelState("Mr", false, false);
            // Severe
            Sv = CreateModelState("Sv", true, false, IncubationEndsSevere);
            // Isolated
            Ih = CreateModelState("Ih", false, false, PersonAdmitToHospital); // no longer spreading (page 7 of August ASSA report)
            Ic = CreateModelState("Ic", false, false, PersonMovedToIcu);
            // Hospitalized
            Hr = CreateModelState("Hr", true, false);
            Hc = CreateModelState("Hc", true, false, PersonMovedToIcu);
            Hd = CreateModelState("Hd", true, false);
            // In ICU
            Cr = CreateModelState("Cr", true, false);
            Cd = CreateModelState("Cd", true, false);
            // Recovered
            Rh = CreateModelState("Rh", false, false);
            Rc = CreateModelState("Rc", false, false);
            // Dead
            Dh = CreateModelState("Dh", false, false);
            Dc = CreateModelState("Dc", false, false);
        }

        private void PersonMovedToIcu(Human agent, IRandomProvider random)
        {
            agent.DiesInIcu = random.Chance(Parameters.PropDieInIcu);
            var meanTimeInIcu = agent.DiesInIcu ? Parameters.TimeCtoD : Parameters.TimeCtoR;
            var rateOfExit = 1d / meanTimeInIcu;
            agent.DaysInIntensiveCare = random.SampleDaysInState(rateOfExit);
        }

        private void PersonAdmitToHospital(Human agent, IRandomProvider random)
        {
            agent.HospitalOutcome = Parameters.HospitalizationOutcomes.Sample(random);
            var meanLengthOfStay = agent.HospitalOutcome switch
            {
                HospitalOutcome.Recover => Parameters.TimeHtoR,
                HospitalOutcome.IntensiveCare => Parameters.TimeHToC,
                HospitalOutcome.Die => Parameters.TimeHtoD,
                _ => throw new InvalidEnumArgumentException(
                    $"Mean time in hospital for outcome {agent.HospitalOutcome} has not been mapped in the disease parameters")
            };

            var dischargeRate = 1d / meanLengthOfStay;
            agent.DaysInHospital = random.SampleDaysInState(dischargeRate);
        }

        private void IncubationEndsSevere(Human agent, IRandomProvider random)
        {
            agent.SevereToHospital = random.Chance(Parameters.PropStoH);
            var meanTimeToAdmission = agent.SevereToHospital ? Parameters.TimeStoH : Parameters.TimeStoC;
            var progressionRate = 1d / meanTimeToAdmission;
            agent.DaysIsolated = random.SampleDaysInState(progressionRate);
        }

        private void PersonExposed(Human agent, IRandomProvider random)
        {
            var symptoms = Parameters.SymptomWeights.Sample(random);
            agent.Symptoms = symptoms;
            var incubationRate = 1d / Parameters.IncubationPeriod;
            agent.IncubationPeriod = random.SampleDaysInState(incubationRate);

            var infectiousRate = 1d / Parameters.TimeInfectious;
            agent.DaysInfectious = random.SampleDaysInState(infectiousRate);


        }
    }
}
