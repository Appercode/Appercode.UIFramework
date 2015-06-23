using System;
using System.Drawing;
using System.Windows.Media;

namespace Appercode.UI.Controls
{
    public partial class MediaElement
    {
        private Nullable<int> NativeAudioStreamIndex { get; set; }

        private bool NativeAutoPlay { get; set; }

        private double NativeBalance { get; set; }

        private bool NativeIsMuted { get; set; }

        private TimeSpan NativePosition { get; set; }

        private Uri NativeSource { get; set; }

        private Stretch NativeStretch { get; set; }

        private double NativeVolume { get; set; }

        private static double GetNativeVolumeInitialValue()
        {
            return 0;
        }

        private void NativePause()
        {
        }

        private void NativePlay()
        {
        }

        private void NativeStop()
        {
        }

        private void NativeArrangeVideoView(RectangleF finalRect)
        {
        }
    }
}
