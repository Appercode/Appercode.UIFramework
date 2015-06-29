using Appercode.UI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Appercode.UI.Internals.PathParser
{
    internal sealed class PropertyPathListener
    {
        private PropertyPathStep first;

        private PropertyPathStep last;

        private object source;

        private BindingExpression expression;

        internal PropertyPathListener(PropertyPath path, object source, bool listenToChanges, Appercode.UI.Data.BindingExpression exp)
        {
            this.expression = exp;
            this.ConnectToSource(path, source, listenToChanges);
        }

        internal event PropertyPathStepChangedHandler PropertyPathChanged;

        internal bool FullPathExists
        {
            get
            {
                return this.last.IsConnected;
            }
        }

        internal object LeafItem
        {
            get
            {
                return this.last.Source;
            }
        }

        internal object LeafProperty
        {
            get
            {
                return this.last.Property;
            }
        }

        internal string LeafPropertyName
        {
            get
            {
                return this.last.PropertyName;
            }
        }

        internal Type LeafType
        {
            get
            {
                return this.last.Type;
            }
        }

        internal object LeafValue
        {
            get
            {
                return this.last.Value;
            }
            set
            {
                this.last.Value = value;
            }
        }

        /*
        //internal string TraceContext
        //{
        //    get
        //    {
        //        if (this.traceContext == null)
        //        {
        //            if (this.expression == null)
        //            {
        //                this.traceContext = "";
        //            }
        //            else
        //            {
        //                this.traceContext = this.expression.GetExpressionTraceString();
        //            }
        //        }
        //        return this.traceContext;
        //    }
        //}
        */

        internal void Disconnect()
        {
            for (PropertyPathStep i = this.first; i != null; i = i.NextStep)
            {
                i.Disconnect();
            }
        }

        internal void RaisePropertyPathStepChanged(PropertyPathStep source)
        {
            PropertyPathStep nextStep = source.NextStep;
            object value = source.Value;
            while (nextStep != null)
            {
                nextStep.ReConnect(value);
                value = nextStep.Value;
                nextStep = nextStep.NextStep;
            }
            if (this.PropertyPathChanged != null)
            {
                this.PropertyPathChanged(this, new PropertyPathChangedEventArgs(source));
            }
        }

        internal void ReConnect(object source)
        {
            for (PropertyPathStep i = this.first; i != null; i = i.NextStep)
            {
                i.ReConnect(source);
                source = i.Value;
            }
        }

        private void ConnectToSource(PropertyPath path, object source, bool listenToChanges)
        {
            this.source = source;
            PropertyPathStep propertyPathStep = null;
            object value = source;
            int i = 1;
            PropertyPathStep propertyPathStep2 = path.Descriptors[0].CreateStep(this, value, listenToChanges);
            this.first = propertyPathStep2;
            propertyPathStep = propertyPathStep2;
            value = propertyPathStep.Value;
            for (i = 1; i < path.Descriptors.Count; i++)
            {
                propertyPathStep = path.Descriptors[i].CreateStep(this, value, listenToChanges);
                propertyPathStep2.NextStep = propertyPathStep;
                propertyPathStep2 = propertyPathStep;
                value = propertyPathStep.Value;
            }
            this.last = propertyPathStep;
        }
    }
}
