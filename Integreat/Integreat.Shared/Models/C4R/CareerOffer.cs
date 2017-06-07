using System.Collections.Generic;
using System.Xml.Serialization;
using Integreat.Shared.Models.Sprungbrett;
using Xamarin.Forms;

namespace Integreat.Shared.Models.C4R
{
    //CareerOffer class to save and manipulate the xml elements
    [XmlType(TypeName = "anzeige")]
    public class CareerOffer : JobOfferBase
    {
        [System.Xml.Serialization.XmlElement("id")]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlElement("interneId")]
        public string InternalId { get; set; }
        [System.Xml.Serialization.XmlElement("titel")]
        public string JobTitle { get; set; }
        [System.Xml.Serialization.XmlElement("firma")]
        public string CompanyName { get; set; }
        [XmlArray("regionen")]
        [XmlArrayItem("region", Type = typeof(Region))]
        public List<Region> Regions { get; set; }
        [System.Xml.Serialization.XmlElement("beschreibungOrt")]
        public string PlaceDescription { get; set; }
        [System.Xml.Serialization.XmlElement("link")]
        public string Link { get; set; }
        [System.Xml.Serialization.XmlElement("bewerbungslink")]
        public string Offerlink { get; set; }
        [System.Xml.Serialization.XmlElement("laufendesDatum")]
        public string Date { get; set; }
        [System.Xml.Serialization.XmlElement("volltext")]
        public string Text { get; set; }

    }
}

