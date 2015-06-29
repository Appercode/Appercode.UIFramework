using Appercode.UI.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Data
{
    public class RelativeSource : IMarkupExtension
    {
        private bool isInitialized;

        private RelativeSourceMode mode = RelativeSourceMode.TemplatedParent;

        public RelativeSource()
        {
        }

        public RelativeSource(RelativeSourceMode mode)
        {
            this.Mode = mode;
        }

        public RelativeSourceMode Mode
        {
            get
            {
                return this.mode;
            }
            set
            {
                if (!this.isInitialized)
                {
                    this.mode = value;
                    this.isInitialized = true;
                    return;
                }
                if (this.mode != value)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        #region IMarkupExtension Members

        bool IMarkupExtension.IsValidTargetForExtension(object target, object dp)
        {
            return false;
        }

        void IMarkupExtension.SetupExtension(object target, object dp)
        {
        }

        #endregion
    }
}