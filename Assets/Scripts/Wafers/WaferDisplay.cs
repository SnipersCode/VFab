using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaferDisplay : MonoBehaviour, IWaferUserInterface
{
    public bool DrawWafer;

    public GameObject DisplayCanvas;
    public GameObject SelectSystem;
    public Button DisplayButton;
    public GameObject BaseDie;

    public Text WaferIdText;
    public Text WaferYieldText;
    public Text WaferFlowText;
    public Text WaferLogText;

    public Slider[] BinBarSlider;
    public Text[] BinBarText;

    public GameObject BaseWaferButton;
    public GridLayoutGroup InspectionGroup;
    private readonly List<GameObject> _waferButtonGroup = new List<GameObject>();
    public Text WaferCountText;
    
    private int _displayIndex;
    public GridLayoutGroup DieGroup;
    [SerializeField]
    private List<WaferObject> _waferGroup = new List<WaferObject>();
    private readonly List<GameObject> _dieGroup = new List<GameObject>();

    private bool CanvasEnabled
    {
        get { return DisplayCanvas.GetComponent<Canvas>().enabled; }
        set { DisplayCanvas.GetComponent<Canvas>().enabled = value; }
    }
    
    // ReSharper disable once UnusedMember.Local
    private void Start()
    {
        Reset();
    }

    public void ShowDisplay(bool state)
    {
        CanvasEnabled = state;
        if (state && _waferGroup.Count > 0)
        {
            DisplayWafer(_waferGroup[_displayIndex]);
        }
    }

    public void ToggleDisplay()
    {
        ShowDisplay(!CanvasEnabled);
    }

    public void AddWafer(WaferObject selectedWafer, bool sort)
    {
        DisplayButton.interactable = true;
        if (!_waferGroup.Contains(selectedWafer))
        {
            _waferGroup.Add(selectedWafer);

            var button = Instantiate(BaseWaferButton);
            button.InitializeButtonReference(this, selectedWafer);
            button.GetComponentInChildren<Text>().text = selectedWafer.Id;
            button.transform.SetParent(InspectionGroup.transform);
            _waferButtonGroup.Add(button);

            WaferCountText.text = string.Format("Inspected Wafers\nCount: {0}", ViewCount());
            
        }
        if (sort) _waferGroup.Sort();
    }

    public void AddWaferList(List<WaferObject> waferList)
    {
        waferList.Sort();
        // Slow version in order to prevent use of LINQ
        foreach (var wafer in waferList)
        {
            AddWafer(wafer, false);
        }
        _waferGroup.Sort();
    }

    public void AddWaferList(IWaferStorage storage)
    {
        AddWaferList(storage.WaferList());
    }

    public void AddWaferList(List<IWaferStorage> storageList)
    {
        foreach (var storage in storageList)
        {
            AddWaferList(storage.WaferList());
        }
    }

    public void AddSelectedStorages()
    {
        var selectSystem = SelectSystem.GetComponent<ISelectSystem<IStorageIcon>>();
        foreach (var storageIcon in selectSystem.ListSelected())
        {
            storageIcon.AddToUserInterface(Constants.UserInterface.Wafer);
            storageIcon.ResetState();
        }
        selectSystem.Clear();
    }

    public void ChangeView(bool forward)
    {
        if (forward)
        {
            _displayIndex++;
            if (_displayIndex >= _waferGroup.Count)
            {
                _displayIndex = 0;
            }
        }
        else
        {
            _displayIndex--;
            if (_displayIndex < 0)
            {
                _displayIndex = _waferGroup.Count - 1;
            }
        }
        if (_waferGroup.Count > 0)
        {
            DisplayWafer(_waferGroup[_displayIndex]);
        }
    }

    public void ChangeView(int index)
    {
        if (_waferGroup.Count <= 0)
        {
            _displayIndex = 0;
            return;
        }
        if (index >= _waferGroup.Count)
        {
            _displayIndex = _waferGroup.Count - 1;
        }
        else
        {
            _displayIndex = index;
        }
        DisplayWafer(_waferGroup[_displayIndex]);
    }

    public void ChangeView(WaferObject targetWafer)
    {
        var index = _waferGroup.IndexOf(targetWafer);
        if (index == -1) return;
        _displayIndex = index;
        DisplayWafer(_waferGroup[_displayIndex]);
    }

    public void RemoveWafer()
    {
        RemoveWafer(_displayIndex);
    }

    public void RemoveWafer(int index)
    {
        if (_waferGroup.Count > 0)
        {
            foreach (var waferButton in _waferButtonGroup)
            {
                if (waferButton.GetComponent<IButtonReference<WaferObject, IWaferUserInterface>>().GetReference() == _waferGroup[index])
                {
                    Destroy(waferButton);
                }
            }
            _waferButtonGroup.RemoveAll(i => i.GetComponent<IButtonReference<WaferObject, IWaferUserInterface>>().GetReference() == _waferGroup[index]);
            _waferGroup.RemoveAt(index);
            WaferCountText.text = string.Format("Inspected Wafers\nCount: {0}", ViewCount());
        }
        if (_waferGroup.Count == 0)
        {
            Reset();
            return;
        }
        ChangeView(_displayIndex);
    }

    public void RemoveAllWafers()
    {
        _waferGroup.Clear();
        Reset();
    }

    public byte ViewCount()
    {
        return (byte)_waferGroup.Count;
    }

    private void Reset()
    {
        DisplayButton.interactable = false;
        CanvasEnabled = false;
    }


    private void DisplayWafer(WaferObject selectedWafer)
    {
        // Bin Stats
        var binData = selectedWafer.BinData();
        var realDieCount = selectedWafer.RealDieCount;
        var goodDieCount = selectedWafer.GoodDieCount;

        // Show wafer information
        WaferIdText.text = selectedWafer.Id;
        WaferFlowText.text = string.Format("Wafer Flow Log\nCurrent Log Length: {0}", selectedWafer.Flow.Count);
        WaferLogText.text = string.Join("\n", selectedWafer.Flow.ConvertAll(step => step.ToString()).ToArray());

        // Display wafer map
        DieGroup.cellSize = new Vector2((float) Constants.WaferDiameter / selectedWafer.DieRowCount,
            (float) Constants.WaferDiameter / selectedWafer.DieRowCount);

        if ((_dieGroup.Count != selectedWafer.Map.Length || _dieGroup.Count == 0) && DrawWafer)
        {
            ClearDieGroup();
            InitDie(selectedWafer.Map.Length);
        }
        for (var i = 0; i < selectedWafer.Map.Length; i++)
        {
            if (DrawWafer)
            {
                _dieGroup[i].GetComponent<Image>().color = Constants.BinColor(selectedWafer.Map[i]);
            }
        }

        // Display bin statistics
        var sliderCounter = 0;
        var tempBinList = new List<KeyValuePair<byte, int>>();

        // Complicated sorting in order to prevent use of LINQ
        foreach (var bin in binData)
        {
            tempBinList.Add(bin);
        }
        tempBinList.Sort((x, y) => x.Value.CompareTo(y.Value));
        tempBinList.Reverse();

        foreach(var bin in tempBinList)
        {
            BinBarSlider[sliderCounter].transform.parent.gameObject.SetActive(true);
            BinBarSlider[sliderCounter].value = (float)bin.Value / realDieCount;
            BinBarText[sliderCounter].text = string.Format("Bin {0:D2} {1:P2}", bin.Key, (float)bin.Value / realDieCount);
            sliderCounter++;
        }
        if (BinBarSlider.Length > binData.Count)
        {
            for (var i = sliderCounter; i < BinBarSlider.Length; i++)
            {
                BinBarSlider[i].transform.parent.gameObject.SetActive(false);
            }
        }

        WaferYieldText.text = string.Format("Bin Breakdown\nYield: {0:P2}", (float) goodDieCount/realDieCount);

    }

    private void ClearDieGroup()
    {
        foreach (var instantiatedDie in _dieGroup)
        {
            Destroy(instantiatedDie);
        }
        _dieGroup.Clear();
    }

    private void InitDie(int dieCount)
    {
        for (var i = 0; i < dieCount; i++)
        {
            var currentDie = Instantiate(BaseDie, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            _dieGroup.Add(currentDie);
            if (currentDie != null)
            {
                currentDie.transform.SetParent(DieGroup.transform, false);
            }
        }
    }

}