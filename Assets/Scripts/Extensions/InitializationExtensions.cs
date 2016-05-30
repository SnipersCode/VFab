using UnityEngine;

public static class InitializationExtensions
{
    public static void InitializeIcon(this GameObject icon, IWaferUserInterface waferDisplay, IWaferUserInterface lotDisplay, ISelectSystem<IStorageIcon> selectSystem, IWaferStorage storage)
    {
        icon.GetComponent<IStorageIcon>().Initialize(waferDisplay, lotDisplay, selectSystem, storage);
    }

    public static void InitializeFoup(this GameObject foup, IWaferUserInterface waferDisplay, IWaferUserInterface lotDisplay, ISelectSystem<IStorageIcon> selectSystem, int id)
    {
        foup.GetComponent<IWaferStorage>().Initialize(waferDisplay, lotDisplay, selectSystem, id);
    }

    public static void InitializeButtonReference(this GameObject button, IWaferUserInterface display, WaferObject targetWafer)
    {
        button.GetComponent<IButtonReference<WaferObject, IWaferUserInterface>>().Initialize(display, targetWafer);
    }

    public static void InitializeLotButtonReference(this GameObject button, ILotInterface display, string tool)
    {
        button.GetComponent<IButtonReference<string, ILotInterface>>().Initialize(display, tool);
    }

}