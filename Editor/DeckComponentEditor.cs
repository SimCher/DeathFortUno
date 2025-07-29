using System;
using DeathFortUnoCard.UI.Scripts.Components;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(DeckComponent), true)]
public class DeckComponentEditor : Editor
{
    private static Color FromColorToString(string color) =>
        color switch
        {
            "red" => Color.red,
            "yellow" => Color.yellow,
            "green" => Color.green,
            "blue" => Color.blue,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        };

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var deck = (DeckComponent) target;

        if (deck.Count == 0)
        {
            EditorGUILayout.LabelField("В колоде нет карт.");
            return;
        }
        
        EditorGUILayout.LabelField("Колода:");

        var count = deck.Count;

        var rows = count /= 2;
        var cols = count;
        var total = rows * cols;

        for (int i = 0; i < total; i++)
        {
            if (i % cols == 0)
            {
                EditorGUILayout.BeginHorizontal();
            }

            if (i < count)
            {
                var print = deck.Deck.Cards[i].GetPrint();
                var style = new GUIStyle(GUI.skin.label)
                {
                    normal =
                    {
                        textColor = FromColorToString(print.color)
                    }
                };
                EditorGUILayout.LabelField(new GUIContent(print.print), style, GUILayout.Width(20));
            }
            else
            {
                EditorGUILayout.LabelField("", GUILayout.Width(20));
            }
            
            if((i + 1) % cols == 0)
                EditorGUILayout.EndHorizontal();
        }
    }
}