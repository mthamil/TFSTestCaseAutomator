using Autofac;
using TestCaseAutomator.Configuration;

namespace TFSTestCaseAutomator.Container
{
	/// <summary>
	/// Module that wires up core applicaiton objects.
	/// </summary>
	public class CoreModule : Module
	{
		/// <see cref="Module.Load"/>
		protected override void Load(ContainerBuilder builder)
		{
			builder.Register(c => Settings.Default);

			builder.RegisterType<DotNetSettings>().As<ISettings>()
			       .SingleInstance();
		}
	}
}