using System.Collections.Generic;
using UnityEngine;

public class InformationList : MonoBehaviour
{
    [SerializeField] int poolCount = 10;
    [SerializeField] InformationTab informationTabPrefab;
    [SerializeField] Transform informationList;
    private List<InformationTab> infoTabs = new List<InformationTab>();
    private int busyCount = 0;
    [SerializeField] private Color defaultColor = new Color(255f, 255f, 255f, 30f);

    private void Start()
    {
        for (int i = 0; i < poolCount; i++)
        {
            var info = Instantiate(informationTabPrefab, informationList);
            info.gameObject.SetActive(false);
            infoTabs.Add(info);
        }
    }

    public void AddInformation(string text, Sprite sprite = null)
    {
        AddInformation(text, defaultColor, sprite);
    }

    public void AddInformation(string text, Color backgroundColor, Sprite sprite = null)
    {
        InformationTab tabToActivate = infoTabs[busyCount % poolCount];
        tabToActivate.transform.SetAsLastSibling();

        tabToActivate.CreateInfoIcon(text, backgroundColor, sprite);
        busyCount++;
    }
}
