using UnityEngine;
using UnityEngine.RSUVBitPacker;

[RequireComponent(typeof(PropertyPacker))]
public class SetRendererProperty : MonoBehaviour
{
    PropertyPacker propertyPacker;
    int overrideId, colorId;

    private void Awake()
    {
        propertyPacker = GetComponent<PropertyPacker>();
        overrideId = propertyPacker.GetPropertyIndex("Override Color");
        colorId = propertyPacker.GetPropertyIndex("Color");
    }

    private void Start()
    {
        Color randomColor = Color.HSVToRGB(Random.Range(0f, 1f), 1, 1);
        propertyPacker.TrySetValue(colorId, randomColor);
    }

    private void OnMouseEnter()
    {
        propertyPacker.TrySetValue(overrideId, true);
    }

    private void OnMouseExit()
    {
        propertyPacker.TrySetValue(overrideId, false);
    }
}
