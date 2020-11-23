# Anno 分布式微服务开发框架

**Anno 是一个分布式开发框架,专注于服务治理、监控、链路追踪。RPC可选用高性能跨语言的Thrift（推荐）、Grpc。同时支持 .net core 、.net framework。**
[![downloads](https://img.shields.io/nuget/dt/Anno.EngineData.svg)](https://www.nuget.org/packages/Anno.EngineData)
[![GitHub license](https://img.shields.io/badge/license-Apache%202-blue.svg)](https://raw.githubusercontent.com/duyanming/Anno.Core/master/LICENSE)

![Dashboard](https://s1.ax1x.com/2020/09/26/0iRcIU.png)

[在线演示](http://140.143.207.244) :http://140.143.207.244

[示例项目](https://github.com/duyanming/Viper) :https://github.com/duyanming/Viper

## Nuget 扩展

Package name                           |Description     | Version                     | Downloads
---------------------------------------|----------------|-----------------------------|------------------------
`Anno.EventBus`|EventBus事件总线库 | [![NuGet](https://img.shields.io/nuget/v/Anno.EventBus.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Anno.EventBus/) | ![downloads](https://img.shields.io/nuget/dt/Anno.EventBus.svg)
`Anno.RateLimit`|令牌桶、漏桶限流库 | [![NuGet](https://img.shields.io/nuget/v/Anno.RateLimit.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Anno.RateLimit/) | ![downloads](https://img.shields.io/nuget/dt/Anno.RateLimit.svg)
`Anno.EngineData.RateLimit` |Anno服务限流中间件| [![NuGet](https://img.shields.io/nuget/v/Anno.EngineData.RateLimit.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Anno.EngineData.RateLimit/) | ![downloads](https://img.shields.io/nuget/dt/Anno.EngineData.RateLimit.svg)
`Anno.LRUCache`|缓存库 | [![NuGet](https://img.shields.io/nuget/v/Anno.LRUCache.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Anno.LRUCache/) | ![downloads](https://img.shields.io/nuget/dt/Anno.LRUCache.svg)
`Anno.EngineData.Cache`|Anno服务缓存中间件 | [![NuGet](https://img.shields.io/nuget/v/Anno.EngineData.Cache.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Anno.EngineData.Cache/) | ![downloads](https://img.shields.io/nuget/dt/Anno.EngineData.Cache.svg)
`Anno.Rpc.Client.DynamicProxy`|接口代理Anno客户端扩展 | [![NuGet](https://img.shields.io/nuget/v/Anno.Rpc.Client.DynamicProxy.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Anno.Rpc.Client.DynamicProxy/) | ![downloads](https://img.shields.io/nuget/dt/Anno.Rpc.Client.DynamicProxy.svg)
`Anno.Rpc.ExtClient`|接口代理Anno客户端扩展 | [![NuGet](https://img.shields.io/nuget/v/Anno.Rpc.ExtClient.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Anno.Rpc.ExtClient/) | ![downloads](https://img.shields.io/nuget/dt/Anno.Rpc.ExtClient.svg)

## Anno.Ext
>该项目是Anno的一个扩展库，包括服务缓存、客户端代理、服务限流等组件

# 网关

[参考Viper](https://github.com/duyanming/Viper)
