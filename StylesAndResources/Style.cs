using System;
using System.Windows;

namespace Appercode.UI.StylesAndResources
{
    public partial class Style : DependencyObject
    {
        private readonly SetterBaseCollection setters;
        private Type targetType;
        private Style basedOn;

        public Style()
        {
            this.setters = new SetterBaseCollection();
        }

        public Style(Type targetType) : this()
        {
            this.targetType = targetType;
        }

        public Type TargetType
        {
            get { return this.targetType; }
            set { this.targetType = value; }
        }

        public Style BasedOn
        {
            get { return this.basedOn; }
            set { this.basedOn = value; }
        }

        public sealed override bool IsSealed
        {
            get { return false; }
        }

        public SetterBaseCollection Setters
        {
            get { return this.setters; }
        }

        public object FindValue(DependencyProperty property)
        {
            foreach (var setterBase in this.setters)
            {
                var setter = setterBase as Setter;
                if (setter.Property == property)
                {
                    return setter.Value;
                }
            }

            if (this.basedOn != null)
            {
                return this.basedOn.FindValue(property);
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
