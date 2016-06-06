using System;
using System.Linq;
using System.Threading.Tasks;
using AzureSupport;
using AzureSupport.TheBall.CORE;
using TheBall.CORE;

namespace TheBall
{
    public class FinalizingDependencyAction
    {
        public readonly Type ForType;
        public readonly Type[] DependingFromTypes;
        public readonly Func<Type[], Task> FinalizingAction;

        public FinalizingDependencyAction(Type forType, Type[] dependingFromTypes, Func<Type[], Task> finalizingAction)
        {
            ForType = forType;
            DependingFromTypes = dependingFromTypes;
            FinalizingAction = finalizingAction;
        }
    }

    public class LogicalOperationContext
    {
        private FinalizingDependencyAction[] FinalizingActions = null;

        public static LogicalOperationContext Current => InformationContext.Current.LogicalOperationContext;

        public readonly HttpOperationData HttpParameters;

        public static async Task SetCurrentContext(HttpOperationData httpOperationData, BeginImpersonation beginImpersonation)
        {
            var loCtx = new LogicalOperationContext(httpOperationData);
            if (loCtx.IsImpersonating)
            {
                await beginImpersonation(loCtx.InitialOwner, loCtx.ExecutingOwner);
            }
            loCtx.setInitializeFinalizingActions(InformationContext.Current.OperationFinalizingActions);
            InformationContext.Current.LogicalOperationContext = loCtx;
        }

        public static async Task ReleaseCurrentContext(EndImpersonation endImpersonation)
        {
            if (Current.IsImpersonating)
            {
                await endImpersonation(Current.InitialOwner, Current.ExecutingOwner);
            }
            InformationContext.Current.LogicalOperationContext = null;
        }

        public delegate Task BeginImpersonation(IContainerOwner initialOwner, IContainerOwner executingOwner);
        public delegate Task EndImpersonation(IContainerOwner initialOwner, IContainerOwner executingOwner);

        private LogicalOperationContext(HttpOperationData httpOperationData)
        {
            HttpParameters = httpOperationData;
            var initialOwner = InformationContext.CurrentOwner;
            var executingOwner = VirtualOwner.FigureOwner(httpOperationData.OwnerRootLocation);
            bool isImpersonating = !executingOwner.IsSameOwner(initialOwner);
            IsImpersonating = isImpersonating;
            if (isImpersonating)
            {
                InitialOwner = initialOwner;
                ExecutingOwner = executingOwner;
            }
        }

        private readonly IContainerOwner InitialOwner;
        private readonly IContainerOwner ExecutingOwner;

        public readonly bool IsImpersonating;

        private void setInitializeFinalizingActions(FinalizingDependencyAction[] finalizingActions)
        {
            if (finalizingActions == null)
                return;
            var lookup = finalizingActions.ToDictionary(item => item.ForType);
            FinalizingActions = finalizingActions?.TSort(item => item.DependingFromTypes
                ?.Where(depType => lookup.ContainsKey(depType))
                .Select(depType => lookup[depType]), true).ToArray();
        }

        public async Task ExecuteRegisteredFinalizingActions()
        {
            try
            {
                if (FinalizingActions == null)
                    return;
                var activeChangedTypeSet = InformationContext.Current.OperationTrackedCurrentChangedTypesForOperationFinalizers;
                var currentChangedTypes = activeChangedTypeSet.ToArray();

                foreach (var finalizingAction in FinalizingActions)
                {
                    var activatedOnTypes =
                        currentChangedTypes.Where(
                            changedType => finalizingAction.DependingFromTypes.Any(depType => changedType == depType)).ToArray();
                    if (activatedOnTypes.Length > 0)
                    {
                        activeChangedTypeSet.Clear();
                        await finalizingAction.FinalizingAction(activatedOnTypes);
                        if (activeChangedTypeSet.Count > 0)
                        {
                            currentChangedTypes = currentChangedTypes.Union(activeChangedTypeSet).ToArray();
                        }
                    }
                }
            }
            finally
            {
                InformationContext.Current.OperationTrackedCurrentChangedTypesForOperationFinalizers.Clear();
            }
        }
    }
}