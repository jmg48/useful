using System;

namespace Ariadne.Extensions.ServiceCollection
{
    internal class Factory<T, TService> : FactoryBase<TService>, IFactory<T, TService>
    {
        public Factory(IServiceProvider serviceProvider) : base(serviceProvider, new [] { typeof(T) })
        {
        }

        public TService New(T arg) => this.New(new object[] { arg });
    }
}