using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.VisualStudio.TestPlatform.MSTestFramework;

namespace MSTest.AutomationProvider
{
    public class TestSink : MarshalByRefObject, ITestSink
    {
        public ICollection<UnitTestElement> Tests { get; } = new List<UnitTestElement>();

        [SecurityCritical]
        public override sealed object InitializeLifetimeService()
        {
            return null;
        }
    }
}