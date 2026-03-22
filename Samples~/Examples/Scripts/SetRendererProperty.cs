using UnityEngine;
using UnityEngine.RSUVBitPacker;

[RequireComponent(typeof(RSUVPropertyPacker))]
public class SetRendererProperty : MonoBehaviour
{
    RSUVPropertyPacker propertyPacker;
    int propertyId;

    private void Awake()
    {
        propertyPacker = GetComponent<RSUVPropertyPacker>();
        propertyId = propertyPacker.GetPropertyIndex("Override Color");
    }

    private void OnMouseEnter()
    {
        //propertyPacker.TrySetValue("Override Color", true);
        propertyPacker.TrySetValue(propertyId, true);
    }

    private void OnMouseExit()
    {
        //propertyPacker.TrySetValue("Override Color", false);
        propertyPacker.TrySetValue(propertyId, false);
    }
}
