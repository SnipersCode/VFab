using UnityEngine;
using System.Collections.Generic;

public class WaferFoup : MonoBehaviour, IWaferStorage
{
    public GameObject UserInterfaceIcon;
    public GameObject Watchlist;
    
    private int _storageId;
    private readonly List<WaferObject> _waferList = new List<WaferObject>();

    // Dependency Injection
    private IWaferUserInterface _waferDisplay;
    private IWaferUserInterface _lotDisplay;
    private ISelectSystem<IStorageIcon> _selectSystem;

    public void Initialize(IWaferUserInterface waferDisplay, IWaferUserInterface lotDisplay, ISelectSystem<IStorageIcon> selectSystem, int entityId)
    {
        _storageId = entityId;
        _waferDisplay = waferDisplay;
        _lotDisplay = lotDisplay;
        _selectSystem = selectSystem;
    }

    public bool Insert(WaferObject wafer)
    {
        if (StorageAvailability() <= 0) return false;
        wafer.StorageId = _storageId;
        _waferList.Add(wafer);
        return true;
    }

    public bool Insert(List<WaferObject> waferList)
    {
        if (StorageAvailability() < waferList.Count) return false;
        foreach (var wafer in waferList)
        {
            Insert(wafer);
        }
        return true;
    }
    
    public bool TransferAll(IWaferStorage target)
    {
        if (target.StorageAvailability() < _waferList.Count) return false;
        target.Insert(_waferList);
        _waferList.Clear();
        return true;
    }

    public void Randomize()
    {
        _waferList.Shuffle();
    }

    public List<WaferObject> WaferList()
    {
        return _waferList;
    }

    public int StorageId()
    {
        return _storageId;
    }

    public byte StorageAvailability()
    {
        return (byte)(Constants.FoupSize - _waferList.Count);
    }

    public void Watch()
    {
        Watchlist.SetActive(true);
        var userInterfaceIcon = Instantiate(UserInterfaceIcon);
        userInterfaceIcon.transform.SetParent(Watchlist.transform);
        userInterfaceIcon.InitializeIcon(_waferDisplay, _lotDisplay, _selectSystem, this);
    }

}