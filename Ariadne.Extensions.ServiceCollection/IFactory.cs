namespace Ariadne.Extensions.ServiceCollection
{
    /// <summary>
    /// Specifies the contract for an abstract factory, allowing instances of TService to be created on demand. 
    /// </summary>
    /// <typeparam name="TService">The type of the service to be created by the factory.</typeparam>
    public interface IFactory<out TService>
    {
        /// <summary>
        /// Resolve an instance of TService
        /// </summary>
        /// <returns>A new instance of TService</returns>
        TService New();
    }

    /// <summary>
    /// Specifies the contract for an abstract factory, allowing instances of TService to be created on demand. 
    /// </summary>
    /// <typeparam name="T">The type of the parameter used to create service instances.</typeparam>
    /// <typeparam name="TService">The type of the service to be created by the factory.</typeparam>
    public interface IFactory<in T, out TService>
    {
        /// <summary>
        /// Resolve an instance of TService, based on one argument
        /// </summary>
        /// <param name="arg">The argument used to create the service instance.</param>
        /// <returns>The service instance</returns>
        TService New(T arg);
    }

    /// <summary>
    /// Specifies the contract for an abstract factory, allowing instances of TService to be created on demand. 
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter used to create service instances.</typeparam>
    /// <typeparam name="T2">The type of the second parameter used to create service instances.</typeparam>
    /// <typeparam name="TService">The type of the service to be created by the factory.</typeparam>
    public interface IFactory<in T1, in T2, out TService>
    {
        /// <summary>
        /// Resolve an instance of TService, based on two arguments
        /// </summary>
        /// <param name="arg1">The first argument used to create the service instance.</param>
        /// <param name="arg2">The second argument used to create the service instance.</param>
        /// <returns>The service instance</returns>
        TService New(T1 arg1, T2 arg2);
    }

    /// <summary>
    /// Specifies the contract for an abstract factory, allowing instances of TService to be created on demand. 
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter used to create service instances.</typeparam>
    /// <typeparam name="T2">The type of the second parameter used to create service instances.</typeparam>
    /// <typeparam name="T3">The type of the third parameter used to create service instances.</typeparam>
    /// <typeparam name="TService">The type of the service to be created by the factory.</typeparam>
    public interface IFactory<in T1, in T2, in T3, out TService>
    {
        /// <summary>
        /// Resolve an instance of TService, based on three arguments
        /// </summary>
        /// <param name="arg1">The first argument used to create the service instance.</param>
        /// <param name="arg2">The second argument used to create the service instance.</param>
        /// <param name="arg3">The third argument used to create the service instance.</param>
        /// <returns>The service instance</returns>
        TService New(T1 arg1, T2 arg2, T3 arg3);
    }
}