using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TestCaseAutomator.Utilities.Concurrency.Processes
{
	/// <summary>
	/// Provides methods that enable using a Process as a Task.
	/// </summary>
	public static class ProcessTaskExtensions
	{
		/// <summary>
		/// Creates a Task representing an external background process that takes a stream
		/// as input and returns its output.
		/// </summary>
		/// <param name="taskFactory">Creates tasks</param>
		/// <param name="executable">The name of the executable to run</param>
		/// <param name="arguments">Any arguments to the process</param>
		/// <param name="input">The process input stream</param>
		/// <param name="cancellationToken">An optional token that can cancel the task</param>
		/// <returns>A Task representing the process</returns>
		public static Task<ProcessResult> FromProcess(this TaskFactory taskFactory, string executable, string arguments, Stream input, CancellationToken cancellationToken = default(CancellationToken))
		{
			return taskFactory.FromProcess(new ProcessStartInfo
			{
				FileName = executable,
				Arguments = arguments,
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				RedirectStandardInput = true,
				UseShellExecute = false
			}, input, cancellationToken);
		}

		/// <summary>
		/// Creates a Task representing an external process that takes a stream
		/// as input and returns its output.
		/// </summary>
		/// <param name="taskFactory">Creates tasks</param>
		/// <param name="processInfo">Describes the process to execute</param>
		/// <param name="input">The process input stream</param>
		/// <param name="cancellationToken">An optional token that can cancel the task</param>
		/// <returns>A Task representing the process</returns>
		public static Task<ProcessResult> FromProcess(this TaskFactory taskFactory, ProcessStartInfo processInfo, Stream input, CancellationToken cancellationToken = default(CancellationToken))
		{
			return ProcessAdapterFactory(taskFactory).StartNew(processInfo, input, cancellationToken);
		}

		/// <summary>
		/// Creates a Task representing an external process that takes
		/// no input and returns no output.
		/// </summary>
		/// <param name="taskFactory">Creates tasks</param>
		/// <param name="executable">The name of the executable to run</param>
		/// <param name="arguments">Any arguments to the process</param>
		/// <param name="cancellationToken">An optional token that can cancel the task</param>
		/// <returns>A Task representing the process</returns>
		public static Task FromProcess(this TaskFactory taskFactory, string executable, string arguments, CancellationToken cancellationToken = default(CancellationToken))
		{
			return taskFactory.FromProcess(new ProcessStartInfo
			{
				FileName = executable,
				Arguments = arguments,
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				RedirectStandardError = true,
				UseShellExecute = false
			}, cancellationToken);
		}

		/// <summary>
		/// Creates a Task representing an external process that takes
		/// no input and returns no output.
		/// </summary>
		/// <param name="taskFactory">Creates tasks</param>
		/// <param name="processInfo">Describes the process to execute</param>
		/// <param name="cancellationToken">An optional token that can cancel the task</param>
		/// <returns>A Task representing the process</returns>
		public static Task FromProcess(this TaskFactory taskFactory, ProcessStartInfo processInfo, CancellationToken cancellationToken = default(CancellationToken))
		{
			return ProcessAdapterFactory(taskFactory).StartNew(processInfo, cancellationToken);
		}

		internal static Func<TaskFactory, IProcessTaskAdapter> ProcessAdapterFactory = tf => new ProcessTaskAdapter(tf, TaskScheduler.Default);
	}
}