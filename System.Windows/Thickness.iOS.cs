using Appercode.UI;
using UIKit;

namespace System.Windows
{
    public partial struct Thickness
    {
        public static explicit operator UIEdgeInsets(Thickness thickness)
        {
            return new UIEdgeInsets(thickness.TopF(), thickness.LeftF(), thickness.BottomF(), thickness.RightF());
        }
    }
}
