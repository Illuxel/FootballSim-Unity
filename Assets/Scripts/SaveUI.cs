using System.IO;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using DatabaseLayer;
using DatabaseLayer.Repositories;
using BusinessLogicLayer.Services;
using DatabaseLayer.Enums;

public class SaveUI : MonoBehaviour
{
    private SaveInfo saveInfo;
    public TextMeshProUGUI Savename, Clubname, Currentrole, Dateofcreation, Recentactivity;
    public SpriteRenderer clubSpriteRenderer;
    public Button[] SaveSlots;
    public Button[] DeleteSlots;
    public Button[] AddSave;
    public Button StartSave;

    public static string SaveNameSlot;

    void Start()
    {
        var savesManager = SavesManager.GetInstance();
        var saves = savesManager.GetAllSaves();
       
        for (int i = 0; i < saves.Count; i++)
        {
            var childObject = SaveSlots[i].transform.Find("ActiveFields");
            childObject.gameObject.SetActive(true);
            childObject = SaveSlots[i].transform.Find("NewSlotButton");
            childObject.gameObject.SetActive(false);

            saveInfo = saves[i];

            var saveText = SaveSlots[i].transform.Find("ActiveFields/SavenameText").GetComponent<TextMeshProUGUI>();
            saveText.text = saveInfo.SaveName;
            saveText.text = saveInfo.SaveDate.ToString();
            saveText = SaveSlots[i].transform.Find("ActiveFields/SaveCreationDate").GetComponent<TextMeshProUGUI>();

            SaveSlots[i].onClick.AddListener(() => SaveWindow(i));
            DeleteSlots[i].onClick.AddListener(() => DeleteSave(i));
            AddSave[i].onClick.AddListener(() => NewGame());
        }
    }

    void SaveWindow(int i)
    {
        var savesManager = SavesManager.GetInstance();

        saveInfo = savesManager.GetAllSaves()[i];
        Savename.text = saveInfo.SaveName;

        var team = new TeamRepository().Retrieve(saveInfo.UserGameData.ClubId);

        Clubname.text = team.ExtName;

        switch (saveInfo.UserGameData.Role)
        {
            case UserRole.Scout:
                Currentrole.text = "Scout";
                break;
            case UserRole.Manager:
                Currentrole.text = "Manager";
                break;
            case UserRole.Director:
                Currentrole.text = "Director";
                break;
        }

        string[] files = Directory.GetFiles("Assets/Resources/FCLogos/", team.ExtName + ".png", SearchOption.AllDirectories);
        string imagePath = files[0];
        string path = imagePath;
        path = path.Replace(".png", "");
        string keyword = "FCLogos";
        int index = path.IndexOf(keyword);
        string result = path.Substring(index);
        Sprite clubSprite = Resources.Load<Sprite>(result);

        if (clubSprite != null)
        {
         
            clubSpriteRenderer.sprite = clubSprite;
        }

        Recentactivity.text = saveInfo.UserGameData.GameDate;
        Dateofcreation.text = saveInfo.SaveDate.ToString();

        StartSave.onClick.AddListener(() => LoadSaveScene(i));
    }

    public void NewGame()
    {
        SceneManager.LoadScene("NewGameScene");
    }
    void DeleteSave(int i)
    {
        var savesManager = SavesManager.GetInstance();
        saveInfo = savesManager.GetAllSaves()[i];
        savesManager.Delete(saveInfo.SaveName);

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    void LoadSaveScene(int i)
    {
        var savesManager = SavesManager.GetInstance();
        saveInfo = savesManager.GetAllSaves()[i];
        SaveNameSlot = saveInfo.SaveName;

        switch (saveInfo.UserGameData.Role)
        {
            case UserRole.Scout:
                Currentrole.text = "Scout";
                break;
            case UserRole.Manager:
                Currentrole.text = "Manager";
                break;
            case UserRole.Director:
                Currentrole.text = "Director";
                break;
        }
    }
}



