using Autofac;
using TestCaseAutomator.Configuration;
using TestCaseAutomator.ViewModels;

namespace TestCaseAutomator.Container
{
	/// <summary>
	/// Module responsible for wiring up presentation layer dependencies.
	/// </summary>
	public class PresentationModule : Module
	{
		/// <see cref="Module.Load"/>
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<MainViewModel>()
			       .OnActivating(c =>
			       {
				       c.Instance.ServerUri = c.Context.Resolve<ISettings>().TfsServerLocation;
				       c.Instance.ProjectName = c.Context.Resolve<ISettings>().TfsProjectName;
			       })
			       .As<MainViewModel, IApplication>()
				   .SingleInstance();
		}
	}
}