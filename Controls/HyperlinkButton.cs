using System;
using System.Windows;
using Appercode.UI.Controls.Primitives;
using Appercode.UI.Controls.Media;
using System.Windows.Media;

namespace Appercode.UI.Controls
{
    public partial class HyperlinkButton : ButtonBase
    {
        public static readonly DependencyProperty NavigateUriProperty =
            DependencyProperty.Register("NavigateUri", typeof(Uri), typeof(HyperlinkButton),
            new PropertyMetadata(null, (d, e) =>
            {
                ((HyperlinkButton)d).NativeNavigateUri = (Uri)e.NewValue;
            }));

        static HyperlinkButton()
        {
            ForegroundProperty.AddOwner(typeof(HyperlinkButton), new PropertyMetadata(new SolidColorBrush(AppercodeColors.Blue)));
        }

        public HyperlinkButton()
        {
        }

        public Uri NavigateUri
        {
            get { return (Uri)this.GetValue(NavigateUriProperty); }
            set { this.SetValue(NavigateUriProperty, value); }
        }
    }
}