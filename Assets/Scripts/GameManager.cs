using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject Player
    { 
        get
        {
            return player;
        }
        set 
        {
            player = value;
            vehicleHealth = player.GetComponent<VehicleHealth>();
            resourceManager = player.GetComponent<ResourceManager>();
            vehicleHealth.OnDie += HandleDefeat;
            ObjectiveManager.Instance.Player = player;
            InGameUIManager.Instance.Player = player;
        } 
    }
    private GameObject player;
    private ResourceManager resourceManager;
    private VehicleHealth vehicleHealth;

    private void HandleDefeat()
    {
        Console.WriteLine("You've lost!");
    }

    public void HandleEvacuation()
    {
        SaveCollectedResources();
        SceneManager.LoadScene("Hangar");
    }

    private void SaveCollectedResources()
    {
        var saveData = GameSaver.Load();
        var resources = saveData.resourcesSaveData;
        resourceManager.PrepareSaveData(resources);
        GameSaver.Save(saveData);
    }

    private IEnumerator DefeatCoroutine()
    {
        yield return new WaitForSeconds(5f);
    }
}
