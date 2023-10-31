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

public class MenuManager : MonoBehaviour
{
    
    private PlayerGameData playerData;
    public TextMeshProUGUI TMP, Name, Money, SName, Date, Clubname;  
    public SpriteRenderer spriteRenderer, clubicon;
    public TMP_InputField SaveName, FirstName, LastName;
    public Dropdown leagueDropdown;
    public string BuffPlName, BuffPlSurname, BuffClub, BuffDate;
    public Button Level2, Level3, Level4;

    public static bool Show;

    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        
        DirectoryInfo di = new DirectoryInfo(Application.dataPath);
        LoadGameManagerSettings.BasePath = di.Parent.FullName;

        if (currentScene.buildIndex == 2)
        {
            var loadGamemanager = LoadGameManager.GetInstance();
            
            
            SaveInfo saveInfo = loadGamemanager.Load(SaveUI.SaveNameSlot);
            Debug.Log(SaveUI.SaveNameSlot);
            PlayerGameData playerDataResult = saveInfo.PlayerData;
            Name.text = playerDataResult.PlayerName + " " + playerDataResult.PlayerSurname;
            Money.text = Convert.ToString(playerDataResult.Money);
            Date.text = playerDataResult.GameDate;
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

                clubicon.sprite = clubSprite;
            }
           
            BuffPlName = playerDataResult.PlayerName;
            BuffPlSurname = playerDataResult.PlayerSurname;
            BuffClub = playerDataResult.ClubId;     
            BuffDate= playerDataResult.GameDate;
            int i = (int)playerDataResult.CurrentLevel;           
            ScoutLevelUpgrader upgrader = new ScoutLevelUpgrader();
            upgrader.Upgrade(saveInfo.PlayerData.CurrentLevel, playerDataResult);
            Level2.onClick.AddListener(() => ScoutUpgrade());
            Level3.interactable = false; Level4.interactable = false;
            Level3.onClick.AddListener(() => ScoutUpgrade());
            Level4.onClick.AddListener(() => Transition());
            if (i == 1)
            {
                Level3.interactable = true;
                Level2.interactable = false;
            }
            else if(i == 2)
            {
                Level4.interactable = true;
                Level3.interactable = false;
                Level2.interactable = false;
            }
        }     
        
    }

    public void AddMoney()
    {
        int money = Convert.ToInt32(Money.text);
        int addmoney = money + 100;
        Money.text = Convert.ToString(addmoney);
    }

    public void ContinueGame()
    {
        var loadGamemanager = LoadGameManager.GetInstance();
        var saveInfo = loadGamemanager.Load(SaveUI.SaveNameSlot);

        switch (saveInfo.PlayerData.Role)
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
        var instance = LoadGameManager.GetInstance();
        SaveInfo saveInfo = new SaveInfo(new PlayerGameData()
        {
            RealDate = DateTime.Now.ToString(),
            PlayerName = BuffPlName,
            PlayerSurname = BuffPlSurname,
            ClubId = BuffClub,
            Money = Convert.ToDouble(Money.text),

        }, PlayerPrefs.GetString("SaveName")) ;
        instance.SaveGame(saveInfo);
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
        TeamRepository teamRepository = new TeamRepository();

        List<Team> teams = teamRepository.Retrieve();
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
        if(SaveName != null)
        {
            saveName = SaveName.text;
        }
        string name = "player";
        if (FirstName != null)
        {
            name = FirstName.text;
        }
        string lname = "player";
        if(LastName != null)
        {
            lname = LastName.text;
        }
        var loadGamemanager = LoadGameManager.GetInstance();
        var seasonValueCreator = new SeasonValueCreator();
        var startDate = seasonValueCreator.GetSeasonStartDate(DateTime.Now.Year).ToString("dd.MM.yyyy");
        playerData = new PlayerGameData();
        playerData.PlayerName = name;
        playerData.PlayerSurname = lname;
        playerData.ClubId = targetId;
        playerData.RealDate = DateTime.Now.ToString();
        playerData.GameDate = startDate;
        playerData.Money = 0;
        playerData.Role = 0;
        playerData.CurrentLevel = 0;
        loadGamemanager.NewGame(playerData, saveName);
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
        var loadGamemanager = LoadGameManager.GetInstance();
        SaveInfo saveInfo = loadGamemanager.Load(SaveUI.SaveNameSlot);
        PlayerGameData playerDataResult = saveInfo.PlayerData;
        ScoutLevelUpgrader upgrader = new ScoutLevelUpgrader();
        (ScoutSkillLevel newLevel, double newMoney) upgradeResult = upgrader.Upgrade(saveInfo.PlayerData.CurrentLevel+1, playerDataResult);
        saveInfo.PlayerData.CurrentLevel = upgradeResult.newLevel;
        saveInfo.PlayerData.Money = upgradeResult.newMoney;
        LoadGameManager loadGameManager = LoadGameManager.GetInstance();
        loadGameManager.SaveGame(saveInfo);
        SceneManager.LoadScene("ScoutScene");
    }
}
