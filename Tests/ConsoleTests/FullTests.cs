using System;
using System.IO;
using Fluency.Common;
using Fluency.Execution;
using Fluency.Execution.Functions;
using Fluency.Execution.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fluency.Tests.Console
{
    [TestClass]
    public class FullTests
    {
        // raise this for a more thorough test.
        const int countTo = 20001;

        [TestMethod]
        public void PrimeNumbersCorrect()
        {
            var interpreter = new Interpreter(new Parser());
            var primeprog = File.ReadAllLines(@"..\..\..\..\..\Examples\prime.fl".Replace('\\', Path.DirectorySeparatorChar));
            int countFrom = 2;

            var primes = interpreter.Execute(primeprog, () =>
                        countFrom <= countTo ? new Value((countFrom++).ToString(), FluencyType.String) : Value.Finished);

            int lastNumber = 1;
            foreach (var prime in primes)
            {
                if (prime.Done)
                    break;

                // output lines look like:
                //5: True
                var sides = prime.Get<string>(FluencyType.String, "Could not read output as string.").Split(':');

                var number = int.Parse(sides[0]);
                var isprime = bool.Parse(sides[1].Trim());

                Assert.AreEqual(lastNumber, number - 1);
                Assert.AreEqual(IsPrime(number), isprime);

                lastNumber = number;
            }
        }

        private bool IsPrime(int n)
        {
            for (int i = 2; i <= (int)Math.Ceiling(Math.Sqrt(n)); i++)
            {
                if (n % i == 0)
                    return false;
            }
            return true;
        }
    }
}