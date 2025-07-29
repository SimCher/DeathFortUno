using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.Extensions
{
    public static class GameObjetExtensions
    {
        public static void InitValuesForPooled(this GameObject obj, Vector3 position, Quaternion rotation,
            bool isLocal = false)
        {
            if (isLocal)
            {
                var t = obj.transform;
                t.localPosition = position;
                t.localRotation = rotation;
            }
            else
                obj.transform.SetPositionAndRotation(position, rotation);
        }
    }
}