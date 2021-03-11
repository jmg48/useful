using System;

namespace Ariadne.Extensions.ServiceCollection
{
    internal class Factory<T1, T2, T3, TService> : FactoryBase<TService>, IFactory<T1, T2, T3, TService>
    {
        public Factory(IServiceProvider serviceProvider) : base(serviceProvider, new [] { typeof(T1), typeof(T2), typeof(T3) })
        {
        }

        public TService New(T1 arg1, T2 arg2, T3 arg3) => this.New(new object[] { arg1, arg2, arg3 });
    }
}