using System;

namespace Ariadne.Extensions.ServiceCollection
{
    internal class Factory<T1, T2, TResult> : IFactory<T1, T2, TResult>
    {
        private readonly Func<T1, T2, TResult> _func;

        public Factory(Func<T1, T2, TResult> func) => _func = func;

        public TResult New(T1 arg1, T2 arg2) => this._func(arg1, arg2);
    }
}