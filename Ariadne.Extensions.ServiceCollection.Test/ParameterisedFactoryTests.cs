using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Ariadne.Extensions.ServiceCollection.Test
{
    [TestFixture]
    public class ParameterisedFactoryTests
    {
        [Test]
        public void GetRequiredService_NotRegistered_ShouldThrow()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<TestServiceTwoParams> factory = () => serviceProvider.GetRequiredService<TestServiceTwoParams>();

            // This is actually checking already built-in behaviour, but illustrates the kind of exception thrown which we want to align with here
            factory.Should().Throw<InvalidOperationException>()
                .WithMessage($"No service for type '{typeof(TestServiceTwoParams)}' has been registered.");
        }

        [Test]
        public void GetRequiredService_DependencyNotRegistered_ShouldThrow()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<TestServiceTwoParams>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<TestServiceTwoParams> factory = () => serviceProvider.GetRequiredService<TestServiceTwoParams>();

            // This is actually checking already built-in behaviour, but illustrates the kind of exception thrown which we want to align with here
            factory.Should().Throw<InvalidOperationException>()
                .WithMessage($"Unable to resolve service for type '{typeof(string)}' while attempting to activate '{typeof(TestServiceTwoParams)}'.");
        }

        [Test]
        public void GetRequiredService_FactoryOfOneArgForRegisteredType_ShouldResolve()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<TestServiceOneParam<string>>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IFactory<string, TestServiceOneParam<string>>>();

            factory.New("a").Arg.Should().Be("a");
            factory.New("b").Arg.Should().Be("b");
        }

        [Test]
        public void GetRequiredService_FactoryOfOneArgForRegisteredInterface_ShouldResolve()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<ITestInterfaceOneParam<string>, TestServiceOneParam<string>>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IFactory<string, ITestInterfaceOneParam<string>>>();

            factory.New("a").Arg.Should().Be("a");
            factory.New("b").Arg.Should().Be("b");
        }

        [Test]
        public void GetRequiredService_FactoryOfOneArgForInterfaceOfRegisteredType_ShouldThrow()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<TestServiceOneParam<string>>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<IFactory<string, ITestInterfaceOneParam<string>>> factory = () => serviceProvider.GetRequiredService<IFactory<string, ITestInterfaceOneParam<string>>>();

            factory.Should().Throw<InvalidOperationException>()
                .WithMessage($"Unable to resolve service for type '{typeof(ITestInterfaceOneParam<string>)}' while attempting to activate '{typeof(IFactory<string, ITestInterfaceOneParam<string>>)}'.");
        }

        [Test]
        public void GetRequiredService_FactoryOfOneArgForTypeRegisteredAsNonTransient_ShouldThrow()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddScoped<TestServiceOneParam<string>>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<IFactory<string, TestServiceOneParam<string>>> factory = () => serviceProvider.GetRequiredService<IFactory<string, TestServiceOneParam<string>>>();

            factory.Should().Throw<InvalidOperationException>()
                .WithMessage($"In order to resolve a parameterised factory for service type '{typeof(TestServiceOneParam<string>)}', the implementation type '{typeof(TestServiceOneParam<string>)}' must be registered with Transient lifestyle.");
        }

        [Test]
        public void GetRequiredService_FactoryOfOneArgForTypeRegisteredAfterFactoryFacility_ShouldThrow()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddFactoryFacility();
            serviceCollection.AddTransient<TestServiceOneParam<string>>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<IFactory<string, TestServiceOneParam<string>>> factory = () => serviceProvider.GetRequiredService<IFactory<string, TestServiceOneParam<string>>>();

            factory.Should().Throw<InvalidOperationException>()
                .WithMessage($"Unable to resolve service for type '{typeof(TestServiceOneParam<string>)}' while attempting to activate '{typeof(IFactory<string, TestServiceOneParam<string>>)}'.");
        }

        [Test]
        public void GetRequiredService_FactoryOfOneArgForTypeOfRegisteredInterface_ShouldThrow()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<ITestInterfaceOneParam<string>, TestServiceOneParam<string>>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<IFactory<string, TestServiceOneParam<string>>> factory = () => serviceProvider.GetRequiredService<IFactory<string, TestServiceOneParam<string>>>();

            factory.Should().Throw<InvalidOperationException>()
                .WithMessage($"Unable to resolve service for type '{typeof(TestServiceOneParam<string>)}' while attempting to activate '{typeof(IFactory<string, TestServiceOneParam<string>>)}'.");
        }

        [Test]
        public void GetRequiredService_FactoryOfOneArgOfImplicitlyConvertibleType_ShouldThrow()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<TestServiceOneParam<double>>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<IFactory<int, TestServiceOneParam<double>>> factory = () => serviceProvider.GetRequiredService<IFactory<int, TestServiceOneParam<double>>>();

            factory.Should().Throw<InvalidOperationException>()
                .WithMessage($"In order to resolve a parameterised factory for service type '{typeof(TestServiceOneParam<double>)}', the implementation type '{typeof(TestServiceOneParam<double>)}' must contain a constructor whose last parameter is assignable from the factory argument type '{typeof(int)}'.");
        }

        [Test]
        public void GetRequiredService_FactoryOfOneArgOfAssignableType_ShouldResolve()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<TestServiceOneParam<IEnumerable<string>>>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IFactory<string[], TestServiceOneParam<IEnumerable<string>>>>();

            factory.New(new[] {"a"}).Arg.Single().Should().Be("a");
            factory.New(new[] {"b"}).Arg.Single().Should().Be("b");
        }

        [Test]
        public void GetRequiredService_FactoryOfTwoArgsForRegisteredType_ShouldResolve()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<TestServiceTwoParams>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IFactory<string, double, TestServiceTwoParams>>();

            var x = factory.New("a", 1.0);
            x.Arg1.Should().Be("a");
            x.Arg2.Should().Be(1.0);

            var y = factory.New("b", 2.0);
            y.Arg1.Should().Be("b");
            y.Arg2.Should().Be(2.0);
        }

        [Test]
        public void GetRequiredService_FactoryOfTwoArgsForRegisteredInterface_ShouldResolve()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<ITestInterfaceTwoParams, TestServiceTwoParams>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IFactory<string, double, ITestInterfaceTwoParams>>();

            var x = factory.New("a", 1.0);
            x.Arg1.Should().Be("a");
            x.Arg2.Should().Be(1.0);

            var y = factory.New("b", 2.0);
            y.Arg1.Should().Be("b");
            y.Arg2.Should().Be(2.0);
        }

        [Test]
        public void GetRequiredService_FactoryOfThreeArgsForRegisteredType_ShouldResolve()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<TestServiceThreeParams>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IFactory<string, double, bool, TestServiceThreeParams>>();

            var x = factory.New("a", 1.0, false);
            x.Arg1.Should().Be("a");
            x.Arg2.Should().Be(1.0);
            x.Arg3.Should().BeFalse();

            var y = factory.New("b", 2.0, true);
            y.Arg1.Should().Be("b");
            y.Arg2.Should().Be(2.0);
            y.Arg3.Should().BeTrue();
        }

        [Test]
        public void GetRequiredService_FactoryOfOneArgForTypeWithNoConstructor_ShouldThrow()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<NoConstructor>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<IFactory<string, NoConstructor>> factory = () => serviceProvider.GetRequiredService<IFactory<string, NoConstructor>>();

            factory.Should().Throw<InvalidOperationException>()
                .WithMessage($"In order to resolve a parameterised factory for service type '{typeof(NoConstructor)}', the implementation type '{typeof(NoConstructor)}' must contain a constructor whose last parameter is assignable from the factory argument type '{typeof(string)}'.");
        }

        [Test]
        public void GetRequiredService_FactoryOfTwoArgsForTypeWithNoConstructor_ShouldThrow()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<NoConstructor>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<IFactory<string, double, NoConstructor>> factory = () => serviceProvider.GetRequiredService<IFactory<string, double, NoConstructor>>();

            factory.Should().Throw<InvalidOperationException>()
                .WithMessage($"In order to resolve a parameterised factory for service type '{typeof(NoConstructor)}', the implementation type '{typeof(NoConstructor)}' must contain a constructor whose last 2 parameters are assignable from the factory argument types '{typeof(string)}', '{typeof(double)}'.");
        }

        [Test]
        public void GetRequiredService_FactoryOfOneArgForTypeWithWrongConstructor_ShouldThrow()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<WrongConstructor>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<IFactory<string, WrongConstructor>> factory = () => serviceProvider.GetRequiredService<IFactory<string, WrongConstructor>>();

            factory.Should().Throw<InvalidOperationException>()
                .WithMessage($"In order to resolve a parameterised factory for service type '{typeof(WrongConstructor)}', the implementation type '{typeof(WrongConstructor)}' must contain a constructor whose last parameter is assignable from the factory argument type '{typeof(string)}'.");
        }

        [Test]
        public void GetRequiredService_FactoryOfTwoArgsForTypeWithWrongConstructor_ShouldThrow()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<WrongConstructor>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<IFactory<string, double, WrongConstructor>> factory = () => serviceProvider.GetRequiredService<IFactory<string, double, WrongConstructor>>();

            factory.Should().Throw<InvalidOperationException>()
                .WithMessage($"In order to resolve a parameterised factory for service type '{typeof(WrongConstructor)}', the implementation type '{typeof(WrongConstructor)}' must contain a constructor whose last 2 parameters are assignable from the factory argument types '{typeof(string)}', '{typeof(double)}'.");
        }

        [Test]
        public void New_FactoryOfOneArgWithMissingServices_ShouldThrow()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<WrongConstructor>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IFactory<int, WrongConstructor>>();

            Func<WrongConstructor> instance = () => factory.New(1);

            instance.Should().Throw<InvalidOperationException>()
                .WithMessage($"Unable to resolve service for type '{typeof(double)}' while attempting to activate '{typeof(WrongConstructor)}'.");

        }

        [Test]
        public void New_FactoryOfTwoArgWithMissingService_ShouldThrow()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<WrongConstructor>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IFactory<string, int, WrongConstructor>>();

            Func<WrongConstructor> instance = () => factory.New("a", 1);

            instance.Should().Throw<InvalidOperationException>()
                .WithMessage($"Unable to resolve service for type '{typeof(double)}' while attempting to activate '{typeof(WrongConstructor)}'.");
        }

        [Test]
        public void New_FactoryOfOneArgWithMissingSecondService_ShouldThrow()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<WrongConstructor>();
            serviceCollection.AddSingleton(typeof(double), _ => 0);
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IFactory<int, WrongConstructor>>();

            Func<WrongConstructor> instance = () => factory.New(1);

            instance.Should().Throw<InvalidOperationException>()
                .WithMessage($"Unable to resolve service for type '{typeof(string)}' while attempting to activate '{typeof(WrongConstructor)}'.");
        }

        private class WrongConstructor
        {
            public WrongConstructor(double a, string b, int c)
            {
            }
        }

        private class NoConstructor
        {
        }
        
        private interface ITestInterfaceOneParam<out T>
        {
            T Arg { get; }
        }

        private class TestServiceOneParam<T> : ITestInterfaceOneParam<T>
        {
            public TestServiceOneParam(T arg) => Arg = arg;

            public T Arg { get; }
        }

        private interface ITestInterfaceTwoParams
        {
            string Arg1 { get; }

            double Arg2 { get; }
        }

        private class TestServiceTwoParams : ITestInterfaceTwoParams
        {
            public TestServiceTwoParams(string arg1, double arg2)
            {
                Arg1 = arg1;
                Arg2 = arg2;
            }

            public string Arg1 { get; }

            public double Arg2 { get; }
        }

        private class TestServiceThreeParams
        {
            public TestServiceThreeParams(string arg1, double arg2, bool arg3)
            {
                Arg1 = arg1;
                Arg2 = arg2;
                Arg3 = arg3;
            }

            public string Arg1 { get; }

            public double Arg2 { get; }

            public bool Arg3 { get; }
        }
    }
}