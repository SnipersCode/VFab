using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base wafer object. Only used for data storage.
/// </summary>
/// <remarks>
/// Do not use for manipulating wafers.
/// Initializations and Fields/Properties only.
/// </remarks>
[Serializable]
public class WaferObject : IComparable<WaferObject>
{
    public int Lot;
    public byte Number;
    public int StorageId;
    public List<WaferStep> Flow;

    public byte[] Map;
    public byte DieRowCount;
    public bool Tracked;

    public string Id
    {
        get { return Lot.ToString("D7") + "-" + Number.ToString("D2"); }
    }

    public int RealDieCount
    {
        get
        {
            var count = 0;
            foreach (var die in Map)
            {
                if (die == 0) continue;
                count += 1;
            }
            return count;
        }
    }

    public int GoodDieCount
    {
        get
        {
            var count = 0;
            foreach (var die in Map)
            {
                if (!Constants.IsGoodDie(die)) continue;
                count += 1;
            }
            return count;
        }
    }

    /// <summary>
    /// Base wafer constructor.
    /// </summary>
    /// <param name="fileName">File path to json-formatted wafer map</param>
    /// <param name="lot">Intial lot number</param>
    /// <param name="storageId">Initial foup id</param>
    public WaferObject(string fileName, int lot, int storageId)
    {
        var sigFile = Resources.Load(fileName) as TextAsset;
        if (sigFile != null)
        {
            var initialMap = JsonUtility.FromJson<WaferObject>(sigFile.text);
            Map = initialMap.Map;
            DieRowCount = initialMap.DieRowCount;
        }
        else
        {
            Map = new byte[3600];
            Array.Clear(Map, 0, 3600);
            DieRowCount = 60;
        }

        Lot = lot;
        StorageId = storageId;
        Flow = new List<WaferStep>();
        Tracked = false;
    }
    /// <summary>
    /// Empty wafer
    /// </summary>
    public WaferObject()
    {
        Map = new byte[3600];
        Array.Clear(Map, 0, 3600);
        DieRowCount = 60;

        Lot = 0;
        StorageId = 0;
        Flow = new List<WaferStep>();
    }

    public int CompareTo(WaferObject other)
    {
        return string.Compare(Id, other.Id, StringComparison.Ordinal);
    }

    public Dictionary<byte, int> BinData()
    {
        var binData = new Dictionary<byte, int>();

        foreach (var die in Map)
        {
            if(die == 0) continue;
            int oldValue;
            binData.TryGetValue(die, out oldValue);
            if (oldValue > 0)
            {
                binData[die]++;
            }
            else
            {
                binData[die] = 1;
            }
        }

        return binData;
    }

}