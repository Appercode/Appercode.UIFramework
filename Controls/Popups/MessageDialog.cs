using System.Collections.Generic;

namespace Appercode.UI.Controls.Popups
{
    public sealed partial class MessageDialog
    {
        public MessageDialog(string content)
            : this(content, null)
        {
        }

        public MessageDialog(string content, string title)
        {
            this.Content = content;
            this.Title = title;
            this.Commands = new List<UICommand>();
        }

        public uint CancelCommandIndex { get; set; }

        public IList<UICommand> Commands { get; private set; }

        public string Content { get; set; }

        public uint DefaultCommandIndex { get; set; }

        public string Title { get; set; }

        public void ShowAsync()
        {
            this.NativeShowAsync();
        }
    }
}