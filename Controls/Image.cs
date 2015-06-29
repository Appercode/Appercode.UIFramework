using Appercode.UI.Controls.Media;
using Appercode.UI.Controls.Media.Imaging;
using Appercode.UI.Device;
using System;
using System.Windows;
using SWM = System.Windows.Media;

namespace Appercode.UI.Controls
{
    public sealed partial class Image : UIElement
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(Image), new PropertyMetadata(null, (d, e) =>
            {
                    ((Image)d).ApplyNativeSource();
                    ((Image)d).OnLayoutUpdated();
            }));

        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(SWM.Stretch), typeof(Image), new PropertyMetadata(SWM.Stretch.Uniform, (d, e) =>
            {
                ((Image)d).ApplyNativeStretch();
                ((Image)d).OnLayoutUpdated();
            }));

        public event EventHandler<ExceptionRoutedEventArgs> ImageFailed = delegate { };

        public event EventHandler<RoutedEventArgs> ImageOpened = delegate { };

        public ImageSource Source
        {
            get
            {
                return (ImageSource)this.GetValue(Image.SourceProperty);
            }
            set
            {
                this.SetValue(Image.SourceProperty, value);
            }
        }

        public SWM.Stretch Stretch
        {
            get
            {
                return (SWM.Stretch)this.GetValue(Image.StretchProperty);
            }
            set
            {
                this.SetValue(Image.StretchProperty, value);
            }
        }
    }
}