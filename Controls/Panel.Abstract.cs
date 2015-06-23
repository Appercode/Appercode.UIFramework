using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Markup;

namespace Appercode.UI.Controls
{
    [ContentProperty("Children")]
    public abstract partial class Panel
    {
        private void NativeOnbackgroundChange()
        {
        }

        internal void AddNativeChildView(UIElement item)
        {
        }

        internal void RemoveNativeChildView(UIElement item)
        {
        }

        /// <summary>
        /// Arranges childs of panel in <paramref name="size"/>
        /// </summary>
        /// <param name="size"></param>
        protected virtual void ArrangeChilds(System.Drawing.SizeF size)
        {
        }

        /// <summary>
        /// Cache measured size of panel
        /// </summary>
        /// <param name="measuredSize"></param>
        protected void SetMeasuredSize(SizeF measuredSize)
        {
        }
    }
}