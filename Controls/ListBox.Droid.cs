using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Appercode.UI.Controls
{
    public partial class ListBox
    {
        public override View NativeUIElement
        {
            get
            {
                return this.scrollViewer == null ? base.NativeUIElement : this.scrollViewer.NativeUIElement;
            }
            protected internal set
            {
                if(this.scrollViewer == null)
                {
                    base.NativeUIElement = value;
                    return;
                }
                this.scrollViewer.NativeUIElement = value;
            }
        }
    }
}