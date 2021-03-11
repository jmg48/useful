using System;

namespace Ariadne.Extensions.ServiceCollection
{
    internal class Factory<T, TResult> : IFactory<T, TResult>
    {
        private readonly Func<T, TResult> _func;

        public Factory(Func<T, TResult> func) => _func = func;

        public TResult New(T arg) => _func(arg);
    }
}