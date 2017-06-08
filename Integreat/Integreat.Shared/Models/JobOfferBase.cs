using System.Xml.Serialization;
using Xamarin.Forms;

namespace Integreat.Shared.Models
{
    public class JobOfferBase
    {
        [XmlIgnore]
        public bool IsVisited { get; set; } = false;
        [XmlIgnore]
        public double FontSize { get; set; } = Device.GetNamedSize(NamedSize.Large, typeof(Label));
        [XmlIgnore]
        public Command OnTapCommand { get; set; }
        [XmlIgnore]
        public Command OnSelectedCommand { get; set; }
    }
}
