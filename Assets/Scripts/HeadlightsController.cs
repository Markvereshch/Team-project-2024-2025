using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeadlightsController : MonoBehaviour
{
    private List<Light> headlights = new List<Light>();

    private void Start()
    {
        headlights = GetComponentsInChildren<Light>().ToList();
        foreach (Light light in headlights)
        {
            light.enabled = false;
        }
    }

    public void ToggleAllLights(bool isOn)
    {
        foreach (var light in headlights)
        {
            light.enabled = isOn;
        }
    }
}
