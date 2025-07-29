using DeathFortUnoCard.Scripts.CardField.Blocks.Collections;
using DeathFortUnoCard.Scripts.CardField.Cards.Collections;
using DeathFortUnoCard.Scripts.CardField.Generation;
// using DeathFortUnoCard.Scripts.Common.Players.AI.Collections;
using DeathFortUnoCard.Scripts.Common.States.Collections;
using DeathFortUnoCard.UI.Scripts.Collections;
using UnityEditor;

namespace DeathFortUnoCard.Editor
{
    [CustomPropertyDrawer(typeof(StringMaterialDictionary))]
    [CustomPropertyDrawer(typeof(CardViewDictionary))]
    [CustomPropertyDrawer(typeof(ColorGameIntDictionary))]
    [CustomPropertyDrawer(typeof(CardTypeIntDictionary))]
    [CustomPropertyDrawer(typeof(UIBlockDictionary))]
    [CustomPropertyDrawer(typeof(BlockViewDictionary))]
    [CustomPropertyDrawer(typeof(StateDictionary))]
    [CustomPropertyDrawer(typeof(TrapTextDictionary))]
    // [CustomPropertyDrawer(typeof(BehaviourDictionary))]
    public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer
    {
    }
}