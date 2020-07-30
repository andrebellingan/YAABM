using Yaabm.generic;

namespace TestSirModel.Model
{
    public class SirAgent : Agent<SirAgent>
    {
        public bool IsInfectious { get; set; }
    }
}
