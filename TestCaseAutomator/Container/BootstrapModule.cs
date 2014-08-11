using Autofac;
using TestCaseAutomator.AutomationProviders.Container;
using TestCaseAutomator.Configuration;
using TestCaseAutomator.TeamFoundation.Container;

namespace TestCaseAutomator.Container
{
	/// <summary>
	/// Module that registers all other modules.
	/// </summary>
	public class BootstrapModule : Module
	{
		/// <see cref="Module.Load"/>
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterModule<CoreModule>();
			builder.RegisterModule(new PluginModule { PluginLocation = c => c.Resolve<ISettings>().TestDiscoveryPluginLocation });
			builder.RegisterModule<TeamFoundationModule>();
			builder.RegisterModule<PresentationModule>();
		}
	}
}