using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.TeamFoundation.TestManagement.Client;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// An <see cref="IQueryable{T}"/> implementation that wraps a query but also carries
	/// through a <see cref="ITestManagementTeamProject"/> that queried items belong to.
	/// </summary>
	/// <typeparam name="TElement">The type of elements being queried</typeparam>
	internal class TestCaseQueryable<TElement> : IQueryable<TElement>, IQueryProvider
	{
		public TestCaseQueryable(IQueryable<TElement> innerQuery, ITestManagementTeamProject project)
		{
			_innerQuery = innerQuery;
			_project = project;
		}

		/// <summary>
		/// A test management project.
		/// </summary>
		public ITestManagementTeamProject Project
		{
			get { return _project; }
		}

		#region IQueryable Implementation

		/// <see cref="IEnumerable{T}.GetEnumerator"/>
		public IEnumerator<TElement> GetEnumerator()
		{
			return _innerQuery.GetEnumerator();
		}

		/// <see cref="IEnumerable.GetEnumerator"/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <see cref="IQueryable.Expression"/>
		public Expression Expression { get { return _innerQuery.Expression; } }

		/// <see cref="IQueryable.ElementType"/>
		public Type ElementType { get { return _innerQuery.ElementType; } }

		/// <see cref="IQueryable.Provider"/>
		public IQueryProvider Provider { get { return this; } }

		#endregion IQueryable Implementation

		#region IQueryProvider Implementation

		/// <see cref="IQueryProvider.CreateQuery"/>
		public IQueryable CreateQuery(Expression expression)
		{
			return new TestCaseQueryable<TElement>(_innerQuery.Provider.CreateQuery<TElement>(expression), Project);
		}

		/// <see cref="IQueryProvider.CreateQuery{TElement}"/>
		public IQueryable<T> CreateQuery<T>(Expression expression)
		{
			return new TestCaseQueryable<T>(_innerQuery.Provider.CreateQuery<T>(expression), Project);
		}

		/// <see cref="IQueryProvider.Execute"/>
		public object Execute(Expression expression)
		{
			return _innerQuery.Provider.Execute(expression);
		}

		/// <see cref="IQueryProvider.Execute{TResult}"/>
		public TResult Execute<TResult>(Expression expression)
		{
			return _innerQuery.Provider.Execute<TResult>(expression);
		}

		#endregion IQueryProvider Implementation

		private readonly IQueryable<TElement> _innerQuery;
		private readonly ITestManagementTeamProject _project;
	}
}