using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

using System;

using DatabaseLayer.Repositories;
using DatabaseLayer.Services;
using BusinessLogicLayer.Services;

public class NewGame : MonoBehaviour
{
    public TMP_InputField saveName, firstName, lastName;
    public SpriteRenderer clubImage;

    public void CreateNewGame()
    {
        var teams = new TeamRepository().Retrieve();
        var team = teams.Find(team => team.ExtName == clubImage.sprite.name);

        var seasonValueCreator = new SeasonValueCreator();
        var startDate = seasonValueCreator.GetSeasonStartDate(DateTime.Now.Year).ToString("dd.MM.yyyy");

        var playerData = new PlayerGameData()
        {
            PlayerName = firstName.text,
            PlayerSurname = lastName.text,
            ClubId = team.Id,
            GameDate = startDate,
            Money = 0,
            Role = 0,
            CurrentLevel = 0
        };

        SavesManager.GetInstance().NewGame(playerData, saveName.text);
        SceneManager.LoadScene("ScoutScene");
    }
}
