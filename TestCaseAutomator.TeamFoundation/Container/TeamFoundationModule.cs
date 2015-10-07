using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.TeamFoundation.Client;
using TestCaseAutomator.AutomationProviders.Interfaces;
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
			       .As<TfsConnection>();

			builder.Register((c, p) => new TfsServer(c.Resolve<Func<Uri, TfsConnection>>()(p.TypedAs<Uri>()), 
                                                     c.Resolve<TaskScheduler>()))
			       .As<ITfsServer>();

			builder.RegisterType<TfsExplorer>()
			       .As<ITfsExplorer>()
                   .SingleInstance();

		    builder.RegisterType<TestCaseAutomationService>()
		           .AsImplementedInterfaces();

            builder.RegisterType<HashedIdentifierFactory>()
                   .AsImplementedInterfaces();
        }
	}
}