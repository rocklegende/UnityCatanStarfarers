using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class Shuffler
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static T ShuffleAndPop<T>(this IList<T> list)
    {
        list.Shuffle();
        var first = list[0];
        list.RemoveAt(0);
        return first;
    }

    public static T PopAt<T>(this List<T> list, int index)
    {
        T r = list[index];
        list.RemoveAt(index);
        return r;
    }

    public static bool IsNull(this object obj)
    {
        return obj == null;
    }

    public static bool IsNotNull(this object obj)
    {
        return obj != null;
    }
}
