using UnityEngine;
using UnityEngine.UI;
using DatabaseLayer.Services;
using TMPro;
using DatabaseLayer.Repositories;
using DatabaseLayer;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

public class SaveUI : MonoBehaviour
{
    private SaveInfo saveInfo;
    public TextMeshProUGUI Savename;
    public TextMeshProUGUI Clubname;
    public TextMeshProUGUI Currentrole;
    public TextMeshProUGUI Dateofcreation;
    public TextMeshProUGUI Recentactivity;
    public SpriteRenderer clubSpriteRenderer;
    public Button[] SaveSlots;
    public Button[] DeleteSlots;
    public Button[] AddSave;
    public Button StartSave;
    public static string SaveNameSlot;
    void Start()
    {
       
        LoadGameManager loadGameManager = LoadGameManager.GetInstance();
        int length = loadGameManager.GetAllSaves().Count;
       
        for (int i = 0; i < length; i++)
        {
            Transform childObject = SaveSlots[i].transform.Find("ActiveFields");
            childObject.gameObject.SetActive(true);
            childObject = SaveSlots[i].transform.Find("NewSlotButton");
            childObject.gameObject.SetActive(false);
            saveInfo = loadGameManager.GetAllSaves()[i];
            TextMeshProUGUI SaveText = SaveSlots[i].transform.Find("ActiveFields/SavenameText").GetComponent<TextMeshProUGUI>();
            SaveText.text = saveInfo.SaveName;
            SaveText = SaveSlots[i].transform.Find("ActiveFields/SaveCreationDate").GetComponent<TextMeshProUGUI>();
            SaveText.text = saveInfo.PlayerData.RealDate;

        }
        
        

        SaveSlots[0].onClick.AddListener(() => SaveWindow(0));
        SaveSlots[1].onClick.AddListener(() => SaveWindow(1));
        SaveSlots[2].onClick.AddListener(() => SaveWindow(2));
        SaveSlots[3].onClick.AddListener(() => SaveWindow(3));
        SaveSlots[4].onClick.AddListener(() => SaveWindow(4));
        SaveSlots[5].onClick.AddListener(() => SaveWindow(5));
        SaveSlots[6].onClick.AddListener(() => SaveWindow(6));
        SaveSlots[7].onClick.AddListener(() => SaveWindow(7));
        SaveSlots[8].onClick.AddListener(() => SaveWindow(8));
        SaveSlots[9].onClick.AddListener(() => SaveWindow(9));

        DeleteSlots[0].onClick.AddListener(() => DeleteSave(0));
        DeleteSlots[1].onClick.AddListener(() => DeleteSave(1));
        DeleteSlots[2].onClick.AddListener(() => DeleteSave(2));
        DeleteSlots[3].onClick.AddListener(() => DeleteSave(3));
        DeleteSlots[4].onClick.AddListener(() => DeleteSave(4));
        DeleteSlots[5].onClick.AddListener(() => DeleteSave(5));
        DeleteSlots[6].onClick.AddListener(() => DeleteSave(6));
        DeleteSlots[7].onClick.AddListener(() => DeleteSave(7));
        DeleteSlots[8].onClick.AddListener(() => DeleteSave(8));
        DeleteSlots[9].onClick.AddListener(() => DeleteSave(9));

        AddSave[0].onClick.AddListener(() => NewGame());
        AddSave[1].onClick.AddListener(() => NewGame());
        AddSave[2].onClick.AddListener(() => NewGame());
        AddSave[3].onClick.AddListener(() => NewGame());
        AddSave[4].onClick.AddListener(() => NewGame());
        AddSave[5].onClick.AddListener(() => NewGame());
        AddSave[6].onClick.AddListener(() => NewGame());
        AddSave[7].onClick.AddListener(() => NewGame());
        AddSave[8].onClick.AddListener(() => NewGame());
        AddSave[9].onClick.AddListener(() => NewGame());

    }

    void SaveWindow(int i)
    {
        LoadGameManager loadGameManager = LoadGameManager.GetInstance();
        saveInfo = loadGameManager.GetAllSaves()[i];
        Savename.text = saveInfo.SaveName;
        TeamRepository teamRepository = new TeamRepository();
        List<Team> teams = teamRepository.Retrieve();
        string targetID = saveInfo.PlayerData.ClubId;
        string targetClubName = null;
        foreach (Team team in teams)
        {
            if (team.Id == targetID)
            {
                targetClubName = team.ExtName;
                break;
            }
            
        }
        int role = ((int)saveInfo.PlayerData.Role);
        switch (role)
        {
            case 0:
                Currentrole.text = "Scout";
                break;
            case 1:
                Currentrole.text = "Manager";
                break;
            case 2:
                Currentrole.text = "Director";
                break;
        }
        Clubname.text = targetClubName;

        string[] files = Directory.GetFiles("Assets/Resources/FCLogos/", targetClubName + ".png", SearchOption.AllDirectories);
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
        else
        {
            Debug.LogError("������� ��� ����������� ����: " + targetClubName);
        }
        Recentactivity.text = saveInfo.PlayerData.GameDate;
        Dateofcreation.text = saveInfo.PlayerData.RealDate;

        StartSave.onClick.AddListener(() => LoadSaveScene(i));


    }

    public void NewGame()
    {
        SceneManager.LoadScene("NewGameScene");
    }
    void DeleteSave(int i)
    {
        LoadGameManager loadGameManager = LoadGameManager.GetInstance();
        saveInfo = loadGameManager.GetAllSaves()[i];
        loadGameManager.Delete(saveInfo.SaveName);
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
        
    }

    void LoadSaveScene(int i)
    {
        LoadGameManager loadGameManager = LoadGameManager.GetInstance();
        saveInfo = loadGameManager.GetAllSaves()[i];
        SaveNameSlot = saveInfo.SaveName;
        int index = ((int)saveInfo.PlayerData.Role);
       switch (index)
        {
            case 0:
            SceneManager.LoadScene("ScoutScene");
                break;
            case 1:
                SceneManager.LoadScene("ManagerScene");
                break;
            case 2:
                SceneManager.LoadScene("DirectorScene");
                break;
        }
     
    }
}



