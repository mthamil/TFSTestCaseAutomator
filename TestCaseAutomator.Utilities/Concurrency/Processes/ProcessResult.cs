using System.IO;

namespace TestCaseAutomator.Utilities.Concurrency.Processes
{
	/// <summary>
	/// Contains the streams resulting from the execution of an external process.
	/// </summary>
	public class ProcessResult
	{
		/// <summary>
		/// Initializes a new <see cref="ProcessResult"/>.
		/// </summary>
		/// <param name="output">The data read from the output stream</param>
		/// <param name="error">The data read from the error stream</param>
		public ProcessResult(Stream output, Stream error)
		{
			Output = output;
			Error = error;
		}

		/// <summary>
		/// The data read from the output stream.
		/// </summary>
		public Stream Output { get; private set; }

		/// <summary>
		/// The data read from the error stream.
		/// </summary>
		public Stream Error { get; private set; }
	}
}