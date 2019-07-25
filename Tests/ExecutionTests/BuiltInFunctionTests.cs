
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
            new[] { new Value(6, FluencyType.Int), new Value(7, FluencyType.Int), new Value(8, FluencyType.Int), new Value(9, FluencyType.Int), new Value(10, FluencyType.Int), },
            new Value[0]
        };

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void Comment(int sequenceIndex)
        {
            Comment com = new Comment();

            var enumerator = sequences[sequenceIndex].GetEnumerator();

            com.TopInput = () => { while (enumerator.MoveNext()) { return enumerator.Current; } return Value.Finished; };

            var result = ReadOutput(com.Top).ToArray();

            Assert.AreEqual(sequences[sequenceIndex].Count() + 1, result.Length); //five inputs plus one finished
            EqualEnumerables(sequences[sequenceIndex].Select(x => x.Get<int>()), result.TakeWhile(x => !x.Done).Select(x => x.Get<int>()));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void Com(int sequenceIndex)
        {
            Com com = new Com();

            var enumerator = sequences[sequenceIndex].GetEnumerator();

            com.TopInput = () => { while (enumerator.MoveNext()) { return enumerator.Current; } return Value.Finished; };

            var result = ReadOutput(com.Top).ToArray();

            Assert.AreEqual(sequences[sequenceIndex].Count() + 1, result.Length); //all inputs plus one finished
            EqualEnumerables(sequences[sequenceIndex].Select(x => x.Get<int>()), result.TakeWhile(x => !x.Done).Select(x => x.Get<int>()));
        }

        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(0, 0)]
        [DataRow(1, 0)]
        [DataRow(1, 1)]
        [DataRow(1, 2)]
        [DataRow(0, 2)]
        [DataRow(2, 1)]
        [DataRow(2, 0)]
        public void Drain(int topIdx, int bottomIdx)
        {
            Drain drain = new Drain();

            var topEnumerator = sequences[topIdx].GetEnumerator();
            drain.TopInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };
            var bottomEnumerator = sequences[bottomIdx].GetEnumerator();
            drain.BottomInput = () => { while (bottomEnumerator.MoveNext()) { return bottomEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(drain.Top).ToArray();
            var bottomresult = ReadOutput(drain.Bottom).ToArray();

            Assert.AreEqual(1, topresult.Length); //one finished
            Assert.IsTrue(topresult.Single().Done);
            Assert.AreEqual(1, bottomresult.Length); //one finished
            Assert.IsTrue(bottomresult.Single().Done);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void Dup(int sequenceIndex)
        {
            Dup dup = new Dup();

            var topEnumerator = sequences[sequenceIndex].GetEnumerator();
            dup.TopInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(dup.Top).ToArray();
            var bottomresult = ReadOutput(dup.Bottom).ToArray();

            Assert.AreEqual(sequences[sequenceIndex].Count() + 1, topresult.Length); //all inputs plus one finished
            EqualEnumerables(sequences[sequenceIndex].Select(x => x.Get<int>()), topresult.TakeWhile(x => !x.Done).Select(x => x.Get<int>()));
            Assert.AreEqual(sequences[sequenceIndex].Count() + 1, bottomresult.Length); //all inputs plus one finished
            EqualEnumerables(sequences[sequenceIndex].Select(x => x.Get<int>()), bottomresult.TakeWhile(x => !x.Done).Select(x => x.Get<int>()));
        }


        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void Duplicate(int sequenceIndex)
        {
            Duplicate duplicate = new Duplicate();

            var topEnumerator = sequences[sequenceIndex].GetEnumerator();
            duplicate.TopInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(duplicate.Top).ToArray();
            var bottomresult = ReadOutput(duplicate.Bottom).ToArray();

            Assert.AreEqual(sequences[sequenceIndex].Count() + 1, topresult.Length); //all inputs plus one finished
            EqualEnumerables(sequences[sequenceIndex].Select(x => x.Get<int>()), topresult.TakeWhile(x => !x.Done).Select(x => x.Get<int>()));
            Assert.AreEqual(sequences[sequenceIndex].Count() + 1, bottomresult.Length); //all inputs plus one finished
            EqualEnumerables(sequences[sequenceIndex].Select(x => x.Get<int>()), bottomresult.TakeWhile(x => !x.Done).Select(x => x.Get<int>()));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void First(int sequenceIndex)
        {
            First first = new First(new Value[0]);

            var topEnumerator = sequences[sequenceIndex].GetEnumerator();
            first.TopInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(first.Top).ToArray();

            Assert.AreEqual(Min(2, sequences[sequenceIndex].Count()+1), topresult.Length);
            EqualEnumerables(sequences[sequenceIndex].Take(1).Select(x => x.Get<int>()), topresult.TakeWhile(x => !x.Done).Select(x => x.Get<int>()));
        }

        private int Min(int a, int b)
        {
            return a < b ? a : b;
        }


        [TestMethod]
        [DataRow(0, 2)]
        [DataRow(0, 1)]
        [DataRow(0, 3)]
        [DataRow(0, 4)]
        [DataRow(1, 2)]
        [DataRow(1, 3)]
        [DataRow(1, 0)]
        [DataRow(1, 5)]
        [DataRow(2, 2)]
        [DataRow(2, 1)]
        [DataRow(2, 0)]
        public void FirstN(int sequenceIndex, int toTake)
        {
            First first = new First(new Value[] { new Value(toTake, FluencyType.Int) });

            var topEnumerator = sequences[sequenceIndex].GetEnumerator();
            first.TopInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(first.Top).ToArray();

            Assert.AreEqual(Min(toTake + 1, sequences[sequenceIndex].Count() + 1), topresult.Length);
            EqualEnumerables(sequences[sequenceIndex].Take(toTake).Select(x => x.Get<int>()), topresult.TakeWhile(x => !x.Done).Select(x => x.Get<int>()));
        }

        [TestMethod]
        public void FirstNBadArgs()
        {
            Assert.ThrowsException<ExecutionException>(() => new First(new Value[] { new Value("hi", FluencyType.String) }));
            Assert.ThrowsException<ExecutionException>(() => new First(new Value[] { new Value(1, FluencyType.Int) , new Value(1, FluencyType.Int) }));
        }

        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(0, 0)]
        [DataRow(1, 0)]
        [DataRow(1, 1)]
        [DataRow(1, 2)]
        [DataRow(0, 2)]
        [DataRow(2, 1)]
        [DataRow(2, 0)]
        public void MergeTop(int topIdx, int bottomIdx)
        {
            MergeTop mergeTop = new MergeTop();

            var topEnumerator = sequences[topIdx].GetEnumerator();
            mergeTop.TopInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };
            var bottomEnumerator = sequences[bottomIdx].GetEnumerator();
            mergeTop.BottomInput = () => { while (bottomEnumerator.MoveNext()) { return bottomEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(mergeTop.Top).ToArray();

            Assert.AreEqual(sequences[topIdx].Count() + sequences[bottomIdx].Count() + 1, topresult.Length);
            EqualEnumerables(sequences[topIdx].Concat(sequences[bottomIdx]).Select(x => x.Get<int>()), topresult.TakeWhile(x => !x.Done).Select(x => x.Get<int>()));
        }

        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(0, 0)]
        [DataRow(1, 0)]
        [DataRow(1, 1)]
        [DataRow(1, 2)]
        [DataRow(0, 2)]
        [DataRow(2, 1)]
        [DataRow(2, 0)]
        public void MergeBottom(int topIdx, int bottomIdx)
        {
            MergeBottom mergeBottom = new MergeBottom();

            var topEnumerator = sequences[topIdx].GetEnumerator();
            mergeBottom.TopInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };
            var bottomEnumerator = sequences[bottomIdx].GetEnumerator();
            mergeBottom.BottomInput = () => { while (bottomEnumerator.MoveNext()) { return bottomEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(mergeBottom.Top).ToArray();

            Assert.AreEqual(sequences[topIdx].Count() + sequences[bottomIdx].Count() + 1, topresult.Length);
            EqualEnumerables(sequences[bottomIdx].Concat(sequences[topIdx]).Select(x => x.Get<int>()), topresult.TakeWhile(x => !x.Done).Select(x => x.Get<int>()));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void SwitchTop(int sequenceIndex)
        {
            Switch testSwitch = new Switch(new Value[] { new Value(true, FluencyType.Bool) });

            var topEnumerator = sequences[sequenceIndex].GetEnumerator();
            testSwitch.TopInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(testSwitch.Top).ToArray();
            var bottomresult = ReadOutput(testSwitch.Bottom).ToArray();

            Assert.AreEqual(1, bottomresult.Length);
            Assert.AreEqual(sequences[sequenceIndex].Count() + 1, topresult.Length);
            EqualEnumerables(sequences[sequenceIndex].Select(x => x.Get<int>()), topresult.TakeWhile(x => !x.Done).Select(x => x.Get<int>()));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void SwitchBottom(int sequenceIndex)
        {
            Switch testSwitch = new Switch(new Value[] { new Value(false, FluencyType.Bool) });

            var topEnumerator = sequences[sequenceIndex].GetEnumerator();
            testSwitch.TopInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(testSwitch.Top).ToArray();
            var bottomresult = ReadOutput(testSwitch.Bottom).ToArray();

            Assert.AreEqual(1, topresult.Length);
            Assert.AreEqual(sequences[sequenceIndex].Count() + 1, bottomresult.Length);
            EqualEnumerables(sequences[sequenceIndex].Select(x => x.Get<int>()), bottomresult.TakeWhile(x => !x.Done).Select(x => x.Get<int>()));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void SwitchBottomArg(int sequenceIndex)
        {
            Switch testSwitch = new Switch(new Value[0]);

            var topEnumerator = sequences[sequenceIndex].Prepend(new Value(false, FluencyType.Bool)).GetEnumerator();
            testSwitch.TopInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(testSwitch.Top).ToArray();
            var bottomresult = ReadOutput(testSwitch.Bottom).ToArray();

            Assert.AreEqual(1, topresult.Length);
            Assert.AreEqual(sequences[sequenceIndex].Count() + 1, bottomresult.Length);
            EqualEnumerables(sequences[sequenceIndex].Select(x => x.Get<int>()), bottomresult.TakeWhile(x => !x.Done).Select(x => x.Get<int>()));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void SwitchTopArg(int sequenceIndex)
        {
            Switch testSwitch = new Switch(new Value[0]);

            var topEnumerator = sequences[sequenceIndex].Prepend(new Value(true, FluencyType.Bool)).GetEnumerator();
            testSwitch.TopInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(testSwitch.Top).ToArray();
            var bottomresult = ReadOutput(testSwitch.Bottom).ToArray();

            Assert.AreEqual(1, bottomresult.Length);
            Assert.AreEqual(sequences[sequenceIndex].Count() + 1, topresult.Length);
            EqualEnumerables(sequences[sequenceIndex].Select(x => x.Get<int>()), topresult.TakeWhile(x => !x.Done).Select(x => x.Get<int>()));
        }

        [TestMethod]
        public void Add()
        {
            WrapMath<int> add = new WrapMath<int>((a, b) => a + b, FluencyType.Int, "Add", new Value[0]);

            var topEnumerator = sequences[0].GetEnumerator();
            add.TopInput = () => { while (topEnumerator.MoveNext()) { return topEnumerator.Current; } return Value.Finished; };
            var bottomEnumerator = sequences[1].GetEnumerator();
            add.BottomInput = () => { while (bottomEnumerator.MoveNext()) { return bottomEnumerator.Current; } return Value.Finished; };

            var topresult = ReadOutput(add.Top).ToArray();

            Assert.AreEqual(Min(sequences[0].Count(),  sequences[1].Count()) + 1, topresult.Length);
            EqualEnumerables(sequences[0].Zip(sequences[1], (a, b) => a.Get<int>() + b.Get<int>()), topresult.TakeWhile(x => !x.Done).Select(x => x.Get<int>()));
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
