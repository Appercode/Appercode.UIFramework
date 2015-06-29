using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appercode.UI.Controls.Media.Imaging
{
    public partial class BitmapImage
    {
        private void UriChanged(Uri uri1, Uri uri2)
        {
            this.ImageFailed(this, new System.Windows.ExceptionRoutedEventArgs());
            this.ImageOpened(this, new System.Windows.ExceptionRoutedEventArgs());
            this.DownloadProgress(this, new DownloadProgressEventArgs());
        }
    }
}
