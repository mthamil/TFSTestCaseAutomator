using System;
using System.Linq;
using Autofac;
using TestCaseAutomator.AutomationProviders.Interfaces;
using TestCaseAutomator.Configuration;
using TestCaseAutomator.TeamFoundation;
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
			builder.RegisterType<MainViewModel>()
			       .OnActivating(c =>
			       {
				       c.Instance.ServerUri = c.Context.Resolve<ISettings>().TfsServerLocation;
				       c.Instance.ProjectName = c.Context.Resolve<ISettings>().TfsProjectName;
			       })
			       .As<MainViewModel, IApplication>()
				   .SingleInstance();

			builder.RegisterType<TestBrowserViewModel>();

			builder.RegisterType<WorkItemsViewModel>().As<IWorkItems>();

			builder.RegisterType<SolutionViewModel>();

			builder.Register((c, p) => new ProjectViewModel(
				p.TypedAs<TfsSolutionProject>(),
				c.Resolve<Func<TfsFile, AutomationSourceViewModel>>(),
				c.Resolve<IAutomatedTestDiscoverer>().SupportedFileExtensions.ToList()));

			builder.RegisterType<AutomationSourceViewModel>();

			builder.RegisterType<AutomatedTestViewModel>();
		}
	}
}