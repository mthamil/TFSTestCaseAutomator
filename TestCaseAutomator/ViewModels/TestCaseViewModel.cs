using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.TeamFoundation.TestManagement.Client;
using TestCaseAutomator.AutomationProviders.Interfaces;
using TestCaseAutomator.TeamFoundation.TestCaseAssociation;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Controls.Mvvm.Commands;

namespace TestCaseAutomator.ViewModels
{
	/// <summary>
	/// Represents a test case.
	/// </summary>
	public class TestCaseViewModel : ViewModelBase, ITestCaseViewModel
	{
		/// <summary>
		/// Initializes a new <see cref="TestCaseViewModel"/>.
		/// </summary>
		/// <param name="testCase">The backing TFS test case</param>
		/// <param name="automationService">Enables modification of a test case's associated automation</param>
		public TestCaseViewModel(ITestCase testCase, ITestCaseAutomationService automationService)
		{
			_testCase = testCase;
			_automationService = automationService;

			RemoveAutomationCommand = Command.For(this)
			                                 .DependsOn(p => p.CanRemoveAutomation)
			                                 .Executes(RemoveAutomation);

			_testCase.PropertyChanged += testCase_PropertyChanged;
		}

		/// <see cref="ITestCaseViewModel.Id"/>
		public int Id => _testCase.Id;

	    /// <see cref="ITestCaseViewModel.Title"/>
		public string Title => _testCase.Title;

	    /// <see cref="ITestCaseViewModel.AssociatedAutomation"/>
		public string AssociatedAutomation 
            => _testCase.IsAutomated 
                    ? _testCase.Implementation.DisplayText 
                    : null;

	    /// <see cref="ITestCaseViewModel.UpdateAutomation"/>
		public void UpdateAutomation(ITestAutomation testAutomation)
		{
			_automationService.AssociateWithAutomation(_testCase, testAutomation);
		}

		/// <summary>
		/// Command that invokes <see cref="RemoveAutomation"/>.
		/// </summary>
		public ICommand RemoveAutomationCommand { get; }

		/// <see cref="ITestCaseViewModel.CanRemoveAutomation"/>
		public bool CanRemoveAutomation => _testCase.IsAutomated;

	    /// <see cref="ITestCaseViewModel.RemoveAutomation"/>
		public void RemoveAutomation()
		{
			_automationService.RemoveAutomation(_testCase);
		}

		/// <see cref="ITestCaseViewModel.GetAutomation"/>
		public ITestAutomation GetAutomation() 
            => _automationService.GetExistingAutomation(_testCase);

	    private void testCase_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (PropertyMap.ContainsKey(e.PropertyName))
			{
				foreach (var property in PropertyMap[e.PropertyName])
					OnPropertyChanged(property);
			}
		}

		private readonly ITestCase _testCase;
		private readonly ITestCaseAutomationService _automationService;

		private static readonly IDictionary<string, IEnumerable<string>> PropertyMap = new Dictionary<string, IEnumerable<string>>
		{
			{ nameof(ITestCase.Title), new[] { nameof(Title) } },
			{ nameof(ITestCase.Implementation), new[] { nameof(AssociatedAutomation), nameof(CanRemoveAutomation) } }
		};
	}
}