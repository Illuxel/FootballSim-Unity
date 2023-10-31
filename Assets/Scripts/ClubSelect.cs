using DatabaseLayer.Repositories;
using DatabaseLayer;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ClubSelect : MonoBehaviour
{
    public Dropdown countryDropdown;
    public SpriteRenderer clubSpriteRenderer;
    public Button leftButton;
    public Button rightButton;

    private Dictionary<string, string[]> clubsDict = new Dictionary<string, string[]>();
    private int currentClubIndex = 0;
    void Start()
    {      
        LeagueRepository leagueRepository = new LeagueRepository();
        CountryRepository countryRepository = new CountryRepository();
        TeamRepository teamRepository = new TeamRepository();
        List<League> leagues = leagueRepository.Retrieve();
        List<Country> allCountries = countryRepository.Retrieve();
        List<Country> needCountries = new List<Country>();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        countryDropdown.ClearOptions();

        foreach (League league in leagues)
        {           
            int leagueId = league.Country.Id;
            Country needCountry = allCountries.Find(country => country.Id == leagueId);
            if (needCountry != null)
            {
                needCountries.Add(needCountry);
            }
        }

        foreach (Country country in needCountries)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = country.Name;
            options.Add(option);
        }

        countryDropdown.AddOptions(options);

        
        foreach (Country country in needCountries)    
        {      
            List<Team> teamsList = teamRepository.Retrieve(country.Id );
            teamsList.Sort((x, y) => x.Budget.CompareTo(y.Budget));
            List<Team> lowestBudgetTeams = teamsList.Take(3).ToList();            
            string[] teamNames = lowestBudgetTeams.Select(team => team.ExtName).ToArray();
            clubsDict.Add(country.Name, teamNames);
        }
       
        countryDropdown.onValueChanged.AddListener(OnCountryDropdownValueChanged);
        leftButton.onClick.AddListener(OnLeftButtonClick);
        rightButton.onClick.AddListener(OnRightButtonClick);       
        LoadClubImage(countryDropdown.options[0].text, clubsDict[countryDropdown.options[0].text][currentClubIndex]);
    }
    public void OnCountryDropdownValueChanged(int index)
    {
        currentClubIndex = 0;
        LoadClubImage(countryDropdown.options[index].text, clubsDict[countryDropdown.options[index].text][currentClubIndex]);
    }
    public void OnLeftButtonClick()
    {
        currentClubIndex--;
        if (currentClubIndex < 0)
        {
            currentClubIndex = clubsDict[countryDropdown.captionText.text].Length - 1;
        }
        LoadClubImage(countryDropdown.captionText.text, clubsDict[countryDropdown.captionText.text][currentClubIndex]);
    }
    public void OnRightButtonClick()
    {
        currentClubIndex++;
        if (currentClubIndex >= clubsDict[countryDropdown.captionText.text].Length)
        {
            currentClubIndex = 0;
        }
        LoadClubImage(countryDropdown.captionText.text, clubsDict[countryDropdown.captionText.text][currentClubIndex]);
    }
    void LoadClubImage(string countryName, string clubName)
    {        
        string imagePath = "FCLogos/" + countryName + "/" + clubName;       
        Sprite clubSprite = Resources.Load<Sprite>(imagePath);
        clubSpriteRenderer.sprite = clubSprite;
    }
}