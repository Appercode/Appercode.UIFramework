using System;

namespace Appercode.UI
{
    public sealed class RoutedEvent
    {
        public RoutedEvent(string name, Type handlerType)
        {
            this.Name = name;
            this.HandlerType = handlerType;
        }

        public Type HandlerType { get; private set; }
        public string Name { get; private set; }
        public Type OwnerType { get; private set; }
        public RoutingStrategy RoutingStrategy { get; private set; }

        public RoutedEvent AddOwner(Type ownerType)
        {
            this.OwnerType = ownerType;
            return this;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
