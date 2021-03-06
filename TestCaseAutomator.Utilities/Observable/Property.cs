using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using TestCaseAutomator.Utilities.Reflection;

namespace TestCaseAutomator.Utilities.Observable
{
	/// <summary>
	/// Allows convenient creation of Property objects.
	/// </summary>
	public static class Property
	{
		/// <summary>
		/// Creates a new Property&lt;V&gt;.
		/// </summary>
		/// <typeparam name="T">The type that contains the property</typeparam>
		/// <typeparam name="V">The type of the property</typeparam>
		/// <param name="owner">An instance of the type that contains the property</param>
		/// <param name="propertyAccessor">An expression that references the desired property</param>
		/// <param name="propertyChangedHandler">A function that handles property changes</param>
		/// <returns>A new ObservableProperty&lt;V&gt;</returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Factory method uses parameter for type inference.")]
		public static PropertyBuilder<T, V> New<T, V>(T owner, Expression<Func<T, V>> propertyAccessor, Action<string> propertyChangedHandler)
		{
			return new PropertyBuilder<T, V>(owner, propertyAccessor, propertyChangedHandler);
		}
	}

	/// <summary>
	/// Class that helps configure a Property&lt;V&gt;.
	/// </summary>
	/// <typeparam name="T">The type that contains the property</typeparam>
	/// <typeparam name="V">The type of the property</typeparam>
	public class PropertyBuilder<T, V>
	{
		/// <summary>
		/// Creates a new PropertyBuilder&lt;T,V&gt;.
		/// </summary>
		/// <param name="owner">An instance of the type that contains the property</param>
		/// <param name="propertyAccessor">An expression that references the desired property</param>
		/// <param name="propertyChangedHandler">A function that handles property changes</param>
		public PropertyBuilder(T owner, Expression<Func<T, V>> propertyAccessor, Action<string> propertyChangedHandler)
		{
			_propertyName = Reflect.PropertyOf(typeof(T), UnwrapPropertyExpression(propertyAccessor)).Name;
			_propertyChangedHandler = propertyChangedHandler;
		}

		/// <summary>
		/// Indicates that another property's value changes as a result of changing THIS property's value.
		/// Only properties with Get-accessibility only are allowed.
		/// </summary>
		/// <typeparam name="VOther">The type of the dependent property</typeparam>
		/// <param name="otherPropertyAccessor">An expression that references the dependent property</param>
		public PropertyBuilder<T, V> AlsoChanges<VOther>(Expression<Func<T, VOther>> otherPropertyAccessor)
		{
			var dependentProperty = Reflect.PropertyOf(typeof(T), UnwrapPropertyExpression(otherPropertyAccessor));
			if (dependentProperty.SetMethod != null)
				throw new ArgumentException("Properties with setters cannot be dependent!");

			_dependentPropertyNames.Add(dependentProperty.Name);
			return this;
		}

		/// <summary>
		/// Specifies a custom comparison to use to determine when a property's value changes.
		/// </summary>
		/// <param name="equalityComparison">Custom equality comparison, if Equals is not suitable for comparing objects of type V</param>
		public PropertyBuilder<T, V> EqualWhen(Func<V, V, bool> equalityComparison)
		{
			_customComparison = equalityComparison;
			return this;
		}

		/// <summary>
		/// Finishes building the Property&lt;V&gt;.
		/// </summary>
		/// <returns>The newly created Property&lt;V&gt;</returns>
		public Property<V> Get()
		{
			return new Property<V>(_propertyName, _propertyChangedHandler, _dependentPropertyNames, _customComparison);
		}

		/// <summary>
		/// Converts a PropertyBuilder into its output Property.
		/// </summary>
		public static implicit operator Property<V>(PropertyBuilder<T, V> builder)
		{
			return builder.Get();
		}

		/// <summary>
		/// The following method is necessary to allow a Property to be created that is accessed using a 
		/// property of a different type than it is actually declaring.
		/// For example, we have the following property:
		/// <code>
		/// public IEnumerable&lt;int&gt; Integers { get { return _integers.Value; } }
		/// private readonly Property&lt;IList&lt;int&gt;&gt; _integers;
		/// </code>
		/// That is created using the following:
		/// <code>
		/// _integers = Property.New(this, p => p.Integers as IList&lt;int&gt;, OnPropertyChanged);
		/// </code>
		/// </summary>
		private static LambdaExpression UnwrapPropertyExpression<TValue>(Expression<Func<T, TValue>> propertyAccessor)
		{
			if (propertyAccessor.Body.NodeType == ExpressionType.TypeAs)
			{
				var typeAs = (UnaryExpression)propertyAccessor.Body;
				return Expression.Lambda(typeAs.Operand, propertyAccessor.Parameters);
			}

			return propertyAccessor;
		}

		private readonly string _propertyName;
		private readonly Action<string> _propertyChangedHandler;
		private readonly ICollection<string> _dependentPropertyNames = new List<string>();
		private Func<V, V, bool> _customComparison;
	}

	/// <summary>
	/// Encapsulates a property.
	/// </summary>
	/// <typeparam name="V">The type of the property value</typeparam>
	public class Property<V>
	{
		/// <summary>
		/// Creates a new property.
		/// </summary>
		/// <param name="propertyName">The name of the property</param>
		/// <param name="propertyChangedHandler">Handler invoked when a property's value changes</param>
		public Property(string propertyName, Action<string> propertyChangedHandler)
			: this(propertyName, propertyChangedHandler, Enumerable.Empty<string>(), null) { }

		/// <summary>
		/// Creates a new property.
		/// </summary>
		/// <param name="propertyName">The name of the property</param>
		/// <param name="propertyChangedHandler">Handler invoked when a property's value changes</param>
		/// <param name="dependentPropertyNames">Names of properties that are also changed when the property's value changes</param>
		/// <param name="equalityComparison">An optional custom equality comparison, if Object.Equals is not suitable for comparing objects of type V</param>
		public Property(string propertyName, Action<string> propertyChangedHandler, IEnumerable<string> dependentPropertyNames, Func<V, V, bool> equalityComparison)
		{
			_propertyChangedHandler = propertyChangedHandler;
			_name = propertyName;
			_dependentPropertyNames = dependentPropertyNames;
			if (equalityComparison != null)
				_equalityComparison = equalityComparison;
		}

		/// <summary>
		/// The property value.
		/// </summary>
		public V Value
		{
			get { return _value; }
			set { TrySetValue(value); }
		}

		/// <summary>
		/// Attempts to update a property's value.  If the new value is not equal
		/// to the old value, the value will be updated and the method will return true.
		/// </summary>
		/// <param name="newValue">The property's new value</param>
		/// <returns>True if the value actually changed, false otherwise</returns>
		public bool TrySetValue(V newValue)
		{
			if (!_equalityComparison(_value, newValue))
			{
				_value = newValue;
				_propertyChangedHandler(_name);
				foreach (var dependentPropertyName in _dependentPropertyNames)
					_propertyChangedHandler(dependentPropertyName);

				return true;
			}

			return false;
		}

		/// <summary>
		/// The property's name.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		private readonly string _name;
		private V _value;
		private readonly Action<string> _propertyChangedHandler;
		private readonly IEnumerable<string> _dependentPropertyNames;
		private readonly Func<V, V, bool> _equalityComparison = (x, y) => EqualityComparer<V>.Default.Equals(x, y);
	}
}