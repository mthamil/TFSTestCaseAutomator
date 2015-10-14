using System;
using TestCaseAutomator.ViewModels.Browser;

namespace TestCaseAutomator.ViewModels
{
    public interface ITestCases
    {
        IWorkItems WorkItems { get; }

        /// <summary>
        /// The currently selected test case.
        /// </summary>
        ITestCaseViewModel SelectedTestCase { get; set; }

        Lazy<TestBrowserViewModel> TestBrowser { get; }
    }
}