﻿using System;

namespace TestCaseAutomator.Utilities
{
	/// <summary>
	/// A convenient base class for Disposable objects.
	/// </summary>
	public abstract class DisposableBase : IDisposable
	{
		/// <summary>
		/// Whether an object has been disposed.
		/// </summary>
		public bool IsDisposed { get; private set; }

		/// <summary>
		/// Checks whether an object has already been disposed and throws an exception if it has.
		/// </summary>
		/// <exception cref="ObjectDisposedException">The object has been disposed</exception>
		protected void ThrowIfDisposed()
		{
			if (IsDisposed)
				throw new ObjectDisposedException(GetType().FullName);
		}

		/// <summary>
		/// Implements the actual disposal logic.  Subclasses should
		/// override this method to clean up resources.
		/// </summary>
		/// <param name="disposing">Whether the class is disposing from the Dispose() method</param>
		protected void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
					OnDisposing();

				OnDispose();

				IsDisposed = true;
			}
		}

		/// <summary>
		/// Should be overriden to implement actual managed dispose logic. That is, this
		/// method will be called ONLY when the Dispose() method is invoked.
		/// </summary>
		protected abstract void OnDisposing();

		/// <summary>
		/// Should be overriden to implement unmanaged dispose logic. That is, this
		/// method will be called from both the Dispose() method and the finalizer.
		/// </summary>
		protected virtual void OnDispose()
		{
			// Implementors should dispose of unmanaged resources here.
		}

		#region IDisposable Members

		/// <see cref="IDisposable.Dispose"/>
		public void Dispose()
		{
			Dispose(true);
			// This object will be cleaned up by the Dispose method.
			// Therefore, you should call GC.SupressFinalize to
			// take this object off the finalization queue
			// and prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		#endregion

		/// <summary>
		/// Use C# destructor syntax for finalization code.
		/// This destructor will run only if the Dispose method
		/// does not get called.
		/// It gives your base class the opportunity to finalize.
		/// Do not provide destructors in types derived from this class.
		/// </summary>
		~DisposableBase()
		{
			// Do not re-create Dispose clean-up code here.
			// Calling Dispose(false) is optimal in terms of
			// readability and maintainability.
			Dispose(false);
		}
	}
}
