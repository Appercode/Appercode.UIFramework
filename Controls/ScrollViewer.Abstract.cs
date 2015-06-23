using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appercode.UI.Controls
{
    public partial class ScrollViewer
    {
        public ScrollBarVisibility NativeHorizontalScrollBarVisibility { get; set; }

        public ScrollBarVisibility NativeVerticalScrollBarVisibility { get; set; }

        private void SetContentScrolableSize(SizeF contentSize)
        {
        }

        private SizeF MeasureContent(SizeF sizeF)
        {
            return sizeF;
        }

        private void NativeScrollToHorizontalOffset(double offset)
        {
        }

        private void NativeScrollToVerticalOffset(double offset)
        {
        }

        private void NativeMoveToVerticalOffset(double offset)
        {
        }
    }
}
