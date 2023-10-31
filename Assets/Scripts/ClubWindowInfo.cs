using DatabaseLayer;
using DatabaseLayer.Repositories;
using BusinessLogicLayer.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DatabaseLayer.Model;
using System.Collections.Generic;
using System.IO;

public class ClubWindowInfo : MonoBehaviour
{
    public string prefabPath;
    public Transform Content;
    public SpriteRenderer clubSpriteRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        ClubInfo();
    }

    private void ClubInfo()
    {
        PersonRepository personRepository = new PersonRepository();
        Person person = new Person();
        Transform CanvasFind = GameObject.Find("Canvas").GetComponent<Transform>();
        Transform ClubWindow = CanvasFind.Find("ClubWindowInfo").GetComponent<Transform>();
        
        prefabPath = "Prefabs/ClubHistory";
        TeamRepository teamRepository = new TeamRepository();
        Team team = new Team();
        team = teamRepository.Retrieve("38BDDCDA1B8E1958CEDEB7AAD0DBA58F");

        Transform ClubNameBack = ClubWindow.transform.Find("ClubNameBack");
        TextMeshProUGUI ClubName = ClubNameBack.Find("ClubName").GetComponent<TextMeshProUGUI>();

        Transform TotalratingBack = ClubWindow.transform.Find("TotalratingBack");
        TextMeshProUGUI Totalrating = TotalratingBack.Find("Totalrating").GetComponent<TextMeshProUGUI>();

        Transform BudgetBack = ClubWindow.transform.Find("BudgetBack");
        TextMeshProUGUI Budget = BudgetBack.Find("Budget").GetComponent <TextMeshProUGUI>();

        Transform LeagueBack = ClubWindow.transform.Find("LeagueBack");
        TextMeshProUGUI League = LeagueBack.Find("League").GetComponent<TextMeshProUGUI>();

        Transform StrategyBack = ClubWindow.transform.Find("StrategyBack");
        TextMeshProUGUI Strategy = StrategyBack.Find("Strategy").GetComponent<TextMeshProUGUI>();

        Transform SchemeBack = ClubWindow.transform.Find("SchemeBack");
        TextMeshProUGUI Scheme = SchemeBack.Find("Scheme").GetComponent<TextMeshProUGUI>();

        Transform ScoutBack = ClubWindow.transform.Find("ScoutBack");
        TextMeshProUGUI Scout = ScoutBack.Find("Scout").GetComponent<TextMeshProUGUI>();

        Transform TrainerBack = ClubWindow.transform.Find("TrainerBack");
        TextMeshProUGUI Trainer = TrainerBack.Find("Trainer").GetComponent<TextMeshProUGUI>();

        Transform DirectorBack = ClubWindow.transform.Find("DirectorBack");
        TextMeshProUGUI Director = DirectorBack.Find("Director").GetComponent<TextMeshProUGUI>();

        ClubName.text = team.Name;
        Totalrating.text = team.GlobalRating.ToString();
        Budget.text = FormatCurrency(team.Budget);
        League.text = team.League.Name;
        Strategy.text = team.Strategy.ToString();
        Scheme.text = team.TacticSchema.ToString();
        //person = personRepository.Retrieve(team.ScoutID);
        //Scout.text = person.Name + " " + person.Surname;
        //person = personRepository.Retrieve(team.CoachID);
        //Trainer.text = person.Name + " " + person.Surname;
        //person = personRepository.Retrieve(team.SportsDirectorId);
        //Director.text = person.Name + " " + person.Surname;
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
            Debug.LogError("Не знайдено: " + team.ExtName);
        }

        List<TeamSuccessHistory> teamSuccessHistories = teamRepository.GetHistory(team.Id);
       
        foreach (Transform child in Content)
        {
            Destroy(child.gameObject);
        }
        foreach(var teamSuccessHistorie  in teamSuccessHistories)
        {
            GameObject prefab = Resources.Load<GameObject>(prefabPath);

            GameObject headInstance = Instantiate(prefab, Content);

            Transform LeagueFrame = headInstance.transform.Find("LeagueFrame");
            TextMeshProUGUI League1 = LeagueFrame.Find("League").GetComponent<TextMeshProUGUI>();

            Transform TotalPlaceFrame = headInstance.transform.Find("TotalPlaceFrame1");
            TextMeshProUGUI TotalPlaceNumber = TotalPlaceFrame.Find("TotalPlaceNumber1").GetComponent<TextMeshProUGUI>();

            Transform SeasonFrame = headInstance.transform.Find("SeasonFrame");
            TextMeshProUGUI Season = SeasonFrame.Find("Season").GetComponent<TextMeshProUGUI>();

            League1.text = teamSuccessHistorie.LeagueID.ToString();
            TotalPlaceNumber.text = teamSuccessHistorie.TotalPosition.ToString();
            Season.text = teamSuccessHistorie.Season;

        }
    }
    string FormatCurrency(double value)
    {
        double fvalue = value;
        if (fvalue >= 1000000)
        {
            double millions = fvalue / 1000000;
            return millions.ToString("F1") + "М";
        }
        else if (value >= 10000)
        {
            double thousands = fvalue / 1000;
            return thousands.ToString("F0") + "К";
        }
        else
        {
            return fvalue.ToString("F0");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
