namespace Yaabm.generic
{
    public interface ILocalResourceSystem<T> where T : Agent<T>
    {
        void IterateOneDay(LocalArea<T> localContext);
    }
}