/****************************************************** 
Writer:Du YanMing
Mail:dym880@163.com
Create Date:2020/8/18 16:55:16 
Functional description： AnnoProxy
******************************************************/
using Natasha.CSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Anno.Rpc.Client.Ext
{
    public class AnnoProxy
    {
        public const string NoNeedWriting = "NW1000-NoNeedToWrite";
        internal readonly NClass _builder;
        private readonly ConcurrentDictionary<string, MethodInfo> _methodMapping;
        private readonly ConcurrentDictionary<Delegate, string> _staticDelegateOrderScriptMapping;
        private readonly List<(string memberName, Delegate @delegate, string typeScript)> _staticNameDelegateMapping;
        private bool _needReComplie;
        private bool _useSingleton;
        public StringBuilder ProxyBody;
        public readonly ConcurrentDictionary<string, string> NeedReWriteMethods;


        public AnnoProxy()
        {

            _builder = NClass.DefaultDomain().Public().Namespace("Anno.Proxy");
            NeedReWriteMethods = new ConcurrentDictionary<string, string>();
            _methodMapping = new ConcurrentDictionary<string, MethodInfo>();
            _staticDelegateOrderScriptMapping = new ConcurrentDictionary<Delegate, string>();
            _staticNameDelegateMapping = new List<(string memberName, Delegate @delegate, string typeScript)>();
            _needReComplie = true;
            ProxyBody = new StringBuilder();

        }




        public string CurrentProxyName
        {
            get { return _builder.NameScript; }
        }




        public AnnoProxy UseSingleton(bool singleton = true)
        {
            _useSingleton = singleton;
            return this;
        }




        /// <summary>
        /// 额外添加 Using 引用
        /// </summary>
        /// <param name="namespace"> 命名空间的来源 </param>
        /// <returns></returns>
        public AnnoProxy Using(NamespaceConverter @namespace)
        {

            _builder.Using(@namespace);
            return this;

        }
        /// <summary>
        /// 额外添加 dll 引用
        /// </summary>
        /// <param name="path">dll文件路径</param>
        /// <returns></returns>
        public AnnoProxy AddDll(string path)
        {
            _builder.AssemblyBuilder.Compiler.Domain.LoadPluginFromStream(path);
            return this;
        }
        /// <summary>
        /// 根据方法名称获取方法信息
        /// </summary>
        /// <param name="methodName">方法名称</param>
        /// <returns></returns>
        public MethodInfo TryGetMethod(string methodName)
        {
            if (_methodMapping.ContainsKey(methodName))
            {
                return _methodMapping[methodName];
                //var type = method.DeclaringType;
            }
            return null;
        }

        /// <summary>
        /// 操作当前函数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public StringOrDelegate this[string key]
        {

            set
            {

                if (value.StringParameter != default)
                {

                    SetMethod(key, value.StringParameter);

                }
                else
                {

                    SetMethod(key, value.DelegateParameter);

                }

            }

        }




        public AnnoProxy Implement(Type type)
        {

            _needReComplie = true;
            _builder.Inheritance(type);
            var methods = type.GetMethods();
            foreach (var item in methods)
            {

                if (item.IsHideBySig)
                {

                    _methodMapping[item.Name] = item;
                    if (type.IsInterface)
                    {

                        SetMethod(item.Name, item.ReturnType == typeof(void) ? default : "return default;");

                    }
                    else if (item.IsAbstract)
                    {

                        if (!item.Equals(item.GetBaseDefinition()))
                        {

                            SetMethod(item.Name, NoNeedWriting);

                        }
                        else
                        {

                            SetMethod(item.Name, item.ReturnType == typeof(void) ? default : "return default;");

                        }

                    }
                    else
                    {

                        SetMethod(item.Name, NoNeedWriting);

                    }


                }

            }
            return this;

        }
        public AnnoProxy Implement<T>()
        {
            return Implement(typeof(T));
        }




        public void SetMethod(string name, string script)
        {

            if (_methodMapping.ContainsKey(name))
            {

                _needReComplie = true;
                var method = _methodMapping[name];
                var type = method.DeclaringType;
                var template = FakeMethodOperator.DefaultDomain();

                if (!type.IsInterface)
                {

                    template.Modifier((method.IsAbstract || method.IsVirtual) ? "override" : "new");

                }
                var result = template
                    .UseMethod(method)
                    .MethodBody(script)
                    .Script;


                NeedReWriteMethods[name] = result;

            }

        }
        public void SetMethod(string name, Delegate @delegate)
        {

            if (_methodMapping.ContainsKey(name))
            {

                _needReComplie = true;
                StringBuilder builder = new StringBuilder();
                var methodInfo = @delegate.Method;
                if (methodInfo.ReturnType != typeof(void))
                {

                    builder.Append("Func<");
                    var parameters = methodInfo.GetParameters();
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        builder.Append(parameters[i].ParameterType.GetDevelopName());
                        builder.Append(',');
                    }
                    builder.Append(methodInfo.ReturnType.GetDevelopName());
                    builder.Append('>');

                }
                else
                {

                    builder.Append("Action");
                    var parameters = methodInfo.GetParameters();
                    if (parameters.Length > 0)
                    {
                        builder.Append('<');
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            builder.Append(parameters[i].ParameterType.GetDevelopName());
                            if (i != parameters.Length - 1)
                            {
                                builder.Append(',');
                            }
                        }
                        builder.Append('>');
                    }


                }



                _staticDelegateOrderScriptMapping[@delegate] = "_func" + _staticNameDelegateMapping.Count;
                _staticNameDelegateMapping.Add((name, @delegate, builder.ToString()));
            }

        }




        private AnnoProxy Complie()
        {

            if (_needReComplie)
            {

                _builder.UseRandomName();
                _builder.BodyScript.Clear();
                StringBuilder _fieldBuilder = new StringBuilder();
                StringBuilder _methodBuilder = new StringBuilder();
                HashSet<string> _fieldCache = new HashSet<string>();

                for (int i = 0; i < _staticNameDelegateMapping.Count; i += 1)
                {
                    var temp = _staticNameDelegateMapping[i];
                    var script = _staticDelegateOrderScriptMapping[temp.@delegate];
                    if (!_fieldCache.Contains(script))
                    {
                        _fieldCache.Add(script);
                        _fieldBuilder.AppendLine($"public static {temp.typeScript} {script};");
                        _methodBuilder.AppendLine($"{script} = ({temp.typeScript})(delegatesInfo[{i}].@delegate);");
                    }



                    //添加委托调用
                    var method = _methodMapping[temp.memberName];
                    StringBuilder builder = new StringBuilder();
                    var infos = method.GetParameters().OrderBy(item => item.Position);
                    foreach (var item in infos)
                    {

                        builder.Append(item.Name + ",");

                    }
                    if (builder.Length > 0)
                    {

                        builder.Length -= 1;

                    }


                    if (method.ReturnType != typeof(void))
                    {

                        builder.Insert(0, $"return {script}(");

                    }
                    else
                    {

                        builder.Insert(0, $"{script}(");

                    }

                    builder.Append(");");
                    SetMethod(temp.memberName, builder.ToString());

                }


                _fieldBuilder.AppendLine($@"public static void SetDelegate(List<(string memberName, Delegate @delegate, string typeScript)> delegatesInfo){{");
                _fieldBuilder.Append(_methodBuilder);
                _fieldBuilder.Append('}');


                foreach (var item in NeedReWriteMethods)
                {

                    if (!item.Value.Contains(NoNeedWriting))
                    {
                        _fieldBuilder.AppendLine(item.Value);
                    }

                }
                if (_useSingleton)
                {
                    _fieldBuilder.Append($@"public static readonly {CurrentProxyName} Instance;");
                    _fieldBuilder.Append($@"static {CurrentProxyName}(){{ Instance = new {CurrentProxyName}(); }}");
                }
                _fieldBuilder.Append(ProxyBody);
                _builder.Body(_fieldBuilder.ToString());
                var type = _builder.GetType();


                var action = NDelegate.UseCompiler(_builder.AssemblyBuilder).Action<List<(string memberName, Delegate @delegate, string typeScript)>>($@"
                    {_builder.NameScript}.SetDelegate(obj);
                ", "Anno.Proxy");
                action(_staticNameDelegateMapping);


                _needReComplie = false;

            }

            return this;
        }


        public Func<TInterface> GetCreator<TInterface>()
        {

            Complie();
            if (_useSingleton)
            {

                return NDelegate.UseCompiler(_builder.AssemblyBuilder).Func<TInterface>($@"
                     return {_builder.NameScript}.Instance;
                ", "Anno.Proxy");

            }
            else
            {

                return NDelegate.UseCompiler(_builder.AssemblyBuilder).Func<TInterface>($@"
                     return new {_builder.NameScript}();
                ", "Anno.Proxy");

            }


        }
    }


    public class AnnoProxy<TInterface1> : AnnoProxy
    {
        public AnnoProxy()
        {
            Implement<TInterface1>();
        }
    }
    public class AnnoProxy<TInterface1, TInterface2> : AnnoProxy
    {
        public AnnoProxy()
        {
            Implement<TInterface1>();
            Implement<TInterface2>();
        }
    }
    public class AnnoProxy<TInterface1, TInterface2, TInterface3> : AnnoProxy
    {
        public AnnoProxy()
        {
            Implement<TInterface1>();
            Implement<TInterface2>();
            Implement<TInterface3>();
        }
    }
    public class AnnoProxy<TInterface1, TInterface2, TInterface3, TInterface4> : AnnoProxy
    {
        public AnnoProxy()
        {
            Implement<TInterface1>();
            Implement<TInterface2>();
            Implement<TInterface3>();
            Implement<TInterface4>();
        }
    }
    public class AnnoProxy<TInterface1, TInterface2, TInterface3, TInterface4, TInterface5> : AnnoProxy
    {
        public AnnoProxy()
        {
            Implement<TInterface1>();
            Implement<TInterface2>();
            Implement<TInterface3>();
            Implement<TInterface4>();
            Implement<TInterface5>();
        }
    }
    public class AnnoProxy<TInterface1, TInterface2, TInterface3, TInterface4, TInterface5, TInterface6> : AnnoProxy
    {
        public AnnoProxy()
        {
            Implement<TInterface1>();
            Implement<TInterface2>();
            Implement<TInterface3>();
            Implement<TInterface4>();
            Implement<TInterface5>();
            Implement<TInterface6>();
        }
    }
    public class AnnoProxy<TInterface1, TInterface2, TInterface3, TInterface4, TInterface5, TInterface6, TInterface7> : AnnoProxy
    {
        public AnnoProxy()
        {
            Implement<TInterface1>();
            Implement<TInterface2>();
            Implement<TInterface3>();
            Implement<TInterface4>();
            Implement<TInterface5>();
            Implement<TInterface6>();
            Implement<TInterface7>();
        }
    }
    public class AnnoProxy<TInterface1, TInterface2, TInterface3, TInterface4, TInterface5, TInterface6, TInterface7, TInterface8> : AnnoProxy
    {
        public AnnoProxy()
        {
            Implement<TInterface1>();
            Implement<TInterface2>();
            Implement<TInterface3>();
            Implement<TInterface4>();
            Implement<TInterface5>();
            Implement<TInterface6>();
            Implement<TInterface7>();
            Implement<TInterface8>();
        }
    }
    public class AnnoProxy<TInterface1, TInterface2, TInterface3, TInterface4, TInterface5, TInterface6, TInterface7, TInterface8, TInterface9> : AnnoProxy
    {
        public AnnoProxy()
        {
            Implement<TInterface1>();
            Implement<TInterface2>();
            Implement<TInterface3>();
            Implement<TInterface4>();
            Implement<TInterface5>();
            Implement<TInterface6>();
            Implement<TInterface7>();
            Implement<TInterface8>();
            Implement<TInterface9>();
        }
    }
    public class AnnoProxy<TInterface1, TInterface2, TInterface3, TInterface4, TInterface5, TInterface6, TInterface7, TInterface8, TInterface9, TInterface10> : AnnoProxy
    {
        public AnnoProxy()
        {
            Implement<TInterface1>();
            Implement<TInterface2>();
            Implement<TInterface3>();
            Implement<TInterface4>();
            Implement<TInterface5>();
            Implement<TInterface6>();
            Implement<TInterface7>();
            Implement<TInterface8>();
            Implement<TInterface9>();
            Implement<TInterface10>();
        }
    }
}
