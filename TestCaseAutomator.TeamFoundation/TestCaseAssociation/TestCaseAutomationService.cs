﻿using System;
using Microsoft.TeamFoundation.TestManagement.Client;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace TestCaseAutomator.TeamFoundation.TestCaseAssociation
{
	/// <summary>
	/// Provides operations related to test case automation association.
	/// </summary>
	public class TestCaseAutomationService : ITestCaseAutomationService
	{
		/// <summary>
		/// Associates a test case with an automated test.
		/// </summary>
		public void AssociateWithAutomation(ITestCase testCase, ITestAutomation automation)
		{
			// Create the associated automation.
			var implementation = testCase.Project.CreateTmiTestImplementation(
				automation.Name, automation.TestType, automation.Storage, automation.Identifier);

			// Save the test. If you are doing this for lots of test, you can consider
			// bulk saving too (outside of this method) for performance reason.
			testCase.WorkItem.Open();
			testCase.Implementation = implementation;
			testCase.Save();
		}

		/// <summary>
		/// Removes automation from a test case.
		/// </summary>
		public void RemoveAutomation(ITestCase testCase)
		{
			testCase.WorkItem.Open();
			testCase.Implementation = null;
			testCase.CustomFields["Automation status"].Value = "Not Automated";
			testCase.Save();
		}

		/// <summary>
		/// Returns an <see cref="ITestAutomation"/> representing a <see cref="ITestCase"/>'s
		/// existing automation if it has any.
		/// </summary>
		public ITestAutomation GetExistingAutomation(ITestCase testCase)
		{
			return testCase.IsAutomated ? new ExistingTestAutomation((ITmiTestImplementation)testCase.Implementation) : null;
		}

		private class ExistingTestAutomation : ITestAutomation
		{
			public ExistingTestAutomation(ITmiTestImplementation existingAutomation)
			{
				Identifier = existingAutomation.TestId;
				Name = existingAutomation.TestName;
				TestType = existingAutomation.TestType;
				Storage = existingAutomation.Storage;
			}

			public Guid Identifier { get; private set; }
			public string Name { get; private set; }
			public string TestType { get; private set; }
			public string Storage { get; private set; }
		}
	}
}