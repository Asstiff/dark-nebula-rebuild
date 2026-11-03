using UnityEngine;

public interface ICamera
{
	ICamera _parent { get; set; }

	Vector2 WorldPos();

	void SetWorldPos(Vector2 p);

	void Reset();

	void Remove();

	void UpdateCamera();

	void UpdateCascading();
}
