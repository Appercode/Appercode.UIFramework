using Appercode.UI.Controls;
using System;
using System.Windows;

namespace Appercode.UI
{
    public abstract partial class FrameworkTemplate : DependencyObject
    {
        #region Declarations
        private InternalFlags flags;
        private FrameworkElementFactory templateRoot;

        [Flags]
        private enum InternalFlags : uint
        {
            CanBuildVisualTree = 4,
            HasLoadedChangeHandler = 8,
            HasContainerResourceReferences = 16,
            HasChildResourceReferences = 32
        }
        #endregion

        #region Properties
        /// <summary>Gets or sets the root node of the template.</summary>
        /// <returns>The root <see cref="FrameworkElementFactory"/> of the template.</returns>
        public FrameworkElementFactory VisualTree
        {
            get
            {
                this.VerifyAccess();
                return this.templateRoot;
            }
            set
            {
                this.VerifyAccess();
                this.CheckSealed();
                this.templateRoot = value;
            }
        }

        public new bool IsSealed { get; private set; }

        #region Internal members
        internal bool CanBuildVisualTree
        {
            get { return this.ReadInternalFlag(InternalFlags.CanBuildVisualTree); }
            set { this.WriteInternalFlag(InternalFlags.CanBuildVisualTree, value); }
        }
        #endregion

        #endregion

        #region Methods
        /// <summary>Locks the template so it cannot be changed.</summary>
        public new void Seal()
        {
            this.VerifyAccess();
            this.IsSealed = true;
            ////StyleHelper.SealTemplate(this, ref this._sealed, this._templateRoot, this.TriggersInternal, this._resources, this._childIndexFromChildName, ref this.ChildRecordFromChildIndex, ref this.TriggerSourceRecordFromChildIndex, ref this.ContainerDependents, ref this.ResourceDependents, ref this.EventDependents, ref this._triggerActions, ref this._dataTriggerRecordFromBinding, ref this._hasInstanceValues, ref this._eventHandlersStore);
        }

        /// <summary>Loads the content of the template as an instance of an object and returns the root element of the content.</summary>
        /// <returns>The root element of the content. Calling this multiple times returns separate instances.</returns>
        public virtual DependencyObject LoadContent()
        {
            this.VerifyAccess();
            this.Seal();
            if (this.VisualTree == null)
            {
                return null;
            }
            var content = this.VisualTree.InstantiateUnoptimizedTree();
            var uiElement = content as UIElement;
            if (uiElement != null)
            {
                uiElement.IsTemplateRoot = true;
            }

            return content;
        }
        #endregion

        internal virtual bool BuildVisualTree(UIElement container)
        {
            return false;
        }

        internal void CheckSealed()
        {
            if (this.IsSealed)
            {
                throw new InvalidOperationException("Template can't be changed after Sealed");
            }
        }

        #region Implementation

        private bool ReadInternalFlag(InternalFlags reqFlag)
        {
            return (int)(this.flags & reqFlag) != 0;
        }

        private void WriteInternalFlag(InternalFlags reqFlag, bool set)
        {
            if (set)
            {
                var frameworkTemplate = this;
                frameworkTemplate.flags = frameworkTemplate.flags | reqFlag;
                return;
            }
            var frameworkTemplate1 = this;
            frameworkTemplate1.flags = frameworkTemplate1.flags & ~reqFlag;
        }
        #endregion
    }
}