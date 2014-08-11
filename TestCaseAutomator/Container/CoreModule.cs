using System.Threading.Tasks;
using Autofac;
using TestCaseAutomator.Configuration;
using TestCaseAutomator.Container.Support;

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

		    builder.RegisterType<DotNetSettings>()
		           .FindConstructorsWith(new NonPublicConstructorFinder())
		           .As<ISettings>()
		           .SingleInstance();

			builder.RegisterType<SettingsPropagator>()
			       .AutoActivate()
			       .SingleInstance();
		}
	}
}