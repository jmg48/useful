using System;

namespace Ariadne.Extensions.ServiceCollection
{
    internal class Factory<T1, T2, T3, TResult> : IFactory<T1, T2, T3, TResult>
    {
        private readonly Func<T1, T2, T3, TResult> _func;

        public Factory(Func<T1, T2, T3, TResult> func) => _func = func;

        public TResult New(T1 arg1, T2 arg2, T3 arg3) => this._func(arg1, arg2, arg3);
    }
}