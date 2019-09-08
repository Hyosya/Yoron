using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Yoron.Model.Parser;

namespace Yoron.Model
{
    public static class Compiler
    {
        public static void Compile(string source,string outputExeFileName)
        {
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName($"{outputExeFileName}"), AssemblyBuilderAccess.Save);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule($"{outputExeFileName}.exe");
            var typeBuilder = moduleBuilder.DefineType("Program", TypeAttributes.Class);
            var methodBuilder = typeBuilder.DefineMethod("Main", MethodAttributes.Static, typeof(void), new[] { typeof(string[]) });

            var lambda = Parser.Parser.GetLambdaExpression(source);
            lambda.CompileToMethod(methodBuilder);

            typeBuilder.CreateType();
            assemblyBuilder.SetEntryPoint(methodBuilder);
            assemblyBuilder.Save($"{outputExeFileName}.exe");
        }
    }
}
