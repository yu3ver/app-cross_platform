using System.Xml.Serialization;

namespace Integreat.Shared.Models.C4R
{
    public enum RegionType
    {
        [XmlEnum(Name = "")]
        Unknown = 0, // default fallback value
        [XmlEnum(Name = "PLZ")]
        PostalCode,
        [XmlEnum(Name = "ORT")]
        Place,
        [XmlEnum(Name = "GEBIET")]
        Region
    }
}

