using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class WaferAdmin : MonoBehaviour, IWaferTracker
{
    public static readonly string[] SigList60 =
    {
        "wafers/clean60", "wafers/clean60", "wafers/clean60",
        "wafers/clean60", "wafers/sigWedge"
    };

    public static WaferAdmin Singleton;
    
    private readonly List<WaferObject> _waferList = new List<WaferObject>();
    private readonly Dictionary<int, byte> _lotSet = new Dictionary<int, byte>();

    private byte _lastLot;
    
    // ReSharper disable once UnusedMember.Local
    private void Awake()
    {
        if (Singleton == null)
        {
            DontDestroyOnLoad(gameObject);
            Singleton = this;

            // Initializations
            _lastLot = 0;
        }
        else if (Singleton != this)
        {
            Destroy(gameObject);
        }
    }

    public bool CreateWafer(int lot, int waferSize, IWaferStorage waferStorage)
    {
        // Normally "wafers/clean" + waferSize.ToString();
        // Simulates random signatures
        var newWafer = new WaferObject(SigList60[Random.Range(0, SigList60.Length)], lot, waferStorage.StorageId());
        if (waferStorage.StorageAvailability() <= 0 || LotAvailability(lot) <= 0) return false;
        AddWafer(newWafer);
        return waferStorage.Insert(newWafer);
    }

    public bool CreateLot(byte waferCount, int waferSize, IWaferStorage waferStorage)
    {

        // Do not create any wafers if wafers would not all fit in storage
        if (waferCount > waferStorage.StorageAvailability())
        {
            return false;
        }

        // Find empty lot
        while (LotAvailability(Singleton._lastLot) != Constants.FoupSize || Singleton._lastLot == 0)
        {
            Singleton._lastLot += 1;
        }

        for (var i = 0; i < waferCount; i++)
        {
            var success = CreateWafer(Singleton._lastLot, waferSize, waferStorage);
            if (!success) return false;
        }

        return true;
    }

    public List<WaferObject> AllWafers()
    {
        return Singleton._waferList;
    }

    private static void AddWafer(WaferObject wafer)
    {
        if (LotAvailability(wafer.Lot) == Constants.FoupSize)
        {
            Singleton._lotSet[wafer.Lot] = 0;
        }
        Singleton._lotSet[wafer.Lot] += 1;
        wafer.Number = Singleton._lotSet[wafer.Lot];
        Singleton._waferList.Add(wafer);

        wafer.Tracked = true;
    }

    private static byte LotAvailability(int lot)
    {
        byte number;
        Singleton._lotSet.TryGetValue(lot, out number);
        return (byte)(Constants.FoupSize - number);
    }
}