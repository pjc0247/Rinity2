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

            var weaver = new Aspects.Weaver();

            weaver.ProcessModule(unityScript);
                        
            unityScript.Write("tmp.dll");
            unityScript.Dispose();

            System.IO.File.Replace("tmp.dll", targetPath, null);
        }
    }
}
