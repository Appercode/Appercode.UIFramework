using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class VirtualizingStackPanel
    {
        private nfloat rowHeight = -1;

        private VirtualizingStackPanelSource TableViewSource 
        {
            get
            {
                return (VirtualizingStackPanelSource)((UITableView)this.NativeUIElement).Source;
            }
        }

        protected internal override void NativeInit()
        {
            if (this.Parent != null)
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new UITableView
                    {
                        AllowsSelection = false,
                        BackgroundColor = UIColor.Clear,
                        SeparatorStyle = UITableViewCellSeparatorStyle.None,
                        Source = this.CreateTableSource()
                    };
                }

                base.NativeInit();
            }
        }

        protected virtual VirtualizingStackPanelSource CreateTableSource()
        {
            return new VirtualizingStackPanelSource(this);
        }

        protected override CGSize NativeMeasureOverride(CGSize availableSize)
        {
            var height = base.NativeMeasureOverride(availableSize).Height;
            int i = 0;
            var avalibleItemSize = new CGSize(availableSize.Width, nfloat.PositiveInfinity);
            while (height < availableSize.Height && i < this.Generator.Collection.Count)
            {
                var measuredSize = this.TableViewSource.MeasureItemAt(i++, avalibleItemSize);
                height += measuredSize.Height;
            }
            return new CGSize(availableSize.Width, Math.Min(availableSize.Height, height));
        }

        private void ReloadNativeItems()
        {
            if (this.measuredSize.IsEmpty)
            {
                return;
            }

            if (rowHeight == -1 && this.Generator.Collection.Count > 0)
            {
                var listItem = (UIElement)this.Generator.Generate(0);
                LogicalTreeHelper.AddLogicalChild(this, listItem);

                var availableSize = this.Orientation == Orientation.Horizontal
                    ? new CGSize(nfloat.PositiveInfinity, this.measuredSize.Height)
                    : new CGSize(this.measuredSize.Width, nfloat.PositiveInfinity);

                rowHeight = listItem.MeasureOverride(availableSize).Height;
                ((UITableView)this.NativeUIElement).RowHeight = rowHeight;
            }

            UIView.AnimationsEnabled = false;
            ((UITableView)this.NativeUIElement).ReloadSections(NSIndexSet.FromIndex(0), UITableViewRowAnimation.None);
            UIView.AnimationsEnabled = true;
        }

        private bool NativeSetIsSelectedOnRealizedItem(int index, bool value)
        {
            return false;
        }

        partial void ApplyPadding(Thickness padding)
        {
            var insets = (UIEdgeInsets)padding;
            var tableView = (UITableView)this.NativeUIElement;
            tableView.ContentInset = insets;
            tableView.ScrollIndicatorInsets = insets;
        }

        protected class VirtualizingStackPanelSource : UITableViewSource
        {
            public Dictionary<UIView, UIElement> NativeViewContainers = new Dictionary<UIView, UIElement>();

            private static readonly NSString CellIdentifier = new NSString("cell");
            private readonly VirtualizingStackPanel panel;
            private UIElement measurementListItem = null;
            private List<UIElement> listItems = new List<UIElement>();

            public VirtualizingStackPanelSource(VirtualizingStackPanel panel)
            {
                this.panel = panel;
            }

            public override nint RowsInSection(UITableView tableView, nint section)
            {
                var count = this.panel.Generator != null ? this.panel.Generator.Collection.Count : 0;
                return count;
            }

            // float rowHeight = -1;

//            public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
//            {
////                if (indexPath.Row >= this.listItems.Count)
////                {
////                    var listItem = (UIElement)this.panel.Generator.Generate(indexPath.Row);
////                    LogicalTreeHelper.AddLogicalChild(this.panel, listItem);
////                
////                    var availableSize = this.panel.Orientation == Orientation.Horizontal
////                        ? new SizeF(float.PositiveInfinity, this.panel.measuredSize.Height)
////                        : new SizeF(this.panel.measuredSize.Width, float.PositiveInfinity);
////
////                    listItem.MeasureOverride(availableSize);
////                    this.listItems.Add(listItem);
////                }
////                else
////                {
////                    this.listItems[indexPath.Row] = this.panel.Generator.Reuse(indexPath.Row, this.listItems[indexPath.Row]);
////                }
////
////                return this.listItems[indexPath.Row].measuredSize.Height;
//
//                if(rowHeight == -1)
//                {
//                    var listItem = (UIElement)this.panel.Generator.Generate(indexPath.Row);
//                    LogicalTreeHelper.AddLogicalChild(this.panel, listItem);
//                
//                    var availableSize = this.panel.Orientation == Orientation.Horizontal
//                        ? new SizeF(float.PositiveInfinity, this.panel.measuredSize.Height)
//                        : new SizeF(this.panel.measuredSize.Width, float.PositiveInfinity);
//
//                    rowHeight = listItem.MeasureOverride(availableSize).Height;
//                    this.listItems.Add(listItem);
//                }
//
//                return rowHeight;
//            }

            public virtual CGSize MeasureItemAt(int index, CGSize availableSize)
            {
                if (measurementListItem == null)
                {
                    measurementListItem = (UIElement)this.panel.Generator.Generate(index);
                    LogicalTreeHelper.AddLogicalChild(this.panel, measurementListItem);
                }
                else
                {
                    measurementListItem = this.panel.Generator.Reuse(index, measurementListItem);
                }
                return measurementListItem.MeasureOverride(availableSize);
            }

            private UIElement CreateItemForRow(int row)
            {
                return ((UIElement)this.panel.Generator.Generate(row));
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                UIElement listItem = null;

//                if (indexPath.Row >= this.listItems.Count)
//                {
//                    listItem = (UIElement)this.panel.Generator.Generate(indexPath.Row);
//                    LogicalTreeHelper.AddLogicalChild(this.panel, listItem);
//
//                    var availableSize = this.panel.Orientation == Orientation.Horizontal
//                        ? new SizeF(float.PositiveInfinity, this.panel.measuredSize.Height)
//                        : new SizeF(this.panel.measuredSize.Width, float.PositiveInfinity);
//
//                    listItem.MeasureOverride(availableSize);
//                    this.listItems.Add(listItem);
//                }
//                else
//                {
//                    listItem = this.listItems[indexPath.Row];
//                }

                var cell = tableView.DequeueReusableCell(CellIdentifier);
                if (cell == null)
                {
                    int ind = Array.IndexOf<NSIndexPath>(tableView.IndexPathsForVisibleRows, indexPath);
                    if (ind >= 0 && ind < tableView.VisibleCells.Count())
                    {
                        cell = tableView.VisibleCells[ind];
                        cell.RemoveFromSuperview();
                    }
                    else
                    {
                        cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);

                        listItem = this.CreateItemForRow(indexPath.Row);
                        LogicalTreeHelper.AddLogicalChild(this.panel, listItem);
                        this.NativeViewContainers.Add(listItem.NativeUIElement, listItem);
                        cell.ContentView.AddSubview(listItem.NativeUIElement);
                        var availableSize = this.panel.Orientation == Orientation.Horizontal
                            ? new CGSize(nfloat.PositiveInfinity, this.panel.measuredSize.Height)
                            : new CGSize(this.panel.measuredSize.Width, nfloat.PositiveInfinity);

                        listItem.MeasureOverride(availableSize);
                        listItem.Arrange(new CGRect(CGPoint.Empty, listItem.measuredSize));
                        listItem.LayoutUpdated += this.panel.Child_LayoutUpdated;

                        cell.BackgroundColor = UIColor.Clear;

                        return cell;
                    }
                }
                    var nativeItem = cell.ContentView.Subviews[0];
                    if (nativeItem != null && this.NativeViewContainers.ContainsKey(nativeItem))
                    {
                        listItem = this.NativeViewContainers[nativeItem];
                        this.panel.Generator.Reuse(indexPath.Row, listItem);
                    }
                    else
                    {
                        nativeItem.RemoveFromSuperview();
                        listItem = this.CreateItemForRow(indexPath.Row);
                        LogicalTreeHelper.AddLogicalChild(this.panel, listItem);
                        this.NativeViewContainers.Add(listItem.NativeUIElement, listItem);
                        cell.AddSubview(listItem.NativeUIElement);
                    }
//                var availableSize = this.panel.Orientation == Orientation.Horizontal
//                    ? new SizeF(float.PositiveInfinity, this.panel.measuredSize.Height)
//                    : new SizeF(this.panel.measuredSize.Width, float.PositiveInfinity);
//                    
//                this.itemSizes[indexPath.Row] = listItem.MeasureOverride(availableSize);

//                if(cell.ContentView.Subviews.Length > 0)
//                {
//                    cell.ContentView.Subviews[0].RemoveFromSuperview();
//                }

//                cell.ContentView.AddSubview(listItem.NativeUIElement);

//                listItem.Arrange(new RectangleF(PointF.Empty, listItem.measuredSize));

                return cell;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                tableView.DeselectRow(indexPath, true);
            }
        }
    }
}