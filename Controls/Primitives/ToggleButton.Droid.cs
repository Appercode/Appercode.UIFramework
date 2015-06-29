using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.NativeControl;
using System.Drawing;

namespace Appercode.UI.Controls.Primitives
{
    public partial class ToggleButton
    {
        private bool? nativeIsChecked;

        protected bool? NativeIsChecked
        {
            get
            {
                return this.nativeIsChecked;
            }
            set
            {
                this.nativeIsChecked = value;

                if (this.NativeUIElement != null && this.controlTemplateInstance == null)
                {
                    ((NativeToggleButton)this.NativeUIElement).IsChecked = value.HasValue && value.Value;
                    ((NativeToggleButton)this.NativeUIElement).RefreshDrawableState();
                }
            }
        }

        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null)
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new NativeToggleButton(this.Context);
                    this.ApplyNativeContent(null, this.Content);
                }
            }

            base.NativeInit();
            
            if (this.NativeUIElement != null && !(this is CheckBox) && !(this is RadioButton))
            {
                ((NativeToggleButton)this.NativeUIElement).IsChecked = this.NativeIsChecked.HasValue && this.NativeIsChecked.Value;
                ((NativeContentControl)this.NativeUIElement).SetBackgroundDrawable(new Android.Widget.ToggleButton(this.Context).Background);
            }
        }

    }
}