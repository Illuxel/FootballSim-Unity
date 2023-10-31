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
using System.Text;
using System.Linq;
using BusinessLogicLayer.Scenario;

public class GenerateMatch : MonoBehaviour
{
    public Image[] GoalHomePlayers, GoalGuestPlayers, CardHomePlayers, CardGuestPlayers;
    public TextMeshProUGUI[] HomePlayers, GuestPlayers, CountGoalsHomePlayers, CountGoalsGuestPlayers;
    public TextMeshProUGUI ScoreHomeTeam, ScoreGuestTeam, HomeTeamName, GuestTeamName;
    public RectTransform content;
    public GameObject headPrefab;
    private Material redCardPlayer;
    private Color redCardColor = new Color(0.681f, 0f, 0f);
    private string homeTeamId, guestTeamId; 
    private int MatchDelayMs = 1000;
    public TMP_Dropdown SelectSpeed;
    private SynchronizationContext unityContext;
    private string _chelseaId = "015834FD9556AAEC44DE54CDE350235B";
    private DateTime _gameDate = new DateTime(2023, 8, 12);
    public AutoScrollDown autoScrollDown;
    private void Start()
    {
        var scenario = new GenerateGameActionsToNextMatch(_gameDate, _chelseaId);
        scenario.SimulateActions();

        SelectSpeed.onValueChanged.AddListener(OnDropdownValueChanged);
        unityContext = SynchronizationContext.Current;
        if (unityContext.IsUnityNull() && _generateMatch != null)
        {
            _generateMatch.Abort();
        }
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
            Text matchMinute = infoObject.GetComponentInChildren<Text>();
            TextMeshProUGUI textInfoObject = infoObject.GetComponentInChildren<TextMeshProUGUI>();
            textInfoObject.text = string.Format(LocalizationSettings.StringDatabase.GetLocalizedString(gameEvent), teamOrPlayer);
            matchMinute.text = Convert.ToString(currentMinute);
        }, null);
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

            Text matchMinute = infoObject.GetComponentInChildren<Text>();

            TextMeshProUGUI textInfoObject = infoObject.GetComponentInChildren<TextMeshProUGUI>();

            textInfoObject.text = LocalizationSettings.StringDatabase.GetLocalizedString(gameEvent);

            matchMinute.text = Convert.ToString(currentMinute);
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
            if (gameEvent.YellowCardPlayer != null)
            {
                penaltyCardPlayer(Convert.ToString(gameEvent.YellowCardPlayer), Resources.Load<Sprite>("YellowCard"));
            }
            if (gameEvent.RedCardPlayer != null)
            {
                penaltyCardPlayer(Convert.ToString(gameEvent.RedCardPlayer), Resources.Load<Sprite>("RedCard"));
            }
            InfoDisplayController(gameEvent.HomeTeam.Name, gameEvent.EventCode, gameEvent.MatchMinute);
        }, null);
    }


    public void OnMatchPaused()
    {
        //Thread.Sleep(MatchDelayMs);
        unityContext.Post(state =>
        {
        //можливо підняти екран з детальною схемою команди
        }, null);
    }
    public void OnMatchFinished(MatchResult result)
    {
        Debug.Log(result.Winner);
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
    }

    private Thread _generateMatch;
    
    public void generateMatch()
    {   
        ScoreHomeTeam.text = "0";
        ScoreGuestTeam.text = "0";

        var match = getMatch(_gameDate, _chelseaId);
        var teamRepository = new TeamRepository();
        var teams = teamRepository.Retrieve(1);
        var homeTeam = teamRepository.Retrieve(match.HomeTeamId);
        var guestTeam = teamRepository.Retrieve(match.GuestTeamId);
        
        homeTeamId = homeTeam.Id;
        guestTeamId = guestTeam.Id;
        HomeTeamName.text = homeTeam.Name;
        GuestTeamName.text = guestTeam.Name;

        var teamCreator = new TeamForMatchCreator();
        var homeTeamForMatch = teamCreator.Create(homeTeam);
        var guestTeamForMatch = teamCreator.Create(guestTeam);
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
        
        var matchGen = new MatchGenerator(match);

        matchGen.OnMatchGoal += OnMatchGoal;
        matchGen.OnMatchEventHappend += OnEventHappend;
        matchGen.OnMatchFinished += OnMatchFinished;
        //match.OnMatchPaused += OnMatchPaused;
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
    

}
