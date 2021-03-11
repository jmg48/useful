using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Ariadne.Extensions.ServiceCollection.Test
{
    [TestFixture]
    public class VoidFactoryTests
    {
        [Test]
        public void ShouldResolveFactoryRespectingLifestyle()
        {
            // Arrange
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddSingleton<TestSingleton>();
            serviceCollection.AddTransient<TestTransient>();

            var serviceProvider = serviceCollection.BuildServiceProviderWithAbstractFactorySupport();

            // Act
            var transientFactory = serviceProvider.GetRequiredService<IFactory<TestTransient>>();
            var singletonFactory = serviceProvider.GetRequiredService<IFactory<TestSingleton>>();

            // Assert
            transientFactory.New().Should().NotBeSameAs(transientFactory.New());
            serviceProvider.GetRequiredService<TestTransient>().Should().NotBeSameAs(transientFactory.New());
            
            singletonFactory.New().Should().BeSameAs(singletonFactory.New());
            serviceProvider.GetRequiredService<TestSingleton>().Should().BeSameAs(singletonFactory.New());
        }

        [Test]
        public void ShouldInjectFactoryRespectingLifestyle()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddSingleton<TestSingleton>();
            serviceCollection.AddTransient<TestTransient>();

            serviceCollection.AddTransient<TestService>();

            var serviceProvider = serviceCollection.BuildServiceProviderWithAbstractFactorySupport();

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