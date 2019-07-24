using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fluency.Interpreter.Parsing.Entities;

namespace Fluency.Interpreter.Parsing
{
    /// <summary>
    /// A collection of extension methods to help with parsing.
    /// </summary>
    public static class ParseExtensions
    {

        /// <summary>
        /// Turn an IEnumerable of chars into a string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Stringify(this IEnumerable<char> source) => string.Concat(source);

        /// <summary>
        /// Prettyprint an array of Arguments, with a comma and space between each one.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Stringify(this IEnumerable<Argument> source) => string.Join(", ", source.Select(x => x.ToString()));

        /// <summary>
        /// Return elements from source until startPredicate returns true, then skip them until endPredicate returns true.
        /// Note that the element endPredicate returns true for will be returned.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startPredicate">When this returns true, begin skipping elements.</param>
        /// <param name="endPredicate">When this returns true, stop skipping elements and yield the current one.</param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
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

        /// <summary>
        /// Call predicate on each element. If that predicate returns true, then call merge on the current and previous elements and yield that instead of either the current or previous element.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="predicate">If this is true for current, call merge(current, previous)</param>
        /// <param name="merge">If predicate(current) is true, yield merge(current, previous) instead of current or previous.</param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
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

        /// <summary>
        /// Groups elements according to pickAction. For each element, pickAction is called with the element and whether the function is currently making a group.
        /// Groups will be yielded when complete- that is, when pickAction returns <see cref="GroupWhileAction.LeaveExclude"/> or <see cref="GroupWhileAction.LeaveInclude"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pickAction"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static IEnumerable<Grouped<TSource>> GroupWhile<TSource>(this IEnumerable<TSource> source,
            Func<TSource, bool, GroupWhileAction> pickAction)
        {
            List<TSource> currentChunk = new List<TSource>();
            int startIndex = 0, thisIndex = -1;
            bool inGroup = false;
            foreach (TSource s in source)
            {
                thisIndex++;
                GroupWhileAction nextAction = pickAction(s, inGroup);

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
                        currentChunk = new List<TSource>();
                        inGroup = false;
                        break;
                    case GroupWhileAction.StillOut:
                        inGroup = false;
                        break;
                }

            }

            if (inGroup && currentChunk.Any())
                yield return new Grouped<TSource>(currentChunk, startIndex, thisIndex);
        }


        /// <summary>
        /// Gather elements of source into a group while predicate returns false. If predicate returns true, finish the current group and start a new one.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <param name="inclusive">If true, the element that caused predicate to return false will be put in the current group. Otherwise, it'll be in the next one.</param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
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

    /// <summary>
    /// Represents a group of elements from <see cref="ParseExtensions.GroupUntil{TSource}(IEnumerable{TSource}, Func{TSource, bool}, bool)" /> or <see cref="ParseExtensions.GroupWhile{TSource}(IEnumerable{TSource}, Func{TSource, bool, GroupWhileAction})"/>.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public class Grouped<TSource> : IEnumerable<TSource>
    {
        /// <summary>
        /// The inclusive range of indexes this group was taken from.
        /// </summary>
        /// <value></value>
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

        /// <summary>
        /// Construct a new group.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        public Grouped(List<TSource> source, int startIndex, int endIndex)
        {
            GroupList = source;
            Indexes = new Range(startIndex, endIndex);
        }
    }

    /// <summary>
    /// Actions you can instruct <see cref="ParseExtensions.GroupWhile{TSource}(IEnumerable{TSource}, Func{TSource, bool, GroupWhileAction})"/> to take from its callback.
    /// </summary>
    public enum GroupWhileAction
    {

        /// <summary>
        /// Add the current item into the group, starting a new one if necessary.
        /// </summary>
        In,

        /// <summary>
        /// We're not making a group, and this element should stay out of the group. Only return this if pickAction was passed false.
        /// </summary>
        StillOut,

        /// <summary>
        /// Finish the current group and put this item in it.
        /// </summary>
        LeaveInclude,

        /// <summary>
        /// Finish the current group and don't put this item in it.
        /// </summary>
        LeaveExclude
    }
}