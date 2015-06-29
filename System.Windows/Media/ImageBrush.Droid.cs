using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.Media.Imaging;
using Java.IO;

namespace System.Windows.Media
{
    public partial class ImageBrush
    {
        public override Drawable ToDrawable()
        {
            BitmapDrawable drawable = new BitmapDrawable(this.ImageSource.GetBitmap());
            if (this.Stretch == Stretch.None)
            {
                drawable.Gravity = GravityFlags.Center;
            }
            else
            {
                drawable.Gravity = GravityFlags.Fill;
            }
            return drawable;
        }
    }
}