using UnityEngine;

public class WaferReferenceButton : MonoBehaviour, IButtonReference<WaferObject, IWaferUserInterface>
{
    private WaferObject _waferReference;

    // Dependency Injection
    private IWaferUserInterface _targetDisplay;

    public void Initialize(IWaferUserInterface targetDisplay, WaferObject targetWafer)
    {
        _targetDisplay = targetDisplay;
        _waferReference = targetWafer;
    }

    public void InvokeAction()
    {
        _targetDisplay.ChangeView(_waferReference);
    }

    public WaferObject GetReference()
    {
        return _waferReference;
    }
}
