using UnityEngine;

public class MaintainTextOrientation : MonoBehaviour
{
    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void LateUpdate()
    {
        if (transform.parent != null)
        {
            float parentScaleX = transform.parent.lossyScale.x;
            Vector3 newScale = initialScale;

            // Kompens�r for for�lderens skala for at undg� spejlvending
            newScale.x = initialScale.x * Mathf.Sign(parentScaleX);
            transform.localScale = newScale;
        }
        else
        {
            transform.localScale = initialScale;
        }

        // Nulstil rotationen for at forhindre u�nsket rotation
        transform.rotation = Quaternion.identity;
    }
}
