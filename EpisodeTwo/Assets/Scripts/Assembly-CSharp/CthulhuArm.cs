using System;
using System.Collections.Generic;
using FLD;
using UnityEngine;

public class CthulhuArm : Entity
{
	public enum CthulhuArmState
	{
		CTHULHUARM_SLEEPING = 0,
		CTHULHUARM_INTRODUCTION = 1,
		CTHULHUARM_SWAYING = 2
	}

	private const float maxAngleEndArmsSpeed = 40f;

	private const float maxSwayingDegrees = 40f;

	public CthulhuBar[] Bars;

	public AudioClip damageSound;

	public AudioClip dieSound;

	public AudioClip explodeSound;

	private int[] jointHealth;

	private int[] barHealth;

	private float cooldown;

	private float jointCooldown;

	private float angle1;

	private float angle2;

	private float angle3;

	private float startAngle;

	private CthulhuArmState state;

	private float swayingStartTime;

	private float angle2Speed;

	private float angle3Speed;

	private int[] arms = new int[3] { 1, 4, 0 };

	private int[] armsBroke = new int[3] { 3, 6, 2 };

	private int[] joints = new int[3] { 8, 4, 0 };

	private Cthulhu parent;

	private float difficulty;

	public float Tweak = 0.6f;

	private float currExplosionFrame;

	private Dictionary<int, bool> stateTimeFlag = new Dictionary<int, bool>();

	private int repeatingActionValue = -1;

	private int everySecondFrameCounter;

	public void Setup(Cthulhu pParent, float _startAngle, float _introductionAngle)
	{
		parent = pParent;
		barHealth = new int[3];
		jointHealth = new int[3];
		difficulty = ((!UserSettings.I.expert) ? 1f : 1.2f);
		for (int i = 1; i < 3; i++)
		{
			if (Bars[i] == null)
			{
				Bars[i] = (UnityEngine.Object.Instantiate(Bars[0].gameObject) as GameObject).GetComponent<CthulhuBar>();
			}
		}
		for (int j = 0; j < 3; j++)
		{
			if (j > 0)
			{
				Bars[j].transform.parent = Bars[j - 1].transform;
				Bars[j].transform.localRotation = Quaternion.identity;
				Bars[j].transform.localPosition = new Vector3(0f, 100f, 0f);
			}
			jointHealth[j] = 3;
			barHealth[j] = 1;
			Bars[j].Setup();
			CthulhuJoint cthulhuJoint = Bars[j].cthulhuJoint;
			cthulhuJoint.Hit = (Action<int>)Delegate.Combine(cthulhuJoint.Hit, new Action<int>(OnHitJoint));
			Bars[j].cthulhuJoint.SetIndex(j);
			UpdateBarSprite(j);
			UpdateJointSprite(j);
		}
		startAngle = _startAngle;
		angle1 = _introductionAngle;
		angle2 = (angle3 = 0f);
		cooldown = 0.1f;
		jointCooldown = cooldown;
	}

	private void OnHitJoint(int i)
	{
		if (JointCollision(i))
		{
			Vector3 position = Bars[i].Joint.transform.position;
			SpawnDebrisParticles(position);
			SpawnExplosion(position);
			Audio.Play("jointDamage " + UnityEngine.Random.value, damageSound, position);
			World.TriggerFlag(100);
			if (IsArmDead())
			{
				Bars[0].doesDamage = false;
				Bars[1].doesDamage = false;
				Bars[2].doesDamage = false;
			}
		}
		UpdateBarSprite(i);
		UpdateJointSprite(i);
	}

	private void UpdateBarSprite(int i)
	{
		if (barHealth[i] <= 0)
		{
			Bars[i].Bar.spriteId = Bars[i].Bar.GetSpriteIdByName("cthulhu-arms/" + armsBroke[i]);
			Bars[i].cthulhuJoint.light.SetRenderersEnabled(false);
		}
		else
		{
			Bars[i].Bar.spriteId = Bars[i].Bar.GetSpriteIdByName("cthulhu-arms/" + arms[i]);
		}
	}

	private void UpdateJointSprite(int i)
	{
		Bars[i].Joint.spriteId = Bars[i].Joint.GetSpriteIdByName("cthulhu-lights/" + (joints[i] + GetJointHealthSpriteOffset(i)));
	}

	private int GetJointHealthSpriteOffset(int i)
	{
		return 3 - jointHealth[i];
	}

	private void SpawnDebrisParticles(Vector3 spawnPosition)
	{
		for (int i = 0; i < 10; i++)
		{
			SpawnDebrisParticleAndSize(spawnPosition, 1f);
		}
	}

	private void SpawnDebrisParticleAndSize(Vector3 spawnPosition, float size)
	{
		parent.SpawnDebrisParticleAndSize(spawnPosition, size);
	}

	private void UpdateSwaying()
	{
		if (Time.deltaTime == 0f)
		{
			return;
		}
		if (swayingStartTime == 0f)
		{
			swayingStartTime = Time.time;
		}
		angle1 = startAngle + Mathf.Sin((swayingStartTime - Time.time) * Tweak * difficulty) * 40f;
		angle2 = GetSwayingSpeed(2f) * angle2Speed;
		angle3 = GetSwayingSpeed(3f) * angle3Speed;
		if (jointCooldown > 0f)
		{
			jointCooldown -= Time.deltaTime * 30f;
		}
		if (angle2Speed < 40f)
		{
			angle2Speed += 2f * Time.deltaTime * 30f;
			if (angle2Speed > 40f)
			{
				angle2Speed = 40f;
			}
		}
		if (angle2Speed == 40f && angle3Speed < 40f)
		{
			angle3Speed += 1f * Time.deltaTime * 30f;
			if (angle3Speed > 40f)
			{
				angle3Speed = 40f;
			}
		}
	}

	private float GetSwayingSpeed(float arm)
	{
		arm--;
		float num = arm * 30f;
		float num2 = (arm - 0.002f) * 30f;
		float num3 = Mathf.Sin((swayingStartTime - (Time.time - num)) * Tweak * difficulty) * 40f;
		float num4 = Mathf.Sin((swayingStartTime - (Time.time - num2)) * Tweak * difficulty) * 40f;
		return num4 - num3;
	}

	private void UpdateIntroduction()
	{
		if (startAngle == angle1)
		{
			return;
		}
		if ((double)(startAngle - angle1) > 0.0)
		{
			angle1 += 1.8f * Time.deltaTime * 30f;
			if (angle1 > startAngle)
			{
				angle1 = startAngle;
			}
		}
		else
		{
			angle1 -= 1.8f * Time.deltaTime * 30f;
		}
	}

	public void UpdateArm(float armsAngle)
	{
		if (state == CthulhuArmState.CTHULHUARM_SWAYING)
		{
			UpdateSwaying();
		}
		else if (state == CthulhuArmState.CTHULHUARM_INTRODUCTION)
		{
			UpdateIntroduction();
		}
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f - (angle1 + armsAngle));
		Bars[1].transform.localRotation = Quaternion.Euler(0f, 0f, 0f - angle2);
		Bars[2].transform.localRotation = Quaternion.Euler(0f, 0f, 0f - angle3);
		Render();
	}

	public void TransitTo(CthulhuArmState newstate)
	{
		state = newstate;
	}

	public bool IsArmDead()
	{
		return jointHealth[0] <= 0 && jointHealth[1] <= 0 && jointHealth[2] <= 0;
	}

	private void dontRenderJoint(int i)
	{
		Bars[i].Joint.GetComponent<Renderer>().enabled = false;
	}

	private void dontRenderBar(int i)
	{
		Bars[i].Bar.GetComponent<Renderer>().enabled = false;
	}

	private bool TimeForAction(int StateFrame)
	{
		if (currExplosionFrame < (float)StateFrame)
		{
			return false;
		}
		if (stateTimeFlag.ContainsKey(StateFrame))
		{
			return false;
		}
		stateTimeFlag.Add(StateFrame, true);
		return true;
	}

	private bool TimeForRepeatingAction(int StateFrame)
	{
		if (currExplosionFrame < (float)StateFrame)
		{
			return false;
		}
		if (repeatingActionValue == StateFrame)
		{
			return false;
		}
		repeatingActionValue = StateFrame;
		return true;
	}

	private void Render()
	{
		if (IsArmDead() && !(currExplosionFrame > 50f))
		{
			Vector3 position = Bars[2].Joint.transform.position;
			Vector3 position2 = Bars[1].Joint.transform.position;
			Vector3 position3 = Bars[0].Joint.transform.position;
			Vector3 position4 = base.transform.position;
			bool flag = TimeForAction(0);
			bool flag2 = TimeForAction(16);
			bool flag3 = TimeForAction(32);
			bool flag4 = TimeForAction(48);
			bool flag5 = TimeForRepeatingAction(everySecondFrameCounter);
			if (flag5)
			{
				everySecondFrameCounter += 2;
			}
			if (flag)
			{
				dontRenderJoint(2);
			}
			if (flag2)
			{
				dontRenderBar(2);
				dontRenderJoint(1);
			}
			if (flag3)
			{
				dontRenderBar(1);
				dontRenderJoint(0);
			}
			if (flag4)
			{
				dontRenderBar(0);
				Audio.Play("diearm" + UnityEngine.Random.value, dieSound);
			}
			Vector3 vector = position - position2;
			float num = currExplosionFrame / 16f;
			Vector3 vector2 = position + vector * num;
			if (flag || flag2)
			{
				SpawnShockwave(vector2);
			}
			if (flag)
			{
				SpawnDebrisParticles(vector2);
				Audio.Play("explodeSoundarm1" + UnityEngine.Random.value, explodeSound, vector2);
				World.TriggerFlag(100);
			}
			if (flag5 && currExplosionFrame <= 16f)
			{
				SpawnExplosion(vector2);
			}
			vector = position3 - position3;
			num = (currExplosionFrame - 16f) / 16f;
			vector2 = position2 + vector * num;
			if (flag2 || flag3)
			{
				SpawnShockwave(vector2);
			}
			if (flag2)
			{
				SpawnDebrisParticles(vector2);
				Audio.Play("explodeSoundarm2" + UnityEngine.Random.value, explodeSound, vector2);
				World.TriggerFlag(100);
			}
			if (flag5 && currExplosionFrame >= 16f && currExplosionFrame <= 32f)
			{
				SpawnExplosion(vector2);
			}
			vector = position4 - position3;
			num = (currExplosionFrame - 32f) / 16f;
			vector2 = position3 + vector * num;
			if (flag3 || flag4)
			{
				SpawnShockwave(vector2);
			}
			if (flag5 && currExplosionFrame >= 32f && currExplosionFrame <= 48f)
			{
				SpawnExplosion(vector2);
			}
			if (flag3)
			{
				SpawnDebrisParticles(vector2);
				Audio.Play("explodeSoundarm3" + UnityEngine.Random.value, explodeSound, vector2);
				World.TriggerFlag(100);
			}
			currExplosionFrame += Time.deltaTime * 30f;
		}
	}

	public bool FinishedRendering()
	{
		return true;
	}

	private void SpawnShockwave(Vector3 wPos)
	{
		ObjectPool.instance.GetObjectForType<Shockwave>().Play("Big", wPos, 0.5f, 12);
	}

	private void SpawnExplosion(Vector3 wPos)
	{
		parent.SpawnExplosion(wPos);
	}

	private bool JointCollision(int which)
	{
		if (jointCooldown <= 0f)
		{
			jointCooldown = cooldown;
			if (jointHealth[which] > 0)
			{
				jointHealth[which]--;
				if (jointHealth[which] <= 0)
				{
					barHealth[which]--;
				}
				return true;
			}
			return false;
		}
		return false;
	}
}
