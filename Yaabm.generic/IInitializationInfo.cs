﻿using System.Collections.Generic;

namespace Yaabm.generic
{
    public interface IInitializationInfo
    {
        public IScenario Scenario { get; }

        void LoadScenario(IScenario scenario);

        public InterventionList ModelEvents { get; }
    }
}