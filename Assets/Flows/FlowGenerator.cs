using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Common;
using NaughtyAttributes;

namespace USoil {
    [DefaultExecutionOrder(10000)]
    public class FlowGenerator : MonoBehaviour
    {
        [SerializeField]
        protected Space space;

        // quadrant will have 2^qS + 1 points including edge points  (1 => 3, 2 => 5, 3 => 9)
        // or 2^qS -1 internal
        // or 2^qS internal + one of 2 edges
        [SerializeField]
        protected int quadrantSubdivisions = 3;

        [SerializeField]
        protected float QPercentDistanceFromStartPos = 1f / 5f;

        protected List<Vector3> points = new List<Vector3>();

        public DynamicCurve CurveHolder;

        [ReadOnly]
        public List<Quadrant> QuadrantsWithFlow;

        [ShowNonSerializedField]
        private bool completelyOutOfSpace = false;

        List<Vector3> SubDivide(Vector3 p1, Vector3 p2)
        {
            var res = new List<Vector3>();
            return res;
        }

        List<Vector3> SubDivide(List<Vector3> points, int depth = 1, bool addFirstPoint = true)
        {
            var res = new List<Vector3>();
            if (addFirstPoint) res.Add(points[0]);
            for(int i = 1; i < points.Count; i++)
            {
                var a = points[i - 1];
                var c = points[i];
                var b = PointInShpereBetween(a, c);                
                res.Add(b);
                res.Add(c);
            }
            return depth <= 1 ? res : SubDivide(res, depth - 1);            
        }

        Vector3 PointInShpereBetween(Vector3 p1, Vector3 p2)
        {
            var r = (p1 - p2).magnitude/2;
            var center = p1 + p2 / 2;
            var p = Random.insideUnitSphere * r * 0.6f + center;
            /*
            if ((p1 - p).magnitude < r / 10f || (p2 - p).magnitude < r/10f) 
                p += (center - p) * r / 10f;
            */
            return p;                
        }

        private List<Vector3> GenerateFromStart()
        {
            var startQuadrant = space.StartQuadrant;
            

            // todo add dir1 to ignored coz appent generation may fill it
            // todo 2 make prepend generation
            var dirBackward = Dir3D.Random;
            var dirForward = Dir3D.RandomOtherThan(dirBackward);
            
            var p1 = startQuadrant.WorldSidePos(dirBackward, 0.9999f);
            var p2 = startQuadrant.Root.transform.position + Random.onUnitSphere * space.QuadrantSize * QPercentDistanceFromStartPos;
            var p3 = startQuadrant.WorldSidePos(dirForward, 0.9999f);

            var q0Points = SubDivide(new List<Vector3>() { p1, p2, p3 }, quadrantSubdivisions - 1).Select(p => startQuadrant.ClampPoint(p, 0.9999f));            
            var backQuadrant = startQuadrant.Neighbour(dirBackward);

            QuadrantsWithFlow = new List<Quadrant>() { startQuadrant };
            // temporary add backQuadrant to ignored to avoid filling it by forward generator, because then we have no out for backward part of flow
            QuadrantsWithFlow.Add(backQuadrant);
            var p = GenerateForNextQuadrantRecursive(q0Points, startQuadrant.Neighbour(dirForward), -dirForward, QuadrantsWithFlow);
            QuadrantsWithFlow.Remove(backQuadrant);
            p = GenerateForNextQuadrantRecursive(p, startQuadrant.Neighbour(dirBackward), -dirBackward, QuadrantsWithFlow, false);
            return p;
        }
                
        void Start()
        {
            
        }

        [Button]
        public void Init()
        {
            points = GenerateFromStart();
            CurveHolder.SetPoints(points);
        }
        
        List<Vector3> GenerateForNextQuadrantRecursive(IEnumerable<Vector3> points, Quadrant q, Vector3Int inSide, List<Quadrant> ignored, bool append = true)
        {
            
            ignored.Add(q);

            Vector3Int outSide;
            Quadrant next;
            int i = 0;
            do // find outSide with NULL (space end) or non-ignored quadrant
            {
                outSide = Dir3D.RandomOtherThan(inSide);
                next = q.Neighbour(outSide);
                i++;
                
            } while (next != null && ignored.Contains(next) & i < 100);
            if (i == 100)
            {
                Debug.LogError("Inf loop 1");
                return points.ToList();
            }
            //Debug.LogError("Next:" + ((next == null) ? "null" : next.AbsolutePos.ToString()) + "dir:" + outSide);

            var qPoints = SubDivide(
                new List<Vector3> { q.WorldSidePos(inSide), q.WorldSidePos(outSide) }, 
                quadrantSubdivisions,
                false
            ).Select(p => q.ClampPoint(p,0.999f));
            
            
            var result = new List<Vector3>();
            if (append)
            {
                result.AddRange(points);
                result.AddRange(qPoints);
            } else
            {
                qPoints.Reverse();
                result.AddRange(qPoints);
                result.AddRange(points);
            }
                        
            return (next == null) ? result : GenerateForNextQuadrantRecursive(result, next, -outSide, ignored, append);
        }        

        public void OnCompletelyOutOfSpace(Vector3Int movDir)
        {
            Debug.LogError("Flow completely moved out of space");
            //completelyOutOfSpace = true;
            GenerateNewInDirection(movDir);
        }

        public void GenerateNewInDirection(Vector3Int dir)
        {
            var refreshedQuadrants = space.QuadrantsInDir(dir).ToList();
            var start = refreshedQuadrants[Random.Range(0, refreshedQuadrants.Count - 1)];
            
            Vector3Int startDir;
            do
            {
                startDir = Dir3D.Random;
            } while (start.Neighbour(startDir) != null);
            var p = new List<Vector3> { start.WorldSidePos(startDir, 0.999f) };
            var ignored = space.QuadrantsNotInDir(dir).ToList();
            points = GenerateForNextQuadrantRecursive(p, start, -startDir, ignored, append: true);
            RecalculateQuadrantsWithFlow();            
            CurveHolder.SetPoints(points);
        }        

        void RecalculateQuadrantsWithFlow()
        {
            QuadrantsWithFlow = space.quadrantsList.Where(q => points.Any(p => q.ContainsPoint(p))).ToList();
        }

        public void OnQuadrantChange(Vector3Int dir)
        {
            if (completelyOutOfSpace) return;
            Debug.Log($"Flow.OnQuadrantChange, Direction: " + dir.ToString());
            // Quadrant that was shifted in space due to space looping
            var refreshedQuadrants = space.QuadrantsInDir(dir);
            var notRefreshedQuadrants = space.QuadrantsNotInDir(dir).ToList();

            bool needToRemoveFlowParts = false;
            // Remove refreshed quadrants from list of quadrants having flow
            foreach (var q in refreshedQuadrants)
            {
                
                if (QuadrantsWithFlow.Contains(q))
                {
                    Debug.Log($"Refreshed Q: {q.Root.name} Had flow" );
                    QuadrantsWithFlow.Remove(q);
                    needToRemoveFlowParts = true;
                }              
            }
            List<Vector3> newPoints;
            if (needToRemoveFlowParts)
            {
                Debug.Log($"Removing flow points out of space...");
                // Remove all points that shifted outside of observable space
                newPoints = new List<Vector3>();
                foreach (var p in points) if (Space.Inst.InObservableSpace(p)) newPoints.Add(p);                
                Debug.Log($"Old points qty {points.Count}: after removing looped quadrants:{newPoints.Count}");
                if (newPoints.Count == 0)
                {                    
                    OnCompletelyOutOfSpace(dir);
                    return;
                }
            } else
            {
                Debug.Log($"Flow points rmoving not required");
                newPoints = points;
            }
            
            // ============ Generate Flow Backward

            // Find Actual Flow Start quadrant, out direction and corresponding neighbour
            var q1 = Space.Inst.GetQuadrant(newPoints[0]);
            Debug.Log($"Q1: {q1.Root.name}, newpoints[0]: {newPoints[0].ToString()}");
            var q1OutDir = q1.ClosestSide(newPoints[0]);
            var q1Next = q1.Neighbour(q1OutDir);
            
            var newFilledQuadrants = new List<Quadrant>();

            // If that neighbour exists and it is not filled we should generate flow that side
            if (q1Next != null && !QuadrantsWithFlow.Contains(q1Next))
            {
                // But we can generate flow only in refreshed quadrants, so we ignore all not refreshed
                var ignoredQuadrants = new List<Quadrant>(notRefreshedQuadrants);
                newPoints = GenerateForNextQuadrantRecursive(newPoints, q1Next, -q1OutDir, ignoredQuadrants, append:false);
                // now we need to check which of new quadrants now have a flow and write it to QuadrantsWithFlow
                
                foreach (var q in refreshedQuadrants) if (ignoredQuadrants.Contains(q)) newFilledQuadrants.Add(q);
                //QuadrantsWithFlow.AddRange(newFilledQuadrants);
            }

            // ============ Generate Flow Forward
            
            var q2 = Space.Inst.GetQuadrant(newPoints[newPoints.Count-1]);
            var q2OutDir = q2.ClosestSide(newPoints[newPoints.Count - 1]);
            var q2Next = q2.Neighbour(q1OutDir);
            
            if (q2Next != null && !QuadrantsWithFlow.Contains(q2Next))
            {
                // In second part of generation we exclude not refreshed quadrants + quadrants filled on prev. step
                var ignoredQuadrants = new List<Quadrant>(notRefreshedQuadrants);
                ignoredQuadrants.AddRange(newFilledQuadrants);
                //var ignoredQuadrants = Space.Inst.quadrantsList.Where(q => !refreshedQuadrants.Contains(q) && !QuadrantsWithFlow.Contains(q)).ToList();

                newPoints = GenerateForNextQuadrantRecursive(newPoints, q2Next, -q2OutDir, ignoredQuadrants, append:true);
                // now we also consider that quadrant may be already filled with flow from code above
                //foreach (var q in refreshedQuadrants) if (ignoredQuadrants.Contains(q) && !QuadrantsWithFlow.Contains(q)) QuadrantsWithFlow.Add(q);
            }

            Debug.Log($"Flow.OnQuadrantChange, updating CurveHandler, points before: {points.Count}; after: {newPoints.Count}");
            points = newPoints;
            RecalculateQuadrantsWithFlow();
            CurveHolder.SetPoints(points);
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
