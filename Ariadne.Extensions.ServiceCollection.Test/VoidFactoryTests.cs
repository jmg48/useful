using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Ariadne.Extensions.ServiceCollection.Test
{
    [TestFixture]
    public class VoidFactoryTests
    {
        [TestCase(false)]
        [TestCase(true)]
        public void ShouldResolveFactoryRespectingLifestyle(bool useFacility)
        {
            // Arrange
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddSingleton<TestSingleton>();
            serviceCollection.AddTransient<TestTransient>();

            if (useFacility)
            {
                serviceCollection.AddFactoryFacility();
            }
            else
            {
                serviceCollection.AddFactory<TestSingleton>();
                serviceCollection.AddFactory<TestTransient>();
            }

            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Act
            var transientFactory = serviceProvider.GetRequiredService<IFactory<TestTransient>>();
            var singletonFactory = serviceProvider.GetRequiredService<IFactory<TestSingleton>>();

            // Assert
            transientFactory.New().Should().NotBeSameAs(transientFactory.New());
            serviceProvider.GetRequiredService<TestTransient>().Should().NotBeSameAs(transientFactory.New());
            
            singletonFactory.New().Should().BeSameAs(singletonFactory.New());
            serviceProvider.GetRequiredService<TestSingleton>().Should().BeSameAs(singletonFactory.New());
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ShouldInjectFactoryRespectingLifestyle(bool useFacility)
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddSingleton<TestSingleton>();
            serviceCollection.AddTransient<TestTransient>();

            if (useFacility)
            {
                serviceCollection.AddFactoryFacility();
            }
            else
            {
                serviceCollection.AddFactory<TestSingleton>();
                serviceCollection.AddFactory<TestTransient>();
            }

            serviceCollection.AddTransient<TestService>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var testService = serviceProvider.GetRequiredService<TestService>();

            testService.TransientFactory.New().Should().NotBeSameAs(testService.TransientFactory.New());

            testService.TestTransient.Should().NotBeSameAs(testService.TransientFactory.New());

            testService.SingletonFactory.New().Should().BeSameAs(testService.SingletonFactory.New());

            testService.TestSingleton.Should().BeSameAs(testService.SingletonFactory.New());
        }

        private class TestTransient
        {
        }

        private class TestSingleton
        {
        }

        private class TestService
        {
            public TestService(TestTransient testTransient, TestSingleton testSingleton, IFactory<TestTransient> transientFactory, IFactory<TestSingleton> singletonFactory)
            {
                TestTransient = testTransient;
                TestSingleton = testSingleton;
                TransientFactory = transientFactory;
                SingletonFactory = singletonFactory;
            }
            
            public TestTransient TestTransient { get; }
            
            public TestSingleton TestSingleton { get; }
            
            public IFactory<TestTransient> TransientFactory { get; }

            public IFactory<TestSingleton> SingletonFactory { get; }
        }
    }
}