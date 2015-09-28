using System;
using System.Linq.Expressions;
using Autofac;
using Autofac.Builder;
using SharpEssentials.Reflection;
using TestCaseAutomator.Configuration;

namespace TestCaseAutomator.Container.Support
{
	/// <summary>
	/// Provides additional registration helpers.
	/// </summary>
	public static class RegistrationExtensions
	{
        /// <summary>
        /// Configures an explicit value for a property.
        /// </summary>
        /// <typeparam name="TLimit">Registration limit type.</typeparam>
        /// <typeparam name="TReflectionActivatorData">Activator data type.</typeparam>
        /// <typeparam name="TStyle">Registration style.</typeparam>
        /// <typeparam name="TValue">The type of the property being set</typeparam>
        /// <param name="registration">Registration to set property on.</param>
        /// <param name="property">An expression referencing a property on the target type.</param>
        /// <param name="propertyValue">Value to supply to the property.</param>
        /// <returns>A registration builder allowing further configuration of the component.</returns>
        public static IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> WithProperty<TLimit, TReflectionActivatorData, TStyle, TValue>(
                this IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> registration,
                Expression<Func<TLimit, TValue>> property, TValue propertyValue) where TReflectionActivatorData : ReflectionActivatorData
        {
            return registration.WithProperty(Reflect.PropertyOf(property).Name, propertyValue);
        }

        /// <summary>
        /// Configures a constructor parameter based on the given value. The provided value's type
        /// must match the constructor argument's type.
        /// </summary>
        /// <typeparam name="TLimit">Registration limit type.</typeparam>
        /// <typeparam name="TReflectionActivatorData">Activator data type.</typeparam>
        /// <typeparam name="TStyle">Registration style.</typeparam>
        /// <typeparam name="TValue">The type of the constructor parameter and the argument being provided.</typeparam>
        /// <param name="registration">The registration to configure.</param>
        /// <param name="valueProvider">The value provider.</param>
        /// <returns>A registration builder allowing further configuration of the component.</returns>
        public static IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> WithParameter<TLimit, TReflectionActivatorData, TStyle, TValue>(
                this IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> registration,
                Func<IComponentContext, TValue> valueProvider) where TReflectionActivatorData : ReflectionActivatorData
        {
            return registration.WithParameter((p, c) => p.ParameterType == typeof(TValue), (p, c) => valueProvider(c));
        }

        /// <summary>
        /// Configures a constructor parameter based on the given value. The provided value's type
        /// must match the constructor argument's type.
        /// </summary>
        /// <typeparam name="TLimit">Registration limit type.</typeparam>
        /// <typeparam name="TReflectionActivatorData">Activator data type.</typeparam>
        /// <typeparam name="TStyle">Registration style.</typeparam>
        /// <typeparam name="TValue">The type of the constructor parameter and the argument being provided.</typeparam>
        /// <param name="registration">The registration to configure.</param>
        /// <param name="valueProvider">The value provider.</param>
        /// <returns>A registration builder allowing further configuration of the component.</returns>
        public static IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> WithParameter<TLimit, TReflectionActivatorData, TStyle, TValue>(
                this IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> registration,
                Func<TValue> valueProvider) where TReflectionActivatorData : ReflectionActivatorData
        {
            return registration.WithParameter(c => valueProvider());
        }

        /// <summary>
        /// Provides a more convenient way to configure an instance using application settings.
        /// </summary>
        /// <typeparam name="TLimit">Registration limit type.</typeparam>
        /// <typeparam name="TActivatorData">Activator data type.</typeparam>
		/// <typeparam name="TRegistrationStyle">Registration style.</typeparam>
        /// <param name="registration">A registration for instances that require configuration.</param>
        /// <param name="applicator">Performs instance configuration using settings.</param>
        /// <returns>A registration builder allowing further configuration of the component.</returns>
        public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> ApplySettings<TLimit, TActivatorData, TRegistrationStyle>(
                this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registration,
                Action<ISettings, TLimit> applicator)
        {
            return registration.OnActivating(c => applicator(c.Context.Resolve<ISettings>(), c.Instance));
        }
    }
}