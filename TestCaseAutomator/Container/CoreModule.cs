using System.Threading.Tasks;
using Autofac;
using TestCaseAutomator.AutomationProviders;
using TestCaseAutomator.AutomationProviders.Interfaces;
using TestCaseAutomator.Configuration;

namespace TestCaseAutomator.Container
{
	/// <summary>
	/// Module that wires up core applicaiton objects.
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

			builder.RegisterType<PluginComposer>()
			       .OnActivating(c => c.Instance.PluginLocation = c.Context.Resolve<ISettings>().TestDiscoveryPluginLocation);

			builder.Register(c =>
					{
						var composer = c.Resolve<PluginComposer>();
						composer.Compose();
						return composer.RootDiscoverer;
					})
			       .As<IAutomatedTestDiscoverer>()
			       .SingleInstance()
			       .AutoActivate();
		}
	}
}