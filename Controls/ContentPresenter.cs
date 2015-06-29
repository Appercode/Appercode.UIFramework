using Appercode.UI.Data;
using Appercode.UI.Internals;
using Appercode.UI.Internals.Boxes;
using Appercode.UI.Markup;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Appercode.UI.Controls
{
    public partial class ContentPresenter : UIElement
    {
        /// <summary>Identifies the <see cref="P:System.Windows.Controls.ContentPresenter.RecognizesAccessKey" /> dependency property. </summary>
        /// <returns>The identifier for the <see cref="P:System.Windows.Controls.ContentPresenter.RecognizesAccessKey" /> dependency property.</returns>
        public static readonly DependencyProperty RecognizesAccessKeyProperty;

        /// <summary>Identifies the <see cref="P:System.Windows.Controls.ContentPresenter.Content" /> dependency property. </summary>
        /// <returns>The identifier for the <see cref="P:System.Windows.Controls.ContentPresenter.Content" /> dependency property.</returns>
        public static readonly DependencyProperty ContentProperty;

        /// <summary>Identifies the <see cref="P:System.Windows.Controls.ContentPresenter.ContentTemplate" /> dependency property. </summary>
        /// <returns>The identifier for the <see cref="P:System.Windows.Controls.ContentPresenter.ContentTemplate" /> dependency property.</returns>
        public static readonly DependencyProperty ContentTemplateProperty;

        /// <summary>Identifies the <see cref="P:System.Windows.Controls.ContentPresenter.ContentTemplateSelector" /> dependency property. </summary>
        /// <returns>The identifier for the <see cref="P:System.Windows.Controls.ContentPresenter.ContentTemplateSelector" /> dependency property.</returns>
        public static readonly DependencyProperty ContentTemplateSelectorProperty;

        /// <summary>Identifies the <see cref="P:System.Windows.Controls.ContentPresenter.ContentStringFormat" /> dependency property.</summary>
        /// <returns>The identifier for the <see cref="P:System.Windows.Controls.ContentPresenter.ContentStringFormat" /> dependency property.</returns>
        public static readonly DependencyProperty ContentStringFormatProperty;

        /// <summary>Identifies the <see cref="P:System.Windows.Controls.ContentPresenter.ContentSource" /> dependency property. </summary>
        /// <returns>The identifier for the <see cref="P:System.Windows.Controls.ContentPresenter.ContentSource" /> dependency property.</returns>
        public static readonly DependencyProperty ContentSourceProperty;

        internal static readonly DependencyProperty TemplateProperty;

        private static DataTemplate stringTemplate;

        private static DataTemplate uiElementTemplate;

        private static DataTemplate defaultTemplate;

        private static ContentPresenter.DefaultSelector defaultTemplateSelector;

        ////private static readonly  UncommonField<DataTemplate> XMLFormattingTemplateField;

        ////private static readonly  UncommonField<DataTemplate> StringFormattingTemplateField;

        ////private static readonly  UncommonField<DataTemplate> AccessTextFormattingTemplateField;

        private bool templateIsCurrent;

        private bool contentIsItem = true;

        private UIElement templateInstance;

        static ContentPresenter()
        {
            ContentPresenter.ContentTemplateProperty = ContentControl.ContentTemplateProperty.AddOwner(typeof(ContentPresenter), new PropertyMetadata(new ContentPresenter.DefaultTemplate(), new PropertyChangedCallback(ContentPresenter.OnContentTemplateChanged)));
            ContentPresenter.RecognizesAccessKeyProperty = DependencyProperty.Register("RecognizesAccessKey", typeof(bool), typeof(ContentPresenter), new PropertyMetadata(BooleanBoxes.FalseBox));
            ContentPresenter.ContentProperty = ContentControl.ContentProperty.AddOwner(typeof(ContentPresenter), new PropertyMetadata(null, new PropertyChangedCallback(ContentPresenter.OnContentChanged)));
            ContentPresenter.ContentTemplateSelectorProperty = ContentControl.ContentTemplateSelectorProperty.AddOwner(typeof(ContentPresenter), new PropertyMetadata(null, new PropertyChangedCallback(ContentPresenter.OnContentTemplateSelectorChanged)));
            ContentPresenter.ContentStringFormatProperty = DependencyProperty.Register("ContentStringFormat", typeof(string), typeof(ContentPresenter), new PropertyMetadata(null, new PropertyChangedCallback(ContentPresenter.OnContentStringFormatChanged)));
            ContentPresenter.ContentSourceProperty = DependencyProperty.Register("ContentSource", typeof(string), typeof(ContentPresenter), new PropertyMetadata("Content"));
            ContentPresenter.TemplateProperty = DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ContentPresenter), new PropertyMetadata(null, new PropertyChangedCallback(ContentPresenter.OnTemplateChanged)));
            /*
            //ContentPresenter.XMLFormattingTemplateField = new UncommonField<DataTemplate>();
            //ContentPresenter.StringFormattingTemplateField = new UncommonField<DataTemplate>();
            //ContentPresenter.AccessTextFormattingTemplateField = new UncommonField<DataTemplate>();
            */
            DataTemplate dataTemplate = new DataTemplate();
            FrameworkElementFactory frameworkElementFactory;

            /*
            //FrameworkElementFactory frameworkElementFactory = ContentPresenter.CreateAccessTextFactory();
            //frameworkElementFactory.SetValue(AccessText.TextProperty, new TemplateBindingExtension(ContentPresenter.ContentProperty));
            //dataTemplate.VisualTree = frameworkElementFactory;
            //dataTemplate.Seal();
            //ContentPresenter.s_AccessTextTemplate = dataTemplate;
            //dataTemplate = new DataTemplate();
            */

            frameworkElementFactory = ContentPresenter.CreateTextBlockFactory();
            frameworkElementFactory.SetValue(TextBlock.TextProperty, new TemplateBindingExtension(ContentPresenter.ContentProperty));
            dataTemplate.VisualTree = frameworkElementFactory;
            dataTemplate.Seal();
            ContentPresenter.stringTemplate = dataTemplate;

            /*
            //dataTemplate = new DataTemplate();
            //frameworkElementFactory = ContentPresenter.CreateTextBlockFactory();
            //Binding binding = new Binding();
            //binding.XPath = ".";
            //frameworkElementFactory.SetBinding(TextBlock.TextProperty, binding);
            //dataTemplate.VisualTree = frameworkElementFactory;
            //dataTemplate.Seal();
            //ContentPresenter.s_XmlNodeTemplate = dataTemplate;
            //dataTemplate = new ContentPresenter.UseContentTemplate();
            //dataTemplate.Seal();
            */ 

            frameworkElementFactory = new FrameworkElementFactory(typeof(ContentControl));
            frameworkElementFactory.SetValue(ContentControl.ContentProperty, new TemplateBindingExtension(ContentPresenter.ContentProperty));
            dataTemplate = new DataTemplate();
            dataTemplate.VisualTree = frameworkElementFactory;

            ContentPresenter.uiElementTemplate = dataTemplate;
            dataTemplate = new ContentPresenter.DefaultTemplate();
            dataTemplate.Seal();
            ContentPresenter.defaultTemplate = dataTemplate;
            ContentPresenter.defaultTemplateSelector = new ContentPresenter.DefaultSelector();
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Controls.ContentPresenter" /> class.</summary>
        public ContentPresenter()
        {
            this.Initialize();
        }

        /// <summary>Gets or sets the data used to generate the child elements of a <see cref="T:System.Windows.Controls.ContentPresenter" />. This is a dependency property. </summary>
        /// <returns>The data used to generate the child elements. The default is null.</returns>
        public object Content
        {
            get
            {
                return this.GetValue(ContentControl.ContentProperty);
            }
            set
            {
                this.SetValue(ContentControl.ContentProperty, value);
            }
        }

        /// <summary>Gets or sets the base name to use during automatic aliasing. This is a dependency property. </summary>
        /// <returns>The base name to use during automatic aliasing. The default is "Content".</returns>
        public string ContentSource
        {
            get
            {
                return this.GetValue(ContentPresenter.ContentSourceProperty) as string;
            }
            set
            {
                this.SetValue(ContentPresenter.ContentSourceProperty, value);
            }
        }

        /// <summary>Gets or sets a composite string that specifies how to format the <see cref="P:System.Windows.Controls.ContentPresenter.Content" /> property if it is displayed as a string.</summary>
        /// <returns>A composite string that specifies how to format the <see cref="P:System.Windows.Controls.ContentPresenter.Content" /> property if it is displayed as a string. The default is null.</returns>
        public string ContentStringFormat
        {
            get
            {
                return (string)this.GetValue(ContentPresenter.ContentStringFormatProperty);
            }
            set
            {
                this.SetValue(ContentPresenter.ContentStringFormatProperty, value);
            }
        }

        /// <summary>Gets or sets the template used to display the content of the control.  This is a dependency property. </summary>
        /// <returns>A <see cref="T:System.Windows.DataTemplate" /> that defines the visualization of the content. The default is null.</returns>
        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(ContentControl.ContentTemplateProperty);
            }
            set
            {
                this.SetValue(ContentControl.ContentTemplateProperty, value);
            }
        }

        /// <summary>Gets or sets the <see cref="T:System.Windows.Controls.DataTemplateSelector" />, which allows the application writer to provide custom logic for choosing the template that is used to display the content of the control. This is a dependency property. </summary>
        /// <returns>A <see cref="T:System.Windows.Controls.DataTemplateSelector" /> object that supplies logic to return a <see cref="T:System.Windows.DataTemplate" /> to apply. The default is null.</returns>
        public DataTemplateSelector ContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)this.GetValue(ContentControl.ContentTemplateSelectorProperty);
            }
            set
            {
                this.SetValue(ContentControl.ContentTemplateSelectorProperty, value);
            }
        }

        /// <summary>Gets or sets a value that indicates whether the <see cref="T:System.Windows.Controls.ContentPresenter" /> should use <see cref="T:System.Windows.Controls.AccessText" /> in its style.  This is a dependency property. </summary>
        /// <returns>true if the <see cref="T:System.Windows.Controls.ContentPresenter" /> should use <see cref="T:System.Windows.Controls.AccessText" /> in its style; otherwise, false. The default is false.</returns>
        public bool RecognizesAccessKey
        {
            get
            {
                return (bool)this.GetValue(ContentPresenter.RecognizesAccessKeyProperty);
            }
            set
            {
                this.SetValue(ContentPresenter.RecognizesAccessKeyProperty, BooleanBoxes.Box(value));
            }
        }

        internal bool TemplateIsCurrent
        {
            get
            {
                return this.templateIsCurrent;
            }
        }

        private static DataTemplate StringTemplate
            {
            get
                {
                return ContentPresenter.stringTemplate;
                }
                }

        private static DataTemplate DefaultContentTemplate
        {
            get
            {
                return ContentPresenter.defaultTemplate;
            }
        }

        private static ContentPresenter.DefaultSelector DefaultTemplateSelector
        {
            get
            {
                return ContentPresenter.defaultTemplateSelector;
            }
        }

        private static DataTemplate UIElementContentTemplate
        {
            get
            {
                return ContentPresenter.uiElementTemplate;
            }
        }

        private UIElement TemplateInstance
        {
            get
            {
                return this.templateInstance;
            }
            set
            {
                if (value == this.templateInstance)
                {
                    return;
                }
                var oldValue = this.templateInstance;
                if (oldValue != null)
                {
                    LogicalTreeHelper.RemoveLogicalChild(this, oldValue);
                }

                this.templateInstance = value; 
                this.AddLogicalChild(this.templateInstance);
                this.NativeTemplateUpdate(oldValue, value);
            }
        }

        /*private DataTemplate FormattingAccessTextContentTemplate
        //{
        //    get
        //    {
        //        DataTemplate value = ContentPresenter.AccessTextFormattingTemplateField.GetValue(this);
        //        if (value == null)
        //        {
        //            Binding binding = new Binding();
        //            binding.StringFormat = this.ContentStringFormat;
        //            FrameworkElementFactory frameworkElementFactory = ContentPresenter.CreateAccessTextFactory();
        //            frameworkElementFactory.SetBinding(AccessText.TextProperty, binding);
        //            value = new DataTemplate();
        //            value.VisualTree = frameworkElementFactory;
        //            value.Seal();
        //            ContentPresenter.AccessTextFormattingTemplateField.SetValue(this, value);
        //        }
        //        return value;
        //    }
        //}

        //private DataTemplate FormattingStringContentTemplate
        //{
        //    get
        //    {
        //        DataTemplate value = ContentPresenter.StringFormattingTemplateField.GetValue(this);
        //        if (value == null)
        //        {
        //            Binding binding = new Binding();
        //            binding.StringFormat = this.ContentStringFormat;
        //            FrameworkElementFactory frameworkElementFactory = ContentPresenter.CreateTextBlockFactory();
        //            frameworkElementFactory.SetBinding(TextBlock.TextProperty, binding);
        //            value = new DataTemplate();
        //            value.VisualTree = frameworkElementFactory;
        //            value.Seal();
        //            ContentPresenter.StringFormattingTemplateField.SetValue(this, value);
        //        }
        //        return value;
        //    }
        //}

        //private DataTemplate FormattingXmlNodeContentTemplate
        //{
        //    get
        //    {
        //        DataTemplate value = ContentPresenter.XMLFormattingTemplateField.GetValue(this);
        //        if (value == null)
        //        {
        //            Binding binding = new Binding();
        //            binding.XPath = ".";
        //            binding.StringFormat = this.ContentStringFormat;
        //            FrameworkElementFactory frameworkElementFactory = ContentPresenter.CreateTextBlockFactory();
        //            frameworkElementFactory.SetBinding(TextBlock.TextProperty, binding);
        //            value = new DataTemplate();
        //            value.VisualTree = frameworkElementFactory;
        //            value.Seal();
        //            ContentPresenter.XMLFormattingTemplateField.SetValue(this, value);
        //        }
        //        return value;
        //    }
        //}
        */

        private DataTemplate Template
        {
            get
            {
                return (DataTemplate)this.GetValue(ContentPresenter.TemplateProperty);
            }
            set
            {
                this.SetValue(ContentPresenter.TemplateProperty, value);
            }
        }

        protected internal override IEnumerator LogicalChildren
        {
            get
            {
                var children = new List<object>();
                if (this.Content != null)
                {
                    children.Add(this.Content);
                }
                if (this.templateInstance != null)
                {
                    children.Add(this.templateInstance);
                }
                return children.GetEnumerator();
            }
        }

        internal static FrameworkElementFactory CreateTextBlockFactory()
        {
            var tf = new FrameworkElementFactory(typeof(TextBlock));
            tf.SetBinding(TextBlock.TextProperty, new Binding());
            return tf;
        }        

        internal void PrepareContentPresenter(object item, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector, string stringFormat)
        {
            if (item != this)
            {
                if (this.contentIsItem || !this.HasNonDefaultValue(ContentPresenter.ContentProperty))
                {
                    this.Content = item;
                    this.contentIsItem = true;
                }
                if (itemTemplate != null)
                {
                    this.SetValue(ContentPresenter.ContentTemplateProperty, itemTemplate);
                }
                if (itemTemplateSelector != null)
                {
                    this.SetValue(ContentPresenter.ContentTemplateSelectorProperty, itemTemplateSelector);
                }
                if (stringFormat != null)
                {
                    this.SetValue(ContentPresenter.ContentStringFormatProperty, stringFormat);
                }
            }
        }

        /*
        internal override void OnPreApplyTemplate()
        {
            base.OnPreApplyTemplate();
            if (base.TemplatedParent == null)
            {
                base.InvalidateProperty(ContentPresenter.ContentProperty);
            }
            if (!this._templateIsCurrent)
            {
                this.EnsureTemplate();
                this._templateIsCurrent = true;
            }
        }
        */
        
        /// <summary>Returns the template to use. This may depend on the content or other properties.</summary>
        /// <returns>The <see cref="T:System.Windows.DataTemplate" /> to use.</returns>
        protected virtual DataTemplate ChooseTemplate()
        {
            DataTemplate contentTemplate = null;
            object content = this.Content;
            contentTemplate = this.ContentTemplate;
            if (contentTemplate == null && this.ContentTemplateSelector != null)
            {
                contentTemplate = this.ContentTemplateSelector.SelectTemplate(content, this);
            }
            if (contentTemplate == null)
            {
                contentTemplate = ContentPresenter.DefaultTemplateSelector.SelectTemplate(content, this);
            }
            return contentTemplate;
        }
        
        /// <summary>Invoked when the <see cref="P:System.Windows.Controls.ContentPresenter.ContentStringFormat" /> property changes.</summary>
        /// <param name="oldContentStringFormat">The old value of the <see cref="P:System.Windows.Controls.ContentPresenter.ContentStringFormat" /> property.</param>
        /// <param name="newContentStringFormat">The new value of the <see cref="P:System.Windows.Controls.ContentPresenter.ContentStringFormat" /> property.</param>
        protected virtual void OnContentStringFormatChanged(string oldContentStringFormat, string newContentStringFormat)
        {
        }

        /// <summary>Invoked when the <see cref="P:System.Windows.Controls.ContentPresenter.ContentTemplateSelector" /> property changes. </summary>
        /// <param name="oldContentTemplateSelector">The old value of the <see cref="P:System.Windows.Controls.ContentPresenter.ContentTemplateSelector" /> property.</param>
        /// <param name="newContentTemplateSelector">The new value of the <see cref="P:System.Windows.Controls.ContentPresenter.ContentTemplateSelector" /> property.</param>
        protected virtual void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector)
        {
            ////Helper.CheckTemplateAndTemplateSelector("Content", ContentPresenter.ContentTemplateProperty, ContentPresenter.ContentTemplateSelectorProperty, this);
            this.Template = null;
        }

        /// <summary>Invoked when the <see cref="P:System.Windows.Controls.ContentPresenter.ContentTemplate" /> changes. </summary>
        /// <param name="oldContentTemplate">The old value of the <see cref="P:System.Windows.Controls.ContentPresenter.ContentTemplate" /> property.</param>
        /// <param name="newContentTemplate">The new value of the <see cref="P:System.Windows.Controls.ContentPresenter.ContentTemplate" /> property.</param>
        protected virtual void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate)
        {
            ////Helper.CheckTemplateAndTemplateSelector("Content", ContentPresenter.ContentTemplateProperty, ContentPresenter.ContentTemplateSelectorProperty, this);
            this.Template = newContentTemplate;
        }

        /// <summary>Invoked when the <see cref="P:System.Windows.Controls.ContentPresenter.ContentTemplate" /> changes </summary>
        /// <param name="oldTemplate">The old <see cref="T:System.Windows.DataTemplate" /> object value.</param>
        /// <param name="newTemplate">The new <see cref="T:System.Windows.DataTemplate" /> object value.</param>
        protected virtual void OnTemplateChanged(DataTemplate oldTemplate, DataTemplate newTemplate)
        {
        }

        private static TextBlock CreateTextBlock(ContentPresenter container)
        {
            return new TextBlock();
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool flag;
            Type oldType;
            Type newType;
            ContentPresenter contentPresenter = (ContentPresenter)d;
            if (!contentPresenter.templateIsCurrent)
            {
                return;
            }
            if (contentPresenter.ContentTemplate != null)
            {
                flag = false;
            }
            else if (contentPresenter.Template == ContentPresenter.UIElementContentTemplate)
            {
                flag = true;
                contentPresenter.Template = null;
            }
            else if (contentPresenter.Template != ContentPresenter.DefaultContentTemplate)
            {
                if (e.OldValue != null)
                {
                    oldType = e.OldValue.GetType();
                }
                else
                {
                    oldType = null;
                }
                if (e.NewValue != null)
                {
                    newType = e.NewValue.GetType();
                }
                else
                {
                    newType = null;
                }
                flag = oldType != newType;
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                contentPresenter.templateIsCurrent = false;
            }
            if (contentPresenter.templateIsCurrent && contentPresenter.Template != ContentPresenter.UIElementContentTemplate)
            {
                contentPresenter.DataContext = e.NewValue;
            }
            if (contentPresenter.TemplateInstance != null)
            {
                contentPresenter.TemplateInstance.DataContext = contentPresenter.Content;
            }
        }

        private static void OnContentStringFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentPresenter contentPresenter = (ContentPresenter)d;
            contentPresenter.OnContentStringFormatChanged((string)e.OldValue, (string)e.NewValue);
        }

        private static void OnContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentPresenter contentPresenter = (ContentPresenter)d;
            contentPresenter.templateIsCurrent = false;
            contentPresenter.OnContentTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }

        private static void OnContentTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentPresenter contentPresenter = (ContentPresenter)d;
            contentPresenter.templateIsCurrent = false;
            contentPresenter.OnContentTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue);
        }

        private static void OnTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentPresenter contentPresenter = (ContentPresenter)d;
            StyleHelper.UpdateTemplateCache(contentPresenter, (FrameworkTemplate)e.OldValue, (FrameworkTemplate)e.NewValue, ContentPresenter.TemplateProperty);
            contentPresenter.EnsureTemplate();
            ////if (contentPresenter.TemplateInstance == null)
            ////{
            ////contentPresenter.Template.VisualTree.SetBinding(UIElement.DataContextProperty, new Binding("") { Source = ((ContentPresenter)d).Content });
            contentPresenter.TemplateInstance = (UIElement)contentPresenter.Template.LoadContent();
            ////contentPresenter.TemplateInstance.SetBinding(UIElement.DataContextProperty, new Binding());
            ////contentPresenter.TemplateInstance.DataContext = contentPresenter.Content;
            ////}
        }

        private void EnsureTemplate()
        {
            DataTemplate template = this.Template;
            DataTemplate dataTemplate = null;
            this.templateIsCurrent = false;
            while (!this.templateIsCurrent)
            {
                this.templateIsCurrent = true;
                dataTemplate = this.ChooseTemplate();
                if (template != dataTemplate)
                {
                    this.Template = null;
                }
                if (dataTemplate == ContentPresenter.UIElementContentTemplate)
                {
                    this.ClearValue(UIElement.DataContextProperty);
                }
                else
                {
                    this.DataContext = this.Content;
                }
            }
            this.Template = dataTemplate;
            if (template == dataTemplate)
            {
                StyleHelper.DoTemplateInvalidations(this, template);
            }
        }

        private void Initialize()
        {
            PropertyMetadata metadata = ContentPresenter.TemplateProperty.GetMetadata(this.DependencyObjectType);
            DataTemplate defaultValue = (DataTemplate)metadata.DefaultValue;
            if (defaultValue != null)
            {
                ContentPresenter.OnTemplateChanged(this, new DependencyPropertyChangedEventArgs(ContentPresenter.TemplateProperty, null, defaultValue));
            }
            metadata = ContentPresenter.ContentTemplateProperty.GetMetadata(this.DependencyObjectType);
            defaultValue = (DataTemplate)metadata.DefaultValue;
            if (defaultValue != null)
            {
                ContentPresenter.OnContentTemplateChanged(this, new DependencyPropertyChangedEventArgs(ContentPresenter.TemplateProperty, null, defaultValue));
            }
            this.DataContext = null;
        }

        private DataTemplate SelectTemplateForString(string s)
        {
            var textBoxFactory = ContentPresenter.CreateTextBlockFactory();
            var binding = new Binding("DataContext");
            binding.Source = this;
            textBoxFactory.SetBinding(TextBlock.TextProperty, binding);
            return new DataTemplate() { VisualTree = textBoxFactory };
        }

        private class DefaultSelector : DataTemplateSelector
        {
            public DefaultSelector()
            {
            }

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                DataTemplate uiElementContentTemplate = null;
                TypeConverter typeConverter = null;
                string str = item as string;
                if (str != null)
                {
                    uiElementContentTemplate = ((ContentPresenter)container).SelectTemplateForString(str);
                }
                else if (item is UIElement)
                {
                    uiElementContentTemplate = ContentPresenter.UIElementContentTemplate;
                }
                else if (item != null)
                {
                    typeConverter = TypeDescriptor.GetConverter(item.GetType());
                    if (typeConverter != null && typeConverter.CanConvertTo(typeof(UIElement)))
                    {
                        uiElementContentTemplate = ContentPresenter.UIElementContentTemplate;
                        return uiElementContentTemplate;
                    }
                    uiElementContentTemplate = ((ContentPresenter)container).SelectTemplateForString(item.ToString());
                }
                else
                {
                    uiElementContentTemplate = ContentPresenter.DefaultContentTemplate;
                }
                return uiElementContentTemplate;
            }
        }

        private class DefaultTemplate : DataTemplate
        {
            public DefaultTemplate()
            {
                this.CanBuildVisualTree = true;                
                var frameworkElementFactory = ContentPresenter.CreateTextBlockFactory();
                this.VisualTree = frameworkElementFactory;
            }

            internal override bool BuildVisualTree(UIElement container)
            {                
                this.VisualTree.SetValue(TextBlock.TextProperty, new TemplateBindingExtension(ContentPresenter.ContentProperty));
                return true;
            }
        }

        private class UseContentTemplate : DataTemplate
        {
            public UseContentTemplate()
            {
                this.CanBuildVisualTree = true;
            }

            internal override bool BuildVisualTree(UIElement container)
            {
                object content = ((ContentPresenter)container).Content;
                UIElement uiElement = content as UIElement;
                if (uiElement == null)
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(content.GetType());
                    uiElement = (UIElement)converter.ConvertTo(content, typeof(UIElement));
                }
                StyleHelper.AddCustomTemplateRoot(container, uiElement);
                return true;
            }
        }
    }
}