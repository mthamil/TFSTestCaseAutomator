using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Wraps a <see cref="CatalogNode"/>.
	/// </summary>
	public class CatalogNodeAdapter : ICatalogNode
	{
	    /// <summary>
	    /// Initializes a new <see cref="CatalogNodeAdapter"/>.
	    /// </summary>
	    /// <param name="node">The wrapped catalog node.</param>
	    /// <param name="scheduler">Schedules asynchronous tasks.</param>
	    public CatalogNodeAdapter(CatalogNode node, TaskScheduler scheduler)
	    {
	        _node = node;
	        _scheduler = scheduler;
	    }

	    /// <summary>
		/// A node's resource display name.
		/// </summary>
		public string Name => _node.Resource.DisplayName;

	    /// <summary>
		/// A node's resource description.
		/// </summary>
		public string Description => _node.Resource.Description;

	    /// <see cref="ICatalogNode.QueryChildrenAsync"/>
		public Task<IEnumerable<ICatalogNode>> QueryChildrenAsync(IEnumerable<Guid> resourceTypeFilters, bool recurse, CatalogQueryOptions queryOptions)
	    {
            return Task.Factory.StartNew<IEnumerable<ICatalogNode>>(() =>
                    _node.QueryChildren(resourceTypeFilters, recurse, queryOptions)
                         .Select(n => new CatalogNodeAdapter(n, _scheduler))
                         .ToList(), 
                    CancellationToken.None, TaskCreationOptions.None, _scheduler);
	    }

	    private readonly CatalogNode _node;
	    private readonly TaskScheduler _scheduler;
	}
}