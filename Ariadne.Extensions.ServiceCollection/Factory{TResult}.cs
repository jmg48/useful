using System;

namespace Ariadne.Extensions.ServiceCollection
{
    internal class Factory<TResult> : IFactory<TResult>
    {
        private readonly Func<TResult> _func;

        public Factory(Func<TResult> func) => _func = func;

        public TResult New() => this._func();
    }
}