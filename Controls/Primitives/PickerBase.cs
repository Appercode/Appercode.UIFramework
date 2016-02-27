using Appercode.UI.Input;
using Appercode.UI.Internals;
using System;
using System.Windows;

#if __IOS__
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
#else
using System.Drawing;
using nfloat = System.Single;
#endif

namespace Appercode.UI.Controls.Primitives
{
    /// <summary>
    /// Base class for pickers
    /// </summary>
    public abstract partial class PickerBase : ItemsControl
    {
        public static readonly DependencyProperty PickerTitleProperty =
            DependencyProperty.Register("PickerTitle", typeof(string), typeof(PickerBase), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty OkButtonTitleProperty =
            DependencyProperty.Register("OkButtonTitle", typeof(string), typeof(PickerBase), new PropertyMetadata(
#if __ANDROID__ || WINDOWS_PHONE
                "Ok"
#else
                "Done"
#endif
        ));

        public static readonly DependencyProperty CancelButtonTitleProperty =
            DependencyProperty.Register("CancelButtonTitle", typeof(string), typeof(PickerBase), new PropertyMetadata("Cancel"));

        public static readonly DependencyProperty HideSystemButtonsProperty =
            DependencyProperty.Register("HideSystemButtons", typeof(bool), typeof(PickerBase), new PropertyMetadata(false));

        public static readonly DependencyProperty PickerProperty =
            DependencyProperty.RegisterAttached("Picker", typeof(UIElement), typeof(PickerBase), new PropertyMetadata(OnPickerChanged));



        /// <summary>
        /// button that showning in layout and callind pickerViewe
        /// </summary>
        protected Button pickerButton;

        /// <summary>
        /// Initialize <see cref="pickerButton"/>
        /// </summary>
        public PickerBase()
        {
            this.pickerButton = new Button();
            this.pickerButton.Click += (s, e) => this.Show();
            this.AddLogicalChild(this.pickerButton);
        }

        /// <summary>
        /// Occures when user accept new value
        /// </summary>
        public event EventHandler PikerCompleted = delegate { };

        /// <summary>
        /// Title on the piker's popup
        /// </summary>
        public string PickerTitle
        {
            get { return (string)this.GetValue(PickerTitleProperty); }
            set { this.SetValue(PickerTitleProperty, value); }
        }

        /// <summary>
        /// Hide Ok and Cancel buttons
        /// </summary>
        public string OkButtonTitle
        {
            get { return (string)this.GetValue(OkButtonTitleProperty); }
            set { this.SetValue(OkButtonTitleProperty, value); }
        }

        /// <summary>
        /// Title of Ok button
        /// </summary>
        public bool HideSystemButtons
        {
            get { return (bool)this.GetValue(HideSystemButtonsProperty); }
            set { this.SetValue(HideSystemButtonsProperty, value); }
        }

        /// <summary>
        /// Title of cancel button
        /// </summary>
        public string CancelButtonTitle
        {
            get { return (string)this.GetValue(CancelButtonTitleProperty); }
            set { this.SetValue(CancelButtonTitleProperty, value); }
        }

        public static UIElement GetPicker(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(PickerProperty);
        }

        public static void SetPicker(DependencyObject obj, UIElement value)
        {
            obj.SetValue(PickerProperty, value);
        }

        /// <summary>
        /// Shows picker popup
        /// </summary>
        public void Show()
        {
            this.NativeShow();
        }

        /// <summary>
        /// Hides picker popup
        /// </summary>
        public void Dismiss()
        {
            this.NativeDismiss();
        }

        public override SizeF MeasureOverride(SizeF availableSize)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                this.measuredFor = availableSize;
                return this.measuredSize = SizeF.Empty;
            }

            // TODO: Size caching
            this.measuredFor = availableSize;
            var margin = this.Margin;
            availableSize = new SizeF(
                MathF.Max(0, availableSize.Width - margin.HorizontalThicknessF()),
                MathF.Max(0, availableSize.Height - margin.VerticalThicknessF()));

            if (this.ContainsValue(HeightProperty))
            {
                availableSize.Height = (nfloat)this.Height;
            }

            if (this.ContainsValue(WidthProperty))
            {
                availableSize.Width = (nfloat)this.Width;
            }

            this.measuredFor = availableSize;
            this.measuredSize = this.pickerButton.MeasureOverride(this.SizeThatFitsMaxAndMin(availableSize));

            this.measuredSize.Width += margin.HorizontalThicknessF();
            this.measuredSize.Height += margin.VerticalThicknessF();

            return this.measuredSize;
        }

        public override void Arrange(RectangleF finalRect)
        {
            this.arrangedSize = finalRect.Size;
            this.TranslatePoint = finalRect.Location;
            this.pickerButton.Arrange(finalRect);
        }

        protected void OnPickerCompleted()
        {
            this.PikerCompleted(this, EventArgs.Empty);
        }

        private static void OnPickerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;
            if (element != null)
            {
                var oldValue = e.OldValue as PickerBase;
                if (oldValue != null)
                {
                    element.Tap -= oldValue.ElementTapHandler;
                    element.DataContextChanged -= oldValue.ElementDataContextChanged;
                    element.RemoveLogicalChild(oldValue);
                }

                var newValue = e.NewValue as PickerBase;
                if (newValue != null)
                {
                    element.AddLogicalChild(newValue);
                    element.DataContextChanged += newValue.ElementDataContextChanged;
                    element.Tap += newValue.ElementTapHandler;
                }
            }
        }

        private void ElementDataContextChanged(object sender, DataContextChangedEventArgs e)
        {
            this.OnAncestorDataContextChanged(e);
        }

        private void ElementTapHandler(object sender, GestureEventArgs e)
        {
            this.Show();
        }
    }
}