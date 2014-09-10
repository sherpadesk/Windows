using Windows.UI.Xaml.Media.Imaging;

namespace SherpaDesk.Models
{
    public class AttachmentModel
    {
        public string FileName { get; set; }

        public BitmapImage Image { get; set; }

        public string ImageUrl { get; set; }
    }
}
