using Appercode.UI.Markup;
using System;
using System.ComponentModel;
using System.Windows;

namespace Appercode.UI.Data
{
    public abstract class BindingBase
    {
        internal bool IsSealed = false;
        private object fallbackValue = null;
        private object targetNullValue;
        private string stringFormat;

        /// <summary>Gets or sets the value to use when the binding is unable to return a value.</summary>
        /// <returns>The default value is <see cref="F:System.Windows.DependencyProperty.UnsetValue" />.</returns>
        public object FallbackValue
        {
            get
            {
                return this.fallbackValue;
            }
            set
            {
                this.CheckSealed();
                this.fallbackValue = value;
            }
        }

        /// <summary>Gets or sets a string that specifies how to format the binding if it displays the bound value as a string.</summary>
        /// <returns>A string that specifies how to format the binding if it displays the bound value as a string.</returns>
        [DefaultValue(null)]
        public string StringFormat
        {
            get
            {
                return this.stringFormat;
            }
            set
            {
                this.CheckSealed();
                this.stringFormat = value;
            }
        }

        /// <summary>Gets or sets the value that is used in the target when the value of the source is null.</summary>
        /// <returns>The value that is used in the target when the value of the source is null.</returns>
        public object TargetNullValue
        {
            get
            {
                return this.targetNullValue;
            }
            set
            {
                this.CheckSealed();
                this.targetNullValue = value;
            }
        }

        protected internal void CheckSealed()
        {
            if (this.IsSealed)
            {
                throw new InvalidOperationException("Binding can't be changed aftet sealed");
            }
        }
    }
}
