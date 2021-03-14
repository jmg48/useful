namespace Ariadne.Extensions.ServiceCollection
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    internal abstract class FactoryBase<TService>
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Type[] argTypes;
        private readonly Type implementationType;
        private readonly ConstructorInfo constructor;
        private readonly ParameterInfo[] parameters;

        protected FactoryBase(IServiceProvider serviceProvider, ServiceMap serviceMap, Type[] argTypes)
        {
            this.serviceProvider = serviceProvider;
            this.argTypes = argTypes;
            var serviceDescriptor = serviceMap.TryGetValue(typeof(TService), out var value)
                ? value.Single()
                : throw new InvalidOperationException($"Unable to resolve service for type '{typeof(TService)}' while attempting to activate '{this.GetType().GetInterfaces().Single()}'.");

            this.implementationType = serviceDescriptor.ImplementationType;
            if (serviceDescriptor.Lifetime != ServiceLifetime.Transient)
            {
                throw new InvalidOperationException(
                    $"In order to resolve a parameterised factory for service type '{typeof(TService)}', the implementation type '{this.implementationType}' must be registered with Transient lifestyle.");
            }

            this.constructor = this.implementationType.GetConstructors().Single();
            this.parameters = this.constructor.GetParameters();
            if (!IsMatch(this.constructor, argTypes))
            {
                var argTypesText = string.Join(", ", argTypes.Select(argType => $"'{argType}'"));
                throw new InvalidOperationException(
                    $"In order to resolve a parameterised factory for service type '{typeof(TService)}', the implementation type '{this.implementationType}' must contain a constructor whose last {(argTypes.Length > 1 ? argTypes.Length + " parameters are assignable from the factory argument types" : "parameter is assignable from the factory argument type")} {argTypesText}.");
            }
        }

        protected TService New(object[] argValues)
        {
            if (argValues.Length != this.argTypes.Length)
            {
                // This shouldn't be possible
                throw new NotSupportedException();
            }

            var constructorArgs = new object[this.parameters.Length];
            for (var i = 0; i < this.parameters.Length - this.argTypes.Length; i++)
            {
                // n.b. Resolve services late, in order to respect lifestyles of resolved services
                constructorArgs[i] = this.serviceProvider.GetService(this.parameters[i].ParameterType) ??
                                     throw new InvalidOperationException($"Unable to resolve service for type '{this.parameters[i].ParameterType}' while attempting to activate '{this.implementationType}'.");
            }

            for (var i = this.parameters.Length - this.argTypes.Length; i < this.parameters.Length; i++)
            {
                constructorArgs[i] = argValues[i - this.parameters.Length + this.argTypes.Length];
            }

            return (TService)this.constructor.Invoke(constructorArgs);
        }

        private static bool IsMatch(ConstructorInfo constructor, Type[] argTypes)
        {
            var parameters = constructor.GetParameters();
            if (parameters.Length < argTypes.Length)
            {
                return false;
            }

            for (var i = 0; i < argTypes.Length; i++)
            {
                var argType = argTypes[i];
                var parameterType = parameters[i + parameters.Length - argTypes.Length].ParameterType;
                if (!parameterType.IsAssignableFrom(argType))
                {
                    return false;
                }
            }

            return true;
        }
    }
}