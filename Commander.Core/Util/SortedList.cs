using System.Collections;

namespace Commander.Core.Util;

public class SortedList<T> : ICollection<T>
{
    private LinkedList<T> list = new LinkedList<T>();
    private IComparer<T> comparer;

    public int Count => list.Count;

    public bool IsReadOnly => ((ICollection<T>) list).IsReadOnly;

    public SortedList() : this(Comparer<T>.Default)
    { 
    }
    public SortedList(IComparer<T> comparer)
    {
        this.comparer = comparer;
    }

    public void Add(T item)
    {
        if(list.Count == 0)
        {
            list.AddFirst(item);
            return;
        }
        var current = list.First;
        while (current != null)
        {
            int compare = comparer.Compare(item, current.Value);
            if(compare <= 0)
            {
                list.AddBefore(current, item);
                return;
            }
            current = current.Next;
        }
        list.AddLast(item);
    }

    public void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    public void Clear()
    {
        list.Clear();
    }

    public bool Contains(T item)
    {
        return list.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        list.CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    public bool Remove(T item)
    {
        return list.Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable) list).GetEnumerator();
    }
}