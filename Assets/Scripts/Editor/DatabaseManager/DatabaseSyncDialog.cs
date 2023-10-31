using UnityEngine;
using UnityEditor;

using BusinessLogicLayer.Services;

// можна додати також upload
namespace EditorLayer
{
    public class DatabaseSyncDialog : ScriptableObject
    {
        [MenuItem("Database/Sync")]
        static void PromtDialogSyncDB()
        {
            var download = EditorUtility.DisplayDialog(
                "Database Sync", 
                "Do you want to sync newest database?\nIt will override old one.", 
                "Fetch", 
                "Exit");

            if (download)
            {
                _ = new RemoteDownloader().DatabaseDownload();
            }
        }
    }
}