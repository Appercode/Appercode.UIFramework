using System.Drawing;
using System.Windows.Markup;

namespace Appercode.UI.Controls
{
    [RuntimeNameProperty("Name")]
    public partial class UIElement
    {
        protected internal object NativeUIElement { get; set; }

        internal virtual bool IsFocused
        {
            get { return false; }
        }

        private Visibility NativeVisibility { get; set; }

        private double NativeWidth { get; set; }

        private double NativeHeight { get; set; }

        /// <summary>
        /// Measure <seealso cref="NativeUIElement"/>
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected virtual SizeF NativeMeasureOverride(SizeF availableSize)
        {
            return new SizeF();
        }

        /// <summary>
        /// Arrange <seealso cref="NativeUIElement"/>
        /// </summary>
        /// <param name="finalRect"></param>
        protected virtual void NativeArrange(RectangleF finalRect)
        {
        }

        /// <summary>
        /// Init <seealso cref="NativeUIElement"/>
        /// </summary>
        protected internal virtual void NativeInit()
        {
        }
    }
}