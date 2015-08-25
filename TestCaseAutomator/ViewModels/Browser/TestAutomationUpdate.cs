using System;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace TestCaseAutomator.ViewModels.Browser
{
    public class TestAutomationUpdate : ITestAutomation
    {
        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public string TestType { get; set; }
        public string Storage { get; set; }
    }
}