using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common {
    public static class RectExtentions
    {
        public static Vector3 Random(this Rect r, int z)
        {
            return new Vector3(
                UnityEngine.Random.Range(r.xMin, r.xMax),
                UnityEngine.Random.Range(r.xMin, r.xMax),
                z
            );
        }

    }
}
