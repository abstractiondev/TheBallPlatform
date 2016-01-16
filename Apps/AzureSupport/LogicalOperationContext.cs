using System;
using System.Linq;
using System.Threading.Tasks;
using AzureSupport;
using AzureSupport.TheBall.CORE;

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

        public static void SetCurrentContext(HttpOperationData httpOperationData)
        {
            var loCtx = new LogicalOperationContext(httpOperationData);
            loCtx.setInitializeFinalizingActions(InformationContext.Current.OperationFinalizingActions);
            InformationContext.Current.LogicalOperationContext = loCtx;

        }

        public static void ReleaseCurrentContext()
        {
            InformationContext.Current.LogicalOperationContext = null;
        }

        private LogicalOperationContext(HttpOperationData httpOperationData)
        {
            HttpParameters = httpOperationData;
        }

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