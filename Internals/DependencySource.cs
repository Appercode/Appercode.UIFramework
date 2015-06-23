using System.Windows;

namespace Appercode.UI.Internals
{
    internal sealed class DependencySource
    {
        private DependencyObject dependencyObject;

        private DependencyProperty dp;

        public DependencySource(DependencyObject d, DependencyProperty dp)
        {
            this.dependencyObject = d;
            this.dp = dp;
        }

        public DependencyObject DependencyObject
        {
            get
            {
                return this.dependencyObject;
            }
        }

        public DependencyProperty DependencyProperty
        {
            get
            {
                return this.dp;
            }
        }
    }
}
