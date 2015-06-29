using Android.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Controls
{
    public partial class AppercodePage
    {
        public NativeAppercodeFragment NativeFragment
        {
            get;
            set;
        }

        protected override void OnNavigatedTo(Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected internal override void NativeInit()
        {
            base.NativeInit();
            if (this.Context != null && this.Parent != null)
            {
                if (NativeFragment == null)
                {
                    var nativeApperCodeFragment = new NativeAppercodeFragment(this.NativeUIElement);
                    this.NativeFragment = nativeApperCodeFragment;
                }
            }
        }
    }
}