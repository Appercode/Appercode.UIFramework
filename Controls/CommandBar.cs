using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace Appercode.UI.Controls
{
    public partial class CommandBar : Control
    {
        public static readonly DependencyProperty PrimaryCommandsProperty =
            DependencyProperty.Register("PrimaryCommands", typeof(CommandBarElementCollection), typeof(CommandBar));

        public CommandBar()
        {
            this.PrimaryCommands = new CommandBarElementCollection();
            this.PrimaryCommands.CollectionChanged += this.OnPrimaryCommandsCollectionChanged;
        }

        internal event EventHandler BarUpdated;

        public CommandBarElementCollection PrimaryCommands
        {
            get { return (CommandBarElementCollection)GetValue(PrimaryCommandsProperty); }
            private set { this.SetValue(PrimaryCommandsProperty, value); }
        }

        protected internal override IEnumerator LogicalChildren
        {
            get { return this.PrimaryCommands.GetEnumerator(); }
        }

        private void OnPrimaryCommandsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var oldItem in this.PrimaryCommands)
                {
                    oldItem.VisibilityChanged -= this.BarElementVisibilityChanged;
                    this.RemoveLogicalChild(oldItem);
                }
            }

            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems.OfType<ICommandBarElement>())
                {
                    oldItem.VisibilityChanged -= this.BarElementVisibilityChanged;
                    this.RemoveLogicalChild(oldItem);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems.OfType<ICommandBarElement>())
                {
                    newItem.VisibilityChanged += this.BarElementVisibilityChanged;
                    this.AddLogicalChild(newItem);
                }
            }

            this.OnBarUpdated();
        }

        private void BarElementVisibilityChanged(object sender, EventArgs e)
        {
            this.OnBarUpdated();
        }

        private void OnBarUpdated()
        {
            if (this.BarUpdated != null)
            {
                this.BarUpdated(this, EventArgs.Empty);
            }
        }
    }
}