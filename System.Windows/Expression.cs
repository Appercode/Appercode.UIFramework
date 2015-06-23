using Appercode.UI.Internals;

namespace System.Windows
{
    public class Expression
    {
        internal static readonly object NoValue;

        private Expression.InternalFlags flags;

        static Expression()
        {
            Expression.NoValue = new object();
        }

        internal Expression() : this(ExpressionMode.None)
        {
        }

        internal Expression(ExpressionMode mode)
        {
            this.flags = Expression.InternalFlags.None;
            switch (mode)
            {
                case ExpressionMode.None:
                {
                    return;
                }
                case ExpressionMode.NonSharable:
                {
                    Expression expression = this;
                    expression.flags = expression.flags | Expression.InternalFlags.NonShareable;
                    return;
                }
                case ExpressionMode.ForwardsInvalidations:
                {
                    Expression expression1 = this;
                    expression1.flags = expression1.flags | Expression.InternalFlags.ForwardsInvalidations;
                    Expression expression2 = this;
                    expression2.flags = expression2.flags | Expression.InternalFlags.NonShareable;
                    return;
                }
                case ExpressionMode.SupportsUnboundSources:
                {
                    Expression expression3 = this;
                    expression3.flags = expression3.flags | Expression.InternalFlags.ForwardsInvalidations;
                    Expression expression4 = this;
                    expression4.flags = expression4.flags | Expression.InternalFlags.NonShareable;
                    Expression expression5 = this;
                    expression5.flags = expression5.flags | Expression.InternalFlags.SupportsUnboundSources;
                    return;
                }
            }
            throw new ArgumentException("UnknownExpressionMode");
        }

        [Flags]
        private enum InternalFlags
        {
            None = 0,
            NonShareable = 1,
            ForwardsInvalidations = 2,
            SupportsUnboundSources = 4,
            Attached = 8
        }

        internal bool Attachable
        {
            get
            {
                if (this.Shareable)
                {
                    return true;
                }
                return !this.HasBeenAttached;
            }
        }

        internal bool ForwardsInvalidations
        {
            get
            {
                return (this.flags & Expression.InternalFlags.ForwardsInvalidations) != Expression.InternalFlags.None;
            }
        }

        internal bool HasBeenAttached
        {
            get
            {
                return (this.flags & Expression.InternalFlags.Attached) != Expression.InternalFlags.None;
            }
        }

        internal bool Shareable
        {
            get
            {
                return (this.flags & Expression.InternalFlags.NonShareable) == Expression.InternalFlags.None;
            }
        }

        internal bool SupportsUnboundSources
        {
            get
            {
                return (this.flags & Expression.InternalFlags.SupportsUnboundSources) != Expression.InternalFlags.None;
            }
        }

        internal void ChangeSources(DependencyObject d, DependencyProperty dp, DependencySource[] newSources)
        {
            if (d == null && !this.ForwardsInvalidations)
            {
                throw new ArgumentNullException("d");
            }
            if (dp == null && !this.ForwardsInvalidations)
            {
                throw new ArgumentNullException("dp");
            }
            if (this.Shareable)
            {
                throw new InvalidOperationException("ShareableExpressionsCannotChangeSources");
            }
            DependencyObject.ValidateSources(d, newSources, this);
            if (this.ForwardsInvalidations)
            {
                DependencyObject.ChangeExpressionSources(this, null, null, newSources);
                return;
            }
            DependencyObject.ChangeExpressionSources(this, d, dp, newSources);
        }

        internal virtual Expression Copy(DependencyObject targetObject, DependencyProperty targetDP)
        {
            return this;
        }

        internal virtual DependencySource[] GetSources()
        {
            return null;
        }

        internal virtual object GetValue(DependencyObject d, DependencyProperty dp)
        {
            return DependencyProperty.UnsetValue;
        }

        internal void MarkAttached()
        {
            Expression expression = this;
            expression.flags = expression.flags | Expression.InternalFlags.Attached;
        }

        internal virtual void OnAttach(DependencyObject d, DependencyProperty dp)
        {
        }

        internal virtual void OnDetach(DependencyObject d, DependencyProperty dp)
        {
        }

        internal virtual void OnPropertyInvalidation(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
        }

        internal virtual bool SetValue(DependencyObject d, DependencyProperty dp, object value)
        {
            return false;
        }

        internal bool IsValidValueForUpdate(object value, Type sourceType)
        {
            if (value == null && sourceType.IsValueType)
            {
                return false;
            }
            if (value == null)
            {
                return true;
            }
            if (value.GetType() == sourceType)
            {
                return true;
            }
            return sourceType.IsAssignableFrom(value.GetType());
        }
    }
}