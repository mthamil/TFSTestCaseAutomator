using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestCaseAutomator.ViewModels.Browser;

namespace TestCaseAutomator.ViewModels
{
    public interface ITestCases
    {
        Lazy<TestBrowserViewModel> TestBrowser { get; }

        /// <summary>
        /// The currently selected test case.
        /// </summary>
        ITestCaseViewModel SelectedTestCase { get; set; }

        /// <summary>
        /// The current collection of test cases.
        /// </summary>
        ICollection<ITestCaseViewModel> Items { get; }

        /// <summary>
        /// Loads the test cases for a given project.
        /// </summary>
        /// <param name="projectName">The project for which to load test cases.</param>
        Task LoadAsync(string projectName);
    }
}