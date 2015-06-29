using Appercode.UI.Internals.PathParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows;

namespace Appercode.UI.Internals
{
    internal class PropertyPathParser
    {
        private string propertyPath;

        private bool calledFromParser;

        private int index;

        internal PropertyPathParser(string propertyPath, bool calledFromParser)
        {
            this.propertyPath = propertyPath;
            this.calledFromParser = calledFromParser;
        }

        internal List<PropertyPathStepDescriptor> Parse()
        {
            List<PropertyPathStepDescriptor> propertyPathStepDescriptors = new List<PropertyPathStepDescriptor>();
            if (string.IsNullOrEmpty(this.propertyPath) || this.propertyPath == ".")
            {
                propertyPathStepDescriptors.Add(new SourcePathStepDescriptor());
                return propertyPathStepDescriptors;
            }
            while (!this.Eof())
            {
                propertyPathStepDescriptors.Add(this.ReadStepDescriptor(this.calledFromParser));
            }
            return propertyPathStepDescriptors;
        }

        private bool Eof()
        {
            return this.index >= this.propertyPath.Length;
        }

        /*
        // Code for attached property
        //
        //[SecuritySafeCritical]
        //private DependencyProperty GetDpFromName(string name, bool namespaceMappingAvailable)
        //{
        //    string str = null;
        //    string str1 = null;
        //    DependencyProperty propertyUsingCurrentOWContext = null;
        //    if (namespaceMappingAvailable)
        //    {
        //        if (!QuirksMode.ShouldUseSL3Parser())
        //        {
        //            propertyUsingCurrentOWContext = XcpImports.GetPropertyUsingCurrentOWContext(name);
        //        }
        //        else
        //        {
        //            this.GetNamespaceForProperty(ref name, out str, out str1);
        //        }
        //    }
        //    if (propertyUsingCurrentOWContext == null)
        //    {
        //        if (str == null || str1 == null)
        //        {
        //            propertyUsingCurrentOWContext = DependencyProperty.LookupAttachedCoreProperty(name);
        //        }
        //        else
        //        {
        //            propertyUsingCurrentOWContext = DependencyProperty.GetRegisteredDependencyProperty(null, name, str, str1);
        //        }
        //    }
        //    return propertyUsingCurrentOWContext;
        //}
        */

        /*
        //private void GetNamespaceForProperty(ref string name, out string propertyNamespace, out string propertyAssembly)
        //{
        //    propertyAssembly = propertyNamespace = null;
        //    char[] chrArray = new char[] { ':' };
        //    string[] strArrays = name.Split(chrArray);
        //    if (strArrays == null || (int)strArrays.Length != 2)
        //    {
        //        return;
        //    }
        //    XcpImports.ResolveXamlPrefix(strArrays[0], out propertyNamespace, out propertyAssembly);
        //    name = strArrays[1];
        //}
        */

        private bool IsPropertyChar(char current)
        {
            if (this.IsStartOfPropertyChar(current) || char.IsDigit(current))
            {
                return true;
            }
            return char.IsWhiteSpace(current);
        }

        private bool IsStartOfPropertyChar(char current)
        {
            if (char.IsLetter(current))
            {
                return true;
            }
            return current == '\u005F';
        }

        private bool IsValidAttachedPropertyChar(char current)
        {
            if (this.IsPropertyChar(current))
            {
                return true;
            }
            return current == '.';
        }

        private DependencyPropertyAccessStepDescriptor ReadAttachedPropertyStepDescriptor(bool calledFromParser)
        {
            char chr = this.ReadChar();
            StringBuilder stringBuilder = new StringBuilder();
            while (chr != ')')
            {
                stringBuilder.Append(chr);
                if (this.Eof())
                {
                    break;
                }
                chr = this.ReadChar();
            }
            if (chr != ')')
            {
                StringBuilder stringBuilder1 = new StringBuilder();
                string str = "PropertyPath_InvalidBindingPathNoChar";
                object[] objArray = new object[] { this.propertyPath };
                stringBuilder1.AppendFormat(str, objArray);
                throw new ArgumentException(stringBuilder1.ToString());
            }
            throw new NotSupportedException("Path with attached property not supported yet");
            /*
            //DependencyProperty dpFromName = this.GetDpFromName(stringBuilder.ToString().Trim(), calledFromParser);
            //if (dpFromName == null)
            //{
            //    StringBuilder stringBuilder2 = new StringBuilder();
            //    string str1 = "PropertyPath_CannotResolveAttachedDp";
            //    object[] objArray1 = new object[] { stringBuilder.ToString().Trim(), this._propertyPath };
            //    stringBuilder2.AppendFormat(str1, objArray1);
            //    throw new ArgumentException(stringBuilder2.ToString());
            //}
            //if (!this.Eof())
            //{
            //    chr = this.ReadChar();
            //    if (chr != '.')
            //    {
            //        this.Unread();
            //    }
            //}
            //return new DependencyPropertyAccessStepDescriptor(dpFromName);
            */
        }

        private char ReadChar()
        {
            if (this.Eof())
            {
                StringBuilder stringBuilder = new StringBuilder();
                string str = "PropertyPath_InvalidBindingPathNoChar";
                object[] objArray = new object[] { this.propertyPath };
                stringBuilder.AppendFormat(str, objArray);
                throw new ArgumentException(stringBuilder.ToString());
            }
            char chr = this.propertyPath[this.index];
            PropertyPathParser propertyPathParser = this;
            propertyPathParser.index = propertyPathParser.index + 1;
            return chr;
        }

        private IndexerPathStepDescriptor ReadIndexerPathStepDescriptor()
        {
            char i;
            StringBuilder stringBuilder = new StringBuilder();
            for (i = this.ReadChar(); i != ']'; i = this.ReadChar())
            {
                stringBuilder.Append(i);
                if (this.Eof())
                {
                    break;
                }
            }
            if (i != ']')
            {
                StringBuilder stringBuilder1 = new StringBuilder();
                string str = "PropertyPath_InvalidBindingPathNoChar";
                object[] objArray = new object[] { this.propertyPath };
                stringBuilder1.AppendFormat(str, objArray);
                throw new ArgumentException(stringBuilder1.ToString());
            }
            if (!this.Eof())
            {
                i = this.ReadChar();
                if (i != '.')
                {
                    this.Unread();
                }
            }
            return new IndexerPathStepDescriptor(stringBuilder.ToString().Trim());
        }

        private PropertyAccessPathStepDescriptor ReadPropertyAccessStepDescriptor(char current)
        {
            StringBuilder stringBuilder = new StringBuilder();
            do
            {
                stringBuilder.Append(current);
                if (this.Eof())
                {
                    break;
                }
                current = this.ReadChar();
            }
            while (this.IsPropertyChar(current));
            if (!this.Eof() && current != '.')
            {
                this.Unread();
            }
            return new PropertyAccessPathStepDescriptor(stringBuilder.ToString().Trim());
        }

        private PropertyPathStepDescriptor ReadStepDescriptor(bool calledFromParser)
        {
            char chr = this.ReadChar();
            if (this.IsStartOfPropertyChar(chr))
            {
                return this.ReadPropertyAccessStepDescriptor(chr);
            }
            if (chr == '(')
            {
                return this.ReadAttachedPropertyStepDescriptor(calledFromParser);
            }
            if (chr != '[')
            {
                StringBuilder stringBuilder = new StringBuilder();
                string str = "PropertyPath_InvalidBindingPath";
                object[] objArray = new object[] { chr, this.propertyPath };
                stringBuilder.AppendFormat(str, objArray);
                throw new ArgumentException(stringBuilder.ToString());
            }
            return this.ReadIndexerPathStepDescriptor();
        }

        private void Unread()
        {
            PropertyPathParser propertyPathParser = this;
            propertyPathParser.index = propertyPathParser.index - 1;
        }
    }
}
