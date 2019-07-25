
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fluency.Interpreter.Common;
using Fluency.Interpreter.Execution.Functions;
using Fluency.Interpreter.Execution.Functions.BuiltIn;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]

namespace Fluency.Tests.Execution
{
    [TestClass]
    public class BuiltInFunctionTests
    {

        public static IEnumerable<Value>[] sequences = new IEnumerable<Value>[] 
        {
            new[] { new Value(1, FluencyType.Int), new Value(2, FluencyType.Int), new Value(3, FluencyType.Int), new Value(4, FluencyType.Int), new Value(5, FluencyType.Int), }
        };

        [TestMethod]
        public void Comment()
        {
            Comment com = new Comment();

            var enumerator = sequences[0].GetEnumerator();

            com.TopInput = () => { while (enumerator.MoveNext()) { return enumerator.Current; } return Value.Finished; };

            var result = ReadOutput(com.Top).ToArray();

            Assert.AreEqual(6, result.Length); //five inputs plus one finished
            EqualEnumerables(new[] { 1, 2, 3, 4, 5 }, result.Take(5).Select(x => x.Get<int>()));
        }

        [TestMethod]
        public void Com()
        {
            Com com = new Com();

            var enumerator = sequences[0].GetEnumerator();

            com.TopInput = () => { while (enumerator.MoveNext()) { return enumerator.Current; } return Value.Finished; };

            var result = ReadOutput(com.Top).ToArray();

            Assert.AreEqual(6, result.Length); //five inputs plus one finished
            EqualEnumerables(new[] { 1, 2, 3, 4, 5 }, result.Take(5).Select(x => x.Get<int>()));
        }

        [TestMethod]
        public void Drain()
        {
            Drain drain = new Drain();

            var topEnumerator = sequences[0].GetEnumerator();
            drain.TopInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };
            var bottomEnumerator = sequences[0].GetEnumerator();
            drain.BottomInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(drain.Top).ToArray();
            var bottomresult = ReadOutput(drain.Bottom).ToArray();

            Assert.AreEqual(1, topresult.Length); //one finished
            Assert.IsTrue(topresult.Single().Done);
            Assert.AreEqual(1, bottomresult.Length); //one finished
            Assert.IsTrue(bottomresult.Single().Done);
        }

        public IEnumerable<Value> ReadOutput(GetNext f)
        {
            Value next;
            while (next = f())
            {
                yield return next;
            }
            yield return Value.Finished;
        }

        private void EqualEnumerables<T>(IEnumerable<IEnumerable<T>> expected, IEnumerable<IEnumerable<T>> actual)
        {
            Assert.AreEqual(expected.Count(), actual.Count());
            foreach (var pair in expected.Zip(actual, (ex, act) => (ex, act)))
            {
                EqualEnumerables(pair.ex, pair.act);
            }
        }
        private void EqualEnumerables<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            if (!expected.SequenceEqual(actual))
            {
                Assert.Fail("(Expected subarray: {0} Got: {1})", Stringify(expected), Stringify(actual));
            }
        }

        private string Stringify<T>(IEnumerable<IEnumerable<T>> toPrint)
        {
            StringBuilder sb = new StringBuilder("{");
            bool first = true;
            foreach (var subenum in toPrint)
            {
                if (!first)
                    sb.Append(", ");
                else
                    first = false;

                sb.Append(Stringify(subenum));
            }
            sb.Append("}");
            return sb.ToString();
        }

        private string Stringify<T>(IEnumerable<T> en)
        {
            return "{" + string.Join(", ", en) + "}";
        }
    }
}
