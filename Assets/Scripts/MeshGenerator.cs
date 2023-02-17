using Curve;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
	const float PI2 = Mathf.PI * 2f;
	public static void Build(Mesh mesh, Curve.Curve curve, int tubularSegments, float radius, int radialSegments, bool isHead = true)
	{
		mesh.Clear();
		var vertices = new List<Vector3>();
		var normals = new List<Vector3>();
		var tangents = new List<Vector4>();
		var uvs = new List<Vector2>();
		var indices = new List<int>();

		var frames = curve.ComputeFrenetFrames(tubularSegments, false);


		for (int i = 0; i < tubularSegments; i++)
		{
			float r;
			if (isHead)
			{
				var percent = i / (float)(tubularSegments - 1);
				r = radius * (1f - percent);
			}
			else
			{
				r = radius;
				if (i == tubularSegments - 1) r *= 0.95f;
			}
			GenerateSegment(curve, frames, tubularSegments, r, radialSegments, vertices, normals, tangents, i);
		}
		GenerateSegment(curve, frames, tubularSegments, isHead?0:radius*0.9f, radialSegments, vertices, normals, tangents, tubularSegments);

		for (int i = 0; i <= tubularSegments; i++)
		{
			for (int j = 0; j <= radialSegments; j++)
			{
				float u = 1f * j / radialSegments;
				float v = 1f * i / tubularSegments;
				uvs.Add(new Vector2(u, v));
			}
		}

		for (int j = 1; j <= tubularSegments; j++)
		{
			for (int i = 1; i <= radialSegments; i++)
			{
				int a = (radialSegments + 1) * (j - 1) + (i - 1);
				int b = (radialSegments + 1) * j + (i - 1);
				int c = (radialSegments + 1) * j + i;
				int d = (radialSegments + 1) * (j - 1) + i;

				// faces
				indices.Add(a); indices.Add(d); indices.Add(b);
				indices.Add(b); indices.Add(d); indices.Add(c);
			}
		}
		mesh.vertices = vertices.ToArray();
		mesh.normals = normals.ToArray();
		mesh.tangents = tangents.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
		//mf.sharedMesh = mesh;

	}

	static void GenerateSegment(
			Curve.Curve curve,
			List<FrenetFrame> frames,
			int tubularSegments,
			float radius,
			int radialSegments,
			List<Vector3> vertices,
			List<Vector3> normals,
			List<Vector4> tangents,
			int i
		)
	{
		var u = 1f * i / tubularSegments;
		var p = curve.GetPointAt(u);
		var fr = frames[i];

		var N = fr.Normal;
		var B = fr.Binormal;

		for (int j = 0; j <= radialSegments; j++)
		{
			float v = 1f * j / radialSegments * PI2;
			var sin = Mathf.Sin(v);
			var cos = Mathf.Cos(v);

			Vector3 normal = (cos * N + sin * B).normalized;
			vertices.Add(p + radius * normal);
			normals.Add(normal);

			var tangent = fr.Tangent;
			tangents.Add(new Vector4(tangent.x, tangent.y, tangent.z, 0f));
		}
	}
}
