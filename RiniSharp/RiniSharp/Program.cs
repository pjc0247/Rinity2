using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;

using RiniSharpCore;
using RiniSharpCore.Impl;

using Mono.Cecil;
using Mono.Cecil.Cil;

using UnityEngine;

namespace RiniSharp
{
    class Program
    {
        static List<MethodDefinition> methodsToAdd = new List<MethodDefinition>();

        static void ProcessTrace(MethodDefinition method)
        {
            Console.WriteLine($"   [TRACE] {method.FullName}");

            var ilgen = method.Body.GetILProcessor();
            var beginMethod = typeof(Profiler).GetMethods()
                .Where(x => x.Name == nameof(Profiler.BeginSample))
                .Where(x => x.GetParameters().Length == 1)
                .First();
            var endMethod = typeof(Profiler).GetMethod(nameof(Profiler.EndSample));

            ilgen.InsertBefore(
                method.GetHead(),
                ilgen.Create(OpCodes.Call, Global.module.Import(beginMethod)));
            ilgen.InsertBefore(
                method.GetHead(),
                ilgen.Create(OpCodes.Ldstr, $"{method.DeclaringType.Name}::{method.Name}()"));

            ilgen.InsertBefore(
                method.GetTail(),
                ilgen.Create(OpCodes.Call, Global.module.Import(endMethod)));
        }
        static void ProcessNotifyChange(PropertyDefinition property)
        {
            Console.WriteLine($"   [NOTIFY_CHANGE] {property.Name}");

            var method = property.SetMethod;

            var ilgen = method.Body.GetILProcessor();

            var propertyChanged = method.DeclaringType.Fields
                .Where(x => x.Name == nameof(INotifyPropertyChanged.PropertyChanged))
                .First();

            var propertyChangedEventHandler = Net2Resolver.GetType(nameof(PropertyChangedEventHandler));
            var invokeMethod =
                Net2Resolver.GetMethod(
                    propertyChangedEventHandler,
                    nameof(PropertyChangingEventHandler.Invoke));

            ilgen.InsertBefore(
                method.GetTail(),
                ilgen.Create(OpCodes.Ldarg_0));
            ilgen.InsertBefore(
                method.GetTail(),
                ilgen.Create(OpCodes.Ldfld, propertyChanged));
            ilgen.InsertBefore(
                method.GetTail(),
                ilgen.Create(OpCodes.Ldarg_0));
            ilgen.InsertBefore(
                method.GetTail(),
                ilgen.Create(OpCodes.Ldstr, method.Name));
            ilgen.InsertBefore(
                method.GetTail(),
                ilgen.Create(OpCodes.Newobj, Net2Resolver.GetMethod(typeof(PropertyChangingEventArgs), ".ctor", new Type[] { typeof(string) })));
            ilgen.InsertBefore(
                method.GetTail(),
                ilgen.Create(OpCodes.Callvirt, Global.module.Import(invokeMethod)));
        }
        static void ProcessDispatch(MethodDefinition method, CustomAttribute attr)
        {
            Console.WriteLine($"   [DISPATCH] {method.FullName}");

            var ilgen = method.Body.GetILProcessor();
            var lambda = new LambdaBuilder(method, ilgen);

            method.ClearBody();

            /* Call Dispatcher::Dispatch */
            var dispatchMethod = typeof(Dispatcher).GetMethod(nameof(Dispatcher.Dispatch));

            ilgen.Emit(OpCodes.Ldc_I4, (int)attr.ConstructorArguments[0].Value);
            lambda.EmitLdAction();
            ilgen.Emit(OpCodes.Call, Global.module.Import(dispatchMethod));

            ilgen.Emit(OpCodes.Ret);
        }

        static void ProcessMethod(MethodDefinition method)
        {
            foreach (var attr in method.CustomAttributes)
            {
                if (attr.AttributeType.Name == typeof(TraceAttribute).Name)
                {
                    ProcessTrace(method);
                }
                if (attr.AttributeType.Name == typeof(DispatchAttribute).Name)
                {
                    ProcessDispatch(method, attr);
                }
            }
        }
        static void ProcessType(TypeDefinition type)
        {
            Console.WriteLine($"[CLASS] {type.Name}");

            foreach (var method in type.Methods)
            {
                ProcessMethod(method);
            }
            foreach (var method in methodsToAdd)
            {
                type.Methods.Add(method);
            }

            foreach (var attr in type.CustomAttributes)
            {
                if (attr.AttributeType.Name == typeof(NotifyChangeAttribute).Name)
                {
                    foreach (var prop in type.Properties)
                    {
                        if (prop.SetMethod != null)
                            ProcessNotifyChange(prop);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            var targetPath = "C:\\Users\\hyun\\Documents\\ZinnyTestApp\\RinityTest\\Library\\ScriptAssemblies\\Assembly-CSharp.dll";

            if (args.Length != 0)
            {
                targetPath = args[0];
            }

            var unityScript = ModuleDefinition.ReadModule(targetPath);

            Global.module = unityScript;

            var mscorlibName = unityScript.AssemblyReferences
                .Where(x => x.Name.Contains("mscorlib"))
                .First();
            Global.mscorlib = unityScript.AssemblyResolver.Resolve(mscorlibName).MainModule;

            var systemName = unityScript.AssemblyReferences
                .Where(x => x.Name.Contains("System"))
                .First();
            Global.system = unityScript.AssemblyResolver.Resolve(systemName).MainModule;

            Console.WriteLine(unityScript);
            Console.WriteLine(Global.mscorlib + "  " + Global.mscorlib.RuntimeVersion);
            Console.WriteLine(Global.system + "  " + Global.system.RuntimeVersion);

            foreach (var type in unityScript.Types)
            {
                ProcessType(type);
            }

            unityScript.Write("tmp.dll");
            unityScript.Dispose();

            System.IO.File.Replace("tmp.dll", targetPath, null);
        }
    }
}
