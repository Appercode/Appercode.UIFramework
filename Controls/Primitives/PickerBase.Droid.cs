using System.Globalization;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using Appercode.UI.Device;

namespace Appercode.UI.Controls.Primitives
{
    public abstract partial class PickerBase
    {
        public DialogFragment PickerContainer
        {
            get;
            protected set;
        }

        protected virtual void NativeShow()
        {
            this.PickerContainer.Show(((Activity)this.Context).FragmentManager, "999");
        }

        protected virtual void NativeDismiss()
        {
            this.PickerContainer.Dismiss();
        }

        protected internal override void NativeInit()
        {
            this.pickerButton.NativeUIElement.SetBackgroundResource(Android.Resource.Drawable.SpinnerBackground);
            base.NativeInit();
        }

        public static int GetDialogWidth()
        {
            var isLandscape = (ScreenProperties.InterfaceOrientation == InterfaceOrientation.LandScapeRight ||
                               ScreenProperties.InterfaceOrientation == InterfaceOrientation.LandscapeLeft);
            var widthRatioResName = isLandscape ? "dialog_min_width_major" : "dialog_min_width_minor";
            var widthRatioId = Application.Context.Resources.GetIdentifier(widthRatioResName, "dimen", "android");
            var widthRatio = float.Parse(Android.Content.Res.Resources.System.GetString(widthRatioId).Trim(new[] { '%' }), CultureInfo.InvariantCulture);
            return (int)(ScreenProperties.DisplaySize.Width * widthRatio / 100 - ScreenProperties.ConvertDPIToPixels(32));
        }

        protected class PickerDialogFragment : DialogFragment
        {
            private View pickerView;
            private PickerBase owner;

            public Action DoneButtonAction
            {
                get;
                set;
            }

            public Action ConfigurationChangedAction
            {
                get;
                set;
            }

            public PickerDialogFragment(View pickerView, PickerBase owner)
                : base()
            {
                this.pickerView = pickerView;
                this.owner = owner;
            }

            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                if (this.pickerView.Parent != null)
                {
                    ((ViewGroup)this.pickerView.Parent).RemoveAllViews();
                }

                var dialogBuilder = new AlertDialog.Builder(this.Activity, Android.Resource.Style.ThemeHoloLightDialogMinWidth);
                var dialog = dialogBuilder.SetTitle(this.owner.PickerTitle).SetView(this.pickerView).SetPositiveButton(owner.OkButtonTitle, (s, e) => this.DoneButtonAction()).Create();
                dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                pickerView.Focusable = true;
                return dialog;
            }

            public override void OnConfigurationChanged(Configuration newConfig)
            {
                base.OnConfigurationChanged(newConfig);
                if (ConfigurationChangedAction != null)
                    ConfigurationChangedAction.Invoke();
            }

            public override void Show(FragmentManager manager, string tag)
            {
                if (manager.FindFragmentByTag(tag) == null)
                    base.Show(manager, tag);
            }
        }
    }
}