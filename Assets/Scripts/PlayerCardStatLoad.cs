using DatabaseLayer;
using DatabaseLayer.Repositories;
using BusinessLogicLayer.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class PlayerCardStatLoad : MonoBehaviour
{
    public GameObject PlayerCardPrefab;
    public Transform PlayerContent;
    public TextMeshProUGUI TeamId;
    public TextMeshProUGUI WindowName;
    public void OnTeamNameClicked(string teamName)
    {     
        Transform myPlayersCanvas = GameObject.Find("Canvas").GetComponent<Transform>();
        Transform myPlayersWindow = myPlayersCanvas.Find("MyPlayersWindow").GetComponent<Transform>();
        Transform StatisticWindow = myPlayersCanvas.Find("StatisticWindow");
        Transform CloseButton = myPlayersWindow.Find("CloseButton");
        Transform CloseButtonCanvas = myPlayersCanvas.Find("CloseButton");

        StatisticWindow.gameObject.SetActive(false);
        myPlayersWindow.gameObject.SetActive(true);
        CloseButton.gameObject.SetActive(true);
        CloseButtonCanvas.gameObject.SetActive(false);
        SetPlayers(TeamId.text);
    }
    public void SetAcademyName()
    {
        WindowName.text = LocalizationSettings.StringDatabase.GetLocalizedString("Academy");
    }
    public void SetTeamName()
    {
        WindowName.text = LocalizationSettings.StringDatabase.GetLocalizedString("MyTeam");
    }
    string FormatCurrency(int value)
    {
        float fvalue = value;
        if (fvalue >= 1000000)
        {
            float millions = fvalue / 1000000;
            return millions.ToString("F1") + "М";
        }
        else if (value >= 10000)
        {
            float thousands = fvalue / 1000;
            return thousands.ToString("F0") + "К";
        }
        else
        {
            return fvalue.ToString("F0");
        }
    }
    public void SetPlayers(string teamId)
    {
        PlayerPriceCalculator playerPriceCalculator = new PlayerPriceCalculator();
        ContractRepository contractRepository = new ContractRepository();
        TeamRepository teamRepository = new TeamRepository();
        LeagueRepository leagueRepository = new LeagueRepository();
        NationalResTabRepository nationalResTabRepository = new NationalResTabRepository();
        
        Team teamList = teamRepository.Retrieve(teamId);        
        League league = leagueRepository.Retrieve(teamList.LeagueID);
        var resultTable = nationalResTabRepository.Retrieve(teamId, "2023/2024");      

        string country = league.Country.Name;
        
        string imagePath = "FCLogos/" + country + "/" + teamList.ExtName;
        Image clubSprite = Resources.Load<Image>(imagePath);
       
        Transform CanvasFind = GameObject.Find("Canvas").GetComponent<Transform>();
        Transform myPlayersWindowFind = CanvasFind.Find("MyPlayersWindow").GetComponent<Transform>();
        Transform ScrollFind = myPlayersWindowFind.Find("Scroll").GetComponent<Transform>();
        Transform ViewFind = ScrollFind.Find("View").GetComponent<Transform>();
        Transform Content = ViewFind.Find("Content");

        for (int i = Content.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(Content.GetChild(i).gameObject);
        }

        foreach (Player player in teamList.Players)
        {
            GameObject headInstance = Instantiate(PlayerCardPrefab, Content);

            Image iconClub = headInstance.transform.Find("ClubIcon").GetComponent<Image>();
            TextMeshProUGUI playerNumberText = headInstance.transform.Find("PlayerNumber").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI clubNameText = headInstance.transform.Find("ClubName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI playerNameText = headInstance.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI playerSurnameText = headInstance.transform.Find("PlayerSurname").GetComponent<TextMeshProUGUI>();

            Transform playerRating = headInstance.transform.Find("PlayerRating");
            TextMeshProUGUI playerRatingText = playerRating.Find("PlayerRatingValue").GetComponent<TextMeshProUGUI>();

            Transform playerPosition = headInstance.transform.Find("PlayerPosition");
            TextMeshProUGUI playerPositionText = playerPosition.Find("PlayerPositionValue").GetComponent<TextMeshProUGUI>();

            Transform playerWage = headInstance.transform.Find("PlayerWage");
            TextMeshProUGUI PlayerWageText = playerWage.Find("PlayerWageValue").GetComponent<TextMeshProUGUI>();

            Transform playerCost = headInstance.transform.Find("PlayerCost");
            TextMeshProUGUI playerCostText = playerCost.Find("PlayerCostValue").GetComponent<TextMeshProUGUI>();

            Transform playerGamePlayed = headInstance.transform.Find("PlayerGamePlayed");
            TextMeshProUGUI playerGamePlayedText = playerGamePlayed.Find("GamesPlayedValue").GetComponent<TextMeshProUGUI>();

            Sprite sprite = Resources.Load<Sprite>(imagePath);
            iconClub.sprite = sprite;
            playerNumberText.text = player.Rating.ToString();
            clubNameText.text = teamList.Name;
            playerNameText.text = player.Person.Name;
            playerSurnameText.text = player.Person.Surname;
            playerRatingText.text = player.Rating.ToString();
            playerPositionText.text = player.Position.Code;
                    
            Contract contract = contractRepository.RetrieveOne(player.ContractID);
            PlayerWageText.text = FormatCurrency(playerPriceCalculator.GetPlayerSalary(player));
            playerCostText.text = FormatCurrency(playerPriceCalculator.GetPlayerPrice(player));
            playerGamePlayedText.text = (resultTable.Wins+ resultTable.Loses + resultTable.Draws).ToString() + "/"+ (resultTable.Wins + resultTable.Loses + resultTable.Draws).ToString();
        }
    } 
}