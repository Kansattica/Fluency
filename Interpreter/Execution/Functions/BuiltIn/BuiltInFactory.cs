using System;
using System.Collections.Generic;
using Fluency.Interpreter.Parsing.Entities;

namespace Fluency.Interpreter.Execution.Functions.BuiltIn
{
    static class BuiltInFactory
    {
        private static Dictionary<string, Func<Value[], IFunction>> _builtInFunctions = new Dictionary<string, Func<Value[], IFunction>>()
        {
            {"Switch", (args) => new Switch(args)},
            {"Com", (_) => new Com()},
            {"Comment", (_) => new Comment()},
            {"Dup", (_) => new Dup()},
            {"Duplicate", (_) => new Duplicate()},
            {"First", (args) => new First(args)},
            {"MergeTop", (_) => new MergeTop()},
            {"MergeBottom", (_) => new MergeBottom()},
        };

        public static IFunction Construct(string name, Value[] arguments)
        {
            return _builtInFunctions[name](arguments);
        }
    }
}