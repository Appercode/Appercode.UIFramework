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
            var tableViewSource = (VirtualizingStackPanelSource)((UITableView)this.NativeUIElement).Source;
            return tableViewSource.MeasureAllItems(base.NativeMeasureOverride(availableSize), availableSize);
        }

        private void ReloadNativeItems()
        {
            if (this.measuredSize.IsEmpty)
            {
                return;
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
            private static readonly NSString CellIdentifier = new NSString("cell");

            private readonly Lazy<Dictionary<DataTemplate, NSString>> cellIdentifiers;
            private readonly Lazy<Dictionary<DataTemplate, DependencyObject>> measurementContainers;
            private readonly Dictionary<UIView, UIElement> nativeViewContainers;
            private readonly VirtualizingStackPanel panel;
            private DependencyObject defaultContainer;

            public VirtualizingStackPanelSource(VirtualizingStackPanel panel)
            {
                this.cellIdentifiers = new Lazy<Dictionary<DataTemplate, NSString>>();
                this.measurementContainers = new Lazy<Dictionary<DataTemplate, DependencyObject>>();
                this.nativeViewContainers = new Dictionary<UIView, UIElement>();
                this.panel = panel;
            }

            public virtual CGSize MeasureItemAt(int index, CGSize availableSize)
            {
                var measurementContainer = (UIElement)this.GetMeasurementContainer(index);
                return measurementContainer.MeasureOverride(availableSize);
            }

            public override nint RowsInSection(UITableView tableView, nint section)
            {
                return this.panel.Generator.Items.Count;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                // TODO: Add support of Horizontal orientation
                var availableSize = this.GetItemAvailableSize(this.panel.measuredSize);
                return this.MeasureItemAt(indexPath.Row, availableSize).Height;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cellIdentifier = this.GetCellIdentifier(indexPath.Row);
                var cell = tableView.DequeueReusableCell(cellIdentifier);
                if (cell == null)
                {
                    var visibleRowIndex = Array.IndexOf(tableView.IndexPathsForVisibleRows, indexPath);
                    if (visibleRowIndex >= 0 && visibleRowIndex < tableView.VisibleCells.Length)
                    {
                        cell = tableView.VisibleCells[visibleRowIndex];
                        cell.RemoveFromSuperview();
                    }
                    else
                    {
                        cell = new UITableViewCell(UITableViewCellStyle.Default, cellIdentifier)
                        {
                            BackgroundColor = UIColor.Clear
                        };
                    }
                }

                return cell;
            }

            public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
            {
                UIElement container = null;
                var nativeView = cell.ContentView.Subviews.FirstOrDefault();
                if (nativeView != null)
                {
                    if (this.nativeViewContainers.TryGetValue(nativeView, out container) == false)
                    {
                        nativeView.RemoveFromSuperview();
                    }
                }

                if (container == null)
                {
                    container = (UIElement)this.panel.Generator.ContainerFromIndex(indexPath.Row);
                    this.panel.AddLogicalChild(container);
                    this.nativeViewContainers.Add(container.NativeUIElement, container);
                    cell.ContentView.AddSubview(container.NativeUIElement);
                }
                else
                {
                    this.panel.Generator.Reuse(indexPath.Row, container);
                }

                container.MeasureOverride(this.GetItemAvailableSize(this.panel.measuredSize));
                container.Arrange(new CGRect(CGPoint.Empty, container.measuredSize));
                container.LayoutUpdated += this.panel.Child_LayoutUpdated;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                tableView.DeselectRow(indexPath, true);
            }

            internal CGSize MeasureAllItems(CGSize baseSize, CGSize availableSize)
            {
                // TODO: Add support of Horizontal orientation
                var height = baseSize.Height;
                var itemAvailableSize = this.GetItemAvailableSize(availableSize);
                var index = 0;
                while (height < availableSize.Height && index < this.panel.Generator.Items.Count)
                {
                    height += this.MeasureItemAt(index++, itemAvailableSize).Height;
                }

                return new CGSize(availableSize.Width, NMath.Min(availableSize.Height, height));
            }

            private NSString GetCellIdentifier(int index)
            {
                var itemTemplateSelector = this.panel.Generator.ItemTemplateSelector;
                if (itemTemplateSelector == null)
                {
                    return CellIdentifier; 
                }

                var container = this.defaultContainer ?? this.InitDefaultContainer(index);
                var dataTemplate = itemTemplateSelector.SelectTemplate(this.panel.Generator.Items[index], container);
                NSString identifier;
                if (this.cellIdentifiers.Value.TryGetValue(dataTemplate, out identifier) == false)
                {
                    identifier = new NSString("cell" + dataTemplate.GetHashCode());
                    this.cellIdentifiers.Value.Add(dataTemplate, identifier);
                }

                return identifier;
            }

            private CGSize GetItemAvailableSize(CGSize panelAvailableSize)
            {
                return this.panel.Orientation == Orientation.Horizontal
                    ? new CGSize(nfloat.PositiveInfinity, panelAvailableSize.Height)
                    : new CGSize(panelAvailableSize.Width, nfloat.PositiveInfinity);
            }

            private DependencyObject GetMeasurementContainer(int index)
            {
                var itemTemplateSelector = this.panel.Generator.ItemTemplateSelector;
                if (itemTemplateSelector == null)
                {
                    return this.InitDefaultContainer(index);
                }

                var defaultContainerIsCreatedForCurrentIndex = false;
                if (this.defaultContainer == null)
                {
                    this.InitDefaultContainer(index);
                    defaultContainerIsCreatedForCurrentIndex = true;
                }

                var dataTemplate = itemTemplateSelector.SelectTemplate(this.panel.Generator.Items[index], this.defaultContainer);
                DependencyObject measurementContainer;
                if (this.measurementContainers.Value.TryGetValue(dataTemplate, out measurementContainer) == false)
                {
                    if (defaultContainerIsCreatedForCurrentIndex)
                    {
                        measurementContainer = this.defaultContainer;
                    }
                    else
                    {
                        measurementContainer = this.panel.Generator.ContainerFromIndex(index);
                        this.panel.AddLogicalChild(measurementContainer);
                    }

                    this.measurementContainers.Value.Add(dataTemplate, measurementContainer);
                }

                return measurementContainer;
            }

            // This methods calls Reuse unconditionally, which might lead to data template reaplying.
            // Do not use it if the defaultContainer is already initialized and the data template is calculated with a DataTemplateSelector.
            private DependencyObject InitDefaultContainer(int index)
            {
                var containerIsNew = this.defaultContainer == null;
                this.defaultContainer = this.panel.Generator.Reuse(index, this.defaultContainer);
                if (containerIsNew)
                {
                    this.panel.AddLogicalChild(this.defaultContainer);
                }

                return this.defaultContainer;
            }
        }
    }
}