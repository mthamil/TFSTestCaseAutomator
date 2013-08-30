using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Reflection;
using Autofac;
using TestCaseAutomator.AutomationProviders.Interfaces;
using Module = Autofac.Module;

namespace TestCaseAutomator.AutomationProviders.Container
{
	/// <summary>
	/// Module that wires up plugin related components.
	/// </summary>
	public class PluginModule : Module
	{
		/// <summary>
		/// The location where plugins should be loaded from.
		/// </summary>
		public Func<IComponentContext, DirectoryInfo> PluginLocation { get; set; }

		/// <see cref="Module.Load"/>
		protected override void Load(ContainerBuilder builder)
		{
			builder.Register(c => new AggregateCatalog(
				                      new AssemblyCatalog(Assembly.GetAssembly(GetType())),
				                      new DirectoryCatalog(PluginLocation(c).FullName)))
			       .As<ComposablePartCatalog>()
			       .SingleInstance();

			builder.RegisterType<CompositionContainer>()
			       .SingleInstance();

			builder.Register(c => new CompositeAutomatedTestDiscoverer(
				                      c.Resolve<CompositionContainer>().GetExportedValues<IAutomatedTestDiscoverer>()))
			       .As<IAutomatedTestDiscoverer>()
			       .SingleInstance()
			       .AutoActivate();
		}
	}
}