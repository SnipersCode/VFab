using System;
using System.Collections.Generic;

public static class InternetExtensions{

    // http://stackoverflow.com/a/1262619 :: First fast version
    // Fisher-Yates Shuffle
    // Does not use cryptography, therefore not strongly random
    // Not thread safe
    public static void Shuffle<T>(this IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = UnityEngine.Random.Range(0, n + 1);
            var value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }


    public static double ToEpoch(this DateTime date)
    {
        return date.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }
}
