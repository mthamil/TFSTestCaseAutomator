using System.Collections.Generic;

namespace TestCaseAutomator.Utilities.Collections
{
	/// <summary>
	/// Contains extension methods for dictionaries.
	/// </summary>
	public static class DictionaryExtensions
	{
		/// <summary>
		/// Attempts to get the value associated with the specified key.
		/// </summary>
		/// <typeparam name="TKey">The type of key</typeparam>
		/// <typeparam name="TValue">The type of value</typeparam>
		/// <param name="dictionary">The dictionary to query</param>
		/// <param name="key">The key whose value to get</param>
		/// <returns>Option&lt;TValue&gt;.Some() if the key exists, otherwise none</returns>
		public static Option<TValue> TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			TValue value;
			return dictionary.TryGetValue(key, out value) 
				? Option<TValue>.Some(value) 
				: Option<TValue>.None();
		}
	}
}