namespace Ariadne.Extensions.ServiceCollection
{
    using System;

    internal class Factory<T, TService> : FactoryBase<TService>, IFactory<T, TService>
    {
        public Factory(IServiceProvider serviceProvider, ServiceMap serviceMap)
            : base(serviceProvider, serviceMap, new[] { typeof(T) })
        {
        }

        public TService New(T arg) => this.New(new object[] { arg });
    }
}