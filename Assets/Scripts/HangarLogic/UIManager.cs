using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scrap;
    [SerializeField] private TMP_Text wood;
    [SerializeField] private TMP_Text coins;
    [SerializeField] private TMP_Text electronic;
    [SerializeField] private TMP_Text gasoline;

    public void RefreshResourcesUI(ResourcesData resources)
    {
        scrap.text = resources.Scrap.ToString();
        wood.text = resources.Wood.ToString();
        coins.text = resources.Coins.ToString();
        gasoline.text = resources.Gasoline.ToString();
        electronic.text = resources.Electronic.ToString();
    }

    private void Awake()
    {
        GetComponent<HangarManager>().OnResourcesChanged.AddListener(RefreshResourcesUI);
    }

    private void OnDestroy()
    {
        GetComponent<HangarManager>().OnResourcesChanged.RemoveListener(RefreshResourcesUI);
    }
}
