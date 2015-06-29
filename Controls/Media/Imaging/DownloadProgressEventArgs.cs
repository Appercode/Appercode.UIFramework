using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Appercode.UI.Controls.Media.Imaging
{
    public class DownloadProgressEventArgs : EventArgs
    {
        internal DownloadProgressEventArgs()
        {
        }

        internal DownloadProgressEventArgs(int progress)
        {
            this.Progress = progress;
        }

        public int Progress
        {
            get;
            internal set;
        }
    }
}
