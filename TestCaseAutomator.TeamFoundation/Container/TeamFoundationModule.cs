using System;
using Autofac;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using TestCaseAutomator.TeamFoundation.TestCaseAssociation;

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
			       .As<TfsConnection, TfsTeamProjectCollection>();

			builder.Register((c, p) => new TfsServer(c.Resolve<Func<Uri, TfsTeamProjectCollection>>()(p.TypedAs<Uri>())))
			       .As<ITfsServer>();

			builder.Register((c, p) => new TfsExplorer(
				                           c.Resolve<Func<Uri, ITfsServer>>()(p.TypedAs<Uri>()),
				                           c.Resolve<Func<ITestManagementTeamProject, ITfsProjectWorkItemCollection>>()))
			       .As<ITfsExplorer>();

			builder.RegisterType<TfsProjectWorkItemCollection>()
			       .As<ITfsProjectWorkItemCollection>();

			builder.Register((c, p) => new VersionControlServerAdapter(p.TypedAs<TfsConnection>().GetService<VersionControlServer>()))
			       .As<IVersionControl>();

			builder.RegisterType<TestCaseAutomationService>().As<ITestCaseAutomationService>();
		}
	}
}