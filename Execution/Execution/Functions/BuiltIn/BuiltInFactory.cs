using System;
using System.Collections.Generic;
using Fluency.Common;
using Fluency.Execution.Parsing.Entities;

namespace Fluency.Execution.Functions.BuiltIn
{
    public class BuiltInFactory : IFunctionResolver
    {
        public static IReadOnlyDictionary<string, FunctionMaker> BuiltInFunctions = new Dictionary<string, FunctionMaker>()
        {
            {"Switch", (args) => new Switch(args)},
            {"Com", (_) => new Com()},
            {"Comment", (_) => new Comment()},
            {"Dup", (_) => new Dup()},
            {"Duplicate", (_) => new Duplicate()},
            {"First", (args) => new First(args)},
            {"MergeTop", (_) => new MergeTop()},
            {"MergeBottom", (_) => new MergeBottom()},
            {"Drain", (_) => new Drain()},
            {"Add", (args) => new WrapBinary<int, int, int>((a, b) => a + b, FluencyType.Int, "Add", args )},
            {"AddDouble", (args) => new WrapBinary<double, double, double>((a, b) => a + b, FluencyType.Double, "AddDouble", args )},
            {"ParseInt", (_) => new WrapUnary<string, int>(int.Parse, FluencyType.Int, "ParseInt")},
        };

        public IFunction Resolve(string name, Value[] arguments)
        {
            return BuiltInFunctions[name](arguments);
        }
    }
}