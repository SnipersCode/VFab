using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectAdmin : MonoBehaviour, ISelectSystem<IStorageIcon>
{

    public static SelectAdmin Singleton;

    public Button[] SelectionButtons;

    private readonly List<IStorageIcon> _uiStorageSelections = new List<IStorageIcon>();

    // ReSharper disable once UnusedMember.Local
    private void Awake()
    {
        if (Singleton == null)
        {
            DontDestroyOnLoad(gameObject);
            Singleton = this;
        }
        else if (Singleton != this)
        {
            Destroy(gameObject);
        }
    }

    // ReSharper disable once UnusedMember.Local
    private void Start()
    {
        SetButtonState(false);
    }

    public void Select(IStorageIcon item)
    {
        SetButtonState(true);
        _uiStorageSelections.Add(item);
    }

    public void Deselect(IStorageIcon item)
    {
        _uiStorageSelections.Remove(item);
        SetButtonState(_uiStorageSelections.Count > 0);
    }

    public List<IStorageIcon> ListSelected()
    {
        return _uiStorageSelections;
    }

    public bool IsSelected(IStorageIcon item)
    {
        return _uiStorageSelections.Contains(item);
    }

    public void Clear()
    {
        _uiStorageSelections.Clear();
        SetButtonState(false);
    }

    private void SetButtonState(bool interactable)
    {
        foreach (var button in SelectionButtons)
        {
            button.interactable = interactable;
        }
    }

}
