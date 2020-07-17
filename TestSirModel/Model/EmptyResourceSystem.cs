using Yaabm.generic;

namespace TestSirModel.Model
{
    public class EmptyResourceSystem : ILocalResourceSystem<SirAgent>
    {
        public void IterateOneDay(LocalContext<SirAgent> localContext)
        {
            // Do nothing
        }
    }
}
