using Android.Graphics.Drawables;
using Appercode.UI;

namespace System.Windows.Media
{
    public abstract partial class Brush : DependencyObject
    {
        public virtual Drawable ToDrawable()
        {
            return null;
        }
    }
}