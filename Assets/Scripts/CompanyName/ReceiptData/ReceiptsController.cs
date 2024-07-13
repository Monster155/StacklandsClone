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
    public class ReceiptsController : MonoBehaviour
    {
        private const int ReceiptResourcesCount = 7;
        private readonly List<ResourceType> _resources = new List<ResourceType>();
        private List<Receipt> _receipts = new List<Receipt>();

        private void Start()
        {
            StartCoroutine(LoadReceipts(list =>
            {
                _receipts = list;

                // StringBuilder sb = new StringBuilder();
                // foreach (Receipt receipt in list)
                // {
                //     sb.Append(receipt.Result).Append("=");
                //     foreach (ResourceType resource in receipt.Resources)
                //         sb.Append(resource).Append(",");
                //     sb.Append(receipt.CraftTime).Append(",").Append(receipt.DropChance).Append("\n");
                // }
                //
                // Debug.Log(sb.ToString());
            }));
        }

        public Receipt FindReceipt(List<ResourceType> resources)
        {
            resources.Sort();
            foreach (Receipt receipt in _receipts)
            {
                if (receipt.Resources.Length != resources.Count)
                    continue;

                bool isReceiptsEquals = true;
                for (int i = 0; i < resources.Count; i++)
                {
                    if (!resources[i].Equals(receipt.Resources[i]))
                    {
                        isReceiptsEquals = false;
                        break;
                    }
                }

                if (isReceiptsEquals)
                    return receipt;
            }

            return null;
        }

        private IEnumerator LoadReceipts(Action<List<Receipt>> onComplete)
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

                _resources.Sort();

                Enum.TryParse(string.Join("", columns[ReceiptResourcesCount + 2].Split(' ')), true, out ResourceType resultType);

                receipts.Add(new Receipt(
                    _resources.ToArray(),
                    int.Parse(columns[ReceiptResourcesCount + 0]),
                    int.Parse(columns[ReceiptResourcesCount + 1].Substring(0, columns[ReceiptResourcesCount + 1].Length - 1)) / 100f,
                    resultType
                ));
            }

            return receipts;
        }
    }
}