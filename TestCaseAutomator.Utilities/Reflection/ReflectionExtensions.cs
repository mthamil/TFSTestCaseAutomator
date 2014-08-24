using System;

namespace TestCaseAutomator.Utilities.Reflection
{
	/// <summary>
	/// Contains extension methods for reflection types.
	/// </summary>
	public static class ReflectionExtensions
	{
		/// <summary>
		/// If a type is a primitive such as int, returns its default, otherwise
		/// null is returned.
		/// </summary>
		/// <param name="type">The type to create a value for</param>
		/// <returns>A default value for the type</returns>
		public static object GetDefaultValue(this Type type)
		{
            if (type == null)
                throw new ArgumentNullException("type");

			if (type.IsValueType && type != VoidType)	// can't create an instance of Void
				return Activator.CreateInstance(type);

			return null;
		}
		private static readonly Type VoidType = typeof(void);

        /// <summary>
        /// Checks whether another type is the generic type definition of this type.
        /// </summary>
        /// <param name="type">The closed type to check</param>
        /// <param name="openGenericType">An open generic type that may be the definition of <paramref name="type"/></param>
        /// <returns>True if <paramref name="openGenericType"/> is the generic type definition of <paramref name="type"/></returns>
	    public static bool IsClosedTypeOf(this Type type, Type openGenericType)
	    {
            if (type == null)
                throw new ArgumentNullException("type");

            if (openGenericType == null)
                throw new ArgumentNullException("openGenericType");

            if (!openGenericType.IsGenericTypeDefinition)
                throw new ArgumentException("Must be an open generic type.", "openGenericType");

            return type.IsGenericType && type.GetGenericTypeDefinition() == openGenericType;
	    }
	}
}