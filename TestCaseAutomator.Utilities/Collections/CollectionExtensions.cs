using System.Collections.Generic;

namespace TestCaseAutomator.Utilities.Collections
{
	/// <summary>
	/// Contains extension methods pertaining to collections.
	/// </summary>
	public static class CollectionExtensions
	{
		/// <summary>
		/// Adds the elements of the specified sequence to a collection.
		/// </summary>
		/// <typeparam name="T">The type of items in the collection</typeparam>
		/// <param name="collection">The collection to add to</param>
		/// <param name="newElements">The items to add to the collection</param>
		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> newElements)
		{
			var list = collection as List<T>;
			if (list != null)
			{
				list.AddRange(newElements);
				return;
			}

			foreach (var element in newElements)
				collection.Add(element);
		}

		/// <summary>
		/// Adds the given elements to a collection.
		/// </summary>
		/// <typeparam name="T">The type of items in the collection</typeparam>
		/// <param name="collection">The collection to add to</param>
		/// <param name="firstElement">The first item to add</param>
		/// <param name="secondElement">The second item to add</param>
		/// <param name="remainingElements">The rest of the items to add to the collection</param>
		public static void AddRange<T>(this ICollection<T> collection, T firstElement, T secondElement, params T[] remainingElements)
		{
			collection.Add(firstElement);
			collection.Add(secondElement);

			AddRange(collection, remainingElements);
		}
	}
}