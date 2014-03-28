using System.Runtime.Serialization;
using Windows.UI.Xaml.Media;

namespace SherpaDesk.Models
{
    public class ImageView
    {
        public ImageSource Image { get; set; }
        public string FileName { get; set; }
    }
}