using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace SherpaDesk.Models
{
    public class AttachmentModel
    {
        public string FileName { get; set; }

        public BitmapImage Image { get; set; }
    }
}
