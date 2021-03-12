using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Ariadne.Extensions.ServiceCollection.Test
{
    [TestFixture]
    public class ParameterisedFactoryTests
    {
        [Test]
        public void ShouldResolveImplementationFactory()
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
        public void ShouldResolveInterfaceFactory()
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
        public void ShouldFailToResolveImplementationFactoryByInterface()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<TestServiceOneParam<string>>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<IFactory<string, ITestInterfaceOneParam<string>>> factory = () => serviceProvider.GetRequiredService<IFactory<string, ITestInterfaceOneParam<string>>>();

            // ToDo: More specific exception and useful error message
            factory.Should().Throw<Exception>();
        }

        [Test]
        public void ShouldFailToResolveNonTransientLifestyle()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddScoped<TestServiceOneParam<string>>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<IFactory<string, TestServiceOneParam<string>>> factory = () => serviceProvider.GetRequiredService<IFactory<string, TestServiceOneParam<string>>>();

            // ToDo: More specific exception and useful error message
            factory.Should().Throw<NotSupportedException>();
        }

        [Test]
        public void ShouldFailToResolveServiceAddedAfterFactoryFacility()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddFactoryFacility();
            serviceCollection.AddTransient<TestServiceOneParam<string>>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<IFactory<string, TestServiceOneParam<string>>> factory = () => serviceProvider.GetRequiredService<IFactory<string, TestServiceOneParam<string>>>();

            // ToDo: More specific exception and useful error message
            factory.Should().Throw<NotSupportedException>();
        }

        [Test]
        public void ShouldFailToResolveInterfaceFactoryByImplementation()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<ITestInterfaceOneParam<string>, TestServiceOneParam<string>>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<IFactory<string, TestServiceOneParam<string>>> factory = () => serviceProvider.GetRequiredService<IFactory<string, TestServiceOneParam<string>>>();

            // ToDo: More specific exception and useful error message
            factory.Should().Throw<Exception>();
        }

        [Test]
        public void ShouldResolveFactoryWithImplicitConversion()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<TestServiceOneParam<double>>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IFactory<int, TestServiceOneParam<double>>>();

            factory.New(1).Arg.Should().Be(1);
            factory.New(2).Arg.Should().Be(2);
        }

        [Test]
        public void ShouldResolveFactoryOfTwoArgs()
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
        public void ShouldResolveFactoryOfTwoArgsByInterface()
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
        public void ShouldResolveFactoryOfThreeArgs()
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