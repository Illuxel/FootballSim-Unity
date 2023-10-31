using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DatabaseLayer;
using DatabaseLayer.Repositories;
using BusinessLogicLayer.Services;
using UnityEngine.UI;
using System;

public class PlayerFullStat : MonoBehaviour
{
    public GameObject FullStatprefab;
    public TextMeshProUGUI MyMy;
    public Transform contentTransform;
    public GoalAssistTracker goalAssistTracker;
    public PlayerFullStatDop playerFullStatDop;

    public void OnPlayerNameClicked()
    {
        //Debug.Log("Selected Team: " + MyMy.text);

        Transform myPlayersCanvas = GameObject.Find("Canvas").GetComponent<Transform>();
        Transform myPlayersWindow = myPlayersCanvas.Find("PlayerWindow").GetComponent<Transform>();
        Transform StatisticWindow = myPlayersCanvas.Find("StatisticWindow");
        Transform CloseButton = myPlayersWindow.Find("CloseButton");
        Transform CloseButtonCanvas = myPlayersCanvas.Find("CloseButton");
        //List<PlayerStatistic> playerStatistics = goalAssistTracker.GetPlayerStatistic("0add1a5d-6101-4d27-95c5-0b6df1626753");
        //Debug.Log(playerStatistics.Count);
        StatisticWindow.gameObject.SetActive(false);
        myPlayersWindow.gameObject.SetActive(true);
        //CloseButton.gameObject.SetActive(true);
        CloseButtonCanvas.gameObject.SetActive(false);
       
       
        SetStats(MyMy.text);
    }

    public void SetStats(string PlayerID)
    {
     playerFullStatDop = new PlayerFullStatDop();
        PlayerRepository playerRepository = new PlayerRepository();
        Player player = playerRepository.RetrieveOne(PlayerID);

        Transform myPlayersCanvas = GameObject.Find("Canvas").GetComponent<Transform>();
        Transform myPlayersWindow = myPlayersCanvas.Find("PlayerWindow").GetComponent<Transform>();

        TextMeshProUGUI PlayerName = myPlayersWindow.Find("PlayerName").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Role = myPlayersWindow.Find("Role").GetComponent <TextMeshProUGUI>();
        TextMeshProUGUI Status = myPlayersWindow.Find("Status").GetComponent<TextMeshProUGUI>();

        Transform ClubFrame = myPlayersWindow.transform.Find("ClubFrame");
        TextMeshProUGUI ClubName = ClubFrame.Find("ClubName").GetComponent<TextMeshProUGUI>();

        Transform NationFrame = myPlayersWindow.transform.Find("NationFrame");
        TextMeshProUGUI NationName = NationFrame.Find("NationName").GetComponent<TextMeshProUGUI>();
        //SpriteRenderer NationalIcon = NationFrame.Find("Icon").GetComponent<SpriteRenderer>();
        Transform AgeFrame = myPlayersWindow.transform.Find("AgeFrame");
        TextMeshProUGUI AgeName = AgeFrame.Find("AgeName").GetComponent <TextMeshProUGUI>();

        Transform PriceFrame = myPlayersWindow.transform.Find("PriceFrame");
        TextMeshProUGUI PriceName = PriceFrame.Find("PriceName").GetComponent<TextMeshProUGUI>();

        Transform WageFrame = myPlayersWindow.transform.Find ("WageFrame");
        TextMeshProUGUI WageName = WageFrame.Find("WageName").GetComponent<TextMeshProUGUI>();

        Transform ContractFrame = myPlayersWindow.transform.Find("ContractFrame");
        TextMeshProUGUI ContractName = ContractFrame.Find("ContractName").GetComponent<TextMeshProUGUI>();

        Transform PunchFrame = myPlayersWindow.transform.Find("PunchFrame");
        TextMeshProUGUI PunchValue = PunchFrame.Find("PunchValue").GetComponent<TextMeshProUGUI> ();

        Transform DefendingFrame = myPlayersWindow.transform.Find("DefendingFrame");
        TextMeshProUGUI DefendingValue = DefendingFrame.Find("DefendingValue").GetComponent<TextMeshProUGUI>();

        Transform PhysicalFrame = myPlayersWindow.transform.Find("PhysicalFrame");
        TextMeshProUGUI PhysicalValue = PhysicalFrame.Find("PhysicalValue").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI PhysicalText = PhysicalFrame.Find("PhysicalText").GetComponent<TextMeshProUGUI>();

        Transform TechnicalFrame = myPlayersWindow.transform.Find("TechnicalFrame");
        TextMeshProUGUI TechnicalValue = TechnicalFrame.Find("TechnicalValue").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI TechnicalText = TechnicalFrame.Find("TechnicalText").GetComponent<TextMeshProUGUI>();

        Transform PassingFrame = myPlayersWindow.transform.Find("PassingFrame");
        TextMeshProUGUI PassingValue = PassingFrame.Find("PassingValue").GetComponent <TextMeshProUGUI>();

        Transform SpeedFrame = myPlayersWindow.transform.Find("SpeedFrame");
        TextMeshProUGUI SpeedValue = SpeedFrame.Find("SpeedValue").GetComponent<TextMeshProUGUI>();

       

        PlayerName.text = player.Person.Name + " " + player.Person.Surname;
        Role.text = player.Position.Name;
        TextMeshProUGUI RatingValue = myPlayersWindow.Find("Rating").GetComponent<TextMeshProUGUI>();
        RatingValue.text = player.CurrentPlayerRating.ToString();
        Status.text = player.InjuredTo;
        ClubName.text = CurrentContract(player.ContractID).Team.Name;
        CountryRepository countryRepository = new CountryRepository();
        Country country = countryRepository.Retrieve(player.Person.CountryID);

        NationName.text = country.Name;
        //AgeName.text = player.
        var contractRep = new ContractRepository();
        Contract contract = contractRep.RetrieveOne(player.ContractID);
        WageName.text = FormatCurrency(contract.Salary) + " $";
        PlayerPriceCalculator priceCalculator = new PlayerPriceCalculator();
        PriceName.text = FormatCurrency(priceCalculator.GetPlayerPrice(player)) + " $";
        
        ContractName.text = contract.DateFromString;
      
        SpeedValue.text = player.Speed.ToString();
        PunchValue.text = player.Strike.ToString();
        DefendingValue.text = player.Defending.ToString();
        PhysicalValue.text = player.Physics.ToString();
        TechnicalValue.text = player.Dribbling.ToString();
        PassingValue.text = player.Passing.ToString();
        if (player.PositionCode == "GK")
        {
            PhysicalText.text = "Position";
            TechnicalText.text = "Reflex";
        } 
        else {

            PhysicalText.text = "Physical";
            TechnicalText.text = "Dribling";
        }

        playerFullStatDop.RefreshDop(player.PersonID);

        Image Stamina = myPlayersWindow.Find("StaminaImage").GetComponent<Image>();
        Image Stamina1 = myPlayersWindow.Find("StaminaImage1").GetComponent<Image>();
        Image Stamina2 = myPlayersWindow.Find("StaminaImage2").GetComponent<Image>();
        Image Stamina3 = myPlayersWindow.Find("StaminaImage3").GetComponent<Image>();
        if (player.Endurance <= 100 && player.Endurance >= 75)
        {
            Stamina.gameObject.SetActive(true);
            Stamina1.gameObject.SetActive(false); Stamina2.gameObject.SetActive(false); Stamina3.gameObject.SetActive(false);
        }
        else if (player.Endurance <= 75 && player.Endurance >= 50)
        {

            Stamina1.gameObject.SetActive(true);
            Stamina.gameObject.SetActive(false);
            Stamina2.gameObject.SetActive(false);
            Stamina3.gameObject.SetActive(false);
        }
        else if (player.Endurance <= 50 && player.Endurance >= 25)
        {
            Stamina2.gameObject.SetActive(true);
            Stamina3.gameObject.SetActive(false);
            Stamina1.gameObject.SetActive(false);
            Stamina.gameObject.SetActive(false);
        }
        else
        {

            Stamina3.gameObject.SetActive(true);
            Stamina1.gameObject.SetActive(false);
            Stamina2.gameObject.SetActive(false);
            Stamina3.gameObject.SetActive(false);
        }
        
    }
    string FormatCurrency(double value)
    {
        double fvalue = value;
        if (fvalue >= 1000000)
        {
            double millions = fvalue / 1000000;
            return millions.ToString("F1") + "Ì";
        }
        else if (value >= 10000)
        {
            double thousands = fvalue / 1000;
            return thousands.ToString("F0") + "Ê";
        }
        else
        {
            return fvalue.ToString("F0");
        }
    }
    public static Contract CurrentContract(string playerId)
    {

        var contractRep = new ContractRepository();
        Contract contract = contractRep.RetrieveOne(playerId);
        if (contract == null)
        { Debug.Log("null"); }
        return contract;
    }
    // Start is called before the first frame update
    void Start()
    {
        goalAssistTracker = new GoalAssistTracker();
        FullStatprefab = new GameObject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
