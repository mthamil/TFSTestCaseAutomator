using Autofac;
using TestCaseAutomator.TeamFoundation.Container;

namespace TestCaseAutomator.Container
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
			builder.RegisterModule<PresentationModule>();
		}
	}
}