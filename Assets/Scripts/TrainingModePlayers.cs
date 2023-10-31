using BusinessLogicLayer.Services;
using UnityEngine;
using DatabaseLayer.Enums;
using TMPro;
using DatabaseLayer.Repositories;

public class TrainingModePlayers : MonoBehaviour
{
    public GameObject TrainingModePref;
    public Transform TrainingModeContent;
    private static TrainingMode selectedTrainingMode;
    private string teamId = "015834FD9556AAEC44DE54CDE350235B";



    public void Simplified()
    {
        refreshScrollView(TrainingMode.SimplifiedForEveryone);
        selectedTrainingMode = TrainingMode.SimplifiedForEveryone;
    }
    public void SimplifiedForPlayLastGame()
    {
        refreshScrollView(TrainingMode.SimplifiedForLastGamePlayers);
        selectedTrainingMode = TrainingMode.SimplifiedForLastGamePlayers;
    }
    public void Standart()
    {
        refreshScrollView(TrainingMode.Standart);
        selectedTrainingMode = TrainingMode.Standart;
    }
    public void AdvancedForLastGameBench() 
    {
        refreshScrollView(TrainingMode.AdvancedForLastGameBench);
        selectedTrainingMode = TrainingMode.AdvancedForLastGameBench;
    }
    public void Advanced()
    {
        refreshScrollView(TrainingMode.AdvancedForEveryone);
        selectedTrainingMode = TrainingMode.AdvancedForEveryone;
    }

    public void AcceptTrainingMode()
    {
        PlayerSkillsTrainer playerSkillsTrainer = new PlayerSkillsTrainer();
        playerSkillsTrainer.TrainPlayers(teamId, selectedTrainingMode);
    }

    private void refreshScrollView(TrainingMode trainingMode)
    {
        foreach (Transform child in TrainingModeContent)
        {
            Destroy(child.gameObject);
        }
        PlayerSkillsTrainer playerSkillsTrainer = new PlayerSkillsTrainer();
        var playerList = playerSkillsTrainer.GetPlayersPreview(teamId, trainingMode);
       
        foreach(var player in playerList)
        {
            
            GameObject headInstance = Instantiate(TrainingModePref, TrainingModeContent);
            PlayerRepository playerRepository = new PlayerRepository();
            var plr = playerRepository.RetrieveOne(player.PersonID);
            TextMeshProUGUI nameText = headInstance.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI roleText = headInstance.transform.Find("Role").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI overallText = headInstance.transform.Find("Overall").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI currentText = headInstance.transform.Find("Current").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI afterText = headInstance.transform.Find("After").GetComponent<TextMeshProUGUI>();

            nameText.text = plr.Person.Surname;
            roleText.text = plr.PositionCode;
            overallText.text = plr.Rating.ToString();
            currentText.text = player.CurrentEndurance.ToString() + "%";
            afterText.text = player.AfterTrainEndurance.ToString() + "%";


        }

    }
}
