using System;
using System.Windows.Markup;

namespace Appercode.UI.Controls
{
    [ContentProperty("PrimaryCommands")]
    public partial class CommandBar : IAddChild
    {
        void IAddChild.AddChild(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var barElement = value as ICommandBarElement;
            if (barElement == null)
            {
                throw new ArgumentException(string.Format("Cannot add child of type {0}.", value.GetType()));
            }

            this.PrimaryCommands.Add(barElement);
        }

        void IAddChild.AddText(string text)
        {
            throw new NotSupportedException();
        }
    }
}