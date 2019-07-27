using System;
using System.Collections.Generic;
using Fluency.Common;
using Fluency.Execution.Parsing.Entities;

namespace Fluency.Execution.Functions.BuiltIn
{
    public class BuiltInFactory : IFunctionResolver
    {

        private static readonly Value[] _emptyArgs = new Value[0];
        private static readonly Value[] _false = new Value[] {new Value(false, FluencyType.Bool)};
        public static IReadOnlyDictionary<string, FunctionMaker> BuiltInFunctions = new Dictionary<string, FunctionMaker>()
        {
            {"SwitchIn", (args) => new SwitchIn(args)},
            {"SwitchOut", (args) => new SwitchOut(args)},
            {"Up", (_) => new SwitchIn(_false)},
            {"Down", (_) => new SwitchOut(_false)},
            {"Const", (args) => new Const(args)},
            {"Com", (_) => new Com()},
            {"Comment", (_) => new Comment()},
            {"I", (_) => new I()},
            {"Dup", (_) => new Dup()},
            {"Duplicate", (_) => new Duplicate()},
            {"First", (args) => new First(args)},
            {"MergeTop", (_) => new MergeTop()},
            {"MergeBottom", (_) => new MergeBottom()},
            {"MergeIf", (_) => new MergeIf()},
            {"Zip", (args) => new Zip(args)},
            {"Unzip", (args) => new Unzip(args)},
            {"Drain", (_) => new Drain()},
            {"Add", (args) => new WrapBinary<int, int, int>((a, b) => a + b, FluencyType.Int, FluencyType.Int,"Add", args )},
            {"Mult", (args) => new WrapBinary<int, int, int>((a, b) => a * b, FluencyType.Int, FluencyType.Int,"Mult", args )},
            {"Sqrt", (_) => new WrapUnary<int, double>(a => Math.Sqrt(a), FluencyType.Double, "Sqrt")},
            {"Floor", (_) => new WrapUnary<double, int>(a => (int)a, FluencyType.Int, "Floor")},
            {"Equals", (args) => new WrapBinary<object, object, bool>((a, b) => a.Equals(b) , FluencyType.Any, FluencyType.Bool, "Equals", args)},
            {"And", (_) => new WrapBinary<bool, bool, bool>((a, b) => a && b, FluencyType.Bool, FluencyType.Bool, "And", _emptyArgs)},
            {"All", (args) => new WrapBinaryFold<bool>((a, b) => a && b, FluencyType.Bool, "All", args)}, //implement short circuiting later
            {"Or", (_) => new WrapBinary<bool, bool, bool>((a, b) => a || b, FluencyType.Bool, FluencyType.Bool, "Or", _emptyArgs)},
            {"Any", (args) => new WrapBinaryFold<bool>((a, b) => a || b, FluencyType.Bool, "Any", args)}, //implement short circuiting later
            {"Not", (_) => new WrapUnary<bool, bool>(a => !a, FluencyType.Bool, "Not")},
            {"AddDouble", (args) => new WrapBinary<double, double, double>((a, b) => a + b, FluencyType.Double, FluencyType.Double, "AddDouble", args )},
            {"ParseInt", (_) => new WrapUnary<string, int>(int.Parse, FluencyType.Int, "ParseInt")},
            {"Concat", (args) => new WrapBinary<string, string, string>((a, b) => a + b, FluencyType.String, FluencyType.String, "Concat", args )},
            {"Split", (args) => new WrapBinaryStreamOutput<string, string, string>((a, b) =>
                                             a.Split(new string[] {b}, StringSplitOptions.RemoveEmptyEntries), FluencyType.String, FluencyType.String, "Split", args )},
            {"DivMod", (args) => new WrapBinaryTwoOutputs<int, int, int, int>((a, b) => (a/b, a%b) , FluencyType.Int, FluencyType.Int, FluencyType.Int, "DivMod", args )},
        };

        public IFunction Resolve(string name, Value[] arguments)
        {
            return BuiltInFunctions[name](arguments);
        }
    }
}