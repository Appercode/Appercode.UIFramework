using Appercode.UI.Controls;
using System;
using System.Collections;
using System.Linq;
using System.Windows;

namespace Appercode.UI
{
    public static class LogicalTreeHelper
    {
        public static DependencyObject FindLogicalNode(DependencyObject logicalTreeNode, string elementName)
        {
            if (logicalTreeNode == null)
            {
                throw new ArgumentNullException(nameof(logicalTreeNode));
            }

            if (elementName == null)
            {
                throw new ArgumentNullException(nameof(elementName));
            }

            if (elementName == string.Empty)
            {
                throw new ArgumentException("Parameter cannot be a zero-length string.", nameof(elementName));
            }

            DependencyObject logicalNode = null;
            var logicalTreeNodeUIElement = logicalTreeNode as UIElement;
            if (logicalTreeNodeUIElement != null && logicalTreeNodeUIElement.Name == elementName)
            {
                logicalNode = logicalTreeNode;
            }

            if (logicalNode == null)
            {
                var logicalChildren = GetLogicalChildren(logicalTreeNode);
                if (logicalChildren != null)
                {
                    logicalChildren.Reset();
                    while (logicalNode == null && logicalChildren.MoveNext())
                    {
                        var current = logicalChildren.Current as DependencyObject;
                        if (current == null)
                        {
                            continue;
                        }

                        logicalNode = FindLogicalNode(current, elementName);
                    }
                }
            }

            return logicalNode;
        }

        public static IEnumerable GetChildren(DependencyObject current)
        {
            if (current == null)
            {
                throw new ArgumentNullException(nameof(current));
            }

            var currentUIElement = current as UIElement;
            if (currentUIElement != null)
            {
                return new EnumeratorWrapper(currentUIElement.LogicalChildren);
            }

            return EnumeratorWrapper.Empty;
        }

        public static DependencyObject GetParent(DependencyObject current)
        {
            if (current == null)
            {
                throw new ArgumentNullException(nameof(current));
            }

            return (current as UIElement)?.Parent;
        }

        internal static void AddLogicalChild(DependencyObject parent, object child)
        {
            if (child != null)
            {
                (parent as UIElement)?.AddLogicalChild(child);
            }
        }

        internal static IEnumerator GetLogicalChildren(DependencyObject current)
        {
            return (current as UIElement)?.LogicalChildren;
        }

        internal static void RemoveLogicalChild(DependencyObject parent, object child)
        {
            if (child != null)
            {
                (parent as UIElement)?.RemoveLogicalChild(child);
            }
        }

        internal class EnumeratorWrapper : IEnumerable
        {
            private static readonly Lazy<EnumeratorWrapper> EmptyInstance =
                new Lazy<EnumeratorWrapper>(CreateEmptyInstance);

            private readonly IEnumerator enumerator;

            public EnumeratorWrapper(IEnumerator enumerator)
            {
                this.enumerator = enumerator ?? Enumerable.Empty<object>().GetEnumerator();
            }

            internal static EnumeratorWrapper Empty => EmptyInstance.Value;

            private static EnumeratorWrapper CreateEmptyInstance()
            {
                return new EnumeratorWrapper(null);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.enumerator;
            }
        }
    }
}
