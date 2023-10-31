using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MenuManager;


public class ShowCongratulationWindow : MonoBehaviour
{
    public GameObject congratulationWindow, Button;
   
    private void Start()
    {       
        if (MenuManager.Show == true)
        {
            congratulationWindow.SetActive(true);
            Button.SetActive(true);
        }
        else
        {
            congratulationWindow.SetActive(false);
            Button.SetActive(false);
        }
    }
}

