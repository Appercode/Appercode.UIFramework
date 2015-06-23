using Appercode.UI.StylesAndResources;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Appercode.UI.Internals
{
    internal class DeferredResourceReference : DeferredReference
    {
        protected object keyOrValue;
        
        private ResourceDictionary dictionary;        

        private bool checkIfShared;

        private List<WeakReference> inflatedList;

        internal DeferredResourceReference(ResourceDictionary dictionary, object key)
        {
            this.dictionary = dictionary;
            this.keyOrValue = key;
        }

        internal bool CheckIfShared
        {
            get
            {
                return this.checkIfShared;
            }
            set
            {
                this.checkIfShared = value;
            }
        }

        internal ResourceDictionary Dictionary
        {
            get
            {
                return this.dictionary;
            }
            set
            {
                this.dictionary = value;
            }
        }

        internal bool IsInflated
        {
            get
            {
                return this.dictionary == null;
            }
        }

        internal virtual bool IsUnset
        {
            get
            {
                return false;
            }
        }

        internal virtual object Key
        {
            get
            {
                return this.keyOrValue;
            }
        }

        internal virtual object Value
        {
            get
            {
                return this.keyOrValue;
            }
            set
            {
                this.keyOrValue = value;
            }
        }

        internal virtual void AddInflatedListener(ResourceReferenceExpression listener)
        {
            if (this.inflatedList == null)
            {
                this.inflatedList = new List<WeakReference>();
            }
            this.inflatedList.Add(new WeakReference(listener));
        }

        internal override object GetValue(BaseValueSourceInternal valueSource)
        {
            bool flag;
            if (this.dictionary == null)
            {
                return this.keyOrValue;
            }
            object value = this.dictionary.GetValue(this.keyOrValue, out flag);
            if (!this.CheckIfShared || flag)
            {
                this.keyOrValue = value;
                this.RemoveFromDictionary();
            }
            if (valueSource == BaseValueSourceInternal.ThemeStyle || valueSource == BaseValueSourceInternal.ThemeStyleTrigger || valueSource == BaseValueSourceInternal.Style || valueSource == BaseValueSourceInternal.TemplateTrigger || valueSource == BaseValueSourceInternal.StyleTrigger || valueSource == BaseValueSourceInternal.ParentTemplate || valueSource == BaseValueSourceInternal.ParentTemplateTrigger)
            {
                StyleHelper.SealIfSealable(value);
            }
            this.OnInflated();
            return value;
        }

        internal override Type GetValueType()
        {
            bool flag;
            if (this.dictionary != null)
            {
                return this.dictionary.GetValueType(this.keyOrValue, out flag);
            }
            if (this.keyOrValue == null)
            {
                return null;
            }
            return this.keyOrValue.GetType();
        }

        internal virtual void RemoveFromDictionary()
        {
            if (this.dictionary != null)
            {
                this.dictionary.DeferredResourceReferences.RemoveAll(r => r.Target == this);
                this.dictionary = null;
            }
        }

        internal virtual void RemoveInflatedListener(ResourceReferenceExpression listener)
        {
            if (this.inflatedList != null)
            {
                this.inflatedList.RemoveAll(r => r.Target == listener);
            }
        }

        private void OnInflated()
        {
            if (this.inflatedList != null)
            {
                IEnumerator enumerator = this.inflatedList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ((ResourceReferenceExpression)enumerator.Current).OnDeferredResourceInflated(this);
                }
            }
        }
    }
}
