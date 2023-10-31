using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseLayer.Repositories;
using DatabaseLayer;
using BusinessLogicLayer.Services;
using TMPro;
using UnityEngine.Localization.Settings;


public class TacticPlayers : MonoBehaviour
{
    public GameObject ListPlayerPrefab, PrefabSmallCard;
    public Transform ListPlayerСontentTransform;   
    public GameObject[] Positions;
    private static List<Player> playerListArray, playerSchemeArray;
    private static string teamId = "B5551778D1672E4E544F32BFFAD52BA6", buffListPlId, buffSchemePlId;
    private static MoveThePlayer moveThePlayer = MoveThePlayer.StartMovement;
    private bool ascendingOrder = true;
    private static int countOfInjuredPlayers = 0;
    public RectTransform ContentForDialogWindow;
    public GameObject DialogWindowPrefab;
    public GameObject[] WindowsSetActive;
    public GenerateMatch GenerateMatchScript;
    public void SetPlayerListAndScheme()
    {          
        foreach (Transform child in ListPlayerСontentTransform)
        {
            Destroy(child.gameObject);
        }

        foreach (GameObject position in Positions)
        {
            if (position != null)
            {
                Transform[] childTransforms = position.GetComponentsInChildren<Transform>();

                for (int i = 1; i < childTransforms.Length; i++)
                {
                    Destroy(childTransforms[i].gameObject);
                }
            }
        }
        PlayerRepository playerRepository = new PlayerRepository();
        List<Player> players = playerRepository.Retrieve(teamId);
        List<Player> playersToMove = new List<Player>();
        foreach (Player player in players)
        {
            if (player.IndexPosition != 0)
            {
                playersToMove.Add(player);
            }
        }
        foreach (Player player in playersToMove)
        {
            players.Remove(player);
        }
        playerSchemeArray = playersToMove;
        playerListArray = players;

        sortPlayers();
    }
    
    public void ResetToDefaultScheme()
    {
        TeamForMatchCreator teamForMatchCreator = new TeamForMatchCreator();
        ITeamForMatch players = teamForMatchCreator.Create(teamId);
        PlayerRepository allPlayerRepository = new PlayerRepository();
        var allPlayers = allPlayerRepository.Retrieve(teamId);
        foreach (var player in allPlayers)
        {
            player.IndexPosition = 0;

        }
        allPlayerRepository.Update(allPlayers);

        foreach (var kvp in players.MainPlayers)
        {
            int playerIndex = kvp.Key;
            PlayerRepository playerRepository = new PlayerRepository();
            Player playerScheme = playerRepository.RetrieveOne(kvp.Value.CurrentPlayer.PersonID);
            playerScheme.IndexPosition = playerIndex;
            playerRepository.Update(playerScheme);
        }
        SetPlayerListAndScheme();
    }
    public void InjuredPlayers()
    {
        foreach (var player in playerSchemeArray)
        {
            if (player.InjuredTo != null)
            {
                countOfInjuredPlayers++;
            }
        }
        if (countOfInjuredPlayers == 0)
        {
            WindowsSetActive[0].SetActive(false);
            WindowsSetActive[1].SetActive(false);
            WindowsSetActive[2].SetActive(true);
            WindowsSetActive[3].SetActive(true);
            GenerateMatchScript.OnMatchReturned();
        }
        if (countOfInjuredPlayers > 0)
        {
            DialogWindowPrefabSpawn(DialogWindowPrefab, ContentForDialogWindow);
            countOfInjuredPlayers = 0;
        }
    }
    public static void DialogWindowPrefabSpawn(GameObject dialogWindowPrefab, RectTransform contentForDialogWindow)
    {
        GameObject spawnedPrefab = Instantiate(dialogWindowPrefab, contentForDialogWindow);

        TextMeshProUGUI headlineText = spawnedPrefab.transform.Find("Headline").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI contentText = spawnedPrefab.transform.Find("NotificationContent").GetComponent<TextMeshProUGUI>();

        headlineText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Attention");
        contentText.text = LocalizationSettings.StringDatabase.GetLocalizedString("PlayerInjuryExeption");
    }
    public void SortPlayersByRating()
    {
        ascendingOrder = !ascendingOrder;
        if (ascendingOrder)
        {
            playerListArray.Sort((x, y) => y.Rating.CompareTo(x.Rating));
        }
        else
        {
            playerListArray.Sort((x, y) => x.Rating.CompareTo(y.Rating));
        }
        foreach (Transform child in ListPlayerСontentTransform)
        {
            Destroy(child.gameObject);
        }
        sortPlayers();
    }
    public void SortPlayersByName()
    {
        ascendingOrder = !ascendingOrder;
        if (ascendingOrder)
        {
            playerListArray.Sort((a, b) => a.Person.Surname.CompareTo(b.Person.Surname));
        }
        else
        {
            playerListArray.Sort((a, b) => b.Person.Surname.CompareTo(a.Person.Surname));
        }
        showListPlayers();
    }
    public void SortPlayersByPosition()
    {
        ascendingOrder = !ascendingOrder;
        if (ascendingOrder)
        {
            playerListArray.Sort((a, b) => a.Position.Code.CompareTo(b.Position.Code));
        }
        else
        {
            playerListArray.Sort((a, b) => b.Position.Code.CompareTo(a.Position.Code));
        }
        showListPlayers();
    }
    public enum MatchStatus
    {
        MatchIsOn,
        MatchIsNotOn
    }
    public void OnPlayerListButPrefClick()
    {
        onPrefabClick(ListPlayerPrefab);
        
        if (moveThePlayer == MoveThePlayer.StartMovement)
        {
            moveThePlayer = MoveThePlayer.FromSchemeToList;
        }
        if (moveThePlayer == MoveThePlayer.FromListToScheme)
        {
            PlayerRepository playerRepository = new PlayerRepository();
            Player PlayerList = playerRepository.RetrieveOne(buffListPlId);
            Player PlayerScheme = playerRepository.RetrieveOne(buffSchemePlId);
            PlayerList.IndexPosition = PlayerScheme.IndexPosition;
            PlayerScheme.IndexPosition = 0;
            playerRepository.Update(PlayerList);
            playerRepository.Update(PlayerScheme);
            if (currentlyStatus() == GenerateMatch.MatchStatus.MatchIsOn.ToString())
            {
                GenerateMatchScript.matchGen.MatchData.HomeTeam.SubstitutePlayer(PlayerList.IndexPosition, PlayerList, GenerateMatchScript.matchMinute);
            }
            playerSchemeArray.Add(PlayerList);
            playerSchemeArray.RemoveAll(player => player.PersonID == PlayerScheme.PersonID);
            playerListArray.RemoveAll(player => player.PersonID == PlayerList.PersonID);
            playerListArray.Add(PlayerScheme);
            playerListArray.Sort((x, y) => y.Rating.CompareTo(x.Rating));
            moveThePlayer = MoveThePlayer.StopMovement;
            
        }
    } 
    private void Start()
    {
        for (int i = 0; i < Positions.Length; i++)
        {
            int index = i; 
            Button button = Positions[i].GetComponent<Button>();
            button.onClick.AddListener(() => onPlayerCardButtonClick(index));
        }
    }
    private void Update()
    {
        if(moveThePlayer == MoveThePlayer.StopMovement)
        {
            sortPlayers();
            moveThePlayer = MoveThePlayer.StartMovement;
        }
    }
    private void onPlayerCardButtonClick(int index)
    {       
        Transform smallCardPlayer = Positions[index].transform.Find("SmallCardPlayer(Clone)");
        Transform personIdText = smallCardPlayer.Find("PersonId");
        TextMeshProUGUI personID = personIdText.GetComponent<TextMeshProUGUI>();
        buffSchemePlId = personID.text;

        if (moveThePlayer == MoveThePlayer.StartMovement)
        {
            moveThePlayer = MoveThePlayer.FromListToScheme;
        }

        if (moveThePlayer == MoveThePlayer.FromSchemeToList)
        {
            moveThePlayer = MoveThePlayer.StartMovement;

            PlayerRepository playerRepository = new PlayerRepository();           
            Player newPlayerScheme = playerRepository.RetrieveOne(buffListPlId);
            Player oldPlayerScheme = playerRepository.RetrieveOne(buffSchemePlId);
        
            newPlayerScheme.IndexPosition = index+1;
            oldPlayerScheme.IndexPosition = 0;
            playerRepository.Update(newPlayerScheme);
            playerRepository.Update(oldPlayerScheme);
            if (currentlyStatus() == GenerateMatch.MatchStatus.MatchIsOn.ToString())
            {
                GenerateMatchScript.matchGen.MatchData.HomeTeam.SubstitutePlayer(newPlayerScheme.IndexPosition, newPlayerScheme, GenerateMatchScript.matchMinute);
            }
            playerSchemeArray.Add(newPlayerScheme);      
            playerSchemeArray.RemoveAll(player => player.PersonID == oldPlayerScheme.PersonID);
            playerListArray.RemoveAll(player => player.PersonID == newPlayerScheme.PersonID);
            playerListArray.Add(oldPlayerScheme);
            playerListArray.Sort((x, y) => y.Rating.CompareTo(x.Rating));
            sortPlayers();
        }
    }
    private void onPrefabClick(GameObject prefab)
    {
        TextMeshProUGUI personIdText = prefab.transform.Find("PersonId").GetComponent<TextMeshProUGUI>();
        buffListPlId = personIdText.text;
    }   
    private void showListPlayers()
    {   
        foreach (Transform child in ListPlayerСontentTransform)
        {
            Destroy(child.gameObject);
        }
        foreach (Player playerData in playerListArray)
        {
            bool isInjury = false;
            bool isRedCard = false;
            bool isYellowCard = false;          
                if (currentlyStatus() == GenerateMatch.MatchStatus.MatchIsOn.ToString())
                {
                    foreach (var ePlayer in GenerateMatchScript.matchResult.InjuredPlayers)
                    {
                        if (playerData.PersonID == ePlayer.ToString())
                        {
                            isInjury = true;
                        }
                    }
                    foreach (var ePlayer in GenerateMatchScript.matchResult.RedCardPlayers)
                    {
                        if (playerData.PersonID == ePlayer.ToString())
                        {
                            isRedCard = true;
                        }
                    }
                    foreach (var ePlayer in GenerateMatchScript.matchResult.YellowCardPlayers)
                    {
                        if (playerData.PersonID == ePlayer.ToString())
                        {
                            isYellowCard = true;
                        }
                    }
                }
                GameObject headInstance = Instantiate(ListPlayerPrefab, ListPlayerСontentTransform);

            TextMeshProUGUI nameText = headInstance.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI positionText = headInstance.transform.Find("Role").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI ratingText = headInstance.transform.Find("Overall").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI personIdText = headInstance.transform.Find("PersonId").GetComponent<TextMeshProUGUI>();
            TacticPlayers script = headInstance.transform.Find("Script").GetComponent<TacticPlayers>();
            
            script.GenerateMatchScript = GenerateMatchScript;
            nameText.text = playerData.Person.Surname;
            positionText.text = playerData.Position.Code;
            ratingText.text = playerData.Rating.ToString();
            personIdText.text = playerData.PersonID;
            if (playerData.InjuredTo != null)
            {
                nameText.color = new Color(1.0f, 0.0f, 0.0f);
                ratingText.color = new Color(1.0f, 0.0f, 0.0f);
                positionText.color = new Color(1.0f, 0.0f, 0.0f);
            }
            if(currentlyStatus() == GenerateMatch.MatchStatus.MatchIsOn.ToString() &&(isInjury == true || isRedCard == true || isYellowCard == true))
            {
                nameText.color = new Color(1.0f, 0.0f, 0.0f);
                ratingText.color = new Color(1.0f, 0.0f, 0.0f);
                positionText.color = new Color(1.0f, 0.0f, 0.0f);
            }
        }       
    }
    private void setPlayerSchemeArray()
    {
        foreach (GameObject position in Positions)
        {
            if (position != null)
            {
                Transform[] childTransforms = position.GetComponentsInChildren<Transform>();
                for (int i = 1; i < childTransforms.Length; i++)
                {
                    Destroy(childTransforms[i].gameObject);
                }
            }
        }
        
        foreach (Player player in playerSchemeArray)
        {
            bool isInjury = false;
            bool isRedCard = false;
            bool isYellowCard = false;
            if(player.IndexPosition != 0) 
            {
                if(currentlyStatus() == GenerateMatch.MatchStatus.MatchIsOn.ToString())
                {
                    foreach (var ePlayer in GenerateMatchScript.matchResult.InjuredPlayers)
                    {
                        if (player.PersonID == ePlayer.ToString())
                        {
                            isInjury = true;
                        }
                    }
                    foreach (var ePlayer in GenerateMatchScript.matchResult.RedCardPlayers)
                    {
                        if (player.PersonID == ePlayer.ToString())
                        {
                            isRedCard = true;
                        }
                    }
                    foreach (var ePlayer in GenerateMatchScript.matchResult.YellowCardPlayers)
                    {
                        if (player.PersonID == ePlayer.ToString())
                        {
                            isYellowCard = true;
                        }
                    }
                }
                
                GameObject headInstance = Instantiate(PrefabSmallCard, Positions[player.IndexPosition - 1].GetComponent<Transform>());              
                TextMeshProUGUI Name = headInstance.transform.Find("Name").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI Surname = headInstance.transform.Find("Surname").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI Pos = headInstance.transform.Find("Role").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI Rating = headInstance.transform.Find("Rating").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI PersonId = headInstance.transform.Find("PersonId").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI Endurance = headInstance.transform.Find("Endurance").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI InjText = headInstance.transform.Find("InjText").GetComponent<TextMeshProUGUI>();
                GameObject InjCard = headInstance.transform.Find("Injury").gameObject;
                GameObject InjCardOnly = headInstance.transform.Find("InjuryOnly").gameObject;
                GameObject RedCard = headInstance.transform.Find("RedCard").gameObject;

                GameObject RedCardOnly = headInstance.transform.Find("RedCardOnly").gameObject;
                GameObject YellowCard = headInstance.transform.Find("YellowCard").gameObject;
                GameObject YellowCardOnly = headInstance.transform.Find("YellowCardOnly").gameObject;
                
                Name.text = player.Person.Name;
                Surname.text = player.Person.Surname;
                Pos.text = player.PositionCode;
                Rating.text = player.Rating.ToString();
                PersonId.text = player.PersonID;
                Endurance.text = player.Endurance.ToString()+"%";
                if (player.InjuredTo != null && currentlyStatus() == GenerateMatch.MatchStatus.MatchIsNotOn.ToString())
                {
                    InjText.text = "";
                    InjCardOnly.SetActive(true);
                    Endurance.text = "";
                }
                if(isInjury == true &&(isRedCard == true || isYellowCard == true ) && currentlyStatus() == GenerateMatch.MatchStatus.MatchIsOn.ToString())
                {
                    if(isRedCard == true)
                    {
                       
                        RedCard.SetActive(true);

                    }
                    if(isYellowCard == true)
                    {
                        YellowCard.SetActive(true);
                    }
                    InjText.text = "";
                    Endurance.text = "";
                    InjCard.SetActive(true);
                }
                if (isInjury == false && (isRedCard == true || isYellowCard == true) && currentlyStatus() == GenerateMatch.MatchStatus.MatchIsOn.ToString())
                {
                    if (isRedCard == true)
                    {

                        RedCardOnly.SetActive(true);

                    }
                    if (isYellowCard == true)
                    {
                        YellowCardOnly.SetActive(true);
                    }
                    InjText.text = "";
                    Endurance.text = "";              
                }
                if (isInjury == false && (isRedCard == false || isYellowCard == false) && currentlyStatus() == GenerateMatch.MatchStatus.MatchIsOn.ToString())
                {
                    if (player.InjuredTo != null)
                    {
                        InjText.text = "";
                        InjCardOnly.SetActive(true);
                        Endurance.text = "";
                    }
                }
            }
        }
    } 
   
    private void sortPlayers()
    {
        playerListArray.Sort((x, y) => y.Rating.CompareTo(x.Rating));
        foreach (GameObject position in Positions)
        {
            if (position != null)
            {
                Transform[] childTransforms = position.GetComponentsInChildren<Transform>();

                for (int i = 1; i < childTransforms.Length; i++)
                {
                    Destroy(childTransforms[i].gameObject);
                }
            }
        }

        foreach (Transform child in ListPlayerСontentTransform)
        {
            Destroy(child.gameObject);
        }
        showListPlayers();
        setPlayerSchemeArray();
    }
    private string currentlyStatus()
    {
        if(GenerateMatchScript != null) 
        { 
            string matchStatus = GenerateMatchScript.ReturnMatchStatus().ToString();
            return matchStatus;
        }
        else
        {
            string matchStatus = "MatchIsNotOn";
            return matchStatus;
        }
        
    }
    private enum MoveThePlayer
    {
        StartMovement,
        FromListToScheme,
        FromSchemeToList,
        StopMovement        
    }
}
