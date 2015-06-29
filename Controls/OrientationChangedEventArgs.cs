using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Controls
{
    public class OrientationChangedEventArgs : EventArgs
    {
        private PageOrientation orientation;

        public OrientationChangedEventArgs(PageOrientation orientation)
        {
            this.orientation = orientation;
        }

        public PageOrientation Orientation
        {
            get
            {
                return this.orientation;
            }
        }
    }
}
