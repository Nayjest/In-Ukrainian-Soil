using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Common;
using NaughtyAttributes;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace USoil
{
	[System.Serializable]
	public class Quadrant
    {
		public Vector3Int AbsolutePos;
		public Vector3Int RelativePos;
		public List<GameObject> Objects;
		public GameObject Root;
		public Quadrant(Vector3Int pos, List<GameObject> objects = null, Transform parent = null)
        {
			
			Root = new GameObject($"Quadrant[{pos.x},{pos.y},{pos.z}]");
			Root.transform.parent = parent;
			SetAbsolutePos(pos);
			RelativePos = pos;
			SetObjects(objects ?? new List<GameObject>());
        }

		public void SetObjects(List<GameObject> objects, bool worldPosStays = false)
        {
			Objects = objects;
			foreach (var obj in Objects)
			{
				//obj.transform.position += Root.transform.position;
				obj.transform.SetParent(Root.transform, worldPosStays);
			}
		}

		public Vector3 WorldCenterPos => Root.transform.position;

		public Vector3 WorldSidePos(Vector3 direction, float relativeScale = 1) => Root.transform.position + direction * relativeScale * Space.Inst.QuadrantSize / 2f;

		public Vector3Int ClosestSide(Vector3 worldPos) => Dir3D.All.OrderBy(dir => (WorldSidePos(dir) - worldPos).magnitude).First();        

		public bool ContainsPoint(Vector3 point)
        {
			return Space.Inst.WorldToQuadrantSpaceAbsolutePos(point) == AbsolutePos;
        }

		public Vector3 ClampPoint(Vector3 point, float percentSize = 1)
		{
			var oldOffset = point - this.WorldCenterPos;
			var offset = Vector3.ClampMagnitude(oldOffset, percentSize * Space.Inst.QuadrantSize/2);
			return point - oldOffset + offset;
		}


		public void SetAbsolutePos(Vector3Int newPos, bool regenerate = false)
        {
			AbsolutePos = newPos;
			Root.transform.position = AbsolutePos.ToVector3() * Space.Inst.QuadrantSize;
			if (regenerate)
            {
				Regenerate();
            }
		}

		void Regenerate()
        {
			foreach(var o in Objects)
            {
				var m = o.GetComponent<MapGen3D>();

				if (m.RegenerateOnNewQuadrant)
                {
					m.ReGenerate();
                } else
                {
					m.ReActivate();
                }
            }

        }

		public Quadrant Neighbour(Vector3Int dir)
        {
			var targetPos = dir + AbsolutePos;
			return Space.Inst.quadrantsList.Find(q => q.AbsolutePos == targetPos);
        }

		public void DisableChildren()
        {
			foreach (Transform t in Root.transform) t.gameObject.SetActive(false);
        }

	}
	

	public class Space: Singleton<Space>
	{
		public UnityEvent OnSpaceCreated;
		public UnityEvent<Vector3Int> OnQuadrantChange;
		public Vector3Int WorldToQuadrantSpaceAbsolutePos(Vector3 pos)
		{
			return (pos / QuadrantSize).ToVector3Int();
		}		

		public Quadrant[,,] quadrants;
		public List<Quadrant> quadrantsList;

		public float QuadrantSize = 1600;

		public List<GameObject> GeneratorPrefabs;

		[SerializeField]
		bool fitGeneratorsMagnitude = true;

		[SerializeField]
		[ShowIf("FitGeneratorsMagnitude")]
		bool fitGeneratorsQty = true;



		public Quadrant MakeQuadrant(Vector3Int pos)
        {
			var q = new Quadrant(pos, null, transform);
			

			foreach (var gp in GeneratorPrefabs)
			{
				var o = GameObject.Instantiate(gp, q.Root.transform, false);
				q.Objects.Add(o);
				var m = o.GetComponent<MapGen3D>();
				
				if (fitGeneratorsMagnitude)
				{
					var newMagnitude = QuadrantSize / 2;
					if (fitGeneratorsQty)
					{
						var qtyChange = newMagnitude / m.Magnitude;
						m.Qty = Mathf.Pow(Mathf.Pow(m.Qty, 1/3f) * qtyChange,3);
					}
					m.Magnitude = newMagnitude;
				}
				
				o.GetComponent<MapGen3D>().Generate();
			}
			return q;
		}
		public void Start()
        {
			quadrants = new Quadrant[3,3,3];
			quadrantsList = new List<Quadrant>();			
			for(int x = 0; x <3; x++)
            {
				for (int y = 0; y < 3; y++)
                {
					for (int z = 0; z < 3; z++)
                    {
						var pos = new Vector3Int(x - 1, y - 1, z - 1);
						quadrants[x, y, z] = MakeQuadrant(pos);
						quadrantsList.Add(quadrants[x, y, z]);
                    }
				}
			}
			OnSpaceCreated.Invoke();
		}

		public Quadrant StartQuadrant => quadrants[1, 1, 1];		

		[ShowNonSerializedField]
		private Vector3Int lastQuadrant = Vector3Int.zero;		
        private void Update()
        {
			var pos = WorldToQuadrantSpaceAbsolutePos(Player.Inst.transform.position);
			
			if (!pos.Equals(lastQuadrant))
            {
				
				
				var dir = pos - lastQuadrant;
				//Debug.LogError($"Quadrant Changed: to [{pos.x},{pos.y},{pos.z}], shift: [{shift.x},{shift.y},{shift.z}]");
				foreach (var q in quadrantsList)
                {
					/*
					if ((q.AbsolutePos - pos).magnitude > 1.99f)
                    {
						//Debug.LogError($"Affecting Q: [{q.Pos.x},{q.Pos.y},{q.Pos.z}]");
						q.SetAbsolutePos(q.AbsolutePos + dir * 3, true);
                    }
					*/
					q.RelativePos -= dir;					
					
					var loopingOffset = dir * 3;
					if (Mathf.Abs(q.RelativePos.GetValueInAxis(Dir3D.Axes[dir])) > 1)
                    {
						q.RelativePos += loopingOffset;
						q.SetAbsolutePos(q.AbsolutePos + loopingOffset, true);
					}
                }
				lastQuadrant = pos;
				OnQuadrantChange.Invoke(dir);
			}
        }

		public IEnumerable<Quadrant> QuadrantsInDir(Vector3Int dir)
        {
			var axis = Dir3D.Axes[dir];
			var axisSign = dir.GetValueInAxis(axis);
			return quadrantsList.Where(q => q.RelativePos.GetValueInAxis(axis) == axisSign);		
        }

		public IEnumerable<Quadrant> QuadrantsNotInDir(Vector3Int dir)
        {
			var axis = Dir3D.Axes[dir];
			var axisSign = dir.GetValueInAxis(axis);
			return quadrantsList.Where(q => q.RelativePos.GetValueInAxis(axis) != axisSign);
		}

		public IEnumerable<Quadrant> QuadrantsCentralSliceInDir(Vector3Int dir)
		{
			var axis = Dir3D.Axes[dir];
			return quadrantsList.Where(q => q.RelativePos.GetValueInAxis(axis) == 0);
		}

		public Quadrant GetQuadrant(Vector3 worldPos)
        {
			var qAbsPos = WorldToQuadrantSpaceAbsolutePos(worldPos);
			return quadrantsList.Find(q => q.AbsolutePos == qAbsPos);
        }

		public bool InObservableSpace(Vector3 worldPos)
        {
			return GetQuadrant(worldPos) != null;
        }

		[Button]
		private void DebugDisableQuadrantChildren()
        {
			foreach (var q in quadrantsList) q.DisableChildren();
		}
		protected void OnDrawGizmos()
		{
#if UNITY_EDITOR
			for (int x = 0; x < 3; x++)
			{
				for (int y = 0; y < 3; y++)
				{
					for (int z = 0; z < 3; z++)
					{
						Gizmos.color = new Color(Mathf.Abs(x), Mathf.Abs(y), Mathf.Abs(z));		
						var relPos = new Vector3Int(x - 1, y - 1, z - 1);
						if (quadrants != null)
						{							
							var q = quadrants[x, y, z];
							var pos = q.Root.transform.position;
							Handles.Label(pos, "a:"  + q.AbsolutePos.ToString() + " r:" + q.RelativePos.ToString());
							Gizmos.DrawWireCube(q.Root.transform.position, Vector3.one * QuadrantSize * 0.98f);
						}
						else
						{
							Gizmos.color = Color.Lerp(Gizmos.color, Color.black, 0.4f);
							
							Gizmos.DrawWireCube(relPos.ToVector3() * QuadrantSize, Vector3.one * QuadrantSize * 0.98f);
						}
					}
				}
			}
#endif
		}
	}
}