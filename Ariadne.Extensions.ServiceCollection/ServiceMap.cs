namespace Ariadne.Extensions.ServiceCollection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    internal class ServiceMap : IReadOnlyDictionary<Type, IReadOnlyList<ServiceDescriptor>>
    {
        private Dictionary<Type, IReadOnlyList<ServiceDescriptor>> serviceMap;

        public ServiceMap(IServiceCollection services) =>
            this.serviceMap = services.GroupBy(service => service.ServiceType)
                .ToDictionary(
                    group => group.Key,
                    group => (IReadOnlyList<ServiceDescriptor>)group.ToList());

        public int Count => this.serviceMap.Count;

        public IEnumerable<Type> Keys => this.serviceMap.Keys;

        public IEnumerable<IReadOnlyList<ServiceDescriptor>> Values => this.serviceMap.Values;

        public IReadOnlyList<ServiceDescriptor> this[Type index] => this.serviceMap[index];

        public bool ContainsKey(Type key) => this.serviceMap.ContainsKey(key);

        public bool TryGetValue(Type key, out IReadOnlyList<ServiceDescriptor> value) => this.serviceMap.TryGetValue(key, out value);

        public IEnumerator<KeyValuePair<Type, IReadOnlyList<ServiceDescriptor>>> GetEnumerator() => this.serviceMap.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}