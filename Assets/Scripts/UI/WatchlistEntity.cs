using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WatchlistEntity : Selectable, IStorageIcon, IPointerClickHandler
{
    // Moving with arrow keys or clicking = select.
    // Selecting makes highlighting permanent
    // Mousing over = temp highlight only

    public Text FoupId;
    public Image Background;
    
    // Dependency Injection
    private IWaferUserInterface _waferUserInterface;
    private IWaferUserInterface _lotUserInterface;
    private IWaferStorage _storageReference;
    private ISelectSystem<IStorageIcon> _selectSystem;

    public void Initialize(IWaferUserInterface waferUserInterface, IWaferUserInterface lotUserInterface, ISelectSystem<IStorageIcon> selectSystem, IWaferStorage storageReference)
    {
        _waferUserInterface = waferUserInterface;
        _lotUserInterface = lotUserInterface;
        _storageReference = storageReference;
        _selectSystem = selectSystem;

        FoupId.text = storageReference.StorageId().ToString("D7");
    }

    public void AddToUserInterface(Constants.UserInterface level)
    {
        switch (level)
        {
            case (Constants.UserInterface.Lot):
                _lotUserInterface.ShowDisplay(true);
                _lotUserInterface.AddWaferList(_storageReference.WaferList());
                _lotUserInterface.ChangeView(0);
                break;
            default:
                _waferUserInterface.ShowDisplay(true);
                _waferUserInterface.AddWaferList(_storageReference.WaferList());
                _waferUserInterface.ChangeView(0);
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
        if (_selectSystem.IsSelected(this))
        {
            _selectSystem.Deselect(this);
            image.color = Constants.NotSelectedColor;
        }
        else
        {
            _selectSystem.Select(this);
            image.color = Constants.SelectedColor;
        }
    }

    public void ResetState()
    {
        image.color = Constants.NotSelectedColor;
    }

}
