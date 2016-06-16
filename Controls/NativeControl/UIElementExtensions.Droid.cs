using Appercode.UI.Controls.Media.Imaging;
using System.Windows.Media;

namespace Appercode.UI.Controls.NativeControl
{
    internal static partial class UIElementExtensions
    {
        internal static void SetNativeBackground(this UIElement element, Brush background)
        {
            if (element.NativeUIElement != null)
            {
                var imageBackground = background as ImageBrush;
                if (imageBackground != null)
                {
                    if (element.IsBackgroundValidImageBrush(imageBackground))
                    {
                        // TODO: avoid passing of local variables to lambdas
                        ((BitmapImage)imageBackground.ImageSource).ImageOpened += (s, e) =>
                            {
                                if (element.IsBackgroundValidImageBrush(imageBackground))
                                {
                                    element.NativeUIElement.Post(() =>
                                        {
                                            element.NativeUIElement.Background = imageBackground.ToDrawable();
                                            element.OnLayoutUpdated();
                                        });
                                }
                            };
                    }
                }
                else
                {
                    element.NativeUIElement.Background = background?.ToDrawable();
                }
            }
        }

        private static bool IsBackgroundValidImageBrush(this UIElement element, ImageBrush background)
        {
            var image = background?.ImageSource as BitmapImage;
            return image?.UriSource?.IsAbsoluteUri == true;
        }
    }
}