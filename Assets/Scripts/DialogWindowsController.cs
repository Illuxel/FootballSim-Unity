using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogWindowsController : MonoBehaviour
{
    public Button CloseButton, OkButton;

    private void Start()
    {
        
        if (CloseButton != null && OkButton != null)
        {
            CloseButton.onClick.AddListener(ClosePrefab);
            OkButton.onClick.AddListener(ClosePrefab);
        }
    } 
    public void ClosePrefab()
    {
        Destroy(gameObject);
    }
}
