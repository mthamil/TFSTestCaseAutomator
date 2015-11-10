using System;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Observable;
using TestCaseAutomator.AutomationProviders.Abstractions;

namespace TestCaseAutomator.ViewModels.Browser
{
    public class TestAutomationViewModel : ViewModelBase, ITestAutomation
    {
        public TestAutomationViewModel()
        {
            _identifier = Property.New(this, p => p.Identifier);
            _name = Property.New(this, p => p.Name);
            _testType = Property.New(this, p => p.TestType);
            _storage = Property.New(this, p => p.Storage);
        }

        /// <summary>
        /// A test's unique identifier.
        /// </summary>
        public Guid Identifier
        {
            get { return _identifier.Value; }
            set { _identifier.Value = value; }
        }

        /// <summary>
        /// The test name.
        /// </summary>
        public string Name
        {
            get { return _name.Value; }
            set { _name.Value = value; }
        }

        /// <summary>
        /// A string representation of the test's type.
        /// </summary>
        public string TestType
        {
            get { return _testType.Value; }
            set { _testType.Value = value; }
        }

        /// <summary>
        /// A string representation of where a test is located. For example,
        /// this may be the path of a DLL or source file.
        /// </summary>
        public string Storage
        {
            get { return _storage.Value; }
            set { _storage.Value = value; }
        }

        private readonly Property<Guid> _identifier;
        private readonly Property<string> _name;
        private readonly Property<string> _testType;
        private readonly Property<string> _storage;
    }
}