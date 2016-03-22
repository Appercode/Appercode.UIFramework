using Appercode.UI.Data;
using Appercode.UI.Internals;
using Appercode.UI.Markup;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;

namespace Appercode.UI.Controls
{
    public partial class ContentPresenter : UIElement
    {
        /// <summary>
        /// Identifies the <see cref="Content" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="Content" /> dependency property.</returns>
        public static readonly DependencyProperty ContentProperty;

        /// <summary>
        /// Identifies the <see cref="ContentTemplate" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="ContentTemplate" /> dependency property.</returns>
        public static readonly DependencyProperty ContentTemplateProperty;

        /// <summary>
        /// Identifies the <see cref="ContentTemplateSelector" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="ContentTemplateSelector" /> dependency property.</returns>
        public static readonly DependencyProperty ContentTemplateSelectorProperty;

        /// <summary>
        /// Identifies the <see cref="ContentStringFormat" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="ContentStringFormat" /> dependency property.</returns>
        public static readonly DependencyProperty ContentStringFormatProperty;

        internal static readonly DependencyProperty TemplateProperty;

        private static readonly DataTemplate UIElementContentTemplate;
        private static readonly Lazy<DefaultTemplate> DefaultContentTemplate = new Lazy<DefaultTemplate>();
        private static readonly Lazy<DefaultSelector> DefaultTemplateSelector = new Lazy<DefaultSelector>();

        private readonly Lazy<DataTemplate> stringTemplate;
        private DataTemplate stringFormattingTemplate;
        private bool templateIsCurrent;
        private UIElement templateInstance;

        static ContentPresenter()
        {
            ContentTemplateProperty = ContentControl.ContentTemplateProperty.AddOwner(
                typeof(ContentPresenter), new PropertyMetadata(new DefaultTemplate(), OnContentTemplateChanged));
            ContentProperty = ContentControl.ContentProperty.AddOwner(typeof(ContentPresenter), new PropertyMetadata(OnContentChanged));
            ContentTemplateSelectorProperty = ContentControl.ContentTemplateSelectorProperty.AddOwner(
                typeof(ContentPresenter), new PropertyMetadata(OnContentTemplateSelectorChanged));
            ContentStringFormatProperty = DependencyProperty.Register(
                nameof(ContentStringFormat), typeof(string), typeof(ContentPresenter), new PropertyMetadata(OnContentStringFormatChanged));
            TemplateProperty = DependencyProperty.Register(
                nameof(Template), typeof(DataTemplate), typeof(ContentPresenter), new PropertyMetadata(OnTemplateChanged));

            // TODO: Check if current implementation of UIElementContentTemplate works
            // UIElementContentTemplate = new UseContentTemplate();
            var frameworkElementFactory = new FrameworkElementFactory(typeof(ContentControl));
            frameworkElementFactory.SetValue(ContentControl.ContentProperty, new TemplateBindingExtension(ContentProperty));
            UIElementContentTemplate = new DataTemplate { VisualTree = frameworkElementFactory };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPresenter" /> class.
        /// </summary>
        public ContentPresenter()
        {
            this.stringTemplate = new Lazy<DataTemplate>(this.CreateStringTemplate);
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the data used to generate the child elements of a <see cref="ContentPresenter" />. This is a dependency property.
        /// </summary>
        /// <returns>The data used to generate the child elements. The default is null.</returns>
        public object Content
        {
            get { return this.GetValue(ContentControl.ContentProperty); }
            set { this.SetValue(ContentControl.ContentProperty, value); }
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

        /// <summary>
        /// Gets or sets the template used to display the content of the control. This is a dependency property.
        /// </summary>
        /// <returns>A <see cref="DataTemplate" /> that defines the visualization of the content. The default is null.</returns>
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)this.GetValue(ContentControl.ContentTemplateProperty); }
            set { this.SetValue(ContentControl.ContentTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplateSelector" />, which allows the application writer to provide custom logic
        /// for choosing the template that is used to display the content of the control. This is a dependency property.
        /// </summary>
        /// <returns>A <see cref="DataTemplateSelector" /> object that supplies logic to return a <see cref="DataTemplate" /> to apply. The default is null.</returns>
        public DataTemplateSelector ContentTemplateSelector
        {
            get { return (DataTemplateSelector)this.GetValue(ContentControl.ContentTemplateSelectorProperty); }
            set { this.SetValue(ContentControl.ContentTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Gets an enumerator for logical children of this element.
        /// </summary>
        protected internal override IEnumerator LogicalChildren
        {
            get
            {
                if (this.Content != null)
                {
                    yield return this.Content;
                }

                if (this.TemplateInstance != null)
                {
                    yield return this.TemplateInstance;
                }
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
            get { return (DataTemplate)this.GetValue(TemplateProperty); }
            set { this.SetValue(TemplateProperty, value); }
        }

        internal override FrameworkTemplate InternalTemplate
        {
            get { return this.Template; }
        }

        internal override void ApplyTemplate()
        {
            base.ApplyTemplate();
            if (this.templateIsCurrent == false)
            {
                this.EnsureTemplate();
                this.templateIsCurrent = true;
            }
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

        /// <summary>
        /// Invoked when the <see cref="ContentTemplateSelector" /> property changes.
        /// </summary>
        /// <param name="oldContentTemplateSelector">The old value of the <see cref="ContentTemplateSelector" /> property.</param>
        /// <param name="newContentTemplateSelector">The new value of the <see cref="ContentTemplateSelector" /> property.</param>
        protected virtual void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector)
        {
            this.Template = null;
        }

        /// <summary>
        /// Invoked when the <see cref="ContentTemplate" /> property changes.
        /// </summary>
        /// <param name="oldContentTemplate">The old value of the <see cref="ContentTemplate" /> property.</param>
        /// <param name="newContentTemplate">The new value of the <see cref="ContentTemplate" /> property.</param>
        protected virtual void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate)
        {
            this.Template = newContentTemplate;
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var contentPresenter = (ContentPresenter)d;
            if (contentPresenter.templateIsCurrent == false)
            {
                return;
            }

            bool templateIsChanged;
            if (contentPresenter.ContentTemplate != null)
            {
                templateIsChanged = false;
            }
            else if (contentPresenter.ContentTemplateSelector != null)
            {
                templateIsChanged = true;
            }
            else if (contentPresenter.Template == UIElementContentTemplate)
            {
                templateIsChanged = true;
                contentPresenter.Template = null;
            }
            else if (contentPresenter.Template != DefaultContentTemplate.Value)
            {
                var oldValueType = e.OldValue?.GetType();
                var newValueType = e.NewValue?.GetType();
                templateIsChanged = oldValueType != newValueType;
            }
            else
            {
                templateIsChanged = true;
            }

            if (templateIsChanged)
            {
                contentPresenter.templateIsCurrent = false;
            }

            if (contentPresenter.templateIsCurrent && contentPresenter.Template != UIElementContentTemplate)
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
            var newValue = e.NewValue as FrameworkTemplate;
            if (newValue != null)
            {
                var contentPresenter = (ContentPresenter)d;
                contentPresenter.TemplateInstance = (UIElement)newValue.LoadContent();
            }
        }

        private void EnsureTemplate()
        {
            var oldTemplate = this.Template;
            DataTemplate newTemplate = null;
            this.templateIsCurrent = false;
            while (this.templateIsCurrent == false)
            {
                this.templateIsCurrent = true;
                newTemplate = this.ChooseTemplate();
                if (oldTemplate != newTemplate)
                {
                    this.Template = null;
                }

                if (newTemplate == UIElementContentTemplate)
                {
                    this.ClearValue(DataContextProperty);
                }
                else
                {
                    this.DataContext = this.Content;
                }
            }

            this.Template = newTemplate;
        }

        private DataTemplate ChooseTemplate()
        {
            var content = this.Content;
            var contentTemplate = this.ContentTemplate;

            DataTemplateSelector templateSelector;
            if (contentTemplate == null && (templateSelector = this.ContentTemplateSelector) != null)
            {
                contentTemplate = templateSelector.SelectTemplate(content, this);
            }

            if (contentTemplate == null)
            {
                contentTemplate = DefaultTemplateSelector.Value.SelectTemplate(content, this);
            }

            return contentTemplate;
        }

        private void Initialize()
        {
            var metadata = TemplateProperty.GetMetadata(this.DependencyObjectType);
            var defaultValue = (DataTemplate)metadata.DefaultValue;
            if (defaultValue != null)
            {
                OnTemplateChanged(this, new DependencyPropertyChangedEventArgs(TemplateProperty, null, defaultValue));
            }

            metadata = ContentTemplateProperty.GetMetadata(this.DependencyObjectType);
            defaultValue = (DataTemplate)metadata.DefaultValue;
            if (defaultValue != null)
            {
                OnContentTemplateChanged(this, new DependencyPropertyChangedEventArgs(ContentTemplateProperty, null, defaultValue));
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
                    return DefaultContentTemplate.Value;
                }
            }
        }

        private class DefaultTemplate : DataTemplate
        {
            public DefaultTemplate()
            {
                this.CanBuildVisualTree = true;
                var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                textBlockFactory.SetBinding(TextBlock.TextProperty, new Binding());
                this.VisualTree = textBlockFactory;
                this.Seal();
            }

            // TODO: Check if this override can be safely removed
            internal override bool BuildVisualTree(UIElement container)
            {
                this.VisualTree.SetValue(TextBlock.TextProperty, new TemplateBindingExtension(ContentProperty));
                return true;
            }
        }

        // TODO: Check if this class can be safely removed
        private class UseContentTemplate : DataTemplate
        {
            public UseContentTemplate()
            {
                this.CanBuildVisualTree = true;
                this.Seal();
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