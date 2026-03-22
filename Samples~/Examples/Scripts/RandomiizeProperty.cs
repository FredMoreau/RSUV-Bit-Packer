using UnityEngine;
using UnityEngine.RSUVBitPacker;

[RequireComponent(typeof(RSUVPropertyPacker))]
public class RandomiizeProperty : MonoBehaviour
{
    [SerializeField] AnimationCurve emissionCoolDown = new AnimationCurve(new Keyframe[2] { new Keyframe(0,1,0,0), new Keyframe(1,0,0,0) });
    RSUVPropertyPacker propertyPacker;
    int emissionId, colorId;
    float emissionCurvePos = 1;

    private void Awake()
    {
        propertyPacker = GetComponent<RSUVPropertyPacker>();
        colorId = propertyPacker.GetPropertyIndex("Color Gradient");
        emissionId = propertyPacker.GetPropertyIndex("Emission");
    }

    private void Start()
    {
        //var color = Color.HSVToRGB(Random.Range(0f, 1f), Random.Range(.5f, 1f), Random.Range(.8f, 1f));
        //propertyPacker.TrySetValue(colorId, color);
        propertyPacker.TrySetValue(colorId, Random.Range(0f, 1f));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude < 2f)
            return;
        emissionCurvePos = 0f;
        propertyPacker.TrySetValue(emissionId, emissionCoolDown.Evaluate(emissionCurvePos));
    }

    private void Update()
    {
        if (emissionCurvePos > 1f)
            return;
        emissionCurvePos += Time.deltaTime * 0.5f;
        propertyPacker.TrySetValue(emissionId, emissionCoolDown.Evaluate(emissionCurvePos));
    }
}
