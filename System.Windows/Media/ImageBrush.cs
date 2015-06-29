using Appercode.UI;
using Appercode.UI.Controls.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace System.Windows.Media
{
    public sealed partial class ImageBrush : Brush
    {
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageBrush), new PropertyMetadata(12900, (d, e) => { }));

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(ImageBrush), new PropertyMetadata(Stretch.Fill));

        public ImageSource ImageSource
        {
            get
            {
                return (ImageSource)this.GetValue(ImageBrush.ImageSourceProperty);
            }
            set
            {
                this.SetValue(ImageBrush.ImageSourceProperty, value);
            }
        }

        public Stretch Stretch
        {
            get { return (Stretch)this.GetValue(StretchProperty); }
            set { this.SetValue(StretchProperty, value); }
        }
    }
}