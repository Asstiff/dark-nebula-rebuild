using UnityEngine;

public class MeshDistortion : MonoBehaviour
{
	private MeshFilter _mesh;

	private Vector3[] _vertices;

	private Vector3[] _tmpVertices;

	private float _lastDistortion;

	private void Awake()
	{
		_mesh = GetComponent<MeshFilter>();
		if (!(_mesh == null) && !(_mesh.sharedMesh == null))
		{
			_vertices = _mesh.sharedMesh.vertices;
			_tmpVertices = new Vector3[_vertices.Length];
		}
	}

	private void OnDisable()
	{
		World.SignalUpdate -= VerticesUpdate;
	}

	private void OnBecameVisible()
	{
		World.SignalUpdate += VerticesUpdate;
	}

	private void OnBecameInvisible()
	{
		World.SignalUpdate -= VerticesUpdate;
	}

	public void Release()
	{
		VerticesUpdate();
		_vertices = null;
		_mesh = null;
		base.enabled = false;
	}

	private void VerticesUpdate()
	{
		if (_mesh == null || _mesh.sharedMesh == null || _vertices.Length == 0)
		{
			return;
		}
		float distortion = World.distortion;
		if (distortion < 0.1f)
		{
			if (_lastDistortion != 0f)
			{
				_lastDistortion = 0f;
				_mesh.sharedMesh.vertices = _vertices;
			}
			return;
		}
		_lastDistortion = distortion;
		for (int i = 0; i < _vertices.Length; i++)
		{
			_tmpVertices[i].x = _vertices[i].x + (Random.value - 0.5f) * distortion;
			_tmpVertices[i].y = _vertices[i].y;
			_tmpVertices[i].z = _vertices[i].z + (Random.value - 0.5f) * distortion;
		}
		_mesh.sharedMesh.vertices = _tmpVertices;
	}
}
