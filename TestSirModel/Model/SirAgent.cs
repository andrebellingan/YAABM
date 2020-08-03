using Yaabm.generic;

namespace TestSirModel.Model
{
    public class SirAgent : Agent<SirAgent>
    {
        public SirAgent(int id) : base(id)
        {
        }

        public bool IsInfectious { get; set; }
    }
}
