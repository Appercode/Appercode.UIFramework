using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals
{
    internal struct WeakRefKey
    {
        private WeakReference weakRef;

        private int hashCode;

        internal WeakRefKey(object target)
        {
            this.weakRef = new WeakReference(target);
            this.hashCode = target != null ? target.GetHashCode() : 314159;
        }

        internal object Target
        {
            get
            {
                return this.weakRef.Target;
            }
        }

        public static bool operator ==(WeakRefKey left, WeakRefKey right)
        {
            if ((object)left == null)
            {
                return (object)right == null;
            }
            return left.Equals(right);
        }

        public static bool operator !=(WeakRefKey left, WeakRefKey right)
        {
            return !(left == right);
        }

        public override bool Equals(object o)
        {
            if (!(o is WeakRefKey))
            {
                return false;
            }
            WeakRefKey weakRefKey = (WeakRefKey)o;
            object target = this.Target;
            object obj = weakRefKey.Target;
            if (target != null && obj != null)
            {
                return target == obj;
            }
            return this.weakRef == weakRefKey.weakRef;
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }
    }
}