using System.IO;

namespace TestCaseAutomator.Utilities.InputOutput
{
	/// <summary>
	/// Class that manages a temporary file and aids in clean up
	/// after its use.
	/// </summary>
	public class TemporaryFile : DisposableBase
	{
		/// <summary>
		/// Initializes a new <see cref="TemporaryFile"/>.
		/// </summary>
		public TemporaryFile()
		{
			File = new FileInfo(Path.GetTempFileName());
		}

		/// <summary>
		/// Initializes a new <see cref="TemporaryFile"/> with the given name
		/// in a temporary path.
		/// </summary>
		/// <param name="fileName">The name of the temporary file</param>
		public TemporaryFile(string fileName)
		{
			File = new FileInfo(Path.Combine(Path.GetTempPath(), fileName));
		}

		/// <summary>
		/// Creates an empty temporary file for the file path represented by this <see cref="TemporaryFile"/>.
		/// </summary>
		/// <remarks>This object is returned to enable a more fluent syntax.</remarks>
		public TemporaryFile Touch()
		{
			File.Create().Close();
			return this;
		}

		/// <summary>
		/// Populates a temporary file with the given content.
		/// </summary>
		/// <param name="content">The content to write to the temporary file</param>
		/// <remarks>This object is returned to enable a more fluent syntax.</remarks>
		public TemporaryFile WithContent(string content)
		{
			using (var writer = File.CreateText())
				writer.Write(content);

			return this;
		}

		/// <summary>
		/// The actual temporary file.
		/// </summary>
		public FileInfo File { get; private set; }

		/// <see cref="DisposableBase.OnDisposing"/>
		protected override void OnDisposing() { }

		/// <see cref="DisposableBase.OnDispose"/>
		protected override void OnDispose()
		{
			if (File.Exists)
				File.Delete();
		}
	}
}