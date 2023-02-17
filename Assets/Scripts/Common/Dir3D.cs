using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public static class Dir3D
    {
        public enum Axis { Undefined,x,y,z };
        public static Vector3Int Random => All[UnityEngine.Random.Range(0, All.Count)];
        
        public static Vector3Int RandomOtherThan(Vector3Int other)
        {
            Vector3Int res;
            do res = Random; while (res == other);
            return res;
        }

        public static List<Vector3Int> All = new List<Vector3Int> {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right,
            Vector3Int.forward,
            Vector3Int.back
        };        

        public static Axis GetAxis(this Vector3Int dir)
        {
            if (dir == Vector3Int.left || dir == Vector3Int.right) return Axis.x;
            if (dir == Vector3Int.up || dir == Vector3Int.down) return Axis.y;            
            if (dir == Vector3Int.back || dir == Vector3Int.forward) return Axis.z;
            return Axis.Undefined;
        }

        public static readonly Dictionary<Vector3Int, Axis> Axes = new Dictionary<Vector3Int, Axis>() {
            { Vector3Int.left, Axis.x },
            { Vector3Int.right, Axis.x },
            { Vector3Int.up, Axis.y },
            { Vector3Int.down, Axis.y },
            { Vector3Int.forward, Axis.z },
            { Vector3Int.back, Axis.z },
        };
        public static int GetValueInAxis(this Vector3Int vec, Axis axis)
        {
            switch(axis)
            {
                case Axis.x: return vec.x;
                case Axis.y: return vec.y;
                case Axis.z: return vec.z;
            }
            throw new System.Exception("Wrong Axis");
        }

    }
}