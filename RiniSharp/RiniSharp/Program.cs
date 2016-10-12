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
using Mono.Cecil.Mdb;
using Mono.CompilerServices.SymbolWriter;

using UnityEngine;

namespace RiniSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var targetPath = "C:\\Users\\hyun\\Documents\\ZinnyTestApp\\RinityTest\\Library\\ScriptAssemblies\\Assembly-CSharp.dll";
            var mdbPath = "C:\\Users\\hyun\\Documents\\ZinnyTestApp\\RinityTest\\Library\\ScriptAssemblies\\Assembly-CSharp.dll.mdb";

            if (args.Length != 0)
            {
                targetPath = args[0];
                mdbPath = args[0] + ".mdb";
            }

            var unityScript = ModuleDefinition.ReadModule(targetPath);
            
            var symFile = MonoSymbolFile.ReadSymbolFile(mdbPath);
            var mdb = new MdbReader(unityScript, symFile);

            Global.module = unityScript;
            Global.mdbReader = mdb;

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

            var errors = weaver.ProcessModule(unityScript);

            foreach(var error in errors)
            {
                Console.WriteLine(error);
            }
                        
            unityScript.Write("tmp.dll");
            unityScript.Dispose();

            System.IO.File.Replace("tmp.dll", targetPath, null);
        }
    }
}
