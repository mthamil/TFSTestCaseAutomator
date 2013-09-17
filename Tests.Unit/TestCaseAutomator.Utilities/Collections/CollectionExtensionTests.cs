﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using TestCaseAutomator.Utilities.Collections;
using Xunit;
using Xunit.Extensions;

namespace Tests.Unit.TestCaseAutomator.Utilities.Collections
{
	public class CollectionExtensionTests
	{
		[Theory]
		[InlineData(new[] { "1", "2", "3", "4" }, new[] { "3", "4" })]
		[InlineData(new[] { "1", "2", "3" }, new[] { "3" })]
		[InlineData(new[] { "1", "2" }, new string[0])]
		public void Test_AddRange_Enumerable(IEnumerable<string> expected, IEnumerable<string> additions)
		{
			// Arrange.
			ICollection<string> items = new Collection<string> { "1", "2" };

			// Act.
			items.AddRange(additions);

			// Assert.
			AssertThat.SequenceEqual(expected, items);
		}

		[Fact]
		public void Test_AddRange_Params()
		{
			// Arrange.
			ICollection<string> items = new Collection<string> { "1", "2" };

			// Act.
			items.AddRange("3", "4", "5");

			// Assert.
			AssertThat.SequenceEqual(new[] { "1", "2", "3", "4", "5" }, items);
		}

		[Theory]
		[InlineData(new[] { "1", "2", "3", "4" }, new[] { "3", "4" })]
		[InlineData(new[] { "1", "2", "3" }, new[] { "3" })]
		[InlineData(new[] { "1", "2" }, new string[0])]
		public void Test_AddRange_Enumerable_With_List(IEnumerable<string> expected, IEnumerable<string> additions)
		{
			// Arrange.
			ICollection<string> items = new List<string> { "1", "2" };

			// Act.
			items.AddRange(additions);

			// Assert.
			AssertThat.SequenceEqual(expected, items);
		}

		[Fact]
		public void Test_AddRange_Params_With_List()
		{
			// Arrange.
			ICollection<string> items = new List<string> { "1", "2" };

			// Act.
			items.AddRange("3", "4", "5");

			// Assert.
			AssertThat.SequenceEqual(new[] { "1", "2", "3", "4", "5" }, items);
		}
	}
}