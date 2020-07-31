using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Yaabm.generic
{
    [DataContract(Namespace = "https://YACABM/")]
    public class InterventionParam
    {
        /// <summary>
        /// The name of this parameter
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The qualified type of this parameter. For example System.Integer
        /// </summary>
        [DataMember]
        public string TypeName { get; set; }

        /// <summary>
        /// The value of the parameter
        /// </summary>
        [DataMember]
        public string Value { get; set; }

        public object CreateInstance()
        {
            var pType = Type.GetType(TypeName);
            if (pType == null)
            {
                InternalLog.Error($"Parameter '{Name}: Type {TypeName} does not exist'");
                throw new InvalidCastException(TypeName);
            }

            return !pType.IsEnum ? Convert.ChangeType(Value, pType, CultureInfo.InvariantCulture) : Enum.Parse(pType, Value);
        }
    }

    [CollectionDataContract(Namespace = "https://YACABM/", ItemName = "Parameter")]
    public class ParameterList : List<InterventionParam>
    {
    }

    [DataContract(Namespace = "https://YACABM/")]
    public class InterventionSpec
    {
        /// <summary>
        /// The namespace qualified name of the intervention
        /// </summary>
        [DataMember]
        public string InterventionName { get; set; }

        /// <summary>
        /// The day when the intervention should be applied
        /// </summary>
        [DataMember]
        public int DayToApply { get; set; }

        /// <summary>
        /// List of parameters (in order)
        /// </summary>
        [DataMember]
        public ParameterList Parameters { get; set; } = new ParameterList();

        /// <summary>
        /// Create an instance of IIntervention based on this specification
        /// </summary>
        /// <returns>An object that implements IIntervention</returns>
        public IIntervention CreateInstance()
        {
            var interventionType = Type.GetType(InterventionName);
            if (interventionType == null)
            {
                InternalLog.Error($"There is not type called {InterventionName}");
                InternalLog.Info("If the type is in another assembly then you need to specify as \"namespace.qualified.name, assembly.name\"");
                return null;
            }

            if (!interventionType.GetInterfaces().Contains(typeof(IIntervention)))
            {
                InternalLog.Error($"Intervention type {InterventionName} does not implement interface {nameof(IIntervention)}");
                return null;
            }

            var parameters = GetParameterArray();

            try
            {
                var intervention = (IIntervention) Activator.CreateInstance(interventionType, parameters);
                if (intervention != null)
                {
                    intervention.DayOfIntervention = DayToApply;

                    return intervention;
                }

                return null;
            }
            catch (Exception paramException)
            {
                InternalLog.Error(paramException, $"Could not create an instance of {InterventionName}");
                return null;
            }
        }

        private object[] GetParameterArray()
        {
            var result = new object[Parameters.Count];

            for (var i = 0; i < Parameters.Count; i++)
            {
                result[i] = Parameters[i].CreateInstance();
            }

            return result;
        }
    }
}
