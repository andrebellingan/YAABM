using Yaabm.generic;

namespace TestSirModel.Model
{
    public class SirAgent : Agent<SirAgent>
    {
        public SirAgent(int id) : base(id)
        {
        }

        public SirContext SirContext => (SirContext) HomeArea;

        public int IncubationTime { get; set; }

        public int InfectiousDays { get; set; }
    }
}
