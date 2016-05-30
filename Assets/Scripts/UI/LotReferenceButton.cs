using UnityEngine;

public class LotReferenceButton : MonoBehaviour, IButtonReference<string, ILotInterface>
{
    [SerializeField]
    private string _lotReference;

    // Dependency Injection
    private ILotInterface _targetDisplay;

    public void Initialize(ILotInterface targetDisplay, string targetReference)
    {
        _targetDisplay = targetDisplay;
        _lotReference = targetReference;
    }

    public void InvokeAction()
    {
        _targetDisplay.DisplayTool(_lotReference);
    }

    public string GetReference()
    {
        return _lotReference;
    }
}