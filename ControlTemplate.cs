using Appercode.UI.Controls;
using System;
using System.Windows;

namespace Appercode.UI
{
    /// <summary>
    /// Specifies the visual structure of a Control that can be shared across multiple instances of the control.
    /// </summary>
    public class ControlTemplate : FrameworkTemplate
    {
        public static readonly DependencyProperty TargetTypeProperty =
            DependencyProperty.Register("TargetType", typeof(Type), typeof(ControlTemplate), new PropertyMetadata(typeof(Type)));

        public Type TargetType
        {
            get { return (Type)this.GetValue(TargetTypeProperty); }
            set { this.SetValue(TargetTypeProperty, value); }
        }

        public override DependencyObject LoadContent()
        {
            var content = base.LoadContent();
            var uiElement = content as UIElement;
            if (uiElement != null)
            {
                uiElement.ParentChanged += this.ContentParentChanged;
            }

            return content;
        }

        private void ContentParentChanged(object sender, EventArgs e)
        {
            var uiElement = sender as UIElement;
            if (uiElement != null && uiElement.Parent != null)
            {
                this.VisualTree.SetTemplateBindings(uiElement.Parent);
            }
        }
    }
}