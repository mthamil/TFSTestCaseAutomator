using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TestCaseAutomator.TeamFoundation
{
    /// <summary>
    /// Wraps a <see cref="WorkItemStore"/>.
    /// </summary>
    public class WorkItemStoreAdapter : IWorkItemStore
    {
        private readonly WorkItemStore _workItemStore;

        /// <summary>
        /// Initializes a new <see cref="WorkItemStoreAdapter"/>.
        /// </summary>
        /// <param name="workItemStore">The wrapped <see cref="WorkItemStore"/>.</param>
        public WorkItemStoreAdapter(WorkItemStore workItemStore)
        {
            _workItemStore = workItemStore;
        }

        /// <see cref="IWorkItemStore.QueryAsync"/>
        public async Task<IEnumerable<WorkItem>> QueryAsync(string wiql, IDictionary parameters, CancellationToken cancellationToken)
        {
            var workItemQuery = _workItemStore.Query(wiql, parameters);

            var queryStart = workItemQuery.Query.BeginQuery();
            cancellationToken.Register(q => ((ICancelableAsyncResult)q).Cancel(), queryStart);

            var workItems = await Task.Factory.FromAsync(queryStart, r => workItemQuery.Query.EndQuery((ICancelableAsyncResult)r))
                                              .ConfigureAwait(false);
            return workItems.Cast<WorkItem>();
        }
    }
}