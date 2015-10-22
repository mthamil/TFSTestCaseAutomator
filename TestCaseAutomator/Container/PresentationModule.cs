using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using SharpEssentials.Controls.Mvvm;
using TestCaseAutomator.AutomationProviders.Abstractions;
using TestCaseAutomator.Configuration;
using TestCaseAutomator.ViewModels;
using TestCaseAutomator.ViewModels.Browser.Nodes;
using TestCaseAutomator.Container.Support;

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
                   .ApplySettings((s, vm) => vm.ProjectName = s.TfsProjectName)
			       .As<MainViewModel, IApplication>()
				   .SingleInstance()
                   .OnActivated(c =>
                   {
                       if (c.Context.Resolve<ISettings>().AutoConnectOnStartup && c.Instance.CanConnect)
                           c.Instance.ConnectAsync();
                   });

		    builder.RegisterType<ServerManagementViewModel>()
		           .WithParameter(c => c.Resolve<ISettings>().TfsServers as IEnumerable<Uri>)
                   .As<IServerManagement>()
                   .SingleInstance();

			builder.RegisterType<ProjectViewModel>()
				.OnActivating(c => c.Instance.FileExtensions = 
					c.Context.Resolve<ITestAutomationDiscoverer>().SupportedFileExtensions.ToList());
		}
	}
}