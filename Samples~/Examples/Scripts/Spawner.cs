using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject item;

    [SerializeField]
    private int number = 500;

    [SerializeField]
    private float radius = 50f;

    private void Start()
    {
        for (int i = 0; i < number; i++)
        {
            var pos = transform.TransformPoint(Random.insideUnitSphere * radius);
            GameObject.Instantiate(item, pos, Quaternion.identity, transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Vector3.zero, radius);
    }
}
