using System.Collections.Generic;
using UnityEngine;

public class ResourcePickUpScript : MonoBehaviour
{
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private int resourcesToAdd = 5;
    [SerializeField] private List<AudioClip> pickUpSound = new List<AudioClip>();
    private AudioSource mainCameraAudio;

    private void Start()
    {
        mainCameraAudio = Camera.main.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<PlayerInputController>();
        var health = other.GetComponentInParent<EntityHealth>();
        if (player && !health.IsDead)
        {
            var resourceManager = player.gameObject.GetComponent<ResourceManager>();
            resourceManager.ChangeResourceAmount(resourcesToAdd, resourceType);
            Debug.Log($"CurrentAmountOf {resourceType}: {resourceManager.GetResourceAmount(resourceType)}");
            if(pickUpSound.Count > 0)
            {
                int index = Random.Range(0, pickUpSound.Count);
                mainCameraAudio.PlayOneShot(pickUpSound[index]);
            }
            Destroy(gameObject);
        }
    }

    public void SetAmountToAdd(int amountToAdd)
    {
        resourcesToAdd = amountToAdd;
    }
}
