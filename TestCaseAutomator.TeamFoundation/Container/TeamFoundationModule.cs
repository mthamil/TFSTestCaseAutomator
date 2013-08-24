using System;
using Autofac;
using Microsoft.TeamFoundation.Client;
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

			builder.RegisterType<TfsExplorer>()
			       .As<ITfsExplorer>();

			builder.RegisterType<TfsProjectWorkItemCollection>()
			       .As<ITfsProjectWorkItemCollection>();

			builder.Register((c, p) => new VersionControlServerAdapter(p.TypedAs<TfsConnection>().GetService<VersionControlServer>()))
			       .As<IVersionControl>();
		}
	}
}