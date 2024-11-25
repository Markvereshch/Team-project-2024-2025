using System.Collections.Generic;
using UnityEngine;

public class UpgradeVisualManager : MonoBehaviour
{
    [Header("Models to be activated")]
    [SerializeField] private List<GameObject> hullUpgradeVisuals = new List<GameObject>();
    [SerializeField] private List<GameObject> wheelUpgradesVisuals = new List<GameObject>();
    [SerializeField] private List<GameObject> carriegeUpgradesVisuals = new List<GameObject>();

    public void ShowVisuals(UpgradeInfo upgrades)
    {
        SetVisuals(hullUpgradeVisuals, upgrades.HullLevel);
        SetVisuals(wheelUpgradesVisuals, upgrades.WheelsLevel);
        SetVisuals(carriegeUpgradesVisuals, upgrades.CarriageLevel);
    }

    private void SetVisuals(List<GameObject> visuals, uint level)
    {
        for (int i = 0; i < visuals.Count; i++)
        {
            visuals[i].SetActive(i < level);
        }
    }
}
