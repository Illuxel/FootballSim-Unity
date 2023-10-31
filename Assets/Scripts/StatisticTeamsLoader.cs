using System.Collections.Generic;
using UnityEngine;
using DatabaseLayer;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Scenario;
using DatabaseLayer.Repositories;
using TMPro;
using System;

public class StatisticTeamsLoader : MonoBehaviour
{
    int leagueId = 1;
    string season = "2023/2024";
    public GameObject headPrefab, ScorePrefab;
    public Transform contentTransform, contentTransformScore, contentTransformAssist;
    private NationalResultTable[] nationalResultTable;
    public GoalAssistTracker goalAssistTracker;
    public TextMeshProUGUI NameLeagueText;
    private void Start()
    {
        /*TeamPositionCalculator teamPositionCalculator = new TeamPositionCalculator();
        teamPositionCalculator.CalculatePosition(season);*/
        goalAssistTracker = new GoalAssistTracker();
        SetTable();
    }
    public void SetTable()
    {
        /*SeasonValueCreator seasonValueCreator = new SeasonValueCreator();
        GenerateAllMatchesByTour generateAllMatchesByTour = new GenerateAllMatchesByTour(seasonValueCreator.GetSeasonStartDate(2023));
        generateAllMatchesByTour.Generate();*/
        NationalResTabRepository nationalResTabRepository = new NationalResTabRepository();
        List<NationalResultTable> resultTable = nationalResTabRepository.Retrieve(leagueId, season);
        nationalResultTable = resultTable.ToArray();

        List<PlayerStatistic> topGoalScorers = goalAssistTracker.GetTopGoalScorers(season, leagueId, 10);

        setNameLeague(leagueId);
        SortPlayersByPoints();       
    }
    private void setNameLeague(long leagueId)
    {
        NameLeagueText.text = " ";
        LeagueRepository leagueRepository = new LeagueRepository();
        League league = leagueRepository.Retrieve(leagueId);
        NameLeagueText.text = league.Name;
    }
    private bool ascendingOrder = true;

    public void SortPlayersByPoints()
    {
        ascendingOrder = !ascendingOrder;

        NationalResultTable[] sortedPlayers = nationalResultTable.Clone() as NationalResultTable[];
        Array.Sort(sortedPlayers, (a, b) => a.TotalPosition.CompareTo(b.TotalPosition));

        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
        RefreshScrollViewScore();
        RefreshScrollView(sortedPlayers);
    }
    private void RefreshScrollView(NationalResultTable[] sortedColumn)
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        foreach (NationalResultTable tableData in sortedColumn)
        {
            GameObject headInstance = Instantiate(headPrefab, contentTransform);

            Transform totalPlaceFrame = headInstance.transform.Find("TotalPlaceFrame");
            TextMeshProUGUI totalPlaceText = totalPlaceFrame.Find("TotalPlaceNumber").GetComponent<TextMeshProUGUI>();

            Transform clubNameFrame = headInstance.transform.Find("ClubFrame");
            TextMeshProUGUI clubNameText = clubNameFrame.Find("ClubName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI clubIdText = clubNameFrame.Find("ClubId").GetComponent<TextMeshProUGUI>();

            Transform gameFrame = headInstance.transform.Find("GameFrame");
            TextMeshProUGUI gameText = gameFrame.Find("GameNumber").GetComponent<TextMeshProUGUI>();

            Transform victoryFrame = headInstance.transform.Find("VictoryFrame");
            TextMeshProUGUI victoryText = victoryFrame.Find("WinNumber").GetComponent<TextMeshProUGUI>();

            Transform drawFrame = headInstance.transform.Find("DrawFrame");
            TextMeshProUGUI drawText = drawFrame.Find("DrawNumber").GetComponent<TextMeshProUGUI>();

            Transform defeatFrame = headInstance.transform.Find("DefeatFrame");
            TextMeshProUGUI defeatText = defeatFrame.Find("DefeatNumber").GetComponent<TextMeshProUGUI>();

            Transform scoreConcededFrame = headInstance.transform.Find("ScoreConcededFrame");
            TextMeshProUGUI scoreConcededText = scoreConcededFrame.Find("ScoreConcededNumber").GetComponent<TextMeshProUGUI>();

            Transform pointFrame = headInstance.transform.Find("PointFrame");
            TextMeshProUGUI pointText = pointFrame.Find("PointNumber").GetComponent<TextMeshProUGUI>();

            totalPlaceText.text = Convert.ToString(tableData.TotalPosition);
            clubNameText.text = tableData.Team.Name;
            clubIdText.text = tableData.TeamID;
            gameText.text = Convert.ToString(tableData.Wins + tableData.Draws + tableData.Loses);
            victoryText.text = Convert.ToString(tableData.Wins);
            drawText.text = Convert.ToString(tableData.Draws);
            defeatText.text = Convert.ToString(tableData.Loses);
            scoreConcededText.text = Convert.ToString(tableData.ScoredGoals) + " - " + Convert.ToString(tableData.MissedGoals);
            pointText.text = Convert.ToString(tableData.TotalPoints);
        }
    }
    private void RefreshScrollViewScore()
    {
        if (contentTransformScore != null)
        {

            foreach (Transform child in contentTransformScore)
            {
                Destroy(child.gameObject);
            }

            List<PlayerStatistic> topGoalScorers = goalAssistTracker.GetTopGoalScorers(season, leagueId, 10);
            int i = 0;
            foreach (var topGoalScorer in topGoalScorers)
            {
                GameObject headInstance = Instantiate(ScorePrefab, contentTransformScore);
                Transform totalPlaceFrame1 = headInstance.transform.Find("TotalPlaceFrame1");
                TextMeshProUGUI totalPlaceText1 = totalPlaceFrame1.Find("TotalPlaceNumber1").GetComponent<TextMeshProUGUI>();

                Transform clubNameFrame1 = headInstance.transform.Find("ClubFrame1");
                TextMeshProUGUI clubNameText1 = clubNameFrame1.Find("ClubName1").GetComponent<TextMeshProUGUI>();

                Transform nameFrame = headInstance.transform.Find("NameFrame");
                TextMeshProUGUI nameText = nameFrame.Find("Name").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI playerID = nameFrame.Find("PlayerID").GetComponent<TextMeshProUGUI>();

                Transform goalFrame = headInstance.transform.Find("GoalFrame");
                TextMeshProUGUI goalText = goalFrame.Find("GoalNumber").GetComponent<TextMeshProUGUI>();

                Transform matchFrame = headInstance.transform.Find("MatchFrame");
                TextMeshProUGUI matchText = matchFrame.Find("MatchNumber").GetComponent<TextMeshProUGUI>();


                i++;
                totalPlaceText1.text = i.ToString();
                clubNameText1.text = CurrentContract(topGoalScorer.Player.ContractID).Team.Name;
                nameText.text = topGoalScorer.Player.Person.Name + " " + topGoalScorer.Player.Person.Surname;
                playerID.text = topGoalScorer.PlayerId;
                goalText.text = Convert.ToString(topGoalScorer.CountOfGoals);
                matchText.text = Convert.ToString(topGoalScorer.CountOfPlayedMatches);
            }
        }
        if (contentTransformAssist != null)
        {

            foreach (Transform child in contentTransformAssist)
            {
                Destroy(child.gameObject);
            }
            var topAssists = goalAssistTracker.GetTopAssists(season, leagueId, 10);
            int i = 0;

            foreach (var topAssist in topAssists)
            {
                GameObject headInstance = Instantiate(ScorePrefab, contentTransformAssist);

                Transform totalPlaceFrame1 = headInstance.transform.Find("TotalPlaceFrame1");
                TextMeshProUGUI totalPlaceText1 = totalPlaceFrame1.Find("TotalPlaceNumber1").GetComponent<TextMeshProUGUI>();

                Transform clubNameFrame1 = headInstance.transform.Find("ClubFrame1");
                TextMeshProUGUI clubNameText1 = clubNameFrame1.Find("ClubName1").GetComponent<TextMeshProUGUI>();

                Transform nameFrame = headInstance.transform.Find("NameFrame");
                TextMeshProUGUI nameText = nameFrame.Find("Name").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI playerID = nameFrame.Find("PlayerID").GetComponent<TextMeshProUGUI>();

                Transform goalFrame = headInstance.transform.Find("GoalFrame");
                TextMeshProUGUI goalText = goalFrame.Find("GoalNumber").GetComponent<TextMeshProUGUI>();

                Transform matchFrame = headInstance.transform.Find("MatchFrame");
                TextMeshProUGUI matchText = matchFrame.Find("MatchNumber").GetComponent<TextMeshProUGUI>();
                i++;

                totalPlaceText1.text = i.ToString();
                clubNameText1.text = CurrentContract(topAssist.Player.ContractID).Team.Name;
                nameText.text = topAssist.Player.Person.Name + " " + topAssist.Player.Person.Surname;
                playerID.text = topAssist.PlayerId;
                goalText.text = Convert.ToString(topAssist.CountOfAssists);
                matchText.text = Convert.ToString(topAssist.CountOfPlayedMatches);
            }
        }
    }

    public static Contract CurrentContract(string playerId)
    {

        var contractRep = new ContractRepository();
        Contract contract = contractRep.RetrieveOne(playerId);        
        return contract;
    }
    public void LeftButtonClick()
    {

        leagueId -= 1;

        if (leagueId == 0)
        {
            leagueId = 10;
        }

        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        setNameLeague(leagueId);
        SetTable();
    }
    public void RightButtonClick()
    {
        leagueId++;

        if (leagueId == 11)
        {
            leagueId = 1;
        }

        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        setNameLeague(leagueId);
        SetTable();
    }

}
