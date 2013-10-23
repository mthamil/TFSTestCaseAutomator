using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Wraps a <see cref="CatalogNode"/>.
	/// </summary>
	public class CatalogNodeWrapper : ICatalogNode
	{
		/// <summary>
		/// Initializes a new <see cref="CatalogNodeWrapper"/>.
		/// </summary>
		/// <param name="node">The wrapped node</param>
		public CatalogNodeWrapper(CatalogNode node)
		{
			_node = node;
		}

		/// <summary>
		/// A node's resource display name.
		/// </summary>
		public string Name
		{
			get { return _node.Resource.DisplayName; }
		}

		/// <summary>
		/// A node's resource description.
		/// </summary>
		public string Description
		{
			get { return _node.Resource.Description; }
		}

		/// <see cref="ICatalogNode.QueryChildren"/>
		public IEnumerable<ICatalogNode> QueryChildren(IEnumerable<Guid> resourceTypeFilters, bool recurse, CatalogQueryOptions queryOptions)
		{
			return _node.QueryChildren(resourceTypeFilters, recurse, queryOptions)
			            .Select(n => new CatalogNodeWrapper(n)).ToList();
		}

		private readonly CatalogNode _node;
	}
}