using Castle.DynamicProxy;
using System;

/// <summary>
/// Thrift 动态代理
/// </summary>
namespace Anno.Rpc.Client.DynamicProxy
{
    public static class AnnoProxyBuilder
    {
        private static ProxyGenerator generator = new ProxyGenerator();
        private static IInterceptor rpcInterceptor = new AnnoRpcInterceptor();
        public static TInterface GetService<TInterface>() where TInterface : class
        {
          return generator.CreateInterfaceProxyWithoutTarget<TInterface>(rpcInterceptor);
        }
    }
}
