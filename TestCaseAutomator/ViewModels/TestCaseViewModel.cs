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
	public class TestCaseViewModel : ViewModelBase
	{
		/// <summary>
		/// Initializes a new <see cref="TestCaseViewModel"/>.
		/// </summary>
		/// <param name="testCase">The backing TFS test case</param>
		public TestCaseViewModel(ITestCase testCase)
		{
			_testCase = testCase;

			RemoveAutomationCommand = Command.For(this)
			                                 .DependsOn(p => p.CanRemoveAutomation)
			                                 .Executes(RemoveAutomation);

			_testCase.PropertyChanged += testCase_PropertyChanged;
		}

		/// <summary>
		/// A test case's work item ID.
		/// </summary>
		public int Id
		{
			get { return _testCase.Id; }
		}

		/// <summary>
		/// A test case's title.
		/// </summary>
		public string Title 
		{ 
			get { return _testCase.Title; } 
		}

		/// <summary>
		/// A test case's associated automation if any exists.
		/// </summary>
		public string AssociatedAutomation
		{
			get { return _testCase.IsAutomated ? _testCase.Implementation.DisplayText : string.Empty; }
		}

		/// <summary>
		/// Updates the automation a test case is associated with.
		/// </summary>
		/// <param name="automatedTest">An automated test</param>
		public void UpdateAutomation(IAutomatedTest automatedTest)
		{
			_testCase.AssociateWithAutomation(automatedTest);
		}

		/// <summary>
		/// Command that invokes <see cref="RemoveAutomation"/>.
		/// </summary>
		public ICommand RemoveAutomationCommand { get; private set; }

		/// <summary>
		/// Whether automation can be removed.
		/// </summary>
		public bool CanRemoveAutomation
		{
			get { return _testCase.IsAutomated; }
		}

		/// <summary>
		/// Removes the automation from a test case.
		/// </summary>
		public void RemoveAutomation()
		{
			_testCase.RemoveAutomation();
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