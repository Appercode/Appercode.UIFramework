using System.Windows;

namespace Appercode.UI.StylesAndResources
{
    public sealed partial class Setter : SetterBase
    {
        private DependencyProperty property;
        private object value;

        public Setter()
        {
        }

        public Setter(DependencyProperty property, object value) : this()
        {
            this.property = property;
            this.value = value;
        }
        
        public DependencyProperty Property
        {
            get { return this.property; }
            set { this.property = value; }
        }

        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}
