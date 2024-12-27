using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    [Header("Map settings")]
    [SerializeField] Vector2 mapMarkerSize;
    [SerializeField] Transform mapTransform;
    [SerializeField] Vector2 uiMapSize;
    [SerializeField] Vector2 locationSize;

    private Dictionary<MinimapObject, Image> markers = new();

    public void LoadMap()
    {
        var minimapObjects = FindObjectsOfType<MinimapObject>().Where(obj => obj.DisplayOnFullMap == true).ToArray();

        foreach (var key in new List<MinimapObject>(markers.Keys))
        {
            if (Array.IndexOf(minimapObjects, key) == -1)
            {
                Destroy(markers[key].gameObject);
                markers.Remove(key);
            }
        }

        foreach (MinimapObject obj in minimapObjects)
        {
            if (!markers.ContainsKey(obj))
            {
                Image marker = CreateMarker(obj);
                markers[obj] = marker;
            }

            UpdateMarkerPosition(markers[obj], obj);
        }
    }

    private Image CreateMarker(MinimapObject obj)
    {
        Image marker = new GameObject("Marker").AddComponent<Image>();
        marker.transform.SetParent(mapTransform, false);
        marker.sprite = obj.Sprite;
        marker.color = obj.Color;
        marker.rectTransform.sizeDelta = mapMarkerSize;

        UpdateRotation(marker,obj);

        return marker;
    }

    private void UpdateMarkerPosition(Image marker, MinimapObject obj)
    {
        Vector2 mapPosition = WorldToMapPosition(obj.transform.position);
        marker.rectTransform.anchoredPosition = mapPosition;

        UpdateRotation(marker, obj);
    }

    private void UpdateRotation(Image marker, MinimapObject obj) 
    {
        if (!obj.IsFreezed)
        {
            Transform carTransform = obj.transform.parent;
            if (carTransform == null)
                return;

            float carRotationY = carTransform.eulerAngles.y;
            marker.transform.rotation = Quaternion.Euler(0, 0, -carRotationY);
        }
    }

    private Vector2 WorldToMapPosition(Vector3 worldPosition)
    {
        return new Vector2(
            (worldPosition.x / locationSize.x) * uiMapSize.x,
            (worldPosition.z / locationSize.y) * uiMapSize.y
        );
    }
}
