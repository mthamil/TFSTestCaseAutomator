using System;
using Autofac;
using Autofac.Features.OwnedInstances;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TestCaseAutomator.TeamFoundation.Container
{
	/// <summary>
	/// Modules that wires up Team Foundation Server related components.
	/// </summary>
	public class TeamFoundationModule : Module
	{
		/// <see cref="Module.Load"/>
		protected override void Load(ContainerBuilder builder)
		{
			builder.Register((c, p) => TfsTeamProjectCollectionFactory.GetTeamProjectCollection(p.TypedAs<Uri>()))
			       .As<TfsConnection>();

			builder.Register((c, p) => new TfsExplorer(
				                           c.Resolve<Func<Uri, TfsConnection>>()(p.TypedAs<Uri>()),
				                           c.Resolve<Func<ITestManagementTeamProject, ITfsProjectWorkItemCollection>>(),
				                           c.Resolve<Func<TfsConnection, Owned<IVersionControl>>>()))
			       .As<ITfsExplorer>();

			builder.RegisterType<TfsProjectWorkItemCollection>()
			       .As<ITfsProjectWorkItemCollection>();

			builder.Register((c, p) => new VersionControlServerAdapter(p.TypedAs<TfsConnection>().GetService<VersionControlServer>()))
			       .As<IVersionControl>();
		}
	}
}