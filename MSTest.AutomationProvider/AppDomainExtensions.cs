using System;
using System.Reflection;

namespace MSTest.AutomationProvider
{
    /// <summary>
    /// Provides extensions for <see cref="AppDomain"/>s.
    /// </summary>
    public static class AppDomainExtensions
    {
        /// <summary>
        /// Creates an instance of a type in an <see cref="AppDomain"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="domain">The domain to create the object in.</param>
        /// <param name="args">Any constructor arguments for the type.</param>
        /// <returns>An instance of the desired type.</returns>
        public static T CreateObject<T>(this AppDomain domain, params object[] args)
        {
            return (T)domain.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName, typeof(T).FullName,
                                                     false,
                                                     BindingFlags.Default, null,
                                                     args,
                                                     null, null);
        }

        /// <summary>
        /// Retrieves strongly-typed data from an <see cref="AppDomain"/>.
        /// </summary>
        /// <typeparam name="T">The type of data to retrieve.</typeparam>
        /// <param name="domain">The app domain containing the data.</param>
        /// <param name="key">The data's key.</param>
        /// <returns>The data if it exists.</returns>
        public static T GetData<T>(this AppDomain domain, string key)
        {
            return (T)domain.GetData(key);
        }
    }
}