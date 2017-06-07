using System.Xml.Serialization;

namespace Integreat.Shared.Models.C4R
{
    //CareerOffer class to save and manipulate the region sub elements
    [XmlType(TypeName = "region")]
    public class Region
    {
        [XmlAttribute("typ")]
        public RegionType Type { get; set; }
        [System.Xml.Serialization.XmlAttribute("land")]
        public string Country { get; set; }
        [System.Xml.Serialization.XmlAttribute("plz")]
        public string Zipcode { get; set; }
        [System.Xml.Serialization.XmlText]
        public string Content { get; set; }
    }
}

