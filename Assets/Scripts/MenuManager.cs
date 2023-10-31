using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using DatabaseLayer;
using DatabaseLayer.Settings;
using DatabaseLayer.Enums;
using DatabaseLayer.Repositories;

using BusinessLogicLayer.Services;

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI TMP, Name, Money, SName, Date, Clubname;  
    public SpriteRenderer spriteRenderer, clubicon;
    public TMP_InputField SaveName, FirstName, LastName;
    public Dropdown leagueDropdown;
    public string BuffPlName, BuffPlSurname, BuffClub, BuffDate;
    public Button Level2, Level3, Level4;

    public static bool Show;

    private void Awake()
    {
        GameSettings.BaseGamePath = Application.persistentDataPath;
    }

    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        var teams = new TeamRepository().Retrieve();

        if (currentScene.buildIndex == 2)
        {
            var savesGamemanager = SavesManager.GetInstance();
            var saveInfo = savesGamemanager.Load(SaveUI.SaveNameSlot);

            var userData = saveInfo.UserGameData;

            Date.text = userData.GameDate;
            Name.text = userData.PlayerName + " " + userData.PlayerSurname;
            Money.text = Convert.ToString(userData.Money);

            var team = new TeamRepository().Retrieve(userData.ClubId);

            Clubname.text = team.ExtName;

            var files = Directory.GetFiles("Assets/Resources/FCLogos/", team.ExtName + ".png", SearchOption.AllDirectories);
            var imagePath = Path.GetFileNameWithoutExtension(files[0]);
            var clubSprite = Resources.Load<Sprite>(imagePath);

            if (clubSprite != null)
            {
                clubicon.sprite = clubSprite;
            }
           
            BuffPlName = userData.PlayerName;
            BuffPlSurname = userData.PlayerSurname;
            BuffClub = userData.ClubId;     
            BuffDate = userData.GameDate;

            var upgrader = new ScoutLevelUpgrader().Upgrade(userData.CurrentLevel, userData);

            switch (userData.CurrentLevel)
            {
            case ScoutSkillLevel.Level1:
                Level3.interactable = false;
                Level4.interactable = false;
                break;
            case ScoutSkillLevel.Level2:
                Level2.interactable = false;
                Level3.interactable = true;
                break;
            case ScoutSkillLevel.Level3:
                Level2.interactable = false;
                Level3.interactable = false;
                Level4.interactable = true;
                break;
            }

            Level2.onClick.AddListener(() => ScoutUpgrade());
            Level3.onClick.AddListener(() => ScoutUpgrade());
            Level4.onClick.AddListener(() => Transition());
        }     
    }

    public void AddMoney()
    {
        int money = Convert.ToInt32(Money.text);
        int addmoney = money + 100;
        Money.text = Convert.ToString(addmoney);
    }

    public void LoadGame()
    {
        var loadGamemanager = SavesManager.GetInstance();
        var saveInfo = loadGamemanager.Load(SaveUI.SaveNameSlot);

        switch (saveInfo.UserGameData.Role)
        {
            case UserRole.Scout:
                SceneManager.LoadScene("ScoutScene");
                break;
            case UserRole.Manager:
                SceneManager.LoadScene("ManagerScene");
                break;
            case UserRole.Director:
                SceneManager.LoadScene("DirectorScene");
                break;
            default:
                SceneManager.LoadScene("ScoutScene");
                break;
        }
    }
    public void SaveGame()
    {
        var savesManager = SavesManager.GetInstance();
        var saveInfo = new SaveInfo()
        {
            UserGameData = new UserGameData()
            {
                PlayerName = BuffPlName,
                PlayerSurname = BuffPlSurname,
                GameDate = BuffDate,
                ClubId = BuffClub,
                Money = Convert.ToDouble(Money.text),
            }
        };

        savesManager.Save(saveInfo);
    }
    public void NewGame()
    {
       SceneManager.LoadScene("NewGameScene");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void CreateNewGame()
    {
        var teams = new TeamRepository().Retrieve();

        string targetExtName = spriteRenderer.sprite.name;
        string targetId = null;

        foreach (Team team in teams)
        {
            if (team.ExtName == targetExtName)
            {
                targetId = team.Id;
                break;
            }
        }

        var saveName = string.Empty;
        if (SaveName != null)
        {
            saveName = SaveName.text;
        }
        string name = "player";
        if (FirstName != null)
        {
            name = FirstName.text;
        }
        string lname = "player";
        if (LastName != null)
        {
            lname = LastName.text;
        }

        var savesManager = SavesManager.GetInstance();
        var seasonValueCreator = new SeasonValueCreator();
        var startDate = seasonValueCreator.GetSeasonStartDate(DateTime.Now.Year).ToString("dd.MM.yyyy");

        var userData = new UserGameData()
        {
            PlayerName = name,
            PlayerSurname = lname,
            ClubId = targetId,
            GameDate = startDate,
            Money = 0,
            Role = 0,
            CurrentLevel = 0
        };

        savesManager.NewGame(userData, saveName);

        PlayerPrefs.SetString("SaveName", saveName);
        Show = true;
        SceneManager.LoadScene("ScoutScene");
    }
   

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");      
    }
    public void UpToManager()
    {
        SceneManager.LoadScene("ManagerScene");
    }
    public void OpenManager()
    {
        SceneManager.LoadScene("ManagerScene");
    }
    public void Transition() 
    {
        SceneManager.LoadScene("TransitionScene");
    }
    public void ScoutUpgrade()
    {
        var savesManager = SavesManager.GetInstance();
        var saveInfo = savesManager.Load(SaveUI.SaveNameSlot);

        var userData = saveInfo.UserGameData;
        var upgrader = new ScoutLevelUpgrader();

        (ScoutSkillLevel newLevel, double newMoney) upgradeResult = upgrader.Upgrade(userData.CurrentLevel+1, userData);
        userData.CurrentLevel = upgradeResult.newLevel;
        userData.Money = upgradeResult.newMoney;

        savesManager.Save(saveInfo);
        SceneManager.LoadScene("ScoutScene");
    }
}
