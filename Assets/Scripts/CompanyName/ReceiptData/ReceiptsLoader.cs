using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CompanyName.ReceiptData
{
    public class ReceiptsLoader : MonoBehaviour
    {
        private const int ReceiptResourcesCount = 7;
        private readonly List<ResourceType> _resources = new List<ResourceType>();

        private void Start()
        {
            StartCoroutine(LoadLocalization(list =>
            {
                StringBuilder sb = new StringBuilder();
                foreach (Receipt receipt in list)
                {
                    sb.Append(receipt.Result).Append("=");
                    foreach (ResourceType resource in receipt.Resources)
                        sb.Append(resource).Append(",");
                    sb.Append(receipt.CraftTime).Append(",").Append(receipt.DropChance);
                }

                Debug.Log(sb.ToString());
            }));
        }

        private IEnumerator LoadLocalization(Action<List<Receipt>> onComplete)
        {
            TextAsset textAsset;
            yield return textAsset = Resources.Load("receipts") as TextAsset;

            onComplete?.Invoke(ParsingText(textAsset.text));
        }

        private List<Receipt> ParsingText(string text)
        {
            var receipts = new List<Receipt>();

            if (string.IsNullOrEmpty(text))
                throw new Exception("Parsing text is empty!");

            string[] lines = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            // i = 0 - titles line
            for (int i = 1; i < lines.Length; i++)
            {
                List<string> columns = lines[i]
                    .Split('	')
                    .Select(str => str.Trim())
                    .Select(Regex.Unescape)
                    .ToList();

                _resources.Clear();

                for (int columnIndex = 0; columnIndex < ReceiptResourcesCount; columnIndex++)
                {
                    string value = columns[columnIndex];

                    if (string.IsNullOrEmpty(value))
                        break;

                    Enum.TryParse(value, true, out ResourceType type);
                    _resources.Add(type);
                }

                Enum.TryParse(columns[ReceiptResourcesCount + 1], true, out ResourceType resultType);

                receipts.Add(new Receipt(
                    _resources.ToArray(),
                    float.Parse(columns[ReceiptResourcesCount + 0]),
                    0,
                    // float.Parse(columns[ReceiptResourcesCount + 1]),
                    resultType
                ));
            }

            return receipts;
        }
    }
}