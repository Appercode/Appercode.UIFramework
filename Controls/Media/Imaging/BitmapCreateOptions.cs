using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Controls.Media.Imaging
{
    // http://msdn.microsoft.com/ru-ru/library/system.windows.media.imaging.bitmapcreateoptions(v=vs.110).aspx
    public enum BitmapCreateOptions
    {
        None = 0,
        DelayCreation = 2,
        IgnoreImageCache = 8,
        BackgroundCreation = 16
    }
}