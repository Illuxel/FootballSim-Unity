using System.IO;

using UnityEngine;
using UnityEditor;

using RemoteAccessFiles;

namespace EditorLayer
{
    public class FilesSyncDialog : ScriptableObject
    {
        [MenuItem("Dev/Sync all")]
        static void PromtDialogSyncAll()
        {
            var download = EditorUtility.DisplayDialog(
                "Sync All",
                "Do you want to sync newest files?\nIt will override old.",
                "Sync",
                "Exit");

            var remote = new RemoteDownloader();

            var pathDB = Path.Combine(Application.persistentDataPath, "Database");
            var pathDLLs = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "DLL");

            if (download)
            {
                remote.DatabaseDownload(pathDB);
                remote.DbLayerDownload(pathDLLs);
                remote.BlLayerDownload(pathDLLs);
            }
        }

        [MenuItem("Dev/Database/Sync")]
        static void PromtDialogSyncDB()
        {
            var download = EditorUtility.DisplayDialog(
                "Database Sync", 
                "Do you want to sync newest database?\nIt will override old.",
                "Sync", 
                "Exit");

            if (download)
            {
                var path = Path.Combine(Application.persistentDataPath, "Database");
                new RemoteDownloader().DatabaseDownload(path);
            }
        }

        [MenuItem("Dev/DLLs/DatabaseLayer")]
        static void PromtDialogSyncDBLayer()
        {
            var download = EditorUtility.DisplayDialog(
                "DatabaseLayer Sync",
                "Do you want to sync newest DatabaseLayer.dll?\nIt will override old.",
                "Sync",
                "Exit");

            if (download)
            {
                var path = Path.Combine(Application.dataPath, "DLL");
                new RemoteDownloader().DbLayerDownload(path);
            }
        }
        [MenuItem("Dev/DLLs/BusinessLayer")]
        static void PromtDialogSyncBusinessLayer()
        {
            var download = EditorUtility.DisplayDialog(
                "BusinessLayer Sync",
                "Do you want to sync newest BusinessLayer.dll?\nIt will override old.",
                "Sync",
                "Exit");

            if (download)
            {
                var path = Path.Combine(Application.dataPath, "DLL");
                new RemoteDownloader().BlLayerDownload(path);
            }
        }
    }
}