using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.MSTestFramework;

namespace MSTest.AutomationProvider
{
    public interface ITestSink
    {
        ICollection<UnitTestElement> Tests { get; }
    }
}