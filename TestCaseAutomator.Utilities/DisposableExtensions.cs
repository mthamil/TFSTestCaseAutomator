using System;

namespace TestCaseAutomator.Utilities
{
	/// <summary>
	/// Contains extensions methods for IDisposable.
	/// </summary>
	public static class DisposableExtensions
	{
		/// <summary>
		/// Disposes of a resource in a null-safe way.
		/// </summary>
		/// <param name="disposable">A disposable resource</param>
		public static void DisposeSafely(this IDisposable disposable)
		{
			if (disposable != null)
				disposable.Dispose();
		}
	}
}