namespace Ariadne.Extensions.ServiceCollection
{
    public interface IFactory<out TResult>
    {
        TResult New();
    }

    public interface IFactory<in T, out TResult>
    {
        TResult New(T arg);
    }

    public interface IFactory<in T1, in T2, out TResult>
    {
        TResult New(T1 arg1, T2 arg2);
    }

    public interface IFactory<in T1, in T2, in T3, out TResult>
    {
        TResult New(T1 arg1, T2 arg2, T3 arg3);
    }
}