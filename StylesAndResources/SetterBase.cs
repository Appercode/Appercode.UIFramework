using System.Windows;

namespace Appercode.UI.StylesAndResources
{
    public abstract class SetterBase : DependencyObject
    {
        public static readonly DependencyProperty IsSealedProperty =
            DependencyProperty.Register("IsSealed", typeof(bool), typeof(SetterBase), new PropertyMetadata(false));

        public override bool IsSealed
        {
            get { return (bool)this.GetValue(IsSealedProperty); }
        }
    }
}
