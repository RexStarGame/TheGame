using UnityEngine;

public class MissTextFollower : MonoBehaviour
{
    public Transform enemyTransform;
    public Vector3 offset;

    void Update()
    {
        if (enemyTransform != null)
        {
            Vector3 worldPosition = enemyTransform.position + offset;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
            transform.position = screenPosition;
        }
    }
}
