using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MenuManager;


public class ShowCongratulationWindow : MonoBehaviour
{
    public GameObject congratulationWindow;
    private void Start()
    {
        //Debug.Log(MenuManager.Show);
        //Debug.Log("1");
        if (MenuManager.Show == true)
        {
            congratulationWindow.SetActive(true);
        }
        else
        {
            congratulationWindow.SetActive(false);
        }
    }
}

