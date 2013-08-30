using System.Threading.Tasks;
using Autofac;
using TestCaseAutomator.Configuration;

namespace TestCaseAutomator.Container
{
	/// <summary>
	/// Module that wires up core application objects.
	/// </summary>
	public class CoreModule : Module
	{
		/// <see cref="Module.Load"/>
		protected override void Load(ContainerBuilder builder)
		{
			builder.Register(_ => TaskScheduler.Default);

			builder.Register(_ => Settings.Default);

			builder.Register(c => new DotNetSettings(c.Resolve<Settings>()))
			       .As<ISettings>()
			       .SingleInstance();

			builder.RegisterType<SettingsPropagator>()
			       .AutoActivate()
			       .SingleInstance();
		}
	}
}