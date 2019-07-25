
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fluency.Interpreter.Common;
using Fluency.Interpreter.Execution.Exceptions;
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
            new[] { new Value(1, FluencyType.Int), new Value(2, FluencyType.Int), new Value(3, FluencyType.Int), new Value(4, FluencyType.Int), new Value(5, FluencyType.Int), },
            new[] { new Value(6, FluencyType.Int), new Value(7, FluencyType.Int), new Value(8, FluencyType.Int), new Value(9, FluencyType.Int), new Value(10, FluencyType.Int), }
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
            var bottomEnumerator = sequences[1].GetEnumerator();
            drain.BottomInput = () => { while (bottomEnumerator.MoveNext()) { return bottomEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(drain.Top).ToArray();
            var bottomresult = ReadOutput(drain.Bottom).ToArray();

            Assert.AreEqual(1, topresult.Length); //one finished
            Assert.IsTrue(topresult.Single().Done);
            Assert.AreEqual(1, bottomresult.Length); //one finished
            Assert.IsTrue(bottomresult.Single().Done);
        }

        [TestMethod]
        public void Dup()
        {
            Dup dup = new Dup();

            var topEnumerator = sequences[0].GetEnumerator();
            dup.TopInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(dup.Top).ToArray();
            var bottomresult = ReadOutput(dup.Bottom).ToArray();

            Assert.AreEqual(6, topresult.Length); //five inputs plus one finished
            EqualEnumerables(new[] { 1, 2, 3, 4, 5 }, topresult.Take(5).Select(x => x.Get<int>()));
            Assert.AreEqual(6, bottomresult.Length); 
            EqualEnumerables(new[] { 1, 2, 3, 4, 5 }, bottomresult.Take(5).Select(x => x.Get<int>()));
        }

        [TestMethod]
        public void Duplicate()
        {
            Duplicate duplicate = new Duplicate();

            var topEnumerator = sequences[0].GetEnumerator();
            duplicate.TopInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(duplicate.Top).ToArray();
            var bottomresult = ReadOutput(duplicate.Bottom).ToArray();

            Assert.AreEqual(6, topresult.Length); //five inputs plus one finished
            EqualEnumerables(new[] { 1, 2, 3, 4, 5 }, topresult.Take(5).Select(x => x.Get<int>()));
            Assert.AreEqual(6, bottomresult.Length); 
            EqualEnumerables(new[] { 1, 2, 3, 4, 5 }, bottomresult.Take(5).Select(x => x.Get<int>()));
        }

        [TestMethod]
        public void First()
        {
            First first = new First(new Value[0]);

            var topEnumerator = sequences[0].GetEnumerator();
            first.TopInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(first.Top).ToArray();

            Assert.AreEqual(2, topresult.Length);
            EqualEnumerables(new[] { 1 }, topresult.TakeWhile(x => !x.Done).Select(x => x.Get<int>()));
        }


        [TestMethod]
        public void FirstN()
        {
            First first = new First(new Value[] { new Value(2, FluencyType.Int) });

            var topEnumerator = sequences[0].GetEnumerator();
            first.TopInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(first.Top).ToArray();

            Assert.AreEqual(3, topresult.Length);
            EqualEnumerables(new[] { 1, 2 }, topresult.TakeWhile(x => !x.Done).Select(x => x.Get<int>()));
        }

        [TestMethod]
        public void FirstNBadArgs()
        {
            Assert.ThrowsException<ExecutionException>(() => new First(new Value[] { new Value("hi", FluencyType.String) }));
            Assert.ThrowsException<ExecutionException>(() => new First(new Value[] { new Value(1, FluencyType.Int) , new Value(1, FluencyType.Int) }));
        }

        [TestMethod]
        public void MergeTop()
        {
            MergeTop mergeTop = new MergeTop();

            var topEnumerator = sequences[0].GetEnumerator();
            mergeTop.TopInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };
            var bottomEnumerator = sequences[1].GetEnumerator();
            mergeTop.BottomInput = () => { while (bottomEnumerator.MoveNext()) { return bottomEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(mergeTop.Top).ToArray();

            Assert.AreEqual(11, topresult.Length);
            EqualEnumerables(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, topresult.TakeWhile(x => !x.Done).Select(x => x.Get<int>()));
        }

        [TestMethod]
        public void MergeBottom()
        {
            MergeBottom mergeBottom = new MergeBottom();

            var topEnumerator = sequences[0].GetEnumerator();
            mergeBottom.TopInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };
            var bottomEnumerator = sequences[1].GetEnumerator();
            mergeBottom.BottomInput = () => { while (bottomEnumerator.MoveNext()) { return bottomEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(mergeBottom.Top).ToArray();

            Assert.AreEqual(11, topresult.Length);
            EqualEnumerables(new[] { 6, 7, 8, 9, 10, 1, 2, 3, 4, 5 }, topresult.TakeWhile(x => !x.Done).Select(x => x.Get<int>()));
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
