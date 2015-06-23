using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Media
{
    public enum MediaElementState
    {
        Closed = 0,
        Opening = 1,
        Buffering = 2,
        Playing = 3,
        Paused = 4,
        Stopped = 5,
        Individualizing = 6,
        AcquiringLicense = 7
    }
}