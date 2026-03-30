using UnityEngine;
using UnityEngine.RSUVBitPacker;

[RequireComponent(typeof(RSUVPropertyPacker))]
public class SetRendererProperty : MonoBehaviour
{
    RSUVPropertyPacker propertyPacker;
    int overrideId, colorId;

    private void Awake()
    {
        propertyPacker = GetComponent<RSUVPropertyPacker>();
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
