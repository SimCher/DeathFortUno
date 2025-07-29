using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace DeathFortUnoCard.Scripts.Common.Loaders
{
    [DefaultExecutionOrder(-1000)]
    public class XMLLoader : MonoBehaviour
    {
        [SerializeField] private string fileName;
        [SerializeField] private string language = "ru";
        [SerializeField] private string rootTag;
        [SerializeField] private string textTag = "text";
        [SerializeField] private string idAttr = "id";

        public UnityEvent<string, List<TextSection>> onLoaded;

        private void Awake()
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Debug.LogError("fileName не назначен.");
                return;
            }

            Addressables.LoadAssetAsync<TextAsset>(fileName).Completed += OnCompleted;
        }

        private void OnCompleted(AsyncOperationHandle<TextAsset> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Не удалось загрузить XML-файл.");
                return;
            }

            var sections = Parse(handle.Result.text);

            onLoaded.Invoke(rootTag, sections);
        }

        private void OnDestroy()
        {
            onLoaded.RemoveAllListeners();
        }


        [CanBeNull]
        private List<TextSection> Parse(string xmlContent)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            var rootNode = xmlDoc.SelectSingleNode($"//{rootTag}");
            if (rootNode == null)
            {
                Debug.LogError($"Не найден корневой тег <{rootTag}>.");
                return null;
            }

            var result = new List<TextSection>();
            var sectionMap = new Dictionary<string, TextSection>(capacity: 8);
            var stack = new Stack<XmlNode>();
            stack.Push(rootNode);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                foreach (XmlNode child in current.ChildNodes)
                {
                    if (child.NodeType != XmlNodeType.Element)
                        continue;

                    if (child.Name == textTag)
                    {
                        if (child.Attributes?[idAttr] == null)
                            continue;

                        var id = child.Attributes?[idAttr].Value;
                        var langNode = child.SelectSingleNode(language);
                        if (langNode == null)
                            continue;
                        var sectionName = current.Name;

                        if (!sectionMap.TryGetValue(sectionName, out var section))
                        {
                            section = new TextSection(sectionName);
                            sectionMap[sectionName] = section;
                            result.Add(section);
                        }

                        section.entries.Add(new TextEntry(id, langNode.InnerText));
                    }
                    else
                    {
                        stack.Push(child);
                    }
                }
            }

            return result;
        }
    }
}