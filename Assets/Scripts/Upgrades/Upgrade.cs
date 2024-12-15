using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "UpgradeConfig", order = 2)]
public class Upgrade : ScriptableObject
{
    public List<UpgradeTypeValuePair> upgradeDetails = new List<UpgradeTypeValuePair>();
    public ResourcesData upgradeCost;
}

[Serializable]
public struct UpgradeTypeValuePair
{
    public UpgradeType type;
    public int value;
}

public enum UpgradeType
{
    HealthPoints,
    FuelPoints,
    CarriageCapacity,
    SteeringAngle,
    BrakeSensitivity,
    Acceleration,
}
