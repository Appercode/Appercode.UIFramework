using Android.Content;
using Android.Graphics.Drawables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Controls.NativeControl
{
    public class CheckBoxDrawableHelper : Android.Widget.CheckBox
    {
        private CheckBoxDrawableHelper(Context context)
            : base(context)
        {
        }

        public static Drawable BackgroundImage
        {
            get;
            set;
        }

        public static Drawable GetDrawable(Context context)
        {
            var cb = new CheckBoxDrawableHelper(context);
            return BackgroundImage;
        }

        public override void SetButtonDrawable(Android.Graphics.Drawables.Drawable d)
        {
            BackgroundImage = d;

            base.SetButtonDrawable(d);
        }

        public override void SetButtonDrawable(int resid)
        {
            BackgroundImage = this.Context.Resources.GetDrawable(resid);

            base.SetButtonDrawable(resid);
        }
    }
}