using System.IO;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// A wrapper around <see cref="Item"/>.
	/// </summary>
	public class VersionedItem : IVersionedItem
	{
		/// <summary>
		/// Initializes a new <see cref="VersionedItem"/>.
		/// </summary>
		/// <param name="item">The wrapped item</param>
		public VersionedItem(Item item)
		{
			_item = item;
		}

		/// <see cref="IVersionedItem.ServerItem"/>
		public string ServerItem 
		{
			get { return _item.ServerItem; }
		}

		/// <see cref="IVersionedItem.ItemType"/>
		public ItemType ItemType 
		{ 
			get { return _item.ItemType; } 
		}

		/// <see cref="IVersionedItem.DownloadFile"/>
		public Stream DownloadFile()
		{
			return _item.DownloadFile();
		}

		private readonly Item _item;
	}
}