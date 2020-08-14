using Yaabm.generic;

namespace TestSirModel.Model
{
    public class EmptyResourceSystem : ILocalResourceSystem<SirAgent>
    {
        public void IterateOneDay(LocalArea<SirAgent> localContext)
        {
            // Do nothing
        }
    }
}
