using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using DatabaseLayer.Repositories;
using DatabaseLayer;

public class ClubGenerator : MonoBehaviour
{
    public TextMeshProUGUI clubname;
    public TextMeshProUGUI place;
    public TextMeshProUGUI value;
    public TextMeshProUGUI budget;
    public SpriteRenderer clubSpriteRenderer;
    public Button Next;
    public Button Previous;
    public Button ToManager;
    private List<Team> selectedTeams;
    private int currentTeamIndex = 0;

    private void Start()
    {
        var teamRepository = new TeamRepository();
        var teams = teamRepository.Retrieve().OrderByDescending(t => t.GlobalRating ).Take(20).ToList();
        foreach (var team in teams)
        {
            Debug.Log(team.GlobalRating);
        }
        var random = new System.Random();
        selectedTeams = new List<Team>();

        while (selectedTeams.Count < 10 && teams.Any())
        {
            var index0 = random.Next(teams.Count);
            var selectedTeam = teams[index0];
            selectedTeams.Add(selectedTeam);
            teams.RemoveAt(index0);
        }

        UpdateClubInfo(selectedTeams[currentTeamIndex]);

        // Add click listeners to the Next and Previous buttons
        Next.onClick.AddListener(NextClub);
        Previous.onClick.AddListener(PreviousClub);
        ToManager.onClick.AddListener(Manager);
    }
    public void Manager()
    {
        SceneManager.LoadScene("ManagerScene");
    }
    public void NextClub()
    {
        currentTeamIndex = (currentTeamIndex + 1) % selectedTeams.Count;
        UpdateClubInfo(selectedTeams[currentTeamIndex]);
    }

    public void PreviousClub()
    {
        currentTeamIndex = (currentTeamIndex - 1 + selectedTeams.Count) % selectedTeams.Count;
        UpdateClubInfo(selectedTeams[currentTeamIndex]);
    }

    private void UpdateClubInfo(Team team)
    {
        clubname.text = team.Name;
        place.text = Convert.ToString(team.GlobalRating);
        budget.text = Convert.ToString(team.Budget);
        string[] files = Directory.GetFiles("Assets/Resources/FCLogos/", team.ExtName + ".png", SearchOption.AllDirectories);
        string imagePath = files[0];
        string path = imagePath;
        path = path.Replace(".png", "");
        string keyword = "FCLogos";
        int index = path.IndexOf(keyword);
        string result = path.Substring(index);
        Sprite clubSprite = Resources.Load<Sprite>(result);

        if (clubSprite != null)
        {
            clubSpriteRenderer.sprite = clubSprite;
        }
        else
        {
            Debug.LogError("������� ��� ����������� ����: " + team.ExtName);
        }
    }
}



