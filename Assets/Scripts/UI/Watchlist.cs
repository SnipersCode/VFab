using UnityEngine;

public class Watchlist : MonoBehaviour
{

    public GameObject SelectSystem;

    // ReSharper disable once UnusedMember.Local
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void DeleteSelected()
    {
        var totalChildren = gameObject.transform.childCount;
        var totalDeletions = 0;
        var selectSystem = SelectSystem.GetComponent<ISelectSystem<IStorageIcon>>();
        foreach (Transform child in transform)
        {
            var childIcon = child.GetComponent<IStorageIcon>();
            if (!selectSystem.IsSelected(childIcon)) continue;
            selectSystem.Deselect(childIcon);
            Destroy(child.gameObject);
            totalDeletions += 1;
        }

        if (totalChildren - totalDeletions <= 0)
        {
            gameObject.SetActive(false);
        }
    }

}
