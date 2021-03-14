using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Extensions;
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

            Func<TestServiceTwoParams<string, double>> factory = () => serviceProvider.GetRequiredService<TestServiceTwoParams<string, double>>();

            // This is actually checking already built-in behaviour, but illustrates the kind of exception thrown which we want to align with here
            factory.Should().Throw<InvalidOperationException>()
                .WithMessage($"No service for type '{typeof(TestServiceTwoParams<string, double>)}' has been registered.");
        }

        [Test]
        public void GetRequiredService_DependencyNotRegistered_ShouldThrow()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<TestServiceTwoParams<string, double>>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<TestServiceTwoParams<string, double>> factory = () => serviceProvider.GetRequiredService<TestServiceTwoParams<string, double>>();

            // This is actually checking already built-in behaviour, but illustrates the kind of exception thrown which we want to align with here
            factory.Should().Throw<InvalidOperationException>()
                .WithMessage($"Unable to resolve service for type '{typeof(string)}' while attempting to activate '{typeof(TestServiceTwoParams<string, double>)}'.");
        }

        [Test]
        public void GetRequiredService_NestedDependencyNotRegistered_ShouldThrow()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<TestServiceOneParam<WrongConstructor>>();
            serviceCollection.AddTransient<WrongConstructor>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<TestServiceOneParam<WrongConstructor>> factory = () => serviceProvider.GetRequiredService<TestServiceOneParam<WrongConstructor>>();

            // This is actually checking already built-in behaviour, but illustrates the kind of exception thrown which we want to align with here
            factory.Should().Throw<InvalidOperationException>()
                .WithMessage($"Unable to resolve service for type '{typeof(double)}' while attempting to activate '{typeof(WrongConstructor)}'.");
        }

        [Test]
        public void New_FactoryOfOneArgForRegisteredType_ShouldSucceed()
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
        public void New_FactoryOfOneArgForRegisteredInterface_ShouldSucceed()
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
        public void New_FactoryOfOneArgOfAssignableType_ShouldSucceed()
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
        public void New_FactoryOfTwoArgsForRegisteredType_ShouldSucceed()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<TestServiceTwoParams<string, double>>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IFactory<string, double, TestServiceTwoParams<string, double>>>();

            var x = factory.New("a", 1.0);
            x.Arg1.Should().Be("a");
            x.Arg2.Should().Be(1.0);

            var y = factory.New("b", 2.0);
            y.Arg1.Should().Be("b");
            y.Arg2.Should().Be(2.0);
        }

        [Test]
        public void New_FactoryOfTwoArgsForRegisteredInterface_ShouldSucceed()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<ITestInterfaceTwoParams<string, double>, TestServiceTwoParams<string, double>>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IFactory<string, double, ITestInterfaceTwoParams<string, double>>>();

            var x = factory.New("a", 1.0);
            x.Arg1.Should().Be("a");
            x.Arg2.Should().Be(1.0);

            var y = factory.New("b", 2.0);
            y.Arg1.Should().Be("b");
            y.Arg2.Should().Be(2.0);
        }

        [Test]
        public void New_FactoryOfThreeArgsForRegisteredType_ShouldSucceed()
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

        [Test]
        public void New_FactoryWithFactoryService_ShouldSucceed()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<TestServiceTwoParams<IFactory<string, TestServiceOneParam<string>>, double>>();
            serviceCollection.AddTransient<TestServiceOneParam<string>>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IFactory<double, TestServiceTwoParams<IFactory<string, TestServiceOneParam<string>>, double>>>();

            var instance = factory.New(1);
            instance.Arg2.Should().Be(1);

            var instance2 = factory.New(2);
            instance2.Arg2.Should().Be(2);

            var factory2 = instance.Arg1;

            var instance3 = factory2.New("a");
            instance3.Arg.Should().Be("a");

            var instance4 = factory2.New("b");
            instance4.Arg.Should().Be("b");
        }

        [Test]
        public void GetRequiredService_TwoConstructorsServicesRegisteredForNeither_ShouldThrow()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<TwoConstructors>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<TwoConstructors> factory = () => serviceProvider.GetRequiredService<TwoConstructors>();

            // This is actually checking already built-in behaviour, but illustrates the kind of exception thrown which we want to align with here
            factory.Should().Throw<InvalidOperationException>()
                .WithMessage($"No constructor for type '{typeof(TwoConstructors)}' can be instantiated using services from the service container and default values.");
        }

        [Test]
        public void GetRequiredService_TwoConstructorsServicesRegisteredForOne_ShouldSucceed()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<TwoConstructors>();
            serviceCollection.AddSingleton(typeof(string), "a");
            serviceCollection.AddSingleton(typeof(int), 1);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // This is actually checking already built-in behaviour, but illustrates the kind of exception thrown which we want to align with here
            var instance = serviceProvider.GetRequiredService<TwoConstructors>();
            instance.A.Should().Be("a");
            instance.B.Should().Be(1);
        }

        [Test]
        public void GetRequiredService_TwoConstructorsServicesRegisteredForBoth_ShouldThrow()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<TwoConstructors>();
            serviceCollection.AddSingleton(typeof(string), "a");
            serviceCollection.AddSingleton(typeof(int), 1);
            serviceCollection.AddSingleton(typeof(DateTime), 15.June(1980));
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<TwoConstructors> factory = () => serviceProvider.GetRequiredService<TwoConstructors>();

            // This is actually checking already built-in behaviour, but illustrates the kind of exception thrown which we want to align with here
            factory.Should().Throw<InvalidOperationException>()
                .WithMessage($@"Unable to activate type '{typeof(TwoConstructors)}'. The following constructors are ambigious:
Void .ctor(System.String, Int32)
Void .ctor(System.DateTime, Int32)");
        }
        
        [Test]
        public void GetRequiredService_FactoryOfOneArgForTypeWithTwoMatchingConstructors_ShouldThrow()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddTransient<TwoConstructors>();
            serviceCollection.AddFactoryFacility();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Func<IFactory<int, TwoConstructors>> factory = () => serviceProvider.GetRequiredService<IFactory<int, TwoConstructors>>();

            factory.Should().Throw<InvalidOperationException>()
                .WithMessage($"In order to resolve a parameterised factory for service type '{typeof(TwoConstructors)}', the implementation type '{typeof(TwoConstructors)}' must contain just one constructor whose last parameter is assignable from the factory argument type '{typeof(int)}'.");
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

        private class TwoConstructors
        {
            public TwoConstructors(string a, int b)
            {
                A = a;
                B = b;
            }

            public TwoConstructors(DateTime c, int d)
            {
                C = c;
                D = d;
            }
            
            public string A { get; }
 
            public int B { get; }
            
            public DateTime C { get; }
            
            public int D { get; }
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

        private interface ITestInterfaceTwoParams<out T1, out T2>
        {
            T1 Arg1 { get; }

            T2 Arg2 { get; }
        }

        private class TestServiceTwoParams<T1, T2> : ITestInterfaceTwoParams<T1, T2>
        {
            public TestServiceTwoParams(T1 arg1, T2 arg2)
            {
                Arg1 = arg1;
                Arg2 = arg2;
            }

            public T1 Arg1 { get; }

            public T2 Arg2 { get; }
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