using System;

#if __IOS__
using RectangleF = CoreGraphics.CGRect;
#else
using System.Drawing;
#endif

namespace Appercode.UI.Controls
{
    /// <summary>
    /// Internal class for VisualRoot (AppercodePage Parent)
    /// </summary>
    internal class AppercodeVisualRoot : UIElement
    {
        private AppercodePage child;
        private bool isRearrangeScheduled;

        private AppercodeVisualRoot()
        {
        }

        public static AppercodeVisualRoot Instance { get; } = new AppercodeVisualRoot();

        public AppercodePage Child
        {
            get
            {
                return this.child;
            }
            set
            {
                if (this.child != value)
                {
                    if (this.child != null)
                    {
                        this.RemoveLogicalChild(this.child);
                        this.child.LayoutUpdated -= this.OnChildLayoutUpdated;
                    }

                    this.child = value;
                    if (value != null)
                    {
                        value.LayoutUpdated += this.OnChildLayoutUpdated;
                        this.AddLogicalChild(value);

                        // first of all apply values from style, arrange should be called after that
                        UpdateProperties(this.child);
                    }
                }
            }
        }

        internal RectangleF CurrentRect { get; private set; }

        public override void Arrange(RectangleF finalRect)
        {
            this.CurrentRect = finalRect;

            if (!finalRect.IsEmpty && this.child != null)
            {
                this.child.MeasureOverride(finalRect.Size);
                this.child.Arrange(finalRect);
            }
        }

        internal void Arrange()
        {
            this.Arrange(this.CurrentRect);
        }

        private static void UpdateProperties(UIElement child)
        {
            child.NativeInit();
            var children = LogicalTreeHelper.GetChildren(child);
            foreach (var item in children)
            {
                var childElement = item as UIElement;
                if (childElement != null)
                {
                    UpdateProperties(childElement);
                }
            }

            child.OnLoaded();
        }

        private void OnChildLayoutUpdated(object sender, EventArgs e)
        {
            if (this.isRearrangeScheduled == false)
            {
                this.isRearrangeScheduled = true;
                this.Dispatcher.BeginInvoke(this.Rearrange);
            }
        }

        private void Rearrange()
        {
            try
            {
                if (this.CurrentRect.IsEmpty == false)
                {
                    this.Arrange();
                }
            }
            finally
            {
                this.isRearrangeScheduled = false;
            }
        }
    }
}