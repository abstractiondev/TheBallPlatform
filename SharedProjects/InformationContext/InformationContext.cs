using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace TheBall.CORE
{
    public class InformationContext
    {
        private const string KEYNAME = "INFOCTX";

        public class NoOwner : IContainerOwner
        {
            public string ContainerName => null;
            public string LocationPrefix => null;
        }

        public static IContainerOwner CurrentOwner => Current.Owner;

        internal readonly Stack<IContainerOwner> OwnerStack;
        public IContainerOwner Owner => OwnerStack.Peek();


        private InformationContext(IContainerOwner owner)
        {
            OwnerStack = new Stack<IContainerOwner>();
            OwnerStack.Push(owner);

        }

        public static InformationContext Current => getCurrent(false);

        private readonly HashSet<Type> ChangedTypesForOperationFinalizers = new HashSet<Type>();
        internal HashSet<Type> OperationTrackedCurrentChangedTypesForOperationFinalizers => ChangedTypesForOperationFinalizers;

        private static InformationContext getCurrent(bool allowNotInitialized)
        {
            var logicalContext = CallContext.LogicalGetData(KEYNAME);
            if (logicalContext != null)
                return (InformationContext)logicalContext;
            if (!allowNotInitialized)
                throw new InvalidOperationException("InitializeToLogicalContext is required before use");
            return null;
        }
        public LogicalOperationContext LogicalOperationContext { get; internal set; }
        public FinalizingDependencyAction[] OperationFinalizingActions { get; private set; }

    }
}
