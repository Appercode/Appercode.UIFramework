using Appercode.UI.Data;
using Appercode.UI.Internals;
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
        /// <summary>Identifies the <see cref="P:System.Windows.Controls.ContentPresenter.Content" /> dependency property. </summary>
        /// <returns>The identifier for the <see cref="P:System.Windows.Controls.ContentPresenter.Content" /> dependency property.</returns>
        public static readonly DependencyProperty ContentProperty;

        /// <summary>Identifies the <see cref="P:System.Windows.Controls.ContentPresenter.ContentTemplate" /> dependency property. </summary>
        /// <returns>The identifier for the <see cref="P:System.Windows.Controls.ContentPresenter.ContentTemplate" /> dependency property.</returns>
        public static readonly DependencyProperty ContentTemplateProperty;

        /// <summary>Identifies the <see cref="P:System.Windows.Controls.ContentPresenter.ContentTemplateSelector" /> dependency property. </summary>
        /// <returns>The identifier for the <see cref="P:System.Windows.Controls.ContentPresenter.ContentTemplateSelector" /> dependency property.</returns>
        public static readonly DependencyProperty ContentTemplateSelectorProperty;

        /// <summary>
        /// Identifies the <see cref="ContentStringFormat" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="ContentStringFormat" /> dependency property.</returns>
        public static readonly DependencyProperty ContentStringFormatProperty;

        internal static readonly DependencyProperty TemplateProperty;

        private static DataTemplate uiElementTemplate;
        private static DataTemplate defaultTemplate;
        private static ContentPresenter.DefaultSelector defaultTemplateSelector;

        private readonly Lazy<DataTemplate> stringTemplate;
        private DataTemplate stringFormattingTemplate;
        private bool templateIsCurrent;
        private UIElement templateInstance;

        static ContentPresenter()
        {
            ContentPresenter.ContentTemplateProperty = ContentControl.ContentTemplateProperty.AddOwner(typeof(ContentPresenter), new PropertyMetadata(new ContentPresenter.DefaultTemplate(), new PropertyChangedCallback(ContentPresenter.OnContentTemplateChanged)));
            ContentPresenter.ContentProperty = ContentControl.ContentProperty.AddOwner(typeof(ContentPresenter), new PropertyMetadata(null, new PropertyChangedCallback(ContentPresenter.OnContentChanged)));
            ContentPresenter.ContentTemplateSelectorProperty = ContentControl.ContentTemplateSelectorProperty.AddOwner(typeof(ContentPresenter), new PropertyMetadata(null, new PropertyChangedCallback(ContentPresenter.OnContentTemplateSelectorChanged)));
            ContentStringFormatProperty = DependencyProperty.Register(
                nameof(ContentStringFormat), typeof(string), typeof(ContentPresenter), new PropertyMetadata(OnContentStringFormatChanged));
            ContentPresenter.TemplateProperty = DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ContentPresenter), new PropertyMetadata(null, new PropertyChangedCallback(ContentPresenter.OnTemplateChanged)));

            DataTemplate dataTemplate;
            FrameworkElementFactory frameworkElementFactory;
            //dataTemplate = new ContentPresenter.UseContentTemplate();
            //dataTemplate.Seal();
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPresenter" /> class.
        /// </summary>
        public ContentPresenter()
        {
            this.stringTemplate = new Lazy<DataTemplate>(this.CreateStringTemplate);
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

        /// <summary>
        /// Gets or sets a composite string that specifies how to format the <see cref="Content" /> property if it is displayed as a string.
        /// </summary>
        /// <returns>A composite string that specifies how to format the <see cref="Content" /> property if it is displayed as a string. The default is null.</returns>
        public string ContentStringFormat
        {
            get { return (string)this.GetValue(ContentStringFormatProperty); }
            set { this.SetValue(ContentStringFormatProperty, value); }
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

        internal bool TemplateIsCurrent
        {
            get
            {
                return this.templateIsCurrent;
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

        private DataTemplate FormattingStringContentTemplate
        {
            get
            {
                if (this.stringFormattingTemplate == null)
                {
                    var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                    var binding = new Binding { StringFormat = this.ContentStringFormat };
                    textBlockFactory.SetBinding(TextBlock.TextProperty, binding);
                    this.stringFormattingTemplate = new DataTemplate();
                    this.stringFormattingTemplate.VisualTree = textBlockFactory;
                    this.stringFormattingTemplate.Seal();
                }

                return this.stringFormattingTemplate;
            }
        }

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

        /// <summary>
        /// Invoked when the <see cref="ContentStringFormat" /> property changes.
        /// </summary>
        /// <param name="oldContentStringFormat">The old value of the <see cref="ContentStringFormat" /> property.</param>
        /// <param name="newContentStringFormat">The new value of the <see cref="ContentStringFormat" /> property.</param>
        protected virtual void OnContentStringFormatChanged(string oldContentStringFormat, string newContentStringFormat)
        {
            this.stringFormattingTemplate = null;
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
            return String.IsNullOrEmpty(this.ContentStringFormat)
                ? this.stringTemplate.Value : this.FormattingStringContentTemplate;
        }

        private DataTemplate CreateStringTemplate()
        {
            var textBoxFactory = new FrameworkElementFactory(typeof(TextBlock));
            var binding = new Binding(nameof(DataContext)) { Source = this };
            textBoxFactory.SetBinding(TextBlock.TextProperty, binding);
            return new DataTemplate { VisualTree = textBoxFactory };
        }

        private class DefaultSelector : DataTemplateSelector
        {
            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                var stringItem = item as string;
                if (stringItem != null)
                {
                    return ((ContentPresenter)container).SelectTemplateForString(stringItem);
                }
                else if (item is UIElement)
                {
                    return UIElementContentTemplate;
                }
                else if (item != null)
                {
                    var typeConverter = TypeDescriptor.GetConverter(item.GetType());
                    if (typeConverter != null && typeConverter.CanConvertTo(typeof(UIElement)))
                    {
                        return UIElementContentTemplate;
                    }

                    return ((ContentPresenter)container).SelectTemplateForString(item.ToString());
                }
                else
                {
                    return DefaultContentTemplate;
                }
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