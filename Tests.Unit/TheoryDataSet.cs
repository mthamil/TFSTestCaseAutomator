﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Microsoft Open Technologies would like to thank its contributors, a list
// of whom are at http://aspnetwebstack.codeplex.com/wikipage?title=Contributors.
   
// Licensed under the Apache License, Version 2.0 (the "License"); you
// may not use this file except in compliance with the License. You may
// obtain a copy of the License at
   
// http://www.apache.org/licenses/LICENSE-2.0
   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
// implied. See the License for the specific language governing permissions
// and limitations under the License.

using System.Collections;
using System.Collections.Generic;

namespace Tests.Unit
{
	/// <summary>
	/// Helper class for generating test data for XUnit's <see cref="Xunit.Extensions.TheoryAttribute"/>-based tests.
	/// Should be used in combination with <see cref="Xunit.Extensions.PropertyDataAttribute"/>.
	/// </summary>
	/// <typeparam name="TParam">First parameter type</typeparam>
	public class TheoryDataSet<TParam> : TheoryDataSet
	{
		public void Add(TParam p)
		{
			AddItem(p);
		}
	}

	/// <summary>
	/// Helper class for generating test data for XUnit's <see cref="Xunit.Extensions.TheoryAttribute"/>-based tests.
	/// Should be used in combination with <see cref="Xunit.Extensions.PropertyDataAttribute"/>.
	/// </summary>
	/// <typeparam name="TParam1">First parameter type</typeparam>
	/// <typeparam name="TParam2">Second parameter type</typeparam>
	public class TheoryDataSet<TParam1, TParam2> : TheoryDataSet
	{
		public void Add(TParam1 p1, TParam2 p2)
		{
			AddItem(p1, p2);
		}
	}

	/// <summary>
	/// Helper class for generating test data for XUnit's <see cref="Xunit.Extensions.TheoryAttribute"/>-based tests.
	/// Should be used in combination with <see cref="Xunit.Extensions.PropertyDataAttribute"/>.
	/// </summary>
	/// <typeparam name="TParam1">First parameter type</typeparam>
	/// <typeparam name="TParam2">Second parameter type</typeparam>
	/// <typeparam name="TParam3">Third parameter type</typeparam>
	public class TheoryDataSet<TParam1, TParam2, TParam3> : TheoryDataSet
	{
		public void Add(TParam1 p1, TParam2 p2, TParam3 p3)
		{
			AddItem(p1, p2, p3);
		}
	}

	/// <summary>
	/// Helper class for generating test data for XUnit's <see cref="Xunit.Extensions.TheoryAttribute"/>-based tests.
	/// Should be used in combination with <see cref="Xunit.Extensions.PropertyDataAttribute"/>.
	/// </summary>
	/// <typeparam name="TParam1">First parameter type</typeparam>
	/// <typeparam name="TParam2">Second parameter type</typeparam>
	/// <typeparam name="TParam3">Third parameter type</typeparam>
	/// <typeparam name="TParam4">Fourth parameter type</typeparam>
	public class TheoryDataSet<TParam1, TParam2, TParam3, TParam4> : TheoryDataSet
	{
		public void Add(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4)
		{
			AddItem(p1, p2, p3, p4);
		}
	}

	/// <summary>
	/// Helper class for generating test data for XUnit's <see cref="Xunit.Extensions.TheoryAttribute"/>-based tests.
	/// Should be used in combination with <see cref="Xunit.Extensions.PropertyDataAttribute"/>.
	/// </summary>
	/// <typeparam name="TParam1">First parameter type</typeparam>
	/// <typeparam name="TParam2">Second parameter type</typeparam>
	/// <typeparam name="TParam3">Third parameter type</typeparam>
	/// <typeparam name="TParam4">Fourth parameter type</typeparam>
	/// <typeparam name="TParam5">Fifth parameter type</typeparam>
	public class TheoryDataSet<TParam1, TParam2, TParam3, TParam4, TParam5> : TheoryDataSet
	{
		public void Add(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5)
		{
			AddItem(p1, p2, p3, p4, p5);
		}
	}

	/// <summary>
	/// Base class for <c>TheoryDataSet</c> classes.
	/// </summary>
	public abstract class TheoryDataSet : IEnumerable<object[]>
	{
		private readonly List<object[]> data = new List<object[]>();

		protected void AddItem(params object[] values)
		{
			data.Add(values);
		}

		public IEnumerator<object[]> GetEnumerator()
		{
			return data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}