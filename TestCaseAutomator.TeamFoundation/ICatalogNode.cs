using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Framework.Common;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// A TFS catalog node.
	/// </summary>
	public interface ICatalogNode
	{
        /// <summary>
        /// Queries a node's children.
        /// </summary>
        Task<IEnumerable<ICatalogNode>> QueryChildrenAsync(IEnumerable<Guid> resourceTypeFilters, bool recurse, CatalogQueryOptions queryOptions);

		/// <summary>
		/// A node's resource display name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// A node's resource description.
		/// </summary>
		string Description { get; }
	}
}