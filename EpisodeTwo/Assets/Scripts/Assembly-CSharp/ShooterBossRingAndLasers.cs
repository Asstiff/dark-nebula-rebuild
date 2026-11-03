using UnityEngine;

public class ShooterBossRingAndLasers : ShooterBossPart
{
	public Transform Laser1;

	public Transform Laser2;

	private ShooterBoss.State lastState;

	private float angle;

	private bool laserStopped;

	private float rotationSpeed = 0.0275f;

	private float laserHeightFluctSpeed = 0.1f;

	private float laserHeightOffset = 0.1f;

	public float laserHeightScale
	{
		get
		{
			return Laser1.localScale.y;
		}
		set
		{
			Laser1.localScale = new Vector3(4f, value, 1f);
			Laser2.localScale = new Vector3(4f, value, 1f);
		}
	}

	public void StopLaser()
	{
		laserStopped = true;
		laserHeightScale = 0f;
	}

	private void Start()
	{
		laserHeightScale = 0f;
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (lastState == ShooterBoss.State.FINAL_FIGHT && !laserStopped && collider.CompareTag("Player"))
		{
			World.vessel.TriggerRecieveDamage();
		}
	}

	public override void UpdateState(ShooterBoss.State state)
	{
		if (state == ShooterBoss.State.ACTIVATE_LASERS)
		{
			laserHeightScale += 0.01f * Time.deltaTime * 30f;
			if (laserHeightScale > 1f)
			{
				laserHeightScale = 1f;
			}
		}
		if (state >= ShooterBoss.State.SPIN_LASERS && !laserStopped)
		{
			angle += rotationSpeed * Time.deltaTime * 30f;
			base.transform.localRotation = Quaternion.Euler(0f, 0f, (0f - angle) * 57.29578f);
			if (laserHeightFluctSpeed > 0f)
			{
				if (laserHeightScale > 1f + laserHeightOffset)
				{
					laserHeightFluctSpeed = 0f - laserHeightFluctSpeed;
				}
			}
			else if (laserHeightFluctSpeed < 0f && laserHeightScale < 1f - laserHeightOffset)
			{
				laserHeightFluctSpeed = 0f - laserHeightFluctSpeed;
			}
			laserHeightScale += laserHeightFluctSpeed * Time.deltaTime * 30f;
		}
		lastState = state;
	}
}
