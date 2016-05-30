using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LotDisplay : MonoBehaviour, IWaferUserInterface, ILotInterface
{

    public GameObject DisplayCanvas;
    public GameObject SelectSystem;
    public Button DisplayButton;

    public GameObject BaseReferenceButton;
    public GameObject LotButtonGroup;
    public GameObject BaseLotReferenceButton;
    public GameObject ToolButtonGroup;

    public Slider[] YieldBars;
    public Text[] PositionTexts;
    public Text ToolId;

    public Text LotCount;
    public Text ToolCount;
    public Text PositionCount;
    public Text PositionLog;

    private int _displayIndex;
    private int _waferIndex;
    private readonly List<WaferObject> _waferGroup = new List<WaferObject>();

    private readonly SortedDictionary<string, SortedDictionary<byte, float>> _toolInfo = new SortedDictionary<string, SortedDictionary<byte, float>>();
    private readonly Dictionary<string, Dictionary<byte, string>> _positionInfo = new Dictionary<string, Dictionary<byte, string>>();
    private readonly List<GameObject> _toolButtonGroup = new List<GameObject>();
    private readonly List<GameObject> _lotButtonGroup = new List<GameObject>();

    private readonly  List<string> _orderedToolList = new List<string>();
    private int _toolIndex;
    private string _currentTool;
    private int _currentLot;

    private bool CanvasEnabled
    {
        get { return DisplayCanvas.GetComponent<Canvas>().enabled; }
        set { DisplayCanvas.GetComponent<Canvas>().enabled = value; }
    }

    private void Start()
    {
        Reset();
    }

    public void ShowDisplay(bool state)
    {
        CanvasEnabled = state;
        if (state && _waferGroup.Count > 0)
        {
            DisplayLot(_waferGroup[_displayIndex]);
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
        }
        if (sort) _waferGroup.Sort();
    }

    public void AddWaferList(List<WaferObject> waferList)
    {
        waferList.Sort();
        foreach (var wafer in waferList)
        {
            AddWafer(wafer, false);
        }
        _waferGroup.Sort();
        RefreshWaferButtons();
    }

    public void ChangeView(bool forward)
    {
        if (forward)
        {
            _displayIndex++;
            if (_displayIndex >= ViewCount())
            {
                _displayIndex = 0;
            }
        }
        else
        {
            _displayIndex--;
            if (_displayIndex < 0)
            {
                _displayIndex = ViewCount() - 1;
            }
        }
        if (_waferGroup.Count > 0)
        {
            DisplayLot(_displayIndex);
            DisplayTool(0);
        }
    }

    public void ChangeView(int index)
    {
        if (_waferGroup.Count <= 0)
        {
            _displayIndex = 0;
            return;
        }
        if (index >= ViewCount())
        {
            _displayIndex = ViewCount() - 1;
        }
        else
        {
            _displayIndex = index;
        }
        DisplayLot(_displayIndex);
        DisplayTool(0);
    }

    public void ChangeView(WaferObject targetWafer)
    {
        var index = _waferGroup.IndexOf(targetWafer);
        if (index == -1) return;
        DisplayLot(_waferGroup[index]);
        DisplayTool(0);
    }

    public void RemoveWafer()
    {
        RemoveWafer(_waferIndex);
    }

    public void RemoveWafer(int index)
    {
        var lotRemoved = _waferGroup[index].Lot;
        if (_waferGroup.Count > 0)
        {
            _waferGroup.RemoveAll(i => i.Lot == lotRemoved);
        }
        RefreshWaferButtons();
        if (_waferGroup.Count <= 0)
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
        var lotList = new HashSet<int>();
        foreach (var wafer in _waferGroup)
        {
            lotList.Add(wafer.Lot);
        }
        return (byte) lotList.Count;
    }

    public void AddSelectedStorages()
    {
        var selectSystem = SelectSystem.GetComponent<ISelectSystem<IStorageIcon>>();
        foreach (var storageIcon in selectSystem.ListSelected())
        {
            storageIcon.AddToUserInterface(Constants.UserInterface.Lot);
            storageIcon.ResetState();
        }
        selectSystem.Clear();
        DisplayTool(0);
    }

    private void DisplayLot(WaferObject selectedWafer)
    {
        // Wafer List
        var lotList = new List<WaferObject>();
        var waferIndex = 0;
        _currentLot = selectedWafer.Lot;
        for (var i = 0; i < _waferGroup.Count; i++)
        {
            if (_waferGroup[i].Lot == selectedWafer.Lot)
            {
                waferIndex = i;
                lotList.Add(_waferGroup[i]);
            }
        }
        ToolAggregator(lotList);
        _waferIndex = waferIndex;

        // Refresh Tool Button List
        if (_toolButtonGroup.Count > 0)
        {
            foreach (var button in _toolButtonGroup)
            {
                Destroy(button);
            }
        }
        _toolButtonGroup.Clear();
        foreach (var tool in _toolInfo)
        {
            var newButton = Instantiate(BaseLotReferenceButton);
            newButton.InitializeLotButtonReference(this, tool.Key);
            newButton.transform.SetParent(ToolButtonGroup.transform);
            newButton.GetComponentInChildren<Text>().text = tool.Key;
            _toolButtonGroup.Add(newButton);
        }
        DisplayTool(0);
        
    }

    private void DisplayLot(int lotIndex)
    {
        DisplayLot(_waferGroup[WaferIndex(lotIndex)]);
    }

    public void DisplayTool(bool forward)
    {
        if (forward)
        {
            _toolIndex++;
            if (_toolIndex >= _orderedToolList.Count)
            {
                _toolIndex = 0;
            }
        }
        else
        {
            _toolIndex--;
            if (_toolIndex < 0)
            {
                _toolIndex = _orderedToolList.Count - 1;
            }
        }
        DisplayTool(_toolIndex);
    }

    public void DisplayTool(int index)
    {
        _toolIndex = index;
        DisplayTool(_orderedToolList[index]);
    }

    public void DisplayTool(string tool)
    {
        var sliderCounter = 0;
        var positionList = new List<string>();
        _currentTool = tool;
        _toolIndex = _orderedToolList.IndexOf(tool);
        foreach (var yield in _toolInfo[tool])
        {
            YieldBars[sliderCounter].transform.parent.gameObject.SetActive(true);
            YieldBars[sliderCounter].value = yield.Value;
            PositionTexts[sliderCounter].text = yield.Key.ToString("D2");
            positionList.Add(string.Format(" {0:D2}: {1} {2:P}", yield.Key, _positionInfo[tool][yield.Key], yield.Value));
            sliderCounter++;
        }
        if (YieldBars.Length > _toolInfo[tool].Count)
        {
            for (var i = sliderCounter; i < YieldBars.Length; i++)
            {
                YieldBars[i].transform.parent.gameObject.SetActive(false);
            }
        }

        ToolId.text = string.Format("{0:D7} : {1}", _currentLot, _currentTool);
        ToolCount.text = string.Format("Lot Tools\nCount: {0}", _orderedToolList.Count);
        PositionCount.text = string.Format("Details\nPosition Count: {0}", sliderCounter);
        PositionLog.text = string.Join("\n", positionList.ToArray());

    }

    private void Reset()
    {
        DisplayButton.interactable = false;
        CanvasEnabled = false;
    }

    private void ToolAggregator(List<WaferObject> lotList)
    {
        // Reset Tool List
        _toolInfo.Clear();
        _positionInfo.Clear();
        _orderedToolList.Clear();

        foreach (var wafer in lotList)
        {
            foreach (var step in wafer.Flow)
            {
                if (!_toolInfo.ContainsKey(step.Tool))
                {
                    _toolInfo[step.Tool] = new SortedDictionary<byte, float>();
                    _positionInfo[step.Tool] = new Dictionary<byte, string>();
                    _orderedToolList.Add(step.Tool);
                }
                _toolInfo[step.Tool][step.Position] = (float)wafer.GoodDieCount / wafer.RealDieCount;
                _positionInfo[step.Tool][step.Position] = wafer.Id;
            }
        }
    }

    private void RefreshWaferButtons()
    {
        foreach (var button in _lotButtonGroup)
        {
            Destroy(button);
        }
        _lotButtonGroup.Clear();

        var prevLot = -1;
        foreach (var wafer in _waferGroup)
        {
            if (wafer.Lot == prevLot) continue;
            prevLot = wafer.Lot;
            var button = Instantiate(BaseReferenceButton);
            button.InitializeButtonReference(this, wafer);
            button.GetComponentInChildren<Text>().text = string.Format("{0:D7}", wafer.Lot);
            button.transform.SetParent(LotButtonGroup.transform);
            _lotButtonGroup.Add(button);
        }

        LotCount.text = string.Format("Inspected Lots\nCount: {0}", ViewCount());

    }

    // Complicated find because unity does not support
    // C# SortedSet
    private int WaferIndex(int lotIndex)
    {

        var counter = -1;
        var waferCounter = -1;
        var prevLot = -1;
        _waferGroup.Sort();
        foreach (var wafer in _waferGroup)
        {
            waferCounter++;
            if (wafer.Lot == prevLot) continue;
            prevLot = wafer.Lot;
            counter++;
            if (counter == lotIndex)
            {
                return waferCounter;
            }
        }

        // Lot not found
        return -1;
    }
}
