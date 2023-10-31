using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DatabaseLayer;
using DatabaseLayer.Repositories;
using BusinessLogicLayer.Services;
using UnityEngine.UI;
using System;

public class PlayerFullStatDop : MonoBehaviour
{
    public GameObject FullStatprefab;
    public TextMeshProUGUI MyMy;
    public Transform contentTransform;
    public GoalAssistTracker goalAssistTracker;
    public string prefabPath;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public static Contract CurrentContract(string playerId)
    {

        var contractRep = new ContractRepository();
        Contract contract = contractRep.RetrieveOne(playerId);
        if (contract == null)
        { Debug.Log("null"); }
        return contract;
    }

    public void RefreshDop(string PlayerID)
    {
        Transform CanvasFind = GameObject.Find("Canvas").GetComponent<Transform>();
        Transform myPlayersWindowFind = CanvasFind.Find("PlayerWindow").GetComponent<Transform>();
        Transform team1players = myPlayersWindowFind.Find("Team1Players").GetComponent<Transform>();
        Transform ScrollFind = team1players.Find("Scroll View").GetComponent<Transform>();
        Transform ViewFind = ScrollFind.Find("Viewport").GetComponent<Transform>();
        Transform Content = ViewFind.Find("Content");
        prefabPath = "Prefabs/ClubInfoForFullStat";

        foreach (Transform child in Content)
        {
            Destroy(child.gameObject);
        }
        int i = 0;
        goalAssistTracker = new GoalAssistTracker();
        
        List<PlayerStatistic> playerStatistics = goalAssistTracker.GetPlayerStatistic(PlayerID);


        foreach (var playerStatistic in playerStatistics)
        {

            //Debug.Log(CurrentContract(playerStatistic.Player.ContractID).Team.Name);
            
           
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
       
            GameObject headInstance = Instantiate(prefab, Content);
            Transform totalPlaceFrame1 = headInstance.transform.Find("TotalPlaceFrame1");
            TextMeshProUGUI totalPlaceText1 = totalPlaceFrame1.Find("TotalPlaceNumber1").GetComponent<TextMeshProUGUI>();

            Transform clubNameFrame1 = headInstance.transform.Find("ClubFrame1");
            TextMeshProUGUI clubNameText1 = clubNameFrame1.Find("ClubName1").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI clubID = clubNameFrame1.Find("PlayerID").GetComponent<TextMeshProUGUI>();

            Transform SeasonFrame = headInstance.transform.Find("SeasonFrame");
            TextMeshProUGUI SeasonText = SeasonFrame.Find("Season").GetComponent<TextMeshProUGUI>();

            Transform goalFrame = headInstance.transform.Find("GoalFrame");
            TextMeshProUGUI goalText = goalFrame.Find("GoalNumber").GetComponent<TextMeshProUGUI>();

            Transform assistFrame = headInstance.transform.Find("AssistFrame");
            TextMeshProUGUI assistText = assistFrame.Find("AssistNumber").GetComponent<TextMeshProUGUI>();

            Transform matchFrame = headInstance.transform.Find("MatchFrame");
            TextMeshProUGUI matchText = matchFrame.Find("MatchNumber").GetComponent<TextMeshProUGUI>();


            i++;
            totalPlaceText1.text = i.ToString();
            clubNameText1.text = CurrentContract(playerStatistic.Player.ContractID).Team.Name;
            clubID.text = playerStatistic.PlayerId;
            SeasonText.text = playerStatistic.SeasonFrom + " - " + playerStatistic.SeasonTo;
            assistText.text = Convert.ToString(playerStatistic.CountOfAssists);
            goalText.text = Convert.ToString(playerStatistic.CountOfGoals);
            matchText.text = Convert.ToString(playerStatistic.CountOfPlayedMatches);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
