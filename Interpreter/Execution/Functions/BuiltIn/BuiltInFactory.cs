using System;
using System.Collections.Generic;
using Fluency.Interpreter.Common;
using Fluency.Interpreter.Parsing.Entities;

namespace Fluency.Interpreter.Execution.Functions.BuiltIn
{
    static class BuiltInFactory
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
            {"Add", (args) => new WrapMath<int>((a, b) => a + b, FluencyType.Int, "Add", args )},
        };

    }
}