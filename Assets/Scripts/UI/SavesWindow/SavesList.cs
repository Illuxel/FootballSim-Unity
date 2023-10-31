using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using DatabaseLayer.Enums;
using DatabaseLayer.Repositories;
using DatabaseLayer.Services;

public class SavesList : MonoBehaviour
{
    [SerializeField]
    private GameObject saveSlotPrefab;

    [SerializeField]
    private Transform scrollViewContent;

    void Start()
    {
        var savesManager = SavesManager.GetInstance();
        var saves = savesManager.GetAllSaves();
        
        for (int i = 0; i < saves.Count; i++)
        {

        }
    }
}
