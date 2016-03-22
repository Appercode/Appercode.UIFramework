using Appercode.UI.Controls;
using Appercode.UI.Data;
using Appercode.UI.Internals;
using Appercode.UI.Markup;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace Appercode.UI
{
    [TypeConverter(typeof(FrameworkElementFactoryConverter))]
    public class FrameworkElementFactory
    {
        internal List<PropertyValue> PropertyValues = new List<PropertyValue>();

        private readonly Lazy<Dictionary<string, DependencyObject>> namedChildren = new Lazy<Dictionary<string, DependencyObject>>();
        private string childName;
        private bool @sealed;
        private Type type;
        private string text;
        private FrameworkElementFactory parent;
        private FrameworkElementFactory firstChild;
        private FrameworkElementFactory lastChild;
        private FrameworkElementFactory nextSibling;
        private object synchronized = new object();
        private FrameworkTemplate frameworkTemplate;
        private Dictionary<RoutedEvent, Delegate> eventHandlersStore = new Dictionary<RoutedEvent, Delegate>();
        private Dictionary<WeakReference<DependencyObject>, PropertyValue> templateBindingsStore = new Dictionary<WeakReference<DependencyObject>, PropertyValue>();

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.FrameworkElementFactory" /> class.</summary>
        public FrameworkElementFactory()
            : this(null, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.FrameworkElementFactory" /> class with the specified <see cref="T:System.Type" />.</summary>
        /// <param name="type">The type of instance to create.</param>
        public FrameworkElementFactory(Type type)
            : this(type, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.FrameworkElementFactory" /> class with the specified text to produce.</summary>
        /// <param name="text">The text string to produce.</param>
        public FrameworkElementFactory(string text)
            : this(null, null)
        {
            this.Text = text;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.FrameworkElementFactory" /> class with the specified <see cref="T:System.Type" /> and name.</summary>
        /// <param name="type">The type of instance to create.</param>
        /// <param name="name">The style identifier.</param>
        public FrameworkElementFactory(Type type, string name)
        {
            this.Type = type;
            this.Name = name;
        }

        /// <summary>Gets the first child factory.</summary>
        /// <returns>A <see cref="T:System.Windows.FrameworkElementFactory" /> the first child factory.</returns>
        public FrameworkElementFactory FirstChild
        {
            get
            {
                return this.firstChild;
            }
        }

        /// <summary>Gets a value that indicates whether this object is in an immutable state.</summary>
        /// <returns>true if this object is in an immutable state; otherwise, false.</returns>
        public bool IsSealed
        {
            get
            {
                return this.@sealed;
            }
        }

        /// <summary>Gets or sets the name of a template item.</summary>
        /// <returns>A string that is the template identifier.</returns>
        public string Name
        {
            get
            {
                return this.childName;
            }
            set
            {
                if (this.@sealed)
                {
                    throw new InvalidOperationException("Can't change FrameworkElementFactory after sealed");
                }
                if (value == string.Empty)
                {
                    throw new ArgumentException("Name couldn't be empty string");
                }
                this.childName = value;
            }
        }

        /// <summary>Gets the next sibling factory.</summary>
        /// <returns>A <see cref="T:System.Windows.FrameworkElementFactory" /> that is the next sibling factory.</returns>
        public FrameworkElementFactory NextSibling
        {
            get
            {
                return this.nextSibling;
            }
        }

        /// <summary>Gets the parent <see cref="T:System.Windows.FrameworkElementFactory" />.</summary>
        /// <returns>A <see cref="T:System.Windows.FrameworkElementFactory" /> that is the parent factory.</returns>
        public FrameworkElementFactory Parent
        {
            get
            {
                return this.parent;
            }
        }

        /// <summary>Gets or sets the text string to produce.</summary>
        /// <returns>The text string to produce.</returns>
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                if (this.@sealed)
                {
                    throw new InvalidOperationException("FrameworkElementFactory can't be changed after sealed");
                }
                if (this.firstChild != null)
                {
                    throw new InvalidOperationException("FrameworkElementFactoryCannotAddText");
                }
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.text = value;
            }
        }

        /// <summary>Gets or sets the type of the objects this factory produces.</summary>
        /// <returns>The type of the objects this factory produces.</returns>
        public Type Type
        {
            get
            {
                return this.type;
            }
            set
            {
                if (this.@sealed)
                {
                    throw new InvalidOperationException("FrameworkElementFactory can't be changed after sealed");
                }
                if (this.text != null)
                {
                    throw new InvalidOperationException("FrameworkElementFactoryCannotAddText");
                }
                if (value != null && !(typeof(Appercode.UI.Controls.UIElement).IsAssignableFrom(value) || typeof(Appercode.UI.Controls.DefinitionBase).IsAssignableFrom(value)))
                {
                    throw new ArgumentException(string.Format("Expected UIElement subclass, RowDefenition or HeightDefinition. Got {0}", value), "type");
                }
                this.type = value;
            }
        }

        internal FrameworkTemplate FrameworkTemplate
        {
            get
            {
                return this.frameworkTemplate;
            }
        }

        /// <summary>Adds a child factory to this factory.</summary>
        /// <param name="child">The <see cref="T:System.Windows.FrameworkElementFactory" /> object to add as a child.</param>
        public void AppendChild(FrameworkElementFactory child)
        {
            if (this.@sealed)
            {
                throw new InvalidOperationException("FrameworkElementFactory can't be changed after sealed");
            }
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }
            if (child.parent != null)
            {
                throw new ArgumentException("FrameworkElementFactory already have Parent");
            }
            if (this.text != null)
            {
                throw new InvalidOperationException("FrameworkElementFactoryCannotAddText");
            }
            if (this.firstChild != null)
            {
                this.lastChild.nextSibling = child;
                this.lastChild = child;
            }
            else
            {
                this.firstChild = child;
                this.lastChild = child;
            }
            child.parent = this;
        }

        /// <summary>Sets up data binding on a property.</summary>
        /// <param name="dp">Identifies the property where the binding should be established.</param>
        /// <param name="binding">Description of the binding.</param>
        public void SetBinding(DependencyProperty dp, BindingBase binding)
        {
            this.SetValue(dp, binding);
        }

        /// <summary>Set up a dynamic resource reference on a child property.</summary>
        /// <param name="dp">The property to which the resource is bound.</param>
        /// <param name="name">The name of the resource.</param>
        public void SetResourceReference(DependencyProperty dp, object name)
        {
            if (this.@sealed)
            {
                throw new InvalidOperationException("Can't change FrameworkElementFactory after sealed");
            }
            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }
            this.UpdatePropertyValueList(dp, PropertyValueType.Resource, name);
        }

        /// <summary>Sets the value of a dependency property.</summary>
        /// <param name="dp">The dependency property identifier of the property to set.</param>
        /// <param name="value">The new value.</param>
        public void SetValue(DependencyProperty dp, object value)
        {
            if (this.@sealed)
            {
                throw new InvalidOperationException("Can't change FrameworkElementFactory after sealed");
            }
            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }
            if (!dp.IsValidValue(value) && !(value is MarkupExtension) && !(value is DeferredReference) && !(value is BindingBase))
            {
                object[] objArray1 = new object[] { value, dp.Name };
                throw new ArgumentException("InvalidPropertyValue");
            }

            /*
            //if (StyleHelper.IsStylingLogicalTree(dp, value))
            //{
            //    throw new NotSupportedException("ModifyingLogicalTreeViaStylesNotImplemented");
            //}
            */

            if (dp.ReadOnly)
            {
                throw new ArgumentException("ReadOnlyPropertyNotAllowed");
            }
            ResourceReferenceExpression resourceReferenceExpression = value as ResourceReferenceExpression;
            object resourceKey = null;
            if (resourceReferenceExpression != null)
            {
                resourceKey = resourceReferenceExpression.ResourceKey;
            }
            if (resourceKey != null)
            {
                this.UpdatePropertyValueList(dp, PropertyValueType.Resource, resourceKey);
                return;
            }
            TemplateBindingExtension templateBindingExtension = value as TemplateBindingExtension;
            if (templateBindingExtension != null)
            {
                this.UpdatePropertyValueList(dp, PropertyValueType.TemplateBinding, templateBindingExtension);
                return;
            }
            this.UpdatePropertyValueList(dp, PropertyValueType.Set, value);
        }

        /// <summary>Adds an event handler for the given routed event to the instances created by this factory.</summary>
        /// <param name="routedEvent">Identifier object for the routed event being handled.</param>
        /// <param name="handler">A reference to the handler implementation.</param>
        public void AddHandler(RoutedEvent routedEvent, Delegate handler)
        {
            this.AddHandler(routedEvent, handler, false);
        }

        /// <summary>Adds an event handler for the given routed event to the instances created by this factory, with the option of having the provided handler be invoked even in cases of routed events that had already been marked as handled by another element along the route.</summary>
        /// <param name="routedEvent">Identifier object for the routed event being handled.</param>
        /// <param name="handler">A reference to the handler implementation.</param>
        /// <param name="handledEventsToo">Whether to invoke the handler in cases where the routed event has already been marked as handled in its arguments object. true to invoke the handler even when the routed event is marked handled; otherwise, false. The default is false. Asking to handle already-handled routed events is not common.</param>
        public void AddHandler(RoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
        {
            if (this.@sealed)
            {
                throw new InvalidOperationException("FrameworkElementFactory can't be changed after sealed");
            }
            if (routedEvent == null)
            {
                throw new ArgumentNullException("routedEvent");
            }
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            if (handler.GetType() != routedEvent.HandlerType)
            {
                throw new ArgumentException("HandlerType is illegal");
            }
            this.eventHandlersStore.Add(routedEvent, handler);
            ////if (routedEvent == UIElement.LoadedEvent || routedEvent == UIElement.UnloadedEvent)
            ////{
            ////    this.HasLoadedChangeHandler = true;
            ////}
        }

        internal static void AddNodeToLogicalTree(DependencyObject parent, Type type, bool treeNodeIsFE, Appercode.UI.Controls.UIElement treeNodeFE, Appercode.UI.Controls.UIElement treeNodeFCE)
        {
            Appercode.UI.Controls.UIElement frameworkContentElement = parent as Appercode.UI.Controls.UIElement;
            if (frameworkContentElement != null)
            {
                IEnumerator logicalChildren = frameworkContentElement.LogicalChildren;
                if (logicalChildren != null && logicalChildren.MoveNext())
                {
                    throw new InvalidOperationException("AlreadyHasLogicalChildren");
                }
            }
            IAddChild addChild = parent as IAddChild;
            if (addChild == null)
            {
                throw new InvalidOperationException("CannotHookupFCERoot");
            }
            if (treeNodeFE != null)
            {
                addChild.AddChild(treeNodeFE);
                return;
            }
            addChild.AddChild(treeNodeFCE);
        }

        /*
        //private void AddNodeToParent(DependencyObject parent, FrameworkObject childFrameworkObject)
        //{
        //    object[] name;
        //    RowDefinition rowDefinition = null;
        //    if (childFrameworkObject.IsFCE)
        //    {
        //        Grid grid = parent as Grid;
        //        if (grid != null)
        //        {
        //            ColumnDefinition fCE = childFrameworkObject.FCE as ColumnDefinition;
        //            ColumnDefinition columnDefinition = fCE;
        //            if (fCE == null)
        //            {
        //                RowDefinition fCE1 = childFrameworkObject.FCE as RowDefinition;
        //                rowDefinition = fCE1;
        //                if (fCE1 == null)
        //                {
        //                    if (!(parent is IAddChild))
        //                    {
        //                        name = new object[] { parent.GetType().Name };
        //                        throw new InvalidOperationException(SR.Get("TypeMustImplementIAddChild", name));
        //                    }
        //                    ((IAddChild)parent).AddChild(childFrameworkObject.DO);
        //                    return;
        //                }
        //            }
        //            if (columnDefinition != null)
        //            {
        //                grid.ColumnDefinitions.Add(columnDefinition);
        //                return;
        //            }
        //            if (rowDefinition != null)
        //            {
        //                grid.RowDefinitions.Add(rowDefinition);
        //                return;
        //            }
        //            else
        //            {
        //                return;
        //            }
        //        }
        //    }
        //    if (!(parent is IAddChild))
        //    {
        //        throw new InvalidOperationException("Type Must Implement IAddChild");
        //    }
        //    ((IAddChild)parent).AddChild(childFrameworkObject.DO);
        //}
        */

        internal void Seal(FrameworkTemplate ownerTemplate)
        {
            if (this.@sealed)
            {
                return;
            }
            this.frameworkTemplate = ownerTemplate;
            this.Seal();
        }

        internal DependencyObject InstantiateUnoptimizedTree()
        {
            this.Seal();
            var depObj = (DependencyObject)Activator.CreateInstance(this.Type);
            foreach (var p in this.PropertyValues)
            {
                if (p.Value is Binding)
                {
                    depObj.SetExpression(p.Property, ((Binding)p.Value).CreateBindingExpression());
                }
                else if (p.Value is TemplateBindingExtension)
                {
                    this.templateBindingsStore.Add(new WeakReference<DependencyObject>(depObj), p);
                }
                else
                {
                    depObj.SetValue(p.Property, p.Value);
                }
            }

            foreach (var eventPair in this.eventHandlersStore)
            {
                var eventInfo = this.Type.GetEvent(eventPair.Key.Name);
                if (eventInfo != null)
                {
                    eventInfo.AddEventHandler(depObj, eventPair.Value);
                }
            }

            if (this.namedChildren.IsValueCreated)
            {
                this.namedChildren.Value.Clear();
            }

            if (String.IsNullOrEmpty(this.childName) == false)
            {
                this.namedChildren.Value.Add(this.childName, depObj);
            }

            var nextChild = this.FirstChild;
            while (nextChild != null)
            {
                var childInstance = nextChild.InstantiateUnoptimizedTree();
                if (nextChild.namedChildren.IsValueCreated)
                {
                    foreach (var namedChild in nextChild.namedChildren.Value)
                    {
                        this.namedChildren.Value.Add(namedChild.Key, namedChild.Value);
                    }
                }

                ((IAddChild)depObj).AddChild(childInstance);
                nextChild = nextChild.NextSibling;
            }
            return depObj;
        }

        internal void SetTemplateBindings(DependencyObject source)
        {
            UIElement sourceUIElement = source as UIElement;
            if (sourceUIElement != null)
            {
                foreach (var key in this.templateBindingsStore.Keys.ToList())
                {
                    DependencyObject depObj;
                    if (key.TryGetTarget(out depObj))
                    {
                        UIElement depObjUIElement = depObj as UIElement;
                        if (depObjUIElement != null && depObjUIElement.InDescendantsOf(sourceUIElement))
                        {
                            var value = this.templateBindingsStore[key];
                            depObjUIElement.SetExpression(value.Property, ((TemplateBindingExtension)value.Value).CreateTemplateBindingExpression(source));

                            // Removing element after first set
                            this.templateBindingsStore.Remove(key);
                        }
                    }
                }

                var nextChild = this.FirstChild;
                while (nextChild != null)
                {
                    nextChild.SetTemplateBindings(source);
                    nextChild = nextChild.NextSibling;
                }
            }
        }

        internal DependencyObject GetNamedChild(string childName)
        {
            DependencyObject result = null;
            if (this.namedChildren.IsValueCreated)
            {
                this.namedChildren.Value.TryGetValue(childName, out result);
            }

            return result;
        }

        private void Seal()
        {
            if (this.IsSealed)
            {
                return;
            }

            if (this.type == null && this.text == null)
            {
                throw new InvalidOperationException("NullTypeIllegal");
            }
            if (this.firstChild != null && !typeof(IAddChild).IsAssignableFrom(this.type))
            {
                throw new InvalidOperationException("TypeMustImplementIAddChild");
            }

            if (String.IsNullOrEmpty(this.childName) == false)
            {
                this.childName = String.Intern(this.childName);
            }

            lock (this.synchronized)
            {
                for (int i = 0; i < this.PropertyValues.Count; i++)
                {
                    PropertyValue item = this.PropertyValues[i];
                    item.ChildName = this.childName;
                    StyleHelper.SealIfSealable(item.ValueInternal);
                    this.PropertyValues[i] = item;
                }
            }

            this.@sealed = true;
            if (this.frameworkTemplate != null)
            {
                for (var j = this.firstChild; j != null; j = j.nextSibling)
                {
                    j.Seal(this.frameworkTemplate);
                }
            }
        }

        private void UpdatePropertyValueList(DependencyProperty dp, PropertyValueType valueType, object value)
        {
            int count = -1;
            int i = 0;
            while (i < this.PropertyValues.Count)
            {
                if (this.PropertyValues[i].Property != dp)
                {
                    i++;
                }
                else
                {
                    count = i;
                    break;
                }
            }
            if (count < 0)
            {
                PropertyValue propertyValue = new PropertyValue();
                propertyValue.ValueType = valueType;
                propertyValue.ChildName = null;
                propertyValue.Property = dp;
                propertyValue.ValueInternal = value;
                lock (this.synchronized)
                {
                    this.PropertyValues.Add(propertyValue);
                }
            }
            else
            {
                lock (this.synchronized)
                {
                    PropertyValue item = this.PropertyValues[count];
                    item.ValueType = valueType;
                    item.ValueInternal = value;
                    this.PropertyValues[count] = item;
                }
            }
        }
    }

    public class FrameworkElementFactoryConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
            ////if (sourceType == typeof(string))
            ////{
            ////    return true;
            ////}
            ////return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return new FrameworkElementFactory();
        }
    }
}