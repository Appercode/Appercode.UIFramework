using Appercode.UI.Controls;
using System.Windows;

namespace Appercode.UI.StylesAndResources
{
    public sealed class SetterBaseCollection : PresentationFrameworkCollection<SetterBase>
    {
        public static readonly DependencyProperty IsSealedProperty =
            DependencyProperty.Register("IsSealed", typeof(bool), typeof(SetterBaseCollection), new PropertyMetadata(false));

        public override bool IsSealed
        {
            get { return (bool)this.GetValue(IsSealedProperty); }
        }
    }
}
