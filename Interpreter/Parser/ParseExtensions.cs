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

        public static IEnumerable<TSource> SkipBetween<TSource>(this IEnumerable<TSource> source,
        Func<TSource, bool> startPredicate,
        Func<TSource, bool> endPredicate)
        {
            bool skipping = false;
            foreach (TSource s in source)
            {
                if (skipping)
                    skipping = !endPredicate(s);
                else
                {
                    skipping = startPredicate(s);
                    yield return s;
                    continue;
                }

                if (!skipping)
                    yield return s;
            }
        }

        public static IEnumerable<TSource> MergeLastIf<TSource>(this IEnumerable<TSource> source,
            Func<TSource, bool> predicate,
            Func<TSource, TSource, TSource> merge)
        {
            if (!source.Any())
                yield break;

            TSource current, previous, twoAgo;
            current = previous = twoAgo = default(TSource);

            int inPipeline = 0;
            foreach (TSource s in source)
            {
                twoAgo = previous;
                previous = current;
                current = s;
                inPipeline++;

                // if inPipeline == 1, current has a value
                // if inPipeline == 2, current and previous have a value
                // if inPipeline == 3, current, previous, and twoAgo all have values

                if ((inPipeline >= 2) && predicate(current))
                {
                    if (inPipeline >= 3)
                        yield return twoAgo;
                    yield return merge(previous, current);
                    inPipeline = 0; //flush the pipeline

                }

                if (inPipeline == 3)
                {
                    yield return twoAgo;
                    inPipeline--;
                }
            }

            if (inPipeline >= 2)
                yield return previous;

            if (inPipeline >= 1)
                yield return current;
        }

        public static IEnumerable<Grouped<TSource>> GroupWhile<TSource>(this IEnumerable<TSource> source,
            Func<TSource, bool, GroupWhileAction> predicate)
        {
            List<TSource> currentChunk = new List<TSource>();
            int startIndex = 0, thisIndex = -1;
            bool inGroup = false;
            foreach (TSource s in source)
            {
                thisIndex++;
                GroupWhileAction nextAction = predicate(s, inGroup);

                switch (nextAction)
                {
                    case GroupWhileAction.In:
                        if (!inGroup) startIndex = thisIndex;
                        currentChunk.Add(s);
                        inGroup = true;
                        break;
                    case GroupWhileAction.LeaveInclude:
                        currentChunk.Add(s);
                        goto case GroupWhileAction.LeaveExclude;
                    case GroupWhileAction.LeaveExclude:
                        yield return new Grouped<TSource>(currentChunk, startIndex, thisIndex);
                        currentChunk = (nextAction == GroupWhileAction.LeaveInclude ? new List<TSource>() : new List<TSource> { s });
                        inGroup = false;
                        break;
                    case GroupWhileAction.StillOut:
                        inGroup = false;
                        break;
                }

            }

            if (currentChunk.Any())
                yield return new Grouped<TSource>(currentChunk, startIndex, thisIndex - 1);
        }



        public static IEnumerable<Grouped<TSource>> GroupUntil<TSource>(this IEnumerable<TSource> source,
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

                    yield return new Grouped<TSource>(currentChunk, startIndex, thisIndex - 1);
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
                yield return new Grouped<TSource>(currentChunk, startIndex, thisIndex - 1);
        }

    }

    public class Grouped<TSource> : IEnumerable<TSource>
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
        public Grouped(List<TSource> source, int startIndex, int endIndex)
        {
            GroupList = source;
            Indexes = new Range(startIndex, endIndex);
        }
    }
    public enum GroupWhileAction { In, StillOut, LeaveInclude, LeaveExclude }
}