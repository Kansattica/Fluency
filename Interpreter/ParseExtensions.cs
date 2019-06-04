using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class ParseExtensions
{

    public static string Stringify(this IEnumerable<char> source)
    {
        var sb = new StringBuilder();
        foreach(char c in source)
            sb.Append(c);
        return sb.ToString();
    }

    public static IEnumerable<IGrouping<TKey, TSource>> GroupAdjacent<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
    {
        TKey last = default(TKey);
        bool haveLast = false;
        List<TSource> list = new List<TSource>();
        foreach (TSource s in source)
        {
            TKey k = keySelector(s);
            if (haveLast)
            {
                if (!k.Equals(last))
                {
                    yield return new GroupOfAdjacent<TSource, TKey>(list, last);
                    list = new List<TSource>();
                    list.Add(s);
                    last = k;
                }
                else
                {
                    list.Add(s);
                    last = k;
                }
            }
            else
            {
                list.Add(s);
                last = k;
                haveLast = true;
            }
        }
        if (haveLast)
            yield return new GroupOfAdjacent<TSource, TKey>(list, last);
    }
}

//from https://blogs.msdn.microsoft.com/ericwhite/2008/04/20/the-groupadjacent-extension-method/
public class GroupOfAdjacent<TSource, TKey> : IEnumerable<TSource>, IGrouping<TKey, TSource>
{
    public TKey Key { get; set; }
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
    public GroupOfAdjacent(List<TSource> source, TKey key)
    {
        GroupList = source;
        Key = key;
    }
}