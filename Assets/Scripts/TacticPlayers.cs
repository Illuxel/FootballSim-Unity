using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseLayer.Repositories;
using DatabaseLayer;
using TMPro;


public class TacticPlayers : MonoBehaviour
{
    public GameObject headPrefab, PrefabSmallCard;
    public Transform contentTransform;
    private static string teamId = "B5551778D1672E4E544F32BFFAD52BA6", buffListPlId, buffSchemePlId;   
    public GameObject[] Positions;
    private static List<Player> playerListArray, playerSchemeArray;
    private static int counter = 0;  

    private void Start()
    {      
        for (int i = 0; i < Positions.Length; i++)
        {
            int index = i; 
            Button button = Positions[i].GetComponent<Button>();
            button.onClick.AddListener(() => OnButtonClick(index));
        }
    }
    private void Update()
    {
        if(counter == 5)
        {
            SortPlayers();
            counter = 0;
        }
    }
    private void OnButtonClick(int index)
    {       
        Transform smallCardPlayer = Positions[index].transform.Find("SmallCardPlayer(Clone)");
        Transform personIdText = smallCardPlayer.Find("PersonId");
        TextMeshProUGUI personID = personIdText.GetComponent<TextMeshProUGUI>();
        buffSchemePlId = personID.text;

        if (counter == 0 || counter == 2)
        {
            counter = 2;
        }

        if (counter == 1)
        {                      
            counter = 0;

            PlayerRepository playerRepository = new PlayerRepository();
            PlayerRepository playerRepository1 = new PlayerRepository();            
            Player newPlayerScheme = playerRepository.RetrieveOne(buffListPlId);
            Player oldPlayerScheme = playerRepository1.RetrieveOne(buffSchemePlId);
        
            newPlayerScheme.IndexPosition = index+1;
            oldPlayerScheme.IndexPosition = 0;
            playerRepository.Update(newPlayerScheme);
            playerRepository.Update(oldPlayerScheme);
            playerSchemeArray.Add(newPlayerScheme);      
            playerSchemeArray.RemoveAll(player => player.PersonID == oldPlayerScheme.PersonID);
            playerListArray.RemoveAll(player => player.PersonID == newPlayerScheme.PersonID);
            playerListArray.Add(oldPlayerScheme);
            playerListArray.Sort((x, y) => y.Rating.CompareTo(x.Rating));
            ShowPlayers();
            setPlayerSchemeArray();
        }
       
    }

    void OnPrefabClick(GameObject prefab)
    {
        TextMeshProUGUI personIdText = prefab.transform.Find("PersonId").GetComponent<TextMeshProUGUI>();
        buffListPlId = personIdText.text;     
    }
    public void OnButPrefClick()
    {
        OnPrefabClick(headPrefab);
        if (counter == 1 || counter == 0)
        {
            counter = 1;
        }
        if (counter == 2)
        {
            PlayerRepository playerRepository = new PlayerRepository();
            PlayerRepository playerRepository1 = new PlayerRepository();
            Player PlayerList = playerRepository.RetrieveOne(buffListPlId);
            Player PlayerScheme = playerRepository1.RetrieveOne(buffSchemePlId);
            PlayerList.IndexPosition = PlayerScheme.IndexPosition;
            PlayerScheme.IndexPosition = 0;
            playerRepository.Update(PlayerList);
            playerRepository.Update(PlayerScheme);
            playerSchemeArray.Add(PlayerList);
            playerSchemeArray.RemoveAll(player => player.PersonID == PlayerScheme.PersonID);
            playerListArray.RemoveAll(player => player.PersonID == PlayerList.PersonID);
            playerListArray.Add(PlayerScheme);
            playerListArray.Sort((x, y) => y.Rating.CompareTo(x.Rating));          
            counter = 5;
        }
    }

    public void playerLists()
    {
        foreach (Transform child in contentTransform)
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
        
        ShowPlayers();
        setPlayerSchemeArray();
    }
    public void ShowPlayers()
    {   
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
        foreach (Player playerData in playerListArray)
        {
            GameObject headInstance = Instantiate(headPrefab, contentTransform);

            TextMeshProUGUI nameText = headInstance.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI positionText = headInstance.transform.Find("Role").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI ratingText = headInstance.transform.Find("Overall").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI personIdText = headInstance.transform.Find("PersonId").GetComponent<TextMeshProUGUI>();
            nameText.text = playerData.Person.Surname;
            positionText.text = playerData.Position.Code;
            ratingText.text = playerData.Rating.ToString();
            personIdText.text = playerData.PersonID;

           
        }
    }
    public void setPlayerSchemeArray()
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
            if(player.IndexPosition != 0) 
            {
                GameObject headInstance = Instantiate(PrefabSmallCard, Positions[player.IndexPosition - 1].GetComponent<Transform>());
                TextMeshProUGUI Name = headInstance.transform.Find("Name").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI Surname = headInstance.transform.Find("Surname").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI Pos = headInstance.transform.Find("Role").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI Rating = headInstance.transform.Find("Rating").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI PersonId = headInstance.transform.Find("PersonId").GetComponent<TextMeshProUGUI>();

                Name.text = player.Person.Name;
                Surname.text = player.Person.Surname;
                Pos.text = player.PositionCode;
                Rating.text = player.Rating.ToString();
                PersonId.text = player.PersonID;
            }
        }
    }
    private bool ascendingOrder = true; 
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
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
        ShowPlayers();
        setPlayerSchemeArray();
    }
    public void SortPlayers()
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

        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
        ShowPlayers();
        setPlayerSchemeArray();
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
        ShowPlayers();
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

        ShowPlayers();
    }
    /*private void RefreshScrollView(Player[] sortedPlayers)
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        foreach (Player playerData in sortedPlayers)
        {
            GameObject headInstance = Instantiate(headPrefab, contentTransform);

            TextMeshProUGUI nameText = headInstance.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI positionText = headInstance.transform.Find("Role").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI ratingText = headInstance.transform.Find("Overall").GetComponent<TextMeshProUGUI>();

            nameText.text = playerData.Person.Surname;
            positionText.text = playerData.Position.Code;
            ratingText.text = playerData.Rating.ToString();
        }
    }*/
    /*public void SetPlayersToScheme()
    {
        TeamForMatchCreator teamForMatchCreator = new TeamForMatchCreator();
        ITeamForMatch players = teamForMatchCreator.Create(teamId);
        foreach (var kvp in players.MainPlayers)
        {
            int playerIndex = kvp.Key;
            Transform CanvasFind = GameObject.Find("Canvas").GetComponent<Transform>();
            Transform MatchWindowFind = CanvasFind.Find("MatchWindow").GetComponent<Transform>();
            Transform PlayersFind = MatchWindowFind.Find("Players").GetComponent<Transform>();
            if (playerIndex == 1)
            {
                Transform GK = PlayersFind.Find("1").GetComponent<Transform>();
                SetPlayer(kvp, GK);

            }
            else if (playerIndex >= 2 && playerIndex <= 6)
            {
                Transform DefendersFind = PlayersFind.Find("Defenders").GetComponent<Transform>();
                Transform Defenders = DefendersFind.Find(playerIndex.ToString()).GetComponent<Transform>();
                SetPlayer(kvp, Defenders);
            }
            else if (playerIndex >= 7 && playerIndex <= 11)
            {
                Transform HalfDefendDefendersFind = PlayersFind.Find("HalfDefendDefenders").GetComponent<Transform>();
                Transform HalfDefendDefenders = HalfDefendDefendersFind.Find(playerIndex.ToString()).GetComponent<Transform>();
                SetPlayer(kvp, HalfDefendDefenders);
            }
            else if (playerIndex >= 12 && playerIndex <= 16)
            {
                Transform MidFind = PlayersFind.Find("Mid").GetComponent<Transform>();
                Transform Mid = MidFind.Find(playerIndex.ToString()).GetComponent<Transform>();
                SetPlayer(kvp, Mid);
            }
            else if (playerIndex >= 17 && playerIndex <= 19)
            {
                Transform HalfAttackDefenderFind = PlayersFind.Find("HalfAttackDefender").GetComponent<Transform>();
                Transform HalfAttackDefender = HalfAttackDefenderFind.Find(playerIndex.ToString()).GetComponent<Transform>();
                SetPlayer(kvp, HalfAttackDefender);
            }
            else if (playerIndex >= 20 && playerIndex <= 24)
            {
                Transform PositionForwardFind = PlayersFind.Find("PositionForward").GetComponent<Transform>();
                Transform PositionForward = PositionForwardFind.Find(playerIndex.ToString()).GetComponent<Transform>();
                SetPlayer(kvp, PositionForward);
            }
            else if (playerIndex == 25 || playerIndex == 26)
            {
                Transform Tfts = PlayersFind.Find(playerIndex.ToString()).GetComponent<Transform>();
                SetPlayer(kvp, Tfts);
            }
        }
       
    }
    void SetPlayer(KeyValuePair<int, TacticPlayerPosition> kvp, Transform contentTransform)
    {
        GameObject headInstance = Instantiate(PrefabSmallCard, contentTransform);
        TextMeshProUGUI Name = headInstance.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Surname = headInstance.transform.Find("Surname").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Pos = headInstance.transform.Find("Role").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Rating = headInstance.transform.Find("Rating").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI PersonId = headInstance.transform.Find("PersonId").GetComponent<TextMeshProUGUI>();
        Name.text = kvp.Value.CurrentPlayer.Person.Name;
        Surname.text = kvp.Value.CurrentPlayer.Person.Surname;
        Pos.text = kvp.Value.RealPosition;
        Rating.text = kvp.Value.CurrentPlayer.Rating.ToString();
        PersonId.text = kvp.Value.CurrentPlayer.PersonID;

        PlayerRepository playerRepository = new PlayerRepository();
        Player player = playerRepository.RetrieveOne(kvp.Value.CurrentPlayer.PersonID);
        if (player.IndexPosition == 0 || player.IndexPosition == null)
        {
            player.IndexPosition = kvp.Key;
            playerRepository.Update(player);
        }
    }*/
    
}
