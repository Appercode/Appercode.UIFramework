using System;
using System.Globalization;

namespace Appercode.UI.Data
{
    public class Binding : BindingBase
    {
        private PropertyPath path;
        private object source;
        private BindingSource currentSource;
        private BindingMode mode = BindingMode.OneWay;
        private bool bindsDirectlyToSource;
        private IValueConverter converter;
        private CultureInfo converterCulture;
        private object converterParameter;
        private string elementName;
        private bool validatesOnDataErrors;
        private bool validatesOnExceptions;
        private bool validatesOnNotifyDataErrors;
        private Data.RelativeSource relativeSource;
        private bool notifyOnValidationError;
        private Data.UpdateSourceTrigger updateSourceTrigger;

        public Binding(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            this.Path = new PropertyPath(path, new object[0]);
        }

        public Binding()
        {
            this.Path = new PropertyPath("", new object[0]);
        }

        internal enum BindingSource
        {
            None,
            Source,
            ElementName,
            RelativeSource
        }

        public bool BindsDirectlyToSource
        {
            get
            {
                return this.bindsDirectlyToSource;
            }
            set
            {
                this.CheckSealed();
                this.bindsDirectlyToSource = value;
            }
        }

        public IValueConverter Converter
        {
            get
            {
                return this.converter;
            }
            set
            {
                this.CheckSealed();
                this.converter = value;
            }
        }

        public CultureInfo ConverterCulture
        {
            get
            {
                return this.converterCulture;
            }
            set
            {
                this.CheckSealed();
                this.converterCulture = value;
            }
        }

        public object ConverterParameter
        {
            get
            {
                return this.converterParameter;
            }
            set
            {
                this.CheckSealed();
                this.converterParameter = value;
            }
        }

        public string ElementName
        {
            get
            {
                return this.elementName;
            }
            set
            {
                this.CheckSealed();
                if (this.currentSource != BindingSource.None && this.currentSource != BindingSource.ElementName)
                {
                    string str = string.Format("BindingSource conflict: {0} {1}", BindingSource.RelativeSource, this.currentSource);
                    throw new InvalidOperationException(str);
                }
                this.elementName = value;
                this.currentSource = BindingSource.ElementName;
            }
        }

        public PropertyPath Path
        {
            get
            {
                return this.path;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Path null");
                }
                this.CheckSealed();
                value.ParsePathInternal(false);
                this.path = value;
            }
        }

        public UpdateSourceTrigger UpdateSourceTrigger
        {
            get
            {
                return this.updateSourceTrigger;
            }
            set
            {
                this.CheckSealed();
                this.updateSourceTrigger = value;
            }
        }

        public bool ValidatesOnDataErrors
        {
            get
            {
                return this.validatesOnDataErrors;
            }
            set
            {
                this.CheckSealed();
                this.validatesOnDataErrors = value;
            }
        }

        public bool ValidatesOnExceptions
        {
            get
            {
                return this.validatesOnExceptions;
            }
            set
            {
                this.CheckSealed();
                this.validatesOnExceptions = value;
            }
        }

        public bool ValidatesOnNotifyDataErrors
        {
            get
            {
                return this.validatesOnNotifyDataErrors;
            }
            set
            {
                this.CheckSealed();
                this.validatesOnNotifyDataErrors = value;
            }
        }

        public object Source
        {
            get
            {
                return this.source;
            }
            set
            {
                this.CheckSealed();
                this.source = value;
                if (this.currentSource != BindingSource.None && this.currentSource != BindingSource.Source)
                {
                    string str = string.Format(string.Format("Source conflict: {0} {1}"), BindingSource.Source, this.currentSource);
                    throw new InvalidOperationException(str);
                }
                this.source = value;
                this.currentSource = BindingSource.Source;
            }
        }

        public BindingMode Mode
        {
            get
            {
                return this.mode;
            }
            set
            {
                this.CheckSealed();
                this.mode = value;
            }
        }

        public bool NotifyOnValidationError
        {
            get
            {
                return this.notifyOnValidationError;
            }
            set
            {
                this.CheckSealed();
                this.notifyOnValidationError = value;
            }
        }

        public RelativeSource RelativeSource
        {
            get
            {
                return this.relativeSource;
            }
            set
            {
                this.CheckSealed();
                if (this.currentSource != BindingSource.None && this.currentSource != BindingSource.RelativeSource)
                {
                    string str = string.Format(string.Format("Source conflict: {0} {1}"), BindingSource.Source, this.currentSource);
                    throw new InvalidOperationException(str);
                }
                this.relativeSource = value;
                this.currentSource = BindingSource.RelativeSource;
            }
        }

        internal BindingExpression CreateBindingExpression()
        {
            return new BindingExpression(this);
        }
    }
}