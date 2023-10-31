using DatabaseLayer.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using System.IO;
using UnityEngine.UI;
using DatabaseLayer.Repositories;
using DatabaseLayer;
using System.Collections.Generic;
using BusinessLogicLayer.Services;
using DatabaseLayer.Enums;
using DatabaseLayer.Model;
using JetBrains.Annotations;

public class AcademyPreview : MonoBehaviour
{
    public JuniorTeam juniorTeam;
    public Player player;
    public TextMeshProUGUI Lock2, Name, Position, Age, Rating, AverageratingText, Averagerating;
    public Image Lock3, Lock3Bak;
    // Start is called before the first frame update
    void Start()
    {
        
        var loadGamemanager = LoadGameManager.GetInstance();
        SaveInfo saveInfo = loadGamemanager.Load(SaveUI.SaveNameSlot);
        PlayerGameData playerDataResult = saveInfo.PlayerData;
        int i = (int)playerDataResult.CurrentLevel;
        if (i == 0 ) 
        {
         Lock2.gameObject.SetActive(true);
         Lock3.gameObject.SetActive(true);
         Lock3Bak.gameObject.SetActive(true);
         AverageratingText.gameObject.SetActive(false);
        }
        else if (i == 1) 
        {
            Lock2.gameObject.SetActive(false);
            Lock3.gameObject.SetActive(true);
            Lock3Bak.gameObject.SetActive(true);
            AverageratingText.gameObject.SetActive(true);
        }
        else if (i == 2)
        {
            Lock2.gameObject.SetActive(false);
            Lock3.gameObject.SetActive(false);
            Lock3Bak.gameObject.SetActive(false);
            AverageratingText.gameObject.SetActive(true);
        }
        

        JuniorPreviewTeamGetter juniorPreviewTeamGetter = new JuniorPreviewTeamGetter();
        juniorTeam = juniorPreviewTeamGetter.Get("015834FD9556AAEC44DE54CDE350235B", ref playerDataResult);
        Averagerating.text = juniorTeam.AverageTeamRating.ToString();
        JuniorFinder juniorFinder = new JuniorFinder();
        player = juniorFinder.BestJuniorPlayerByTeam("015834FD9556AAEC44DE54CDE350235B");
        Name.text = player.Person.Name + " " + player.Person.Surname;
        Position.text = player.Position.ToString();
        Rating.text = player.Rating.ToString();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
