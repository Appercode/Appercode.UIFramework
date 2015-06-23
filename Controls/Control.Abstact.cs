using System;
using System.Windows;
using System.Windows.Media;

namespace Appercode.UI.Controls
{
    public partial class Control
    {
        protected bool NativeIsEnabled { get; set; }

        protected double NativeFontSize { get; set; }

        protected TextWrapping NativeTextWrapping { get; set; }

        protected TextTrimming NativeTextTrimming { get; set; }

        protected FontWeight NativeFontWeight { get; set; }

        protected FontStyle NativeFontStyle { get; set; }

        protected FontFamily NativeFontFamily { get; set; }

        protected Brush NativeForeground { get; set; }

        protected Thickness NativePadding { get; set; }

        protected virtual bool InternalFocus()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnIsEnabledChanged()
        {
        }

        protected virtual void NativeOnbackgroundChange()
        {
        }

        private static double GetDefaultFontSize()
        {
            return 0.0;
        }

        private void RemoveControlTemplateInstance()
        {
        }

        private void AddControlTemplateInstance()
        {
        }

        private void ApplyNativePadding(Thickness padding)
        {
        }
    }
}