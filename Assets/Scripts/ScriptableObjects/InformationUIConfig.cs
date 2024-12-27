using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InformationUIConfig", menuName = "UIConfigs/InformationUIConfig", order = 0)]
public class InformationUIConfig : ScriptableObject
{
    [SerializeField] private List<ResourceType> resourceTypesWithIcons;
    [SerializeField] private List<Sprite> resourceIconsList;
    private Dictionary<ResourceType, Sprite> resourceIcons;

    public Sprite GetIconByResourceType(ResourceType resourceType)
    {
        if (resourceIcons == null)
            Initialize();

        if (resourceIcons.TryGetValue(resourceType, out var sprite))
            return sprite;
        return null;
    }

    private void Initialize()
    {
        resourceIcons = new Dictionary<ResourceType, Sprite>();

        int min = Mathf.Min(resourceTypesWithIcons.Count, resourceIconsList.Count);
        for (int i = 0; i < min; i++)
        {
            resourceIcons.TryAdd(resourceTypesWithIcons[i], resourceIconsList[i]);
        }
    }
}
