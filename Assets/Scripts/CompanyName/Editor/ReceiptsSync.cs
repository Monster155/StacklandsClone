using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CompanyName.ReceiptData;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace CompanyName.Editor
{
    public class ReceiptsSync : MonoBehaviour
    {
        private const string TableId = "1MaEvo-LMSjRUDINcaMwaX0u3RiExrmKZgYkzxgXRFPQ";
        private const string SheetId = "2062391938";
        private const string URL = "https://docs.google.com/spreadsheets/d/{0}/export?format=tsv&gid={1}";

        private static readonly string FilePath =
            Path.Combine(Path.Combine(Application.dataPath, "Resources"), "receipts.txt");

        private static string FinalURL => string.Format(URL, TableId, SheetId);


        [MenuItem("Tools/Receipts/Sync")]
        public static async void SyncTranslates()
        {
            UnityWebRequest request = UnityWebRequest.Get(FinalURL);
            request.SendWebRequest();

            while (!request.isDone)
            {
                await Task.Delay(100);
                //Wait each frame in each loop OR Unity would freeze
                //yield return null;
            }

            SaveToFile(request.downloadHandler.text);
            Debug.Log("Loaded Receipts");
        }

        private static void SaveToFile(string text)
        {
            File.WriteAllText(FilePath, text);
            AssetDatabase.Refresh();
        }
    }
}