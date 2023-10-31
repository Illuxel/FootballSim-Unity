using UnityEngine;
using UnityEngine.UI;

public class DisableObjectOnClick : MonoBehaviour
{
    private Camera mainCamera;
    private RectTransform objectRectTransform;

    private void Start()
    {
        mainCamera = Camera.main;
        objectRectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 localMousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(objectRectTransform, Input.mousePosition, mainCamera, out localMousePosition);

            if (!objectRectTransform.rect.Contains(localMousePosition))
            {
                gameObject.SetActive(false);
            }
        }
    }
    public void DisableObject()
    {
        gameObject.SetActive(false);
    }
}








