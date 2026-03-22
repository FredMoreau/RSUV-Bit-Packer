using UnityEngine;
using UnityEngine.RSUVBitPacker;

[RequireComponent(typeof(RSUVPropertyPacker))]
public class RandomiizeProperty : MonoBehaviour
{
    RSUVPropertyPacker propertyPacker;
    int propertyId, colorId;

    private void Awake()
    {
        propertyPacker = GetComponent<RSUVPropertyPacker>();
        propertyId = propertyPacker.GetPropertyId("Override Color");
        colorId = propertyPacker.GetPropertyId("Color");
    }

    private void Start()
    {
        var color = Color.HSVToRGB(Random.Range(0f, 1f), Random.Range(.5f, 1f), Random.Range(.8f, 1f));
        propertyPacker.TrySetValue(propertyId, true);
        propertyPacker.TrySetValue(colorId, color);
    }
}
