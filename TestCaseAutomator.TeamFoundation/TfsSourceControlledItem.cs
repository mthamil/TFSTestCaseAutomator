using System.IO;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Base class for TFS source controlled items.
	/// </summary>
	public abstract class TfsSourceControlledItem
	{
		/// <summary>
		/// Initializes a new <see cref="TfsSourceControlledItem"/>.
		/// </summary>
		/// <param name="item">The source controlled item.</param>
		/// <param name="versionControl">The item's associated TFS source control</param>
		protected TfsSourceControlledItem(IVersionedItem item, IVersionControl versionControl)
		{
			Item = item;
			VersionControl = versionControl;
		}

		/// <summary>
		/// An item's name.
		/// </summary>
		public string Name => Path.GetFileName(Item.ServerItem);

	    /// <summary>
		/// The path to an item in source control.
		/// </summary>
		public string ServerPath => Item.ServerItem;

	    /// <summary>
		/// Downloads the contents of an item from source control.
		/// </summary>
		/// <returns>A stream with an item's contents</returns>
		public Stream Download() => Item.DownloadFile();

	    /// <summary>
		/// The source controlled item.
		/// </summary>
		protected IVersionedItem Item { get; }

		/// <summary>
		/// The item's associated source control.
		/// </summary>
		protected IVersionControl VersionControl { get; }

		/// <see cref="object.ToString"/>
		public override string ToString() => ServerPath;
	}
}