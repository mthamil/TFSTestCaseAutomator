using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TestCaseAutomator.TeamFoundation
{
    /// <summary>
    /// Provides access to TFS work items.
    /// </summary>
    public interface IWorkItemStore
    {
        /// <summary>
        /// Performs a work item query.
        /// </summary>
        /// <param name="wiql">The query text to execute.</param>
        /// <param name="parameters">An optional set of query parameters.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns>Any work items returned by the query.</returns>
        Task<IEnumerable<WorkItem>> QueryAsync(string wiql, IDictionary parameters = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}