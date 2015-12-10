using Appercode.UI.Controls.Primitives;
using System.Collections;
using System.Windows;

#if __IOS__
using RectangleF = CoreGraphics.CGRect;
#else
using System.Drawing;
#endif

namespace Appercode.UI.Controls
{
    public partial class PivotVirtualizingPanel : Panel, IVirtualizingPanel, IPivotItemProvider
    {
        #region Events

        public event SelectionChangedEventHandler SelectionChanged = delegate { };

        #endregion

        public ItemContainerGenerator Generator { get; set; }

        public ScrollViewer ScrollOwner { get; set; }

        public bool HasNativeScroll
        {
            get
            {
                return false;
            }
        }

        public IList DataItems { get; set; }

        public int Count
        {
            get
            {
                if (this.DataItems == null)
                {
                    return 0;
                }

                return this.DataItems.Count;
            }
        }

        public bool SetIsSelectedOnRealizedItem(int index, bool value)
        {
            return false;
        }

        public override void Arrange(RectangleF finalRect)
        {
            base.Arrange(finalRect);
            this.ArrangeCurrentlyInstantiatedElements();
            this.RefreshNativeAdapter();
        }

        UIElement IPivotItemProvider.CreateItemElement(int position)
        {
            var element = (UIElement)this.Generator.Generate(position);
            this.AddLogicalChild(element);
            this.ArrangeItemAtPosition(element, position);
            return element;
        }

        public object GetHeader(int position)
        {
            var res = this.Generator.Generate(position) as PivotItem;

            // TODO: Prevent duplicated instances
            return res == null ? null : res.Header;
        }

        public void SetPadding(Thickness padding)
        {
        }
    }
}