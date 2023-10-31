using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleSpriteRenderer : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float scaleRatio = 0.5f; // Відношення для зменшення масштабу

    private void Start()
    {
        AdjustScale();
    }

    private void Update()
    {
        AdjustScale();
    }

    private void AdjustScale()
    {
        float screenRatio = (float)Screen.width / Screen.height; // Відношення сторін екрану
        float i = 1.41f;
        Debug.LogWarning(screenRatio);
        if (screenRatio < i)
        {
            spriteRenderer.transform.localScale = Vector3.one * scaleRatio;
        }
        else
        {
            Quaternion newRotation = Quaternion.Euler(0f, 0f, 0f);
            spriteRenderer.transform.rotation = newRotation;

        }
    }
}
