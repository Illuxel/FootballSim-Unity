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
    long leagueId = 1;
    string season = "2023/2024";
    public GameObject headPrefab;
    public Transform contentTransform;
    private NationalResultTable[] nationalResultTable;
    public TextMeshProUGUI NameLeagueText;
    private void Start()
    {
        /*TeamPositionCalculator teamPositionCalculator = new TeamPositionCalculator();
        teamPositionCalculator.CalculatePosition(season);*/
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
