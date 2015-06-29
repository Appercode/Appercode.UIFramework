using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Appercode.UI.Controls.Primitives;

namespace Appercode.UI.Controls
{
    public partial class RadioButton : ToggleButton
    {
        public static readonly DependencyProperty GroupNameProperty =
                    DependencyProperty.Register("GroupName", typeof(string), typeof(RadioButton),
                                                new PropertyMetadata("", (s, e) => { }));

        private static Dictionary<string, List<WeakReference>> groupNameToElements;

        public RadioButton()
        {
            RadioButton.Register("", this);
        }

        public string GroupName
        {
            get
            {
                return this.GetValue(RadioButton.GroupNameProperty) as string;
            }
            set
            {
                if (value != this.GetValue(RadioButton.GroupNameProperty) as string)
                {
                    RadioButton.Unregister(this.GetValue(RadioButton.GroupNameProperty) as string, this);
                    RadioButton.Register(value, this);
                    this.SetValue(RadioButton.GroupNameProperty, value);
                }
            }
        }

        protected override void OnClick()
        {
            string groupName = this.GroupName;

            if (groupName != null)
            {
                base.OnClick();

                List<RadioButton> radioButtons = new List<RadioButton>();

                List<DependencyObject> parents = LogicalTreeHelper.GetChildren(AppercodeVisualRoot.Instance.Child as DependencyObject).Cast<DependencyObject>().ToList();

                while (parents.Count > 0)
                {
                    List<DependencyObject> newParents = new List<DependencyObject>();

                    for (int i = 0; i < parents.Count(); i++)
                    {
                        DependencyObject parent = parents.ElementAt(i);

                        if (parent is RadioButton && ((RadioButton)parent).GroupName == groupName)
                        {
                            radioButtons.Add((RadioButton)parent);
                        }

                        var children = LogicalTreeHelper.GetChildren(parent);

                        if (children != LogicalTreeHelper.EnumeratorWrapper.Empty)
                        {
                            foreach (var child in children)
                            {
                                var depObj = child as DependencyObject;
                                if (depObj != null)
                                {
                                    newParents.Add(depObj);
                                }
                            }
                        }
                    }

                    parents = newParents;
                }

                foreach (RadioButton radioButton in radioButtons)
                {
                    if (radioButton != this)
                    {
                        radioButton.IsChecked = false;
                    }
                    else
                    {
                        radioButton.IsChecked = true;
                    }
                }
            }
        }

        private static void PurgeExpiredReferences(List<WeakReference> elements, object elementToRemove)
        {
            int num = 0;
            while (num < elements.Count)
            {
                object target = elements[num].Target;
                if (target == null || target == elementToRemove)
                {
                    elements.RemoveAt(num);
                }
                else
                {
                    num++;
                }
            }
        }

        private static void Register(string groupName, RadioButton radioButton)
        {
            if (groupName == null)
            {
                groupName = "";
            }
            if (RadioButton.groupNameToElements == null)
            {
                RadioButton.groupNameToElements = new Dictionary<string, List<WeakReference>>(1);
            }
            List<WeakReference> weakReferences = null;
            if (!RadioButton.groupNameToElements.TryGetValue(groupName, out weakReferences) || weakReferences == null)
            {
                weakReferences = new List<WeakReference>(1);
                RadioButton.groupNameToElements[groupName] = weakReferences;
            }
            else
            {
                RadioButton.PurgeExpiredReferences(weakReferences, null);
            }
            weakReferences.Add(new WeakReference(radioButton));
        }

        private static void Unregister(string groupName, RadioButton radioButton)
        {
            List<WeakReference> weakReferences;
            if (groupName == null)
            {
                groupName = "";
            }
            if (RadioButton.groupNameToElements != null && RadioButton.groupNameToElements.TryGetValue(groupName, out weakReferences) && weakReferences != null)
            {
                RadioButton.PurgeExpiredReferences(weakReferences, radioButton);
                if (weakReferences.Count == 0)
                {
                    RadioButton.groupNameToElements.Remove(groupName);
                }
            }
        }
    }
}