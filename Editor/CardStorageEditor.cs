using System.Linq;
using DeathFortUnoCard.Scripts.CardField.Cards.Data;
using DeathFortUnoCard.Scripts.CardField.Cards.Storages;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


[CustomEditor(typeof(CardStorage))]
public class CardStorageEditor : Editor
{
    private CardStorage _target;

    private void Clear()
    {
        _target = target as CardStorage;

        var cards = _target.AllCardData;
        
        ArrayUtility.Clear(ref cards);
        
        _target.SetCards(cards);

        _target = null;
    }

    private void SearchCardData(string subFolderName)
    {
        var cardGuids = AssetDatabase.FindAssets($"t:{nameof(CardData)}",
            new[] {$"Assets/DeathFortUnoCard/Data/Cards/{subFolderName}"});

        var cardDatas = cardGuids
            .Select(guid => AssetDatabase.LoadAssetAtPath<CardData>(AssetDatabase.GUIDToAssetPath(guid)))
            .ToArray();

        var newCardData = _target.AllCardData;
        
        Debug.Log($"Нашёл {newCardData.Length} карт!");

        foreach (var cardData in cardDatas)
        {
            if (newCardData.All(data => data != cardData))
            {
                ArrayUtility.Add(ref newCardData, cardData);
            }
        }
        
        _target.SetCards(newCardData);
        
        EditorUtility.SetDirty(_target);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (targets.Length > 1)
        {
            EditorGUILayout.HelpBox("Мультиредактирование объектов не поддерживается", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Найти"))
        {
            _target = target as CardStorage;
            SearchCardData("Reds");
            SearchCardData("Greens");
            SearchCardData("Blues");
            SearchCardData("Yellows");
        }
        
        if(GUILayout.Button("Очистить"))
            Clear();
    }
}