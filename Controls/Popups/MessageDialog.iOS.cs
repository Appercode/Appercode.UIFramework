using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace Appercode.UI.Controls.Popups
{
    public sealed partial class MessageDialog : UIAlertViewDelegate
    {
        // prevent GC from collecting this Dialog
        private static List<MessageDialog> shownDialogs = new List<MessageDialog>();

        private void NativeShowAsync()
        {
            var buttons = new List<UICommand>(this.Commands);
            buttons.RemoveAt((int)this.CancelCommandIndex);
            this.InvokeOnMainThread(() =>
            {
                UIAlertView alert = new UIAlertView(this.Title ?? string.Empty, this.Content, this, this.Commands[(int)this.CancelCommandIndex].Label, buttons.Any() ? buttons.Select(o => o.Label).ToArray() : null);
                alert.Show();
                shownDialogs.Add(this);
            });
        }

        public override void Canceled(UIAlertView alertView)
        {
            var action = this.Commands[(int)this.CancelCommandIndex].Action;
            if (action != null)
            {
                action(this.Commands[(int)this.CancelCommandIndex]);
            }
        }

        public override void Clicked(UIAlertView alertview, nint buttonIndex)
        {
            var command = this.Commands[(int)buttonIndex];
            var action = command.Action;
            if (action != null)
            {
                action(command);
            }
        }

        public override void Dismissed(UIAlertView alertView, nint buttonIndex)
        {
            alertView.Delegate = null;
            shownDialogs.Remove(this);
        }

        public override void Presented(UIAlertView alertView)
        {
        }

        public override bool ShouldEnableFirstOtherButton(UIAlertView alertView)
        {
            return true;
        }

        public override void WillDismiss(UIAlertView alertView, nint buttonIndex)
        {
        }

        public override void WillPresent(UIAlertView alertView)
        {
        }
    }
}