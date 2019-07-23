using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fluency.Interpreter.Parser.Entities;

namespace Fluency.Interpreter.Parser
{
    public static class ParseExtensions
    {

        public static string Stringify(this IEnumerable<char> source)
        {
            var sb = new StringBuilder();
            foreach (char c in source)
                sb.Append(c);
            return sb.ToString();
        }

        public static IEnumerable<UntilGroup<TSource>> GroupUntil<TSource>(this IEnumerable<TSource> source,
            Func<TSource, bool> predicate, bool inclusive = false)
        {
            List<TSource> currentChunk = new List<TSource>();
            int startIndex = 0, thisIndex = 0;
            foreach (TSource s in source)
            {
                bool newGroup = predicate(s);

                if (newGroup && currentChunk.Count > 0)
                {
                    if (inclusive)
                    {
                        currentChunk.Add(s);
                        thisIndex++;
                    }

                    yield return new UntilGroup<TSource>(currentChunk, startIndex, thisIndex - 1);
                    startIndex = thisIndex;

                    if (inclusive)
                        currentChunk = new List<TSource>();
                    else
                        currentChunk = new List<TSource>() { s };
                }
                else
                {
                    currentChunk.Add(s);
                }
                thisIndex++;
            }

            if (currentChunk.Any())
                yield return new UntilGroup<TSource>(currentChunk, startIndex, thisIndex - 1);
        }

    }

    public class UntilGroup<TSource> : IEnumerable<TSource>
    {
        public Range Indexes { get; set; } = null;
        private List<TSource> GroupList { get; set; }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.Generic.IEnumerable<TSource>)this).GetEnumerator();
        }
        System.Collections.Generic.IEnumerator<TSource> System.Collections.Generic.IEnumerable<TSource>.GetEnumerator()
        {
            foreach (var s in GroupList)
                yield return s;
        }
        public UntilGroup(List<TSource> source, int startIndex, int endIndex)
        {
            GroupList = source;
            Indexes = new Range(startIndex, endIndex);
        }

    }
}