using System;
using System.Collections.Generic;
using System.IO;

namespace TestCaseAutomator.Utilities.InputOutput
{
	/// <summary>
	/// An equality comparer that checks whether two file system entities have the same path and filename.
	/// </summary>
	public class FileSystemInfoPathEqualityComparer : IEqualityComparer<FileSystemInfo>
	{
		/// <summary>
        /// Determines whether two file system entities have the same path and filename.
        /// Note: Case is ignored!
		/// </summary>
		/// <param name="x">The first file system entity to compare</param>
		/// <param name="y">The second file system entity to compare</param>
        /// <returns>Whether the file system entities are considered equal</returns>
        public bool Equals(FileSystemInfo x, FileSystemInfo y)
		{
			if (x == y)
				return true;

			if (x == null || y == null)
				return false;

			return String.Equals(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Returns a file system entity full name's hashcode.
		/// </summary>
        public int GetHashCode(FileSystemInfo obj)
		{
			return obj.FullName.ToLowerInvariant().GetHashCode();
		}

		/// <summary>
		/// Gets a <see cref="FileSystemInfoPathEqualityComparer"/> equality comparer.
		/// </summary>
        public static IEqualityComparer<FileSystemInfo> Instance
		{
			get { return instance; }
		}

        private static readonly IEqualityComparer<FileSystemInfo> instance = new FileSystemInfoPathEqualityComparer(); 
	}
}