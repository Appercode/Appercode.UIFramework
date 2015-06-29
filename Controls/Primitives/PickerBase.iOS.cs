using CoreGraphics;
using System;
using UIKit;

namespace Appercode.UI.Controls.Primitives
{
    public abstract partial class PickerBase
    {
        public NativePickerContainer PickerContainer
        {
            get;
            protected set;
        }

        protected virtual void NativeShow()
        {
            this.PickerContainer.Title = this.PickerTitle;
            this.PickerContainer.Show();
        }

        protected virtual void NativeDismiss()
        {
            this.PickerContainer.Hide(true);
        }

        public class NativePickerContainer : UIViewController
        {
            private UIView targetView;
            private UIPopoverController popover;
            private UINavigationController navigation;
            private UIView fadeView = new UIView { BackgroundColor = UIColor.FromWhiteAlpha(0, 0.5f) };
            private UIView pickerView;
            private PickerBase owner;

            private CGSize navigationSize;

            /// <summary>
            /// PickerView Must have non-empty frame
            /// </summary>
            public NativePickerContainer(UIView targetView, UIView pickerView, PickerBase owner)
                : base()
            {
                this.owner = owner;
                this.targetView = targetView;
                this.pickerView = pickerView;
                var scroll = new UIScrollView(this.View.Frame);
                if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                {
                    scroll.AddSubview(new UIToolbar(this.View.Frame) { AutoresizingMask = UIViewAutoresizing.FlexibleDimensions });
                }
                scroll.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
                scroll.AddSubview(this.pickerView);

                this.View.AddSubview(scroll);

                this.navigation = new UINavigationController(this);
                int navBarVisibilityFactor = this.owner.HideSystemButtons ? 0 : 1;
                this.navigation.NavigationBarHidden = this.owner.HideSystemButtons;

                this.navigation.View.Frame = new CGRect(0, 0, this.pickerView.Frame.Width, this.pickerView.Frame.Height + navBarVisibilityFactor * this.navigation.NavigationBar.Frame.Height);
                this.navigationSize = this.navigation.View.Frame.Size;
                if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
                {
                    this.popover = new UIPopoverController(this.navigation);
                    this.popover.PopoverContentSize = this.navigation.View.Frame.Size;
                }
                else
                {
                    this.fadeView.AddSubview(this.navigation.View);
                }

                this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(owner.OkButtonTitle, UIBarButtonItemStyle.Done, (s, e) =>
                {
                    this.Hide(true);
                    if (this.DoneButtonAction != null)
                    {
                        this.DoneButtonAction();
                    }
                });
                this.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(owner.CancelButtonTitle, UIBarButtonItemStyle.Bordered, (s, e) =>
                {
                    this.Hide(true);
                });
            }

            public Action DoneButtonAction
            {
                get;
                set;
            }

            /// <summary>
            /// Shows the picker
            /// </summary>
            public void Show()
            {
                var window = UIApplication.SharedApplication.KeyWindow;
                if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
                {
                    this.popover.PresentFromRect(this.targetView.Superview.ConvertRectToView(this.targetView.Frame, window), window, UIPopoverArrowDirection.Any, true);
                }
                else
                {
                    var vc = window.RootViewController;

                    this.fadeView.Frame = vc.View.Frame;
                    if (this.fadeView.Superview != vc.View)
                    {
                        this.fadeView.RemoveFromSuperview();
                        vc.View.AddSubview(this.fadeView);
                    }
                    this.fadeView.Alpha = 0;
                    var f = this.navigation.View.Frame;
                    f.Y = this.fadeView.Frame.Height - this.navigationSize.Height;
                    f.Size = this.navigationSize;
                    this.navigation.View.Frame = f;
                    this.navigation.View.Transform = CGAffineTransform.MakeTranslation(0, f.Height);
                    UIView.Animate(0.3, () =>
                    {
                        this.fadeView.Alpha = 1;
                        this.navigation.View.Transform = CGAffineTransform.MakeIdentity();
                    });
                }
            }

            /// <summary>
            /// Dismisses the picker
            /// </summary>
            public void Hide(bool animated)
            {
                if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
                {
                    this.popover.Dismiss(animated);
                }
                else
                {
                    if (animated)
                    {
                        UIView.Animate(0.3, () =>
                        {
                            this.fadeView.Alpha = 0;
                            this.navigation.View.Transform = CGAffineTransform.MakeTranslation(0, this.navigation.View.Frame.Height);
                        }, () => this.navigation.View.Transform = CGAffineTransform.MakeIdentity());
                    }
                    else
                    {
                        this.fadeView.Alpha = 0;
                    }
                }
            }
        }
    }
}