using Autofac;
using TestCaseAutomator.AutomationProviders;
using TestCaseAutomator.AutomationProviders.Interfaces;
using TestCaseAutomator.Configuration;
using TestCaseAutomator.TeamFoundation.Container;

namespace TFSTestCaseAutomator.Container
{
	/// <summary>
	/// Module that registers all other modules.
	/// </summary>
	public class MasterModule : Module
	{
		/// <see cref="Module.Load"/>
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterModule<CoreModule>();
			builder.RegisterModule<TeamFoundationModule>();

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