using System.Xml.Serialization;
using Xamarin.Forms;

namespace Integreat.Shared.Models
{
    public class JobOfferBase
    {
        [XmlIgnore]
        public string IsVisitedImage { get; set; } = "Icon_Transparent_Small";
        [XmlIgnore]
        public double FontSize { get; set; } = Device.GetNamedSize(NamedSize.Large, typeof(Label));
        [XmlIgnore]
        public Command OnTapCommand { get; set; }
    }
}
