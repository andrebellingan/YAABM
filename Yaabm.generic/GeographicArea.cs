
namespace Yaabm.generic
{
    public abstract class GeographicArea<T> where T : Agent<T>
    {
        protected GeographicArea(string name, string fullName, string areaType)
        {
            Name = name;
            FullName = fullName;
            AreaType = areaType;
        }

        public Region<T> Parent { get; internal set; }

        public string Name { get; }

        public string FullName { get; }

        public string AreaType { get; }
    }
}