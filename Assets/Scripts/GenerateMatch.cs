using UnityEngine;
using UnityEngine.UI;
using BusinessLogicLayer.Services;
using DatabaseLayer.Repositories;
using DatabaseLayer;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using System.Threading;
using Unity.VisualScripting;
using System.Linq;
using BusinessLogicLayer.Scenario;
using DatabaseLayer.Services;

public class GenerateMatch : MonoBehaviour
{
    public Image[] GoalHomePlayers, GoalGuestPlayers, CardHomePlayers, CardGuestPlayers;
    public Image HomeTeamLogo, GuestTeamLogo;
    public TextMeshProUGUI[] HomePlayers, GuestPlayers, CountGoalsHomePlayers, CountGoalsGuestPlayers;
    public TextMeshProUGUI ScoreHomeTeam, ScoreGuestTeam, HomeTeamName, GuestTeamName, Timer;
    public RectTransform content, ContentForDialogWindow;
    public GameObject headPrefab, TacticPlayersObject, DialogWindowPrefab;
    public GameObject[] WindowsSetActive;
    private Material redCardPlayer;
    private Color redCardColor = new Color(0.681f, 0f, 0f);
    private string homeTeamId, guestTeamId; 
    private int MatchDelayMs = 1000;
    public TMP_Dropdown SelectSpeed;
    private SynchronizationContext unityContext;
    private string _liverpoolId = "B5551778D1672E4E544F32BFFAD52BA6";
    private DateTime _gameDate = new DateTime(2023, 8, 12);
    public AutoScrollDown autoScrollDown;
    private string _gameDateStr = "12.08.2023";
    private SaveInfo _saveInfo;
    private PlayerGameData _gameData;
    private string _saveName = "ya";
    private bool isPaused = false;
    private IMatchGameEvent gameEventBuff;
    internal MatchResult matchResult;
    internal MatchGenerator matchGen;
    public TacticPlayers TacticPlayersScript;
    internal int matchMinute;
    private static MatchStatus matchStatus = MatchStatus.MatchIsNotOn;
    
    public MatchStatus ReturnMatchStatus()
    {
        return matchStatus;
    }
    public void StartGenerateMatch()
    {
        //тимчасово
        _gameData = new PlayerGameData();
        _gameData.ClubId = _liverpoolId;
        _gameData.GameDate = _gameDateStr;
<<<<<<< Updated upstream
        _saveInfo = new SaveInfo(_gameData, _saveName);        
        var settings =  new GenerateGameActionsToNextMatchSettings(Application.dataPath);
        var scenario = new GenerateGameActionsToNextMatch(_saveInfo, settings);
=======
        _saveInfo = new SaveInfo() 
        { 
            //SaveName = _saveName,
            PlayerData = _gameData
        };

        var scenario = new GenerateGameActionsToNextMatch(_saveInfo);
>>>>>>> Stashed changes
        scenario.SimulateActions();

        SelectSpeed.onValueChanged.AddListener(OnDropdownValueChanged);
        unityContext = SynchronizationContext.Current;
        if (unityContext.IsUnityNull() && _generateMatch != null)
        {
            _generateMatch.Abort();
        }
        matchStatus = MatchStatus.MatchIsOn;
        generateMatch();
        loadTeamLogo();
    }

    private void loadTeamLogo()
    {
        TeamRepository teamRepository = new TeamRepository();
        CountryRepository countryRepository = new CountryRepository();
        string homeCountryName = countryRepository.Retrieve(teamRepository.Retrieve(homeTeamId).LeagueID).Name;
        string guestCountryName = countryRepository.Retrieve(teamRepository.Retrieve(guestTeamId).LeagueID).Name;

        string imagePathHome = "FCLogos/" + homeCountryName + "/" + teamRepository.Retrieve(homeTeamId).ExtName;
        Sprite homeSprite = Resources.Load<Sprite>(imagePathHome);
        HomeTeamLogo.sprite = homeSprite;

        string imagePathGuest = "FCLogos/" + guestCountryName + "/" + teamRepository.Retrieve(guestTeamId).ExtName;
        Sprite guestSprite = Resources.Load<Sprite>(imagePathGuest);
        GuestTeamLogo.sprite = guestSprite;
    }
    private void Update()
    {
        if (unityContext.IsUnityNull() && _generateMatch != null)
        {
            _generateMatch.Abort();
        }
    }
    private void OnDestroy()
    {
        if (_generateMatch != null)
        {
            _generateMatch.Abort();
        }
    }
    public void OnDropdownValueChanged(int index)
    {
        int selectedIndex = SelectSpeed.value;
        if (SelectSpeed.value == 0)
        {
            MatchDelayMs = 1000;
        }
        if (SelectSpeed.value == 1)
        {
            MatchDelayMs = 500;
        }
        if (SelectSpeed.value == 2)
        {
            MatchDelayMs = 250;
        }
    }
    public void OnMatchGoal(Goal goal)
    {
        Thread.Sleep(MatchDelayMs);
        unityContext.Post(state =>
        {
           
            TeamRepository teamRepository = new TeamRepository();
            PlayerRepository playerRepository = new PlayerRepository();

            Player player = playerRepository.RetrieveOne(goal.PlayerId);
            Team team = teamRepository.Retrieve(goal.TeamId);
            InfoDisplayController(player.Person.Surname, "GoalPlayer", goal.MatchMinute);

            GetTextMeshProIndex(player.Person.Surname + " (" + player.Rating + ")", goal.TeamId);
            if (goal.TeamId == homeTeamId)
            {
                ScoreHomeTeam.text = Convert.ToString(Convert.ToInt32(ScoreHomeTeam.text) + 1);
            }
            else if (goal.TeamId == guestTeamId)
            {
                ScoreGuestTeam.text = Convert.ToString(Convert.ToInt32(ScoreGuestTeam.text) + 1);
            }
        }, null); 
    }


    void InfoDisplayController(string teamOrPlayer, string gameEvent, int currentMinute)
    {
        unityContext.Post(state =>
        {
            GameObject infoObject = Instantiate(headPrefab, content);
            TextMeshProUGUI textInfoObject = infoObject.GetComponentInChildren<TextMeshProUGUI>();
            textInfoObject.text = string.Format(LocalizationSettings.StringDatabase.GetLocalizedString(gameEvent), teamOrPlayer);
            Timer.text = Convert.ToString(currentMinute);
            
        }, null);
    }
   
    public static void DialogWindowPrefabSpawn(GameObject dialogWindowPrefab, RectTransform contentForDialogWindow, string gameEvent)
    {        
        GameObject spawnedPrefab = Instantiate(dialogWindowPrefab, contentForDialogWindow);

        TextMeshProUGUI headlineText = spawnedPrefab.transform.Find("Headline").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI contentText = spawnedPrefab.transform.Find("NotificationContent").GetComponent < TextMeshProUGUI>();

        headlineText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Attention");
        contentText.text = LocalizationSettings.StringDatabase.GetLocalizedString(gameEvent);
    }

    void clearInfoBox()
    {
        GameObject infoObject = Instantiate(headPrefab, content);

        Text matchMinute = infoObject.GetComponentInChildren<Text>();

        TextMeshProUGUI textInfoObject = infoObject.GetComponentInChildren<TextMeshProUGUI>();
        textInfoObject.ClearMesh();
        matchMinute.text = string.Empty;
    }
    void InfoDisplayController(string gameEvent, int currentMinute)
    {
        unityContext.Post(state =>
        {
            GameObject infoObject = Instantiate(headPrefab, content);      

            TextMeshProUGUI textInfoObject = infoObject.GetComponentInChildren<TextMeshProUGUI>();

            textInfoObject.text = LocalizationSettings.StringDatabase.GetLocalizedString(gameEvent);

            Timer.text = Convert.ToString(currentMinute);
        }, null);
    }
    void penaltyCardPlayer(string playerId, Sprite penaltyCard)
    {       
        PlayerRepository playerRepository = new PlayerRepository();
        TeamRepository teamRepository = new TeamRepository();
        Team homeTeam = teamRepository.Retrieve(homeTeamId);
        Team guestTeam = teamRepository.Retrieve(guestTeamId);
        List<Player> homeTeamPlayers = homeTeam.Players;
        List<Player> guestTeamPlayers= guestTeam.Players;
        Player homePlayer = homeTeamPlayers.Find(player => player.PersonID == playerId);
        Player guestPlayer = guestTeamPlayers.Find(player => player.PersonID == playerId);
        
        if (homePlayer != null)
        {
            for (int i = 0; i < HomePlayers.Length; i++)
            {
                if (HomePlayers[i].text == (homePlayer.Person.Surname + " (" + homePlayer.Rating + ")"))
                {
                    CardHomePlayers[i].enabled = true;
                    CardHomePlayers[i].sprite = penaltyCard;
                    if(penaltyCard == Resources.Load<Sprite>("RedCard"))
                    {
                        HomePlayers[i].color = redCardColor;
                        HomePlayers[i].fontStyle |= FontStyles.Strikethrough;
                        HomePlayers[i].material = redCardPlayer;    
                    }

                }
            }
        }
              
        if(guestPlayer != null)
        {
            for (int i = 0; i < GuestPlayers.Length; i++)
            {
                if (GuestPlayers[i].text == (guestPlayer.Person.Surname + " (" + guestPlayer.Rating + ")"))
                {
                    CardGuestPlayers[i].enabled = true;
                    CardGuestPlayers[i].sprite = penaltyCard;
                    if (penaltyCard == Resources.Load<Sprite>("RedCard"))
                    {
                        GuestPlayers[i].color = redCardColor;
                        GuestPlayers[i].fontStyle |= FontStyles.Strikethrough;
                        GuestPlayers[i].material = redCardPlayer;
                    }
                }
            }
        }               
    }
    
    public int GetTextMeshProIndex(string surname, string teamId)
    {
        if (teamId == homeTeamId)
        {
            for (int i = 0; i < HomePlayers.Length; i++)
            {
                if (HomePlayers[i].text == surname)
                {
                    GoalHomePlayers[i].enabled = true;
                    CountGoalsHomePlayers[i].enabled = true;
                    CountGoalsHomePlayers[i].text = Convert.ToString
                        (Convert.ToInt16(CountGoalsHomePlayers[i].text)+1);
                    return i;
                }
                
            }
        }
        else if(teamId == guestTeamId)
        {
            for (int i = 0; i < GuestPlayers.Length; i++)
            {
                if (GuestPlayers[i].text == surname)
                {                   
                    GoalGuestPlayers[i].enabled = true;
                    CountGoalsGuestPlayers[i].enabled = true;
                    CountGoalsGuestPlayers[i].text = Convert.ToString
                        (Convert.ToInt16(CountGoalsGuestPlayers[i].text) + 1);
                    return i;
                }
               
            }
        }
        return -1;
    }
    public void OnTeamChanged()
    {
        Thread.Sleep(MatchDelayMs);
        unityContext.Post(state =>
        {
           
        }, null);
    }
    public void OnEventHappend(IMatchGameEvent gameEvent)
    {
        Thread.Sleep(MatchDelayMs);
        unityContext.Post(state =>
        {
            
            gameEventBuff = null;
            gameEventBuff = gameEvent;
            if (gameEvent.YellowCardPlayer != null)
            {
                penaltyCardPlayer(Convert.ToString(gameEvent.YellowCardPlayer), Resources.Load<Sprite>("YellowCard"));
            }
            if (gameEvent.RedCardPlayer != null)
            {
                penaltyCardPlayer(Convert.ToString(gameEvent.RedCardPlayer), Resources.Load<Sprite>("RedCard"));
            }
            if(gameEvent.InjuredPlayer != null)
            {
               PlayerRepository playerRepository = new PlayerRepository();
                InfoDisplayController(playerRepository.RetrieveOne(gameEvent.InjuredPlayer.ToString()).Person.Surname, "PlayerInjury", gameEvent.MatchMinute);
            }
           
            matchMinute = gameEvent.MatchMinute;
            InfoDisplayController(gameEvent.HomeTeam.Name, gameEvent.EventCode, gameEvent.MatchMinute);
        }, null);
    }
    public void OnPlayerInjured(Player player)
    {
        unityContext.Post(state =>
        {         
            
            foreach (var homePlayer in matchResult.HomeTeam.AllPlayers)
            {
                if(player.PersonID == homePlayer.PersonID)
                {               
                    DialogWindowPrefabSpawn(DialogWindowPrefab, ContentForDialogWindow, "PlayerInjuryWarning");
                    WindowsSetActive[0].SetActive(true);
                    WindowsSetActive[1].SetActive(true);
                    WindowsSetActive[2].SetActive(false);
                    WindowsSetActive[3].SetActive(false);
                    WindowsSetActive[4].SetActive(false);

                    TacticPlayersObject.GetComponent<TacticPlayers>().SetPlayerListAndScheme();
                }
            }
            
        }, null);
        if (_generateMatch != null)
        {
            _generateMatch.Suspend();
        }
    }
    public void OnMatchPaused()
    {
        unityContext.Post(state =>
        {
            PlayerRepository playerRepository = new PlayerRepository();
            List<Player> players = playerRepository.Retrieve(_liverpoolId);
            List<Player> manPlayers = playerRepository.Retrieve("678065FDDB06C590A0D0F9EDC2B5196F");

            if (gameEventBuff.RedCardPlayer != null)
            {
                foreach (Player player in manPlayers)
                {
                    if (gameEventBuff.RedCardPlayer.ToString() == player.PersonID)
                    {
                             
                        OnMatchReturned();
                        return;             
                    }
                }
            }

            if(gameEventBuff.RedCardPlayer != null )
            {
                foreach (Player player in players)
                {
                    if (gameEventBuff.RedCardPlayer.ToString() == player.PersonID)
                    {
                        DialogWindowPrefabSpawn(DialogWindowPrefab, ContentForDialogWindow, "PlayerRedCardWarning");
                    }
                }
            }
            WindowsSetActive[0].SetActive(true);
            WindowsSetActive[1].SetActive(true);
            WindowsSetActive[2].SetActive(false);
            WindowsSetActive[3].SetActive(false);
            WindowsSetActive[4].SetActive(false);

            TacticPlayersObject.GetComponent<TacticPlayers>().SetPlayerListAndScheme();

        }, null);
       
        if (_generateMatch != null)
        {
            _generateMatch.Suspend();
        }
    }

    public void OnTimeOut()
    {
        unityContext.Post(state =>
        {
            DialogWindowPrefabSpawn(DialogWindowPrefab, ContentForDialogWindow, "FirstHalfOverWarning");

            WindowsSetActive[0].SetActive(true);
            WindowsSetActive[1].SetActive(true);
            WindowsSetActive[2].SetActive(false);
            WindowsSetActive[3].SetActive(false);
            WindowsSetActive[4].SetActive(false);
            TacticPlayersObject.GetComponent<TacticPlayers>().SetPlayerListAndScheme();
        }, null);
        if (_generateMatch != null)
        {
            _generateMatch.Suspend();
        }
    }

    public void OnMatchReturned()
    {
        unityContext.Post(state =>
        {
            updatePlayers();
        }, null);
        if (_generateMatch != null)
        {
            _generateMatch.Resume();
        }
    }
    public void OnMatchFinished(MatchResult result)
    {
        if(result.Winner == result.HomeTeam.Id)
        {
            InfoDisplayController(result.HomeTeam.Name, "WinTeam", 90);
        }
        else if (result.Winner == result.GuestTeam.Id)
        {
            InfoDisplayController(result.GuestTeam.Name, "WinTeam", 90);
        }
        else
        {
            InfoDisplayController("Draw", 90);
        }
        autoScrollDown.AutoScrollToggle(false);
       matchStatus = MatchStatus.MatchIsNotOn;
    }
   
    private Thread _generateMatch;
    private void updatePlayers() 
    {
        var teamCreator = new TeamForMatchCreator();
        var homeTeamForMatch = teamCreator.Create(homeTeamId);
        var guestTeamForMatch = teamCreator.Create(guestTeamId);
        int i = 0;
        foreach (var player in homeTeamForMatch.MainPlayers)
        {
            HomePlayers[i].text = player.Value.CurrentPlayer.Person.Surname +
                " (" + player.Value.CurrentPlayer.Rating + ")";
            i++;
        }
        i = 0;
        foreach (var player in guestTeamForMatch.MainPlayers)
        {
            GuestPlayers[i].text = player.Value.CurrentPlayer.Person.Surname +
                " (" + player.Value.CurrentPlayer.Rating + ")";
            i++;
        }
    } 
    public void generateMatch()
    {   
        ScoreHomeTeam.text = "0";
        ScoreGuestTeam.text = "0";

        var match = getMatch(_gameDate, _liverpoolId);
        var teamRepository = new TeamRepository();
        var teams = teamRepository.Retrieve(1);
        var homeTeam = teamRepository.Retrieve(match.HomeTeamId);
        var guestTeam = teamRepository.Retrieve(match.GuestTeamId);
        var teamCreator = new TeamForMatchCreator();
        var homeTeamForMatch = teamCreator.Create(homeTeam);
        var guestTeamForMatch = teamCreator.Create(guestTeam);

        homeTeamId = homeTeam.Id;
        guestTeamId = guestTeam.Id;
        HomeTeamName.text = homeTeam.Name;
        GuestTeamName.text = guestTeam.Name;
       
        int i = 0;
        foreach (var player in homeTeamForMatch.MainPlayers)
        {
            HomePlayers[i].text = player.Value.CurrentPlayer.Person.Surname + 
                " (" + player.Value.CurrentPlayer.Rating + ")";
            i++;
        }
        i = 0;
        foreach (var player in guestTeamForMatch.MainPlayers)
        {
            GuestPlayers[i].text = player.Value.CurrentPlayer.Person.Surname + 
                " (" + player.Value.CurrentPlayer.Rating + ")";
            i++;
        }
        
        matchGen = new MatchGenerator(match);
        matchResult = matchGen.MatchData;
        matchGen.OnMatchGoal += OnMatchGoal;
        matchGen.OnMatchEventHappend += OnEventHappend;
        matchGen.OnMatchFinished += OnMatchFinished;
        matchGen.OnMatchPaused += OnMatchPaused;
        matchGen.OnTimeOut += OnTimeOut;
        matchGen.OnPlayerInjured += OnPlayerInjured;
        //match.OnMatchTeamChanged += OnTeamChanged;
        //match.OnMatchEventHappend += OnEventHappend;
        _generateMatch = new Thread(matchGen.StartGenerating);
        _generateMatch.Start();
        
    }
    private Match getMatch(DateTime gameDate, string ownerTeamId)
    {
        var matchRepository = new MatchRepository();
        return matchRepository.Retrieve(ownerTeamId).FirstOrDefault(m => m.GetMatchDate() == gameDate);
    }
    public enum MatchStatus
    {
        MatchIsOn,
        MatchIsNotOn
    }
}
