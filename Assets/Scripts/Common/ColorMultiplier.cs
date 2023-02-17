using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class ColorMultiplier : MonoBehaviour
{
    public Color BaseColor = Color.white;

    public List<ColorEntry> Entries;

    [System.Serializable]
    public class ColorEntry
    {
        public string Name;
        public Color Color = Color.white;
        [AllowNesting]
        [Range(0, 1)]
        public float Intensity = 1.0f;
        public List<SpriteRenderer> TargetRenderers;
        private Color lastColor;
        private float lastIntensity = 0;
        public bool Changed => (Color != lastColor || Intensity != lastIntensity);
        public void Apply()
        {
            foreach (var r in TargetRenderers)
            {
                lastColor = Color;
                lastIntensity = Intensity;
                if (lastIntensity == 0) return;
                r.color *= Intensity == 1 ? lastColor : Color.Lerp(Color.white, lastColor, lastIntensity);
            }
        }
    }


    // Update is called once per frame
    [ExecuteInEditMode]
    void Update()
    {
        var changed = new List<SpriteRenderer>();
        foreach (var c in Entries) if (c.Changed)
                foreach (var r in c.TargetRenderers)
                    if (!changed.Contains(r))
                    {
                        changed.Add(r);
                        r.color = BaseColor;
                    }
        if (changed.Count == 0) return;
        foreach (var c in Entries)
        {
            foreach (var r in c.TargetRenderers)
            {
                if (changed.Contains(r)) c.Apply();
            }
        }
    }
}
