using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Appercode.UI.Controls.Media.Imaging
{
    public abstract class BitmapSource : ImageSource
    {
        public static readonly DependencyProperty PixelWidthProperty =
            DependencyProperty.Register("PixelWidth", typeof(int), typeof(BitmapSource), new PropertyMetadata(null, (d, e) => { }));

        public static readonly DependencyProperty PixelHeightProperty =
            DependencyProperty.Register("PixelHeight", typeof(int), typeof(BitmapSource), new PropertyMetadata(null, (d, e) => { }));

        public int PixelHeight
        {
            get
            {
                return this.PixelHeightInternal;
            }
        }

        public int PixelWidth
        {
            get
            {
                return this.PixelWidthInternal;
            }
        }

        internal virtual int PixelHeightInternal
        {
            get
            {
                return (int)this.GetValue(BitmapSource.PixelHeightProperty);
            }
        }

        internal virtual int PixelWidthInternal
        {
            get
            {
                return (int)this.GetValue(BitmapSource.PixelWidthProperty);
            }
        }

        public virtual void SetSource(Stream streamSource)
        {
            // nothing to do here
        } 

        private byte[] EnsureArray(byte[] arr, int currentPosition, int additionalSize)
        {
            int num;
            int length = (int)arr.Length - currentPosition;
            if (length >= additionalSize)
            {
                return arr;
            }
            int num1 = additionalSize - length;
            num = num1 < (int)arr.Length ? (int)arr.Length * 2 : (int)arr.Length + num1;
            byte[] numArray = new byte[num];
            arr.CopyTo(numArray, 0);
            return numArray;
        }       
    }
}