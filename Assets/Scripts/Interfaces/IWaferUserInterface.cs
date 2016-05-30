using System.Collections.Generic;

public interface IWaferUserInterface
{

    void ShowDisplay(bool state);
    void ToggleDisplay();
    void AddWafer(WaferObject selectedWafer, bool sort);
    void AddWaferList(List<WaferObject> waferList);
    void ChangeView(bool forward);
    void ChangeView(int index);
    void ChangeView(WaferObject targetWafer);
    void RemoveWafer();
    void RemoveWafer(int index);
    void RemoveAllWafers();
    byte ViewCount();
    void AddSelectedStorages();

}
