using System;
using System.Reflection;
using Autofac.Core.Activators.Reflection;

namespace TestCaseAutomator.Container.Support
{
    /// <summary>
    /// An Autofac <see cref="IConstructorFinder"/> that returns non-public constructors.
    /// </summary>
    public class NonPublicConstructorFinder : IConstructorFinder
    {
        public ConstructorInfo[] FindConstructors(Type targetType) 
            => targetType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
    }
}