using System.Linq;
using Autofac;
using SharpEssentials.Controls.Mvvm;
using TestCaseAutomator.AutomationProviders.Interfaces;
using TestCaseAutomator.Configuration;
using TestCaseAutomator.ViewModels;
using TestCaseAutomator.ViewModels.Browser;

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
		    builder.RegisterAssemblyTypes(ThisAssembly)
		           .Where(t => t.IsAssignableTo<ViewModelBase>())
                   .AsSelf()
                   .AsImplementedInterfaces();

			builder.RegisterType<MainViewModel>()
			       .OnActivating(c =>
			       {
				       c.Instance.ServerUri = c.Context.Resolve<ISettings>().TfsServerLocation;
				       c.Instance.ProjectName = c.Context.Resolve<ISettings>().TfsProjectName;
			       })
			       .As<MainViewModel, IApplication>()
				   .SingleInstance();

			builder.RegisterType<ProjectViewModel>()
				.OnActivating(c => c.Instance.FileExtensions = 
					c.Context.Resolve<ITestAutomationDiscoverer>().SupportedFileExtensions.ToList());
		}
	}
}