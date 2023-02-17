using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Common
{
    public static class VectorExtentions
    {
        public static Vector2Int ToVector2Int(this Vector2 v)
        {
            return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }

        public static Vector3 ToVector3(this Vector2 v, float z = 0)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector2 ToVector2(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector3Int ToVector3Int(this Vector3 v)
        {
            return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
        }

        public static Vector3 ToVector3(this Vector3Int v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static float Random(this Vector2 v)
        {
            return UnityEngine.Random.Range(v.x, v.y);
        }

        public static int RandomRange(this Vector2Int v)
        {
            return UnityEngine.Random.Range(v.x, v.y+1);
        }

        public static float RandomRange(this Vector2 v)
        {
            return UnityEngine.Random.Range(v.x, v.y);
        }
    }
}

