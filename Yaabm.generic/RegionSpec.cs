using System;

namespace Yaabm.generic
{
    public class RegionSpec : IEquatable<RegionSpec>
    {
        public RegionSpec(string parentRegionName, string name, string fullName, string regionType)
        {
            ParentRegionName = parentRegionName;
            Name = name;
            FullName = fullName;
            RegionType = regionType;
        }

        public string ParentRegionName { get; }

        public string Name { get; }

        public string FullName { get; }

        public string RegionType { get; }

        public bool Equals(RegionSpec other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ParentRegionName == other.ParentRegionName && Name == other.Name && FullName == other.FullName && RegionType == other.RegionType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RegionSpec) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ParentRegionName, Name, FullName, RegionType);
        }

        public string LogString()
        {
            return $"{RegionType} Name={Name} ({FullName}) parent = {ParentRegionName}";
        }
    }
}
