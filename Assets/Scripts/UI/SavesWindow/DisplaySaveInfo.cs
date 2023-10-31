using System.IO;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using DatabaseLayer.Enums;
using DatabaseLayer.Repositories;
using DatabaseLayer.Services;

public class DisplaySaveInfo : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer clubIconSprite;

    [SerializeField]
    private TextMeshProUGUI saveName, saveCreationTime, saveRecentActivity, clubName, currentRole;

    public Button startSave;

    public void SetSaveInfo(SaveInfo saveInfo)
    {
        var team = new TeamRepository().Retrieve(saveInfo.PlayerData.ClubId);

        saveName.text = saveInfo.SaveName;
        clubName.text = team.ExtName;

        switch (saveInfo.PlayerData.Role)
        {
            case UserRole.Scout:
                currentRole.text = "Scout";
                break;
            case UserRole.Manager:
                currentRole.text = "Manager";
                break;
            case UserRole.Director:
                currentRole.text = "Director";
                break;
        }

        string[] files = Directory.GetFiles("Assets/Resources/FCLogos/", team.ExtName + ".png", SearchOption.AllDirectories);
        string imagePath = files[0];
        imagePath = imagePath.Replace(".png", "");
        string keyword = "FCLogos";
        int index = imagePath.IndexOf(keyword);
        string result = imagePath.Substring(index);

        clubIconSprite.sprite = Resources.Load<Sprite>(result);

        saveCreationTime.text = saveInfo.SaveCreationTime.ToString();
        saveRecentActivity.text = saveInfo.SaveLastActivity.ToString();
        //startSave.onClick.AddListener(() => LoadSaveScene(i));
    }
}
