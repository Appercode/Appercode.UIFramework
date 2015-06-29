using CoreGraphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial interface IPivotHeaderControl
    {
        void SetVirtualizingPanel(PivotVirtualizingPanel virtualizingPanel);
        void FreeVirtualizingPanel();
    }

    public partial class PivotHeadersControl
    {
        private PivotVirtualizingPanel virtualizingPanel;

        public void SetVirtualizingPanel(PivotVirtualizingPanel virtualizingPanel)
        {
            if (this.virtualizingPanel != virtualizingPanel)
            {
                if (this.virtualizingPanel != null)
                {
                    this.virtualizingPanel.CurrentPageChanged -= this.PanelCurrentPageChanged;
                }

                this.virtualizingPanel = virtualizingPanel;
                if (this.virtualizingPanel != null)
                {
                    this.virtualizingPanel.CurrentPageChanged += this.PanelCurrentPageChanged;
                }
            }
        }

        public void FreeVirtualizingPanel()
        {
            if (this.virtualizingPanel != null)
            {
                this.virtualizingPanel.CurrentPageChanged -= this.PanelCurrentPageChanged;
            }

            this.virtualizingPanel = null;
        }

        protected override void ApplySelectedIndex(int newSelectedIndex, int oldSelectedIndex)
        {
            base.ApplySelectedIndex(newSelectedIndex, oldSelectedIndex);

            if (this.virtualizingPanel != null)
            {
                if (this.virtualizingPanel.CurrentPage != this.SelectedIndex)
                {
                    this.virtualizingPanel.CurrentPage = newSelectedIndex;
                }
            }
        }

        private void PanelCurrentPageChanged(object sender, EventArgs e)
        {
            this.SelectedIndex = this.virtualizingPanel.CurrentPage;
        }
    }

    public partial class CirclePivotHeaderControl
    {
        private IEnumerable itemSource;
        private PivotVirtualizingPanel virtualizingPanel;

        public IEnumerable ItemsSource
        {
            get
            {
                return this.itemSource;
            }
            set
            {
                if (this.itemSource != value)
                {
                    this.itemSource = value;
                    var pageControl = this.NativeUIElement as UIPageControl;
                    if (pageControl != null)
                    {
                        pageControl.Pages = ((IEnumerable<object>)this.itemSource).Count();
                        this.InvalidateMeasure();
                    }
                }
            }
        }

        public int SelectedIndex
        {
            get
            {
                var pageControl = this.NativeUIElement as UIPageControl;
                return pageControl == null ? 0 : (int)pageControl.CurrentPage;
            }
            set
            {
                var pageControl = this.NativeUIElement as UIPageControl;
                if (pageControl != null)
                {
                    pageControl.CurrentPage = value;
                }
            }
        }

        public void SetVirtualizingPanel(PivotVirtualizingPanel virtualizingPanel)
        {
            if (this.virtualizingPanel != virtualizingPanel)
            {
                if (this.virtualizingPanel != null)
                {
                    this.virtualizingPanel.CurrentPageChanged -= this.PanelCurrentPageChanged;
                }

                this.virtualizingPanel = virtualizingPanel;
                if (this.virtualizingPanel != null)
                {
                    this.virtualizingPanel.CurrentPageChanged += this.PanelCurrentPageChanged;
                }
            }
        }

        public void FreeVirtualizingPanel()
        {
            if (this.virtualizingPanel != null)
            {
                this.virtualizingPanel.CurrentPageChanged -= this.PanelCurrentPageChanged;
            }

            this.virtualizingPanel = null;
        }

        protected internal override void NativeInit()
        {
            if (this.Parent != null)
            {
                if (this.NativeUIElement == null)
                {
                    var pageControl = new UIPageControl();
                    pageControl.HidesForSinglePage = true;
                    pageControl.CurrentPageIndicatorTintColor = (Color)SelectedCircleColorProperty.DefaultMetadata.DefaultValue;
                    pageControl.PageIndicatorTintColor = (Color)CircleColorProperty.DefaultMetadata.DefaultValue;
                    pageControl.ValueChanged += this.PageControlValueChanged;
                    this.NativeUIElement = pageControl;
                }
            }

            base.NativeInit();
        }

        protected override CGSize NativeMeasureOverride(CGSize availableSize)
        {
            var result = base.NativeMeasureOverride(availableSize);
            var pageControl = this.NativeUIElement as UIPageControl;
            if (pageControl != null)
            {
                var nativeSize = pageControl.SizeForNumberOfPages(pageControl.Pages);
                result.Height = MathF.Max(result.Height, nativeSize.Height);
                result.Width = MathF.Max(result.Width, nativeSize.Width);
            }

            return result;
        }

        partial void OnColorChanged(DependencyPropertyChangedEventArgs e)
        {
            var pageControl = this.NativeUIElement as UIPageControl;
            if (pageControl != null && e.NewValue is Color)
            {
                var color = (Color)e.NewValue;
                if (e.Property == CircleColorProperty)
                {
                    pageControl.PageIndicatorTintColor = color;
                }
                else if (e.Property == SelectedCircleColorProperty)
                {
                    pageControl.CurrentPageIndicatorTintColor = color;
                }
            }
        }

        private void PanelCurrentPageChanged(object sender, EventArgs e)
        {
            this.SelectedIndex = this.virtualizingPanel.CurrentPage;
        }

        private void PageControlValueChanged(object sender, EventArgs e)
        {
            var pageControl = sender as UIPageControl;
            if (pageControl != null && this.virtualizingPanel != null)
            {
                this.virtualizingPanel.CurrentPage = (int)pageControl.CurrentPage;
            }
        }
    }
}