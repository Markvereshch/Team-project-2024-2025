using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Resources info")]
    [SerializeField] private TMP_Text scrap;
    [SerializeField] private TMP_Text wood;
    [SerializeField] private TMP_Text coins;
    [SerializeField] private TMP_Text electronic;
    [SerializeField] private TMP_Text gasoline;

    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject upgradesPanel;
    [SerializeField] private GameObject vehiclesPanel;
    [SerializeField] private GameObject upgradeInfoPanel;

    public void Awake()
    {
        GetComponent<HangarManager>().OnResourcesChanged.AddListener(RefreshResourcesUI);
        ShowStartPanel();
    }

    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        upgradesPanel.SetActive(false);
        vehiclesPanel.SetActive(false);
        upgradeInfoPanel.SetActive(false);
    }

    public void RefreshResourcesUI(ResourcesData resources)
    {
        scrap.text = resources.Scrap.ToString();
        wood.text = resources.Wood.ToString();
        coins.text = resources.Coins.ToString();
        gasoline.text = resources.Gasoline.ToString();
        electronic.text = resources.Electronic.ToString();
    }

    private void OnDestroy()
    {
        GetComponent<HangarManager>().OnResourcesChanged.RemoveListener(RefreshResourcesUI);
    }
}
