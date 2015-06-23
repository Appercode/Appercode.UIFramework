using Appercode.UI.Controls;
using System;
using System.Collections;
using System.Windows;

namespace Appercode.UI
{
    public static class LogicalTreeHelper
    {
        public static DependencyObject FindLogicalNode(DependencyObject logicalTreeNode, string elementName)
        {
            if (logicalTreeNode == null)
            {
                throw new ArgumentNullException("logicalTreeNode");
            }
            if (elementName == null)
            {
                throw new ArgumentNullException("elementName");
            }
            if (elementName == string.Empty)
            {
                throw new ArgumentException("Parameter cannot be a zero-length string.", "elementName");
            }

            DependencyObject logicalNode = null;
            Appercode.UI.Controls.UIElement logicalTreeNodeUIElement = logicalTreeNode as Appercode.UI.Controls.UIElement;
            if (logicalTreeNodeUIElement != null && logicalTreeNodeUIElement.Name == elementName)
            {
                logicalNode = logicalTreeNode;
            }
            if (logicalNode == null)
            {
                IEnumerator logicalChildren = LogicalTreeHelper.GetLogicalChildren(logicalTreeNode);
                if (logicalChildren != null)
                {
                    logicalChildren.Reset();
                    while (logicalNode == null && logicalChildren.MoveNext())
                    {
                        DependencyObject current = logicalChildren.Current as DependencyObject;
                        if (current == null)
                        {
                            continue;
                        }
                        logicalNode = LogicalTreeHelper.FindLogicalNode(current, elementName);
                    }
                }
            }
            return logicalNode;
        }

        public static IEnumerable GetChildren(DependencyObject current)
        {
            if (current == null)
            {
                throw new ArgumentNullException("current");
            }

            Appercode.UI.Controls.UIElement currentUIElement = current as Appercode.UI.Controls.UIElement;
            if (currentUIElement != null)
            {
                return new LogicalTreeHelper.EnumeratorWrapper(currentUIElement.LogicalChildren);
            }
            return EnumeratorWrapper.Empty;
        }

        public static DependencyObject GetParent(DependencyObject current)
        {
            if (current == null)
            {
                throw new ArgumentNullException("current");
            }

            Appercode.UI.Controls.UIElement currentUIElement = current as Appercode.UI.Controls.UIElement;
            if (currentUIElement != null)
            {
                return currentUIElement.Parent;
            }
            return null;
        }

#warning was internal, conflicted with maps
        public static void AddLogicalChild(DependencyObject parent, object child)
        {
            if (child != null && parent != null)
            {
                Appercode.UI.Controls.UIElement parentUIElement = parent as Appercode.UI.Controls.UIElement;
                if (parentUIElement != null)
                {
                    parentUIElement.AddLogicalChild(child);
                    return;
                }
            }
        }
        
        internal static IEnumerator GetLogicalChildren(DependencyObject current)
        {
            Appercode.UI.Controls.UIElement currentUIElement = current as Appercode.UI.Controls.UIElement;
            if (currentUIElement != null)
            {
                return currentUIElement.LogicalChildren;
            }
            return null;
        }

        internal static void RemoveLogicalChild(DependencyObject parent, object child)
        {
            if (child != null && parent != null)
            {
                Appercode.UI.Controls.UIElement parentUIElement = parent as Appercode.UI.Controls.UIElement;
                if (parentUIElement != null)
                {
                    parentUIElement.RemoveLogicalChild(child);
                    return;
                }                
            }
        }

        public class EnumeratorWrapper : IEnumerable
        {
            private static LogicalTreeHelper.EnumeratorWrapper emptyInstance;
            
            private IEnumerator enumerator;

            public EnumeratorWrapper(IEnumerator enumerator)
            {
                if (enumerator != null)
                {
                    this.enumerator = enumerator;
                    return;
                }
                this.enumerator = EmptyEnumerator.Instance;
            }

            internal static LogicalTreeHelper.EnumeratorWrapper Empty
            {
                get
                {
                    if (LogicalTreeHelper.EnumeratorWrapper.emptyInstance == null)
                    {
                        LogicalTreeHelper.EnumeratorWrapper.emptyInstance = new LogicalTreeHelper.EnumeratorWrapper(null);
                    }
                    return LogicalTreeHelper.EnumeratorWrapper.emptyInstance;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.enumerator;
            }
        }

        private class EmptyEnumerator : IEnumerator
        {
            private static IEnumerator instance;

            public EmptyEnumerator()
            {
            }

            public static IEnumerator Instance
            {
                get
                {
                    if (instance == null)
                    {
                        instance = new EmptyEnumerator();
                    }
                    return instance;
                }
            }

            public object Current
            {
                get
                {
                    throw new InvalidOperationException();
                }
            }

            public void Reset()
            {
            }

            public bool MoveNext()
            {
                return false;
            }
        }
    }
}
