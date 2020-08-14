namespace Yaabm.generic
{
    public interface IInitializationInfo
    {
        public IScenario Scenario { get; }

        void LoadScenario(IScenario scenario);
    }
}