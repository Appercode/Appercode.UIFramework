using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        internal RectangleF CurrentRect = new RectangleF();

        private static AppercodeVisualRoot instance;
        private AppercodePage child;
        private bool isRearrangeScheduled;

        private AppercodeVisualRoot()
        {
        }

        public static AppercodeVisualRoot Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AppercodeVisualRoot();
                }
                return instance;
            }
        }

        public AppercodePage Child
        {
            get
            {
                return this.child;
            }
            set
            {
                if (this.child == value)
                {
                    return;
                }
                if (this.child != null)
                {
                    LogicalTreeHelper.RemoveLogicalChild(this, this.child);
                    this.child.LayoutUpdated -= this.Child_LayoutUpdated;
                }

                this.child = value;
                if (this.child == null)
                    return;
                this.child.LayoutUpdated += this.Child_LayoutUpdated;
                LogicalTreeHelper.AddLogicalChild(this, this.child);

                // first of all apply values from style, arrange goes next 
                this.UpdateVisualTreeProperties();
                this.Arrange(this.CurrentRect);
            }
        }

        public override void Arrange(RectangleF finalRect)
        {
            this.CurrentRect = finalRect;

            if (!finalRect.IsEmpty && this.child != null)
            {
                this.Child.MeasureOverride(finalRect.Size);
                this.Child.Arrange(finalRect);
            }
        }

        internal void UpdateVisualTreeProperties()
        {
            this.UpdateProperties(this.child);
        }

        private void Child_LayoutUpdated(object sender, EventArgs e)
        {
            this.ScheduleReArrangeIfNeeded();
        }

        private void ScheduleReArrangeIfNeeded()
        {
            if (this.isRearrangeScheduled)
            {
                return;
            }

            this.isRearrangeScheduled = true;

            this.Dispatcher.BeginInvoke(
                delegate
                {
                    try
                    {
                        if (!this.CurrentRect.IsEmpty)
                        {
                            this.Arrange(this.CurrentRect);
                        }
                    }
                    finally
                    {
                        this.isRearrangeScheduled = false;
                    }
                });
        }

        private void UpdateProperties(UIElement child)
        {
            child.NativeInit();
            var children = LogicalTreeHelper.GetChildren(child);
            foreach (var item in children)
            {
                if (item is UIElement)
                {
                    this.UpdateProperties((UIElement)item);
                }
            }
            child.OnLoaded();
        }
    }
}