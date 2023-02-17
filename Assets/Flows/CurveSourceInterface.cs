using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CurveSourceInterface
{
    public Curve.Curve Curve();
    public void RegisterOnCurveChanged(System.Action Handler);
    public void UnregisterOnCurveChanged(System.Action Handler);
}
