﻿using System;
using System.Windows.Markup;
using SharpEssentials.Controls.Markup;
using TestCaseAutomator.ViewModels;

namespace TestCaseAutomator.Views.MarkupExtensions
{
	/// <summary>
	/// A XAML markup extension to allow definition of a generic view-model locator.
	/// </summary>
	[MarkupExtensionReturnType(typeof(object))]
	public class ViewModelLocator : ActivatorExtension
	{
		/// <summary>
		/// Default constructor required by XAML.
		/// </summary>
		public ViewModelLocator()
			: base(ViewModelLocatorType) { }

		/// <summary>
		/// Default constructor required by XAML.
		/// </summary>
		public ViewModelLocator(Type viewModelType)
			: this()
		{
			TypeArgument = viewModelType;
		}

		/// <summary>
		/// The type of the view-model.
		/// </summary>
		public Type TypeArgument { get; set; }

		/// <summary>
		/// The object config name of a container-defined view-model.
		/// </summary>
		public string Name { get; set; }

		#region Overrides of MarkupExtension

		/// <see cref="MarkupExtension.ProvideValue"/>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			TypeArguments.Add(TypeArgument);
			ConstructorArguments.Add(App.Container);
			ConstructorArguments.Add(Name);
			return base.ProvideValue(serviceProvider);
		}

		#endregion

		private static readonly Type ViewModelLocatorType = typeof(AutofacViewModelLocator<>);
	}
}