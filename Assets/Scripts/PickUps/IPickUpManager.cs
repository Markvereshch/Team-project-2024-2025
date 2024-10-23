using UnityEngine;

public interface IPickUpManager
{
    public GameObject Player
    {
        get;
        set;
    }

    void AddPickableItems(PickUpScript pickUp);

    void RemovePickableItems(PickUpScript pickUp);
}
