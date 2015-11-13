using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Threading;
using SharpEssentials.Collections;
using SharpEssentials.Testing;
using SharpEssentials.Testing.Controls.WPF;
using TestCaseAutomator.ViewModels.Browser.Nodes;
using Xunit;

namespace Tests.Unit.TestCaseAutomator.ViewModels.Browser.Nodes
{
    public class VirtualizedNodeTests
    {
        [Fact]
        public void Test_Initial_State()
        {
            // Act.
            var underTest = new TestNode("1");

            // Assert.
            Assert.False(underTest.IsLoading);
            Assert.True(underTest.IsEnabled);
            Assert.False(underTest.IsExpanded);
            Assert.False(underTest.IsRealized);
            AssertThat.SequenceEqual(underTest.Dummy.ToEnumerable(), underTest.Children);
        }

        [Fact]
        [Synchronous]
        public void Test_Expand()
        {
            // Arrange.
            var wasLoading = false;
            var wasEnabled = false;
            var underTest = new TestNode("1", n =>
            {
                wasLoading = n.IsLoading;
                wasEnabled = n.IsEnabled;
            });

            // Act.
            underTest.ExpandedCommand.Execute(null);

            // Assert.
            Assert.True(underTest.IsExpanded);

            Assert.True(wasLoading);
            Assert.False(underTest.IsLoading);

            Assert.False(wasEnabled);
            Assert.True(underTest.IsEnabled);

            Assert.True(underTest.IsRealized);

            AssertThat.SequenceEqual(new[] { "a", "b", "c" }, underTest.Children);
        }

        [Fact]
        [Synchronous]
        public void Test_Refresh()
        {
            // Arrange.
            var underTest = new TestNode("1");
            underTest.ExpandedCommand.Execute(null);

            // Act.
            underTest.RefreshCommand.Execute(null);

            // Assert.
            Assert.False(underTest.IsExpanded);
            Assert.False(underTest.IsRealized);
            AssertThat.SequenceEqual(underTest.Dummy.ToEnumerable(), underTest.Children);
        }

        [Fact]
        [Synchronous]
        public void Test_Can_Refresh()
        {
            // Arrange.
            var underTest = new TestNode("1");

            // Act.
            var couldInitiallyExecute = underTest.RefreshCommand.CanExecute(null);
            underTest.ExpandedCommand.Execute(null);

            var couldExecuteAfterExpansion = underTest.RefreshCommand.CanExecute(null);
            underTest.RefreshCommand.Execute(null);

            var couldExecuteAfterRefresh = underTest.RefreshCommand.CanExecute(null);

            // Assert.
            Assert.False(couldInitiallyExecute);
            Assert.True(couldExecuteAfterExpansion);
            Assert.False(couldExecuteAfterRefresh);
        }

        [WpfTheory]
        [InlineData(false, false, 1)]
        [InlineData(true,  false, 2)]
        [InlineData(false, true,  2)]
        [InlineData(true,  true,  2)]
        public void Test_Invalidation(bool shouldInvalidate, bool throwError, int expectedLoadCount)
        {
            // Arrange.
            int loadCount = 0;
            var underTest = new TestNode("1", _ =>
            {
                int prevCount = loadCount++;
                if (prevCount == 0 && throwError)
                    throw new InvalidOperationException("test error");
            })
            {
                ShouldInvalidate = shouldInvalidate
            };

            try
            {
                Dispatcher.CurrentDispatcher.Invoke(() =>
                    underTest.ExpandedCommand.Execute(null));
            }
            catch (Exception) { }

            // Act.
            Dispatcher.CurrentDispatcher.Invoke(() =>
                underTest.ExpandedCommand.Execute(null));


            // Assert.
            Assert.Equal(expectedLoadCount, loadCount);
        }

        class TestNode : VirtualizedNode<string>
        {
            private readonly Action<TestNode> _loadCallback;

            public TestNode(string name)
                : this(name, _ => { }) { }

            public TestNode(string name, Action<TestNode> loadCallback)
            {
                _loadCallback = loadCallback;
                Name = name;
            }

            public override string Name { get; }

            protected override string DummyNode => "DUMMY";

            public string Dummy => DummyNode;

            public bool ShouldInvalidate { get; set; }

            protected override Task<IReadOnlyCollection<string>> LoadChildrenAsync(IProgress<string> progress)
            {
                if (ShouldInvalidate)
                    Invalidate();

                _loadCallback(this);

                var children = new List<string> { "a", "b", "c" };
                children.ForEach(progress.Report);
                return Task.FromResult<IReadOnlyCollection<string>>(children);
            }
        } 
    }
}