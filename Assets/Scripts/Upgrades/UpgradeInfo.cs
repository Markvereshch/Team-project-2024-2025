using System.Collections.Generic;

public class UpgradeInfo
{
    public int HullLevel
    {
        get => typeLevelPair[VehiclePart.Hull];
        set => typeLevelPair[VehiclePart.Hull] = value;
    }

    public int WheelsLevel
    {
        get => typeLevelPair[VehiclePart.Wheels];
        set => typeLevelPair[VehiclePart.Wheels] = value;
    }

    public int CarriageLevel
    {
        get => typeLevelPair[VehiclePart.Carriage];
        set => typeLevelPair[VehiclePart.Carriage] = value;
    }

    public int FuelTankLevel
    {
        get => typeLevelPair[VehiclePart.FuelTank];
        set => typeLevelPair[VehiclePart.FuelTank] = value;
    }

    public Dictionary<VehiclePart, int> typeLevelPair = new Dictionary<VehiclePart, int>();

    public UpgradeInfo(int hullLevel, int wheelsLevel, int carriageLevel, int fuelTankLevel)
    {
        HullLevel = hullLevel;
        WheelsLevel = wheelsLevel;
        CarriageLevel = carriageLevel;
        FuelTankLevel = fuelTankLevel;
    }

    public UpgradeInfo(UpgradeInfo other)
    {
        HullLevel = other.HullLevel;
        WheelsLevel = other.WheelsLevel;
        CarriageLevel = other.CarriageLevel;
        FuelTankLevel = other.FuelTankLevel;
    }
}

public enum VehiclePart
{
    Hull,
    Carriage,
    Wheels,
    FuelTank,
}