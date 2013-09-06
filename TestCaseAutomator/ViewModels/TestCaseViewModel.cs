using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.TeamFoundation.TestManagement.Client;
using TestCaseAutomator.AutomationProviders.Interfaces;
using TestCaseAutomator.TeamFoundation.TestCaseAssociation;
using TestCaseAutomator.Utilities.Mvvm;
using TestCaseAutomator.Utilities.Mvvm.Commands;
using TestCaseAutomator.Utilities.Reflection;

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
		public int Id
		{
			get { return _testCase.Id; }
		}

		/// <see cref="ITestCaseViewModel.Title"/>
		public string Title 
		{ 
			get { return _testCase.Title; } 
		}

		/// <see cref="ITestCaseViewModel.AssociatedAutomation"/>
		public string AssociatedAutomation
		{
			get { return _testCase.IsAutomated ? _testCase.Implementation.DisplayText : string.Empty; }
		}

		/// <see cref="ITestCaseViewModel.UpdateAutomation"/>
		public void UpdateAutomation(IAutomatedTest automatedTest)
		{
			_automationService.AssociateWithAutomation(_testCase, automatedTest);
		}

		/// <summary>
		/// Command that invokes <see cref="RemoveAutomation"/>.
		/// </summary>
		public ICommand RemoveAutomationCommand { get; private set; }

		/// <see cref="ITestCaseViewModel.CanRemoveAutomation"/>
		public bool CanRemoveAutomation
		{
			get { return _testCase.IsAutomated; }
		}

		/// <see cref="ITestCaseViewModel.RemoveAutomation"/>
		public void RemoveAutomation()
		{
			_automationService.RemoveAutomation(_testCase);
		}

		private void testCase_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_propertyMap.ContainsKey(e.PropertyName))
			{
				foreach (var property in _propertyMap[e.PropertyName])
					OnPropertyChanged(property);
			}
		}

		private readonly ITestCase _testCase;
		private readonly ITestCaseAutomationService _automationService;

		private static readonly IDictionary<string, IEnumerable<string>> _propertyMap = new Dictionary<string, IEnumerable<string>>
		{
			{ Reflect.PropertyOf<ITestCase>(p => p.Title).Name, new[] { Reflect.PropertyOf<TestCaseViewModel>(p => p.Title).Name } },
			{ Reflect.PropertyOf<ITestCase>(p => p.Implementation).Name, new[] 
				{ 
					Reflect.PropertyOf<TestCaseViewModel>(p => p.AssociatedAutomation).Name,
					Reflect.PropertyOf<TestCaseViewModel>(p => p.CanRemoveAutomation).Name
				}
			}
		};
	}
}