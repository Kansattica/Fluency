using System;
using System.Collections.Generic;
using Fluency.Common;
using Fluency.Execution.Parsing.Entities;

namespace Fluency.Execution.Functions.BuiltIn.Factory
{
    public class BuiltInFactory : IFunctionResolver
    {

        private static readonly Value[] _emptyArgs = new Value[0];

        private static readonly Value[] _bools = new Value[] { new Value(false, FluencyType.Bool), new Value(true, FluencyType.Bool) };
        private static readonly Value[] _false = new Value[] { _bools[0] };
        public static IReadOnlyDictionary<string, FunctionMaker> BuiltInFunctions = new Dictionary<string, FunctionMaker>()
        {
            {"SwitchIn", (topArgs, _) => new SwitchIn(topArgs)},
            {"SwitchOut", (topArgs, _) => new SwitchOut(topArgs)},
            {"Up", (_, __) => new SwitchIn(_false)},
            {"Down", (_, __) => new SwitchOut(_false)},
            {"Const", (topArgs, _) => new Const(topArgs)},
            {"Comment", (_, __) => new Comment()},
            {"Com", (_, __) => new Com()},
            {"I", (_, __) => new I()},
            {"Dup", (_, __) => new Dup()},
            {"Duplicate", (_, __) => new Duplicate()},
            {"First", (topArgs, _) => new First(topArgs)},
            {"FirstN", (topArgs, _) => new FirstN(topArgs)},
            {"MergeTop", (_, __) => new MergeTop()},
            {"MergeBottom", (_, __) => new MergeBottom()},
            {"MergeIf", (_, __) => new MergeIf()},
            {"Zip", (topArgs, _) => new Zip(topArgs)},
            {"Unzip", (topArgs, _) => new Unzip(topArgs)},
            {"Drain", (_, __) => new Drain()},
            {"Add", (topArgs, bottomArgs) => new WrapBinary<int, int, int>((a, b) => a + b, FluencyType.Int, FluencyType.Int, "Add", topArgs, bottomArgs )},
            {"Mult", (topArgs, bottomArgs) => new WrapBinary<int, int, int>((a, b) => a * b, FluencyType.Int, FluencyType.Int, "Mult", topArgs, bottomArgs )},
            {"MultDouble", (topArgs, bottomArgs) => new WrapBinary<double, double, double>((a, b) => a * b, FluencyType.Double, FluencyType.Double, "MultDouble", topArgs, bottomArgs )},
            {"Sqrt", (_, __) => new WrapUnary<int, double>(a => Math.Sqrt(a), FluencyType.Double, "Sqrt")},
            {"Floor", (_, __) => new WrapUnary<double, int>(a => (int)a, FluencyType.Int, "Floor")},
            {"Equals", (topArgs, bottomArgs) => new WrapBinary<object, object, bool>((a, b) => a.Equals(b), FluencyType.Any, FluencyType.Bool, "Equals", topArgs, bottomArgs)},
            {"LessThan", (topArgs, _) => new WrapBinary<int, int, bool>((a, b) => a < b, FluencyType.Int, FluencyType.Bool, "LessThan", _emptyArgs, topArgs)},
            {"GreaterThan", (topArgs, _) => new WrapBinary<int, int, bool>((a, b) => a > b, FluencyType.Int, FluencyType.Bool, "GreaterThan", _emptyArgs, topArgs)},
            {"And", (_, __) => new WrapBinary<bool, bool, bool>((a, b) => a && b, FluencyType.Bool, FluencyType.Bool, "And", _emptyArgs)},
            {"All", (topArgs, _) => new WrapBinaryFold<bool>((a, b) => a && b, FluencyType.Bool, "All", topArgs, _bools[0])},
            {"Or", (_, __) => new WrapBinary<bool, bool, bool>((a, b) => a || b, FluencyType.Bool, FluencyType.Bool, "Or", _emptyArgs)},
            {"Any", (topArgs, _) => new WrapBinaryFold<bool>((a, b) => a || b, FluencyType.Bool, "Any", topArgs, _bools[1])},
            {"Not", (_, __) => new WrapUnary<bool, bool>(a => !a, FluencyType.Bool, "Not")},
            {"AddDouble", (topArgs, _) => new WrapBinary<double, double, double>((a, b) => a + b, FluencyType.Double, FluencyType.Double, "AddDouble", topArgs )},
            {"ParseInt", (_, __) => new WrapUnary<string, int>(int.Parse, FluencyType.Int, "ParseInt")},
            {"ParseDouble", (_, __) => new WrapUnary<string, double>(double.Parse, FluencyType.Double, "ParseDouble")},
            {"Concat", (topArgs, bottomArgs) => new WrapBinary<object, object, object>((a, b) => a.ToString() + b.ToString(), FluencyType.Any, FluencyType.String, "Concat", topArgs, bottomArgs )},
            {"Split", (topArgs, _) => new WrapBinaryStreamOutput<string, string, string>((a, b) =>
                                             a.Split(new string[] {b}, StringSplitOptions.RemoveEmptyEntries), FluencyType.String, FluencyType.String, "Split", topArgs )},
            {"DivMod", (topArgs, bottomArgs) => new WrapBinaryTwoOutputs<int, int, int, int>((a, b) => (a/b, a%b) , FluencyType.Int, FluencyType.Int, FluencyType.Int, "DivMod", topArgs, bottomArgs )},
            {"DivDouble", (topArgs, bottomArgs) => new WrapBinary<double, double, double>((a, b) => a/b , FluencyType.Double, FluencyType.Double, "DivDouble", topArgs, bottomArgs )},
        };

        public IFunction Resolve(string name, Value[] topArguments, Value[] bottomArguments = null)
        {
            return BuiltInFunctions[name](topArguments, bottomArguments);
        }
    }
}