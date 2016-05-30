using System.Collections.Generic;

public interface IWaferStorage
{
    void Initialize(IWaferUserInterface waferDisplay, IWaferUserInterface lotDisplay, ISelectSystem<IStorageIcon> storageSystem, int entityId);
    bool Insert(WaferObject wafer);
    bool Insert(List<WaferObject> waferList);
    bool TransferAll(IWaferStorage target);
    void Randomize();
    int StorageId();
    byte StorageAvailability();
    List<WaferObject> WaferList();
    void Watch();
}
