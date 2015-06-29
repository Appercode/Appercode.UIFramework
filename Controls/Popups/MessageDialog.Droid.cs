using Android.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Controls.Popups
{
    public sealed partial class MessageDialog
    {
        private void NativeShowAsync()
        {
            AlertDialog.Builder dialog = new AlertDialog.Builder(UIElement.StaticContext);

            if (this.Content != null)
            {
                dialog.SetMessage(this.Content);
            }

            if (this.Title != null)
            {
                dialog.SetTitle(this.Title);
            }

            dialog.SetIcon(Android.Resource.Drawable.IcDialogAlert);

            if(this.Commands == null || this.Commands.Count() < 1)
            {
                return;
            }

            if (this.Commands.Count() > 0)
            {
                dialog.SetNeutralButton(this.Commands[0].Label, (sender, e) =>
                {
                    if (this.Commands[0].Action != null)
                    {
                        this.Commands[0].Action(this.Commands[0]);
                    }
                });
            }

            if (this.Commands.Count() > 1)
            {
                dialog.SetPositiveButton(this.Commands[1].Label, (sender, e) =>
                {
                    if (this.Commands[1].Action != null)
                    {
                        this.Commands[1].Action(this.Commands[1]);
                    }
                });
            }

            if (this.Commands.Count() > 2)
            {
                dialog.SetNegativeButton(this.Commands[2].Label, (sender, e) =>
                {
                    if (this.Commands[2].Action != null)
                    {
                        this.Commands[2].Action(this.Commands[2]);
                    }
                });
            }
            UIElement.StaticContext.RunOnUiThread( () => dialog.Show());
        }
    }
}