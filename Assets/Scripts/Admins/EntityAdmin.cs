using UnityEngine;

/// <summary>
/// Instantiates entities for the game
/// </summary>
/// <remarks>
/// Highly dependent on other non-base classes.
/// </remarks>
public class EntityAdmin : MonoBehaviour
{

    public static EntityAdmin Singleton;

    public GameObject Foup;
    public GameObject TargetWaferDisplay;
    public GameObject TargetLotDisplay;
    public GameObject WaferAdmin;
    public GameObject SelectAdmin;

    private int _nextEntity;

    // Test ints
    private int _start = 1;

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
        // Initializations
        _nextEntity = 0;
    }

    public IWaferStorage CreateFoup()
    {
        var foup = Instantiate(Foup);
        _nextEntity += 1;
        foup.InitializeFoup(TargetWaferDisplay.GetComponent<IWaferUserInterface>(), 
            TargetLotDisplay.GetComponent<IWaferUserInterface>(),
            SelectAdmin.GetComponent<ISelectSystem<IStorageIcon>>(), _nextEntity);

        return foup.GetComponent<IWaferStorage>();
    }

    public void DummyCreateFoup()
    {
        var foup = CreateFoup();

        // Test Code
        WaferAdmin.GetComponent<IWaferTracker>().CreateLot(Constants.FoupSize, 60, foup);
        var foupWaferList = foup.WaferList();
        // Simulate Machine Process
        for (var x = _start; x <= _start + 34; x++)
        {
            if ((x - 1) % 5 == 0)
            {
                foup.Randomize();
            }
            for (var y = 0; y < foupWaferList.Count; y++)
            {
                foupWaferList[y].Flow.Add(new WaferStep(x.ToString("D2"), (byte)(y + 1)));
            }
        }
        _start += 35;
        foup.Watch();

    }
}
