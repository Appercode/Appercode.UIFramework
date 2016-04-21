using Android.Content;
using Android.Views;
using Appercode.UI.Controls.NativeControl.Wrapers;

namespace Appercode.UI.Controls
{
    public class NativeUserControl : WrapedViewGroup
    {
        public NativeUserControl(Context context)
            : base(context)
        {
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
        }
    }

    public partial class UserControl
    {
        protected View ContentNativeUIElement { get; set; }

        protected internal override void NativeInit()
        {
            base.NativeInit();

            if (this.Parent != null && this.Context != null && this.NativeUIElement == null)
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new NativeUserControl(this.Context);
                }

                this.ApplyNativeContent(this.Content);
                this.ApplyNativePadding(this.Padding);
            }
        }

        protected virtual void OnNativeUIElementChanged(View oldUIElement, View newUIElement)
        {
        }

        protected void OnNativeContentChanged(UIElement oldContent, UIElement newContent)
        {
            this.ApplyNativeContent(newContent);
        }

        protected ViewGroup.LayoutParams CreateLayoutParams(UIElement element)
        {
            return new ViewGroup.LayoutParams(
                element.ContainsValue(WidthProperty) ? (int)element.Width : ViewGroup.LayoutParams.MatchParent,
                element.ContainsValue(HeightProperty) ? (int)element.Height : ViewGroup.LayoutParams.MatchParent);
        }

        private void ApplyNativeContent(UIElement newContent)
        {
            if (this.NativeUIElement != null && newContent != null)
            {
                this.ContentNativeUIElement = newContent.NativeUIElement;
                var nativeUserControl = (NativeUserControl)this.NativeUIElement;
                nativeUserControl.RemoveAllViews();
                nativeUserControl.AddView(this.ContentNativeUIElement);
            }
        }

        private void NativeArrangeContent(System.Drawing.RectangleF rectangleF)
        {
            this.Content.Arrange(rectangleF);
        }
    }
}