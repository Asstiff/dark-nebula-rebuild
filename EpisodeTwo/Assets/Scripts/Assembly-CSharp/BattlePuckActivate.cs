using System;
using FLD;
using UnityEngine;

public class BattlePuckActivate : BattlePuckState
{
	private float timer;

	private float rotationSpeed;

	private float targetRotationSpeed;

	private float spikeScale;

	private float retractionTime;

	private float spikeScaleTime;

	private AudioClip bladesOutSound;

	public BattlePuckActivate(BattlePuck p)
		: base(p)
	{
		if (p.Mini == 4 || p.Mini == 5)
		{
			Audio.Play("clusterpuckactivated" + UnityEngine.Random.value, battlePuck.ClusterpuckActivated_SFX);
		}
		else if (p.Mini == 1)
		{
			Audio.Play("fastenemy_bladesout" + UnityEngine.Random.value, battlePuck.FastEnemyBladesOut_SFX);
		}
		else
		{
			Audio.Play("enemy_blades_out" + UnityEngine.Random.value, battlePuck.EnemyBladesOut_SFX);
		}
		rotationSpeed = 0f;
		targetRotationSpeed = 1f;
		spikeScale = 0f;
		timer = 0f;
		spikeScaleTime = 0.3f;
		battlePuck.IsActivated = true;
		battlePuck.AlertFriends();
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		base.UpdateWithDt(dt);
		timer += dt;
		rotationSpeed += (targetRotationSpeed - rotationSpeed) * dt * 0.3f;
		float num = Mathf.Min(1f, timer / spikeScaleTime);
		float num2 = (float)System.Math.PI * 3f / 4f;
		spikeScale = Mathf.Sin(num * num2) + 0.3f;
		battlePuck.SetRotationSpeedAndSpikeScale(rotationSpeed, spikeScale);
		if (ReadyWithState())
		{
			return new BattlePuckAggressive(battlePuck);
		}
		return this;
	}

	private bool ReadyWithState()
	{
		return timer > spikeScaleTime && Mathf.Abs(targetRotationSpeed - rotationSpeed) < 0.05f;
	}
}
