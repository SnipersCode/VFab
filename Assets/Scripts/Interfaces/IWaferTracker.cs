using System.Collections.Generic;

public interface IWaferTracker
{
    bool CreateWafer(int lot, int waferSize, IWaferStorage waferStorage);
    bool CreateLot(byte waferCount, int waferSize, IWaferStorage waferStorage);
    List<WaferObject> AllWafers();
}