using UnityEngine;
using System;
using UnityEngine.UI;
using DatabaseLayer;
using System.Collections.Generic;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Rules;

public class PositionController : MonoBehaviour
{
    public Dropdown SchemeDropdown,TacticDropdown;
    public GameObject[] SceneObjects;
    private Dictionary<string, GameObject> activeObjects = new Dictionary<string, GameObject>();

    private void Start()
    {
        SchemeDropdown.onValueChanged.AddListener(onDropdownValueChanged);
        //activeObjects = GameObject.FindGameObjectsWithTag("PlayerPositions").ToDictionary(key => key.name, value => value);


        SchemeDropdown.AddOptions(getTacticsSchema());
        TacticDropdown.AddOptions(getStrategyType());
        activateSceneObjects(0);
    }

    public List<string> getTacticsSchema()
    {
        var result = new List<string>();
        foreach (TacticSchema schema in Enum.GetValues(typeof(TacticSchema)))
        {
            result.Add(EnumDescription.GetEnumDescription(schema));
        }
        return result;
    }

    public List<string> getStrategyType()
    {
        var result = new List<string>();
        foreach (StrategyType schema in Enum.GetValues(typeof(StrategyType)))
        {
            result.Add(EnumDescription.GetEnumDescription(schema));
        }
        return result;
    }

    private void onDropdownValueChanged(int index)
    {
        
        deactivateInactiveObjects();

        activateSceneObjects(index);
    }

    private void deactivateInactiveObjects()
    {
        foreach (KeyValuePair<string, GameObject> kvp in activeObjects)
        {
            kvp.Value.SetActive(false);
        }
    }

    private void activateSceneObjects(int index)
    {
        //string[] positions = GetPositionsByIndex(index);
        TacticSchema tacticSchema = (TacticSchema)index;

        var tacticSchemaFactory = new TacticSchemaFactory();
        var positions = tacticSchemaFactory.GetPlayersPosition(tacticSchema);
        foreach (var positionKey in positions.Keys)
        {
            var position = positionKey.ToString();
            if (!activeObjects.ContainsKey(position))
            {
                GameObject obj = FindObjectByName(position);
                if (obj != null)
                {
                    obj.SetActive(true);
                    activeObjects[position] = obj;
                }
            }
            else
            {
                activeObjects[position].SetActive(true);
            }
        }
    }

    private string[] GetPositionsByIndex(int index)
    {
        switch (index)
        {
            case 0:
                return new string[] { "1", "6", "5", "3", "2", "16", "14", "12", "24", "22", "20" };
            case 1:
                return new string[] { "1", "6", "5", "3", "2", "16", "15", "13", "12", "25", "26" };
            case 2:
                return new string[] { "1", "6", "5", "3", "2", "16", "15", "14", "13", "12", "22" };
            case 3:
                return new string[] { "1", "6", "4", "2", "16", "15", "14", "13", "12", "23", "21" };
            case 4:
                return new string[] { "1", "6", "5", "3", "2", "9", "12", "13", "15", "16", "22" };
            case 5:
                return new string[] { "1", "6", "5", "3", "2", "16", "15", "13", "12", "18", "22" };
            default:
                return new string[] { };
        }
    }

    private GameObject FindObjectByName(string name)
    {
        foreach (GameObject obj in SceneObjects)
        {
            if (obj.name == name)
            {
                return obj;
            }
        }
        return null;
    }

    private bool IsObjectActiveInCurrentScheme(string name)
    {
        string[] positions = GetPositionsByIndex(SchemeDropdown.value);
        return Array.IndexOf(positions, name) >= 0;
    }
}


