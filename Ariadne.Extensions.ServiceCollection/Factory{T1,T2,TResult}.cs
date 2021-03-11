using System;

namespace Ariadne.Extensions.ServiceCollection
{
    internal class Factory<T1, T2, TService> : FactoryBase<TService>, IFactory<T1, T2, TService>
    {
        public Factory(IServiceProvider serviceProvider) : base(serviceProvider, new [] { typeof(T1), typeof(T2) })
        {
        }

        public TService New(T1 arg1, T2 arg2) => this.New(new object[] { arg1, arg2 });
    }
}