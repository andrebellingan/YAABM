namespace Yaabm.generic
{
    public interface IIntervention
    {
        /// <summary>
        /// The simulation day when this intervention must be applied
        /// </summary>
        int DayOfIntervention { get; set;  }

        /// <summary>
        /// A description of the intervention
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Apply the simulation to a simulation
        /// </summary>
        /// <param name="targetSimulation">The simulation that must be modified by this intervention</param>
        void Apply(object targetSimulation);
    }
}