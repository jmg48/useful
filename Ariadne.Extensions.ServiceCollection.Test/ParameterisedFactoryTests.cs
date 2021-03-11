using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Ariadne.Extensions.ServiceCollection.Test
{
    [TestFixture]
    public class ParameterisedFactoryTests
    {
        [TestCase(false)]
        [TestCase(true)]
        public void ShouldResolveFactoryOfOneArg(bool useFacility)
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

            if (useFacility)
            {
                serviceCollection.AddFactoryFacility();
            }
            else
            {
                serviceCollection.AddFactory<string, TestServiceOneParam<string>, TestServiceOneParam<string>>();
            }

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IFactory<string, TestServiceOneParam<string>>>();

            factory.New("a").Arg.Should().Be("a");
            factory.New("b").Arg.Should().Be("b");
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ShouldResolveFactoryWithImplicitConversion(bool useFacility)
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

            if (useFacility)
            {
                serviceCollection.AddFactoryFacility();
            }
            else
            {
                serviceCollection.AddFactory<int, TestServiceOneParam<double>, TestServiceOneParam<double>>();
            }

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IFactory<int, TestServiceOneParam<double>>>();

            factory.New(1).Arg.Should().Be(1);
            factory.New(2).Arg.Should().Be(2);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ShouldResolveFactoryOfTwoArgs(bool useFacility)
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

            if (useFacility)
            {
                serviceCollection.AddFactoryFacility();
            }
            else
            {
                serviceCollection.AddFactory<string, double, TestServiceTwoParams, TestServiceTwoParams>();
            }

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IFactory<string, double, TestServiceTwoParams>>();

            var x = factory.New("a", 1.0);
            x.Arg1.Should().Be("a");
            x.Arg2.Should().Be(1.0);

            var y = factory.New("b", 2.0);
            y.Arg1.Should().Be("b");
            y.Arg2.Should().Be(2.0);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ShouldResolveFactoryOfThreeArgs(bool useFacility)
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

            if (useFacility)
            {
                serviceCollection.AddFactoryFacility();
            }
            else
            {
                serviceCollection.AddFactory<string, double, bool, TestServiceThreeParams, TestServiceThreeParams>();
            }

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

        private class TestServiceOneParam<T>
        {
            public TestServiceOneParam(T arg) => Arg = arg;

            public T Arg { get; }
        }

        private class TestServiceTwoParams
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