using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class ParseExtensions
{

    public static string Stringify(this IEnumerable<char> source)
    {
        var sb = new StringBuilder();
        foreach (char c in source)
            sb.Append(c);
        return sb.ToString();
    }

    public static IEnumerable<IEnumerable<TSource>> GroupUntil<TSource>(this IEnumerable<TSource> source,
        Func<TSource, bool> predicate)
    {
        List<TSource> currentChunk = new List<TSource>();
        foreach (TSource s in source)
        {
            bool newGroup = predicate(s);

            if (newGroup)
            {
                yield return new UntilGroup<TSource>(currentChunk);
                currentChunk = new List<TSource>() { s };
            }
            else
            {
                currentChunk.Add(s);
            }
        }
        yield return new UntilGroup<TSource>(currentChunk);
    }


    public class UntilGroup<TSource> : IEnumerable<TSource>
    {
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
        public UntilGroup(List<TSource> source)
        {
            GroupList = source;
        }
    }

}