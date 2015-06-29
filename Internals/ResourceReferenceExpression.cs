using Appercode.UI.Controls;
using System;
using System.Windows;

namespace Appercode.UI.Internals
{
    internal class ResourceReferenceExpression : Expression
    {
        private object resourceKey;

        private object cachedResourceValue;

        private DependencyObject mentorCache;

        private DependencyObject targetObject;

        private DependencyProperty targetProperty;

        private ResourceReferenceExpression.InternalState state;

        ////private ResourceReferenceExpression.ResourceReferenceExpressionWeakContainer weakContainerRRE;

        public ResourceReferenceExpression(object resourceKey)
        {
            this.resourceKey = resourceKey;
        }

        [Flags]
        private enum InternalState : byte
        {
            Default = 0,
            HasCachedResourceValue = 1,
            IsMentorCacheValid = 2,
            DisableThrowOnResourceFailure = 4,
            IsListeningForFreezableChanges = 8,
            IsListeningForInflated = 16
        }

        public object ResourceKey
        {
            get
            {
                return this.resourceKey;
            }
        }

        internal override Expression Copy(DependencyObject targetObject, DependencyProperty targetDP)
        {
            return new ResourceReferenceExpression(this.ResourceKey);
        }

        internal object GetRawValue(DependencyObject d, out object source, DependencyProperty dp)
        {
            Appercode.UI.Controls.UIElement frameworkElement = null;
            object unsetValue = null;
            source = null;
            if (!this.ReadInternalState(ResourceReferenceExpression.InternalState.IsMentorCacheValid))
            {
                ////this.mentorCache = Helper.FindMentor(d);
                this.WriteInternalState(ResourceReferenceExpression.InternalState.IsMentorCacheValid, true);
                if (this.mentorCache != null && this.mentorCache != this.targetObject)
                {
                    ////Helper.DowncastToFEorFCE(this.mentorCache, out frameworkElement, out frameworkContentElement, true);
                    if (frameworkElement != null)
                    {
                        frameworkElement.ResourcesChanged += new EventHandler(this.InvalidateExpressionValue);
                    }
                }
            }

            /*
            if (this.mentorCache == null)
            {
                //unsetValue = Helper.FindResourceFromAppOrSystem(this.resourceKey, out source, false, true, false) ?? DependencyProperty.UnsetValue;                
            }
            else
            {
                //Helper.DowncastToFEorFCE(this.mentorCache, out frameworkElement1, out frameworkContentElement1, true);
                //unsetValue = UIElement.FindResourceInternal(frameworkElement1, frameworkContentElement1, dp, this.resourceKey, null, true, false, null, false, out source);
            }
            */
 
            if (unsetValue == null)
            {
                unsetValue = DependencyProperty.UnsetValue;
            }
            this.cachedResourceValue = unsetValue;
            this.WriteInternalState(ResourceReferenceExpression.InternalState.HasCachedResourceValue, true);
            object value = unsetValue;
            DeferredResourceReference deferredResourceReference = unsetValue as DeferredResourceReference;
            if (deferredResourceReference != null)
            {
                if (deferredResourceReference.IsInflated)
                {
                    value = deferredResourceReference.Value as Freezable;
                }
                else if (!this.ReadInternalState(ResourceReferenceExpression.InternalState.IsListeningForInflated))
                {
                    deferredResourceReference.AddInflatedListener(this);
                    this.WriteInternalState(ResourceReferenceExpression.InternalState.IsListeningForInflated, true);
                }
            }
            this.ListenForFreezableChanges(value);
            return unsetValue;
        }

        internal override DependencySource[] GetSources()
        {
            return null;
        }

        internal override object GetValue(DependencyObject d, DependencyProperty dp)
        {
            object obj;
            if (d == null)
            {
                throw new ArgumentNullException("d");
            }
            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }
            if (this.ReadInternalState(ResourceReferenceExpression.InternalState.HasCachedResourceValue))
            {
                return this.cachedResourceValue;
            }
            return this.GetRawValue(d, out obj, dp);
        }

        internal void InvalidateExpressionValue(object sender, EventArgs e)
        {
            ResourcesChangedEventArgs resourcesChangedEventArg = e as ResourcesChangedEventArgs;
            if (resourcesChangedEventArg == null)
            {
                this.InvalidateMentorCache();
            }
            else if (resourcesChangedEventArg.Info.IsTreeChange)
            {
                this.InvalidateMentorCache();
            }
            else
            {
                this.InvalidateCacheValue();
            }
            this.InvalidateTargetProperty(sender, e);
        }

        internal override void OnAttach(DependencyObject d, DependencyProperty dp)
        {
            throw new NotImplementedException();
            /*
            //this.targetObject = d;
            //this.targetProperty = dp;
            //FrameworkObject frameworkObject = new FrameworkObject(this.targetObject);
            //frameworkObject.HasResourceReference = true;
            //if (!frameworkObject.IsValid)
            //{
            //    this.targetObject.InheritanceContextChanged += new EventHandler(this.InvalidateExpressionValue);
            //}
            */
        }

        internal void OnDeferredResourceInflated(DeferredResourceReference deferredResourceReference)
        {
            if (this.ReadInternalState(ResourceReferenceExpression.InternalState.IsListeningForInflated))
            {
                deferredResourceReference.RemoveInflatedListener(this);
                this.WriteInternalState(ResourceReferenceExpression.InternalState.IsListeningForInflated, false);
            }
            this.ListenForFreezableChanges(deferredResourceReference.Value);
        }

        internal override void OnDetach(DependencyObject d, DependencyProperty dp)
        {
            this.InvalidateMentorCache();
            if (!(this.targetObject is Appercode.UI.Controls.UIElement))
            {
                this.targetObject.InheritanceContextChanged -= new EventHandler(this.InvalidateExpressionValue);
            }
            this.targetObject = null;
            this.targetProperty = null;
            
            ////this.weakContainerRRE = null;
        }

        internal override bool SetValue(DependencyObject d, DependencyProperty dp, object value)
        {
            return false;
        }

        private void InvalidateCacheValue()
        {
            object value = this.cachedResourceValue;
            DeferredResourceReference deferredResourceReference = this.cachedResourceValue as DeferredResourceReference;
            if (deferredResourceReference != null)
            {
                if (deferredResourceReference.IsInflated)
                {
                    value = deferredResourceReference.Value;
                }
                else if (this.ReadInternalState(ResourceReferenceExpression.InternalState.IsListeningForInflated))
                {
                    deferredResourceReference.RemoveInflatedListener(this);
                    this.WriteInternalState(ResourceReferenceExpression.InternalState.IsListeningForInflated, false);
                }
                deferredResourceReference.RemoveFromDictionary();
            }
            this.StopListeningForFreezableChanges(value);
            this.cachedResourceValue = null;
            this.WriteInternalState(ResourceReferenceExpression.InternalState.HasCachedResourceValue, false);
        }

        private void InvalidateMentorCache()
        {
            Appercode.UI.Controls.UIElement frameworkElement = null;
            if (this.ReadInternalState(ResourceReferenceExpression.InternalState.IsMentorCacheValid))
            {
                if (this.mentorCache != null)
                {
                    if (this.mentorCache != this.targetObject)
                    {
                        ////Helper.DowncastToFEorFCE(this.mentorCache, out frameworkElement, out frameworkContentElement, true);
                        if (frameworkElement != null)
                        {
                            frameworkElement.ResourcesChanged -= new EventHandler(this.InvalidateExpressionValue);
                        }
                    }
                    this.mentorCache = null;
                }
                this.WriteInternalState(ResourceReferenceExpression.InternalState.IsMentorCacheValid, false);
            }
            this.InvalidateCacheValue();
        }

        private void InvalidateTargetProperty(object sender, EventArgs e)
        {
            this.targetObject.InvalidateProperty(this.targetProperty);
        }

        private void InvalidateTargetSubProperty(object sender, EventArgs e)
        {
            this.targetObject.NotifySubPropertyChange(this.targetProperty);
        }

        private void ListenForFreezableChanges(object resource)
        {
            /*
            //if (!this.ReadInternalState(ResourceReferenceExpression.InternalState.IsListeningForFreezableChanges))
            //{
            //    Freezable freezable = resource as Freezable;
            //    if (freezable != null && !freezable.IsFrozen)
            //    {
            //        if (this.weakContainerRRE == null)
            //        {
            //            this.weakContainerRRE = new ResourceReferenceExpression.ResourceReferenceExpressionWeakContainer(this);
            //        }
            //        this.weakContainerRRE.AddChangedHandler(freezable);
            //        this.WriteInternalState(ResourceReferenceExpression.InternalState.IsListeningForFreezableChanges, true);
            //    }
            //}
            */
        }

        private bool ReadInternalState(ResourceReferenceExpression.InternalState reqFlag)
        {
            return (byte)(this.state & reqFlag) != 0;
        }

        private void StopListeningForFreezableChanges(object resource)
        {
            /*
            //if (this.ReadInternalState(ResourceReferenceExpression.InternalState.IsListeningForFreezableChanges))
            //{
            //    Freezable freezable = resource as Freezable;
            //    if (freezable != null && this.weakContainerRRE != null)
            //    {
            //        if (freezable.IsFrozen)
            //        {
            //            this.weakContainerRRE = null;
            //        }
            //        else
            //        {
            //            this.weakContainerRRE.RemoveChangedHandler();
            //        }
            //    }
            //    this.WriteInternalState(ResourceReferenceExpression.InternalState.IsListeningForFreezableChanges, false);
            //}
            */ 
        }

        private void WriteInternalState(ResourceReferenceExpression.InternalState reqFlag, bool set)
        {
            if (set)
            {
                ResourceReferenceExpression resourceReferenceExpression = this;
                resourceReferenceExpression.state = (ResourceReferenceExpression.InternalState)((byte)(resourceReferenceExpression.state | reqFlag));
                return;
            }
            ResourceReferenceExpression resourceReferenceExpression1 = this;
            resourceReferenceExpression1.state = (ResourceReferenceExpression.InternalState)((byte)((byte)resourceReferenceExpression1.state & (byte)(~reqFlag)));
        }

        private class ResourceReferenceExpressionWeakContainer : WeakReference
        {
            private Freezable resource;

            public ResourceReferenceExpressionWeakContainer(ResourceReferenceExpression target)
                : base(target)
            {
            }

            public void AddChangedHandler(Freezable resource)
            {
                if (this.resource != null)
                {
                    this.RemoveChangedHandler();
                }
                this.resource = resource;
                this.resource.Changed += new EventHandler(this.InvalidateTargetSubProperty);
            }

            public void RemoveChangedHandler()
            {
                if (!this.resource.IsFrozen)
                {
                    this.resource.Changed -= new EventHandler(this.InvalidateTargetSubProperty);
                    this.resource = null;
                }
            }

            private void InvalidateTargetSubProperty(object sender, EventArgs args)
            {
                ResourceReferenceExpression target = (ResourceReferenceExpression)this.Target;
                if (target == null)
                {
                    this.RemoveChangedHandler();
                    return;
                }
                target.InvalidateTargetSubProperty(sender, args);
            }
        }
    }
}
