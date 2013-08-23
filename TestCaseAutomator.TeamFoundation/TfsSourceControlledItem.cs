using System.IO;
using Microsoft.TeamFoundation.VersionControl.Client;

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
		/// <param name="item">he source controlled item.</param>
		protected TfsSourceControlledItem(Item item)
		{
			Item = item;
		}

		/// <summary>
		/// An item's name.
		/// </summary>
		public string Name
		{
			get { return Path.GetFileName(Item.ServerItem); }
		}

		/// <summary>
		/// The path to an item in source control.
		/// </summary>
		public string ServerPath
		{
			get { return Item.ServerItem; }
		}

		/// <summary>
		/// The source controlled item.
		/// </summary>
		protected Item Item { get; private set; }

		/// <see cref="object.ToString"/>
		public override string ToString()
		{
			return ServerPath;
		}
	}
}