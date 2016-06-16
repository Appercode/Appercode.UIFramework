using Android.Content;
using Android.Text;
using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.NativeControl;
using System;
using System.Windows.Media;

namespace Appercode.UI.Controls
{
    public partial class HyperlinkButton
    {
        protected Uri NativeNavigateUri
        {
            get;
            set;
        }

        protected override View CreateDefaultControl(string value)
        {
            if (this.Content is string)
            {
                var control = new TextBlock
                {
                    FontFamily = this.FontFamily,
                    Text = this.Content as string,
                    FontSize = this.FontSize,
                    FontStyle = this.FontStyle,
                    Foreground = this.Foreground
                };
                LogicalTreeHelper.AddLogicalChild(this, control);
                return control.NativeUIElement;
            }
            else
            {
                var innerDefaultControl = new TextView(this.Context);
                innerDefaultControl.LayoutParameters = this.CreateLayoutParams();
                innerDefaultControl.SetSingleLine(true);
                innerDefaultControl.TextFormatted = Html.FromHtml(String.Format("<a href=\"\">{0}</a>", value));
                return innerDefaultControl;
            }
        }

        protected override void ApplyNativeContentForDefaultControl(string value)
        {
            ((TextView)this.ContentNativeUIElement).TextFormatted = Html.FromHtml(String.Format("<a href=\"\">{0}</a>", value));
        }

        protected override void OnBackgroundChanged(Brush oldValue, Brush newValue)
        {
            this.SetNativeBackground(newValue);
        }

        protected internal override void NativeInit()
        {
            base.NativeInit();
            this.SetNativeBackground(this.Background);
        }

        protected override View CreateLayoutControl(UIElement value)
        {
            return new View(this.Context);
        }

        protected override void OnClick()
        {
            base.OnClick();
            if (NavigateUri != null && NavigateUri.IsAbsoluteUri)
            {
                var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(NavigateUri.AbsoluteUri));
                this.Context.StartActivity(intent);
            }
        }
    }
}