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

        /// <summary>
        /// Disposes the value of a <see cref="Lazy{T}"/> if it has been created.
        /// </summary>
        /// <typeparam name="TDisposable">A disposable type</typeparam>
        /// <param name="lazyDisposable">A lazy disposable value</param>
	    public static void Dispose<TDisposable>(this Lazy<TDisposable> lazyDisposable)
            where TDisposable : IDisposable
	    {
            if (lazyDisposable.IsValueCreated)
                lazyDisposable.Value.Dispose();
	    }
	}
}