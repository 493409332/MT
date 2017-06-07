using System;

namespace MT.AOP.Factory
{
    public class ProxyFactory
    {
        public static T CreateProxy<T>(Type realProxyType) 
        {
            var generator = new DynamicProxyGenerator(realProxyType, typeof(T));
            Type type = generator.GenerateType();

            return (T) Activator.CreateInstance(type);
        }
    }
}
