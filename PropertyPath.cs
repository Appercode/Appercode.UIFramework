using Appercode.UI.Internals;
using Appercode.UI.Internals.PathParser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Appercode.UI
{
    [TypeConverter(typeof(PropertyPathConverter))]
    public sealed class PropertyPath
    {
        private string path;

        private List<PropertyPathStepDescriptor> descriptors;

        private DependencyProperty dp;

        public PropertyPath(string path, params object[] pathParameters)
        {
            if ((int)pathParameters.Length != 0)
            {
                throw new ArgumentOutOfRangeException("pathParameters");
            }
            this.Path = path;
        }

        public PropertyPath(object parameter)
        {
            this.dp = parameter as DependencyProperty;
            if (this.dp != null)
            {
                this.path = "(0)";
            }
        }

        public string Path
        {
            get
            {
                return this.path;
            }
            internal set
            {
                this.path = value;
                this.dp = null;
            }
        }

        internal IList<PropertyPathStepDescriptor> Descriptors
        {
            get
            {
                return this.descriptors;
            }
        }

        internal bool IsPathToSource
        {
            get
            {
                return this.descriptors[0] is SourcePathStepDescriptor;
            }
        }

        internal DependencyProperty GetDependencyProperty()
        {
            return this.dp;
        }

        internal PropertyPathListener GetListener(object source, bool listenToChanges, Appercode.UI.Data.BindingExpression exp)
        {            
            return new PropertyPathListener(this, source, listenToChanges, exp);
        }

        internal bool HasDependencyProperty()
        {
            return this.dp != null;
        }

        internal void ParsePathInternal(bool calledFromParser)
        {
            this.descriptors = PropertyPath.ParsePath(this.path, calledFromParser);
        }

        private static List<PropertyPathStepDescriptor> ParsePath(string path, bool calledFromParser)
        {
            return (new PropertyPathParser(path, calledFromParser)).Parse();
        }
    }

    public class PropertyPathConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            return new PropertyPath((string)value, new object[0]);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertFrom(context, sourceType);
            }
        }
    }
}