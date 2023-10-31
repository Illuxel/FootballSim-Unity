using System;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using DatabaseLayer.Settings;
using DatabaseLayer.Enums;
using DatabaseLayer.Repositories;
using DatabaseLayer.Services;

using BusinessLogicLayer.Services;

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI TMP, Name, Money, SName, Date, Clubname;  
    public SpriteRenderer clubicon;
    
    public Dropdown leagueDropdown;
    public string BuffPlName, BuffPlSurname, BuffClub, BuffDate;
    public Button Level2, Level3, Level4;

    public static bool Show;

    void Awake()
    {
        GameSettings.BaseGamePath = Application.persistentDataPath;
    }

    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        
        DirectoryInfo di = new DirectoryInfo(Application.dataPath);
        LoadGameManagerSettings.BasePath = di.Parent.FullName;

        if (currentScene.buildIndex == 2)
        {
<<<<<<< Updated upstream
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
=======
            var savesGamemanager = SavesManager.GetInstance();
            var saveInfo = savesGamemanager.Load("");
>>>>>>> Stashed changes

            var playerData = saveInfo.PlayerData;

            Date.text = playerData.GameDate;
            Name.text = playerData.PlayerName + " " + playerData.PlayerSurname;
            Money.text = Convert.ToString(playerData.Money);

            var team = new TeamRepository().Retrieve(playerData.ClubId);

            Clubname.text = team.ExtName;

            var files = Directory.GetFiles("Assets/Resources/FCLogos/", team.ExtName + ".png", SearchOption.AllDirectories);
            var imagePath = Path.GetFileNameWithoutExtension(files[0]);
            var clubSprite = Resources.Load<Sprite>(imagePath);

            if (clubSprite != null)
            {
                clubicon.sprite = clubSprite;
            }
           
            BuffPlName = playerData.PlayerName;
            BuffPlSurname = playerData.PlayerSurname;
            BuffClub = playerData.ClubId;     
            BuffDate = playerData.GameDate;

            var upgrader = new ScoutLevelUpgrader().Upgrade(playerData.CurrentLevel, playerData);

            switch (playerData.CurrentLevel)
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

    public void NewGame()
    {
<<<<<<< Updated upstream
        var loadGamemanager = LoadGameManager.GetInstance();
        var saveInfo = loadGamemanager.Load(SaveUI.SaveNameSlot);
=======
        SceneManager.LoadScene("NewGameScene");
    }
    public void LoadGame()
    {
        var savesManager = SavesManager.GetInstance();
        var saveInfo = savesManager.Load("");
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
        var instance = LoadGameManager.GetInstance();
        SaveInfo saveInfo = new SaveInfo(new PlayerGameData()
=======
        var savesManager = SavesManager.GetInstance();
        var saveInfo = new SaveInfo()
>>>>>>> Stashed changes
        {
            PlayerData = new PlayerGameData()
            {
                PlayerName = BuffPlName,
                PlayerSurname = BuffPlSurname,
                GameDate = BuffDate,
                ClubId = BuffClub,
                Money = Convert.ToDouble(Money.text),
            }
        };

        savesManager.SaveGame(saveInfo);
    }
    public void ContinueGame()
    {
        var savesManager = SavesManager.GetInstance();
        var saveInfo = savesManager.Continue();

        if (saveInfo != null)
        { 
            
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
<<<<<<< Updated upstream
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
   
=======
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
        var loadGamemanager = LoadGameManager.GetInstance();
        SaveInfo saveInfo = loadGamemanager.Load(SaveUI.SaveNameSlot);
        PlayerGameData playerDataResult = saveInfo.PlayerData;
        ScoutLevelUpgrader upgrader = new ScoutLevelUpgrader();
        (ScoutSkillLevel newLevel, double newMoney) upgradeResult = upgrader.Upgrade(saveInfo.PlayerData.CurrentLevel+1, playerDataResult);
        saveInfo.PlayerData.CurrentLevel = upgradeResult.newLevel;
        saveInfo.PlayerData.Money = upgradeResult.newMoney;
        LoadGameManager loadGameManager = LoadGameManager.GetInstance();
        loadGameManager.SaveGame(saveInfo);
=======
        var savesManager = SavesManager.GetInstance();
        var saveInfo = savesManager.Load("");

        var playerData = saveInfo.PlayerData;
        var upgrader = new ScoutLevelUpgrader();
        (ScoutSkillLevel newLevel, double newMoney) upgradeResult = upgrader.Upgrade(playerData.CurrentLevel + 1, playerData);

        playerData.CurrentLevel = upgradeResult.newLevel;
        playerData.Money = upgradeResult.newMoney;

        savesManager.SaveGame(saveInfo);
>>>>>>> Stashed changes
        SceneManager.LoadScene("ScoutScene");
    }
}
