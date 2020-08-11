namespace Yaabm.generic
{
    public class Encounter<TAgent> where TAgent: Agent<TAgent>
    {
        public TAgent Agent { get; set; }

        public dynamic EncounterInformation { get; set; }
    }
}
