using System.Collections.Generic;

public class UpgradeInfo
{
    public uint HullLevel
    {
        get => typeLevelPair[VehiclePart.Hull];
        set => typeLevelPair[VehiclePart.Hull] = value;
    }

    public uint WheelsLevel
    {
        get => typeLevelPair[VehiclePart.Wheels];
        set => typeLevelPair[VehiclePart.Wheels] = value;
    }

    public uint CarriageLevel
    {
        get => typeLevelPair[VehiclePart.Carriage];
        set => typeLevelPair[VehiclePart.Carriage] = value;
    }

    public uint FuelTankLevel
    {
        get => typeLevelPair[VehiclePart.FuelTank];
        set => typeLevelPair[VehiclePart.FuelTank] = value;
    }

    public Dictionary<VehiclePart, uint> typeLevelPair = new Dictionary<VehiclePart, uint>();

    public UpgradeInfo(uint hullLevel, uint wheelsLevel, uint carriageLevel, uint fuelTankLevel)
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