using UnityEngine;

public class ShooterBossBars : ShooterBossPart
{
	private const int SHOOTERBOSS_BARS_SIZE = 4;

	private const int SHOOTERBOSS_TURRETS_SIZE = 4;

	public Transform Bar;

	private float distanceFromOrgio;

	private float finalDistanceFromOrigo = 80f;

	private Transform[] bars;

	private ShooterBossTurret[] Turrets;

	private int destroyedTurrets;

	private void Awake()
	{
		bars = new Transform[4];
		bars[0] = Bar;
		for (int i = 0; i < 4; i++)
		{
			if (i > 0)
			{
				bars[i] = Object.Instantiate(Bar) as Transform;
				bars[i].transform.parent = Bar.transform.parent;
			}
			bars[i].transform.localPosition = Vector3.zero;
			bars[i].transform.localRotation = Quaternion.Euler(0f, 0f, 90 * i);
			bars[i].transform.GetChild(0).transform.localPosition = Vector3.zero;
		}
	}

	private void Start()
	{
		Turrets = GetComponentsInChildren<ShooterBossTurret>();
		ShooterBossTurret[] turrets = Turrets;
		foreach (ShooterBossTurret shooterBossTurret in turrets)
		{
			shooterBossTurret.Destroyed += OnTurretDestroyed;
		}
	}

	public override void UpdateState(ShooterBoss.State state)
	{
		switch (state)
		{
		case ShooterBoss.State.TRANSIT_TO_TURRET_STATE:
		{
			distanceFromOrgio += 10f * Time.deltaTime * 30f;
			if (distanceFromOrgio > finalDistanceFromOrigo)
			{
				distanceFromOrgio = finalDistanceFromOrigo;
			}
			for (int j = 0; j < 4; j++)
			{
				if (bars[j] != null)
				{
					bars[j].transform.GetChild(0).transform.localPosition = new Vector3(distanceFromOrgio, 0f, 0f);
				}
			}
			break;
		}
		case ShooterBoss.State.TURRETS_STATE:
		{
			ShooterBossTurret[] turrets = Turrets;
			foreach (ShooterBossTurret shooterBossTurret in turrets)
			{
				shooterBossTurret.UpdateState(state);
			}
			break;
		}
		}
	}

	public override void Update()
	{
		base.Update();
		base.ParentRotationAngle -= ShooterBoss.TurretsRotationSpeed * Time.deltaTime * 30f;
	}

	public void OnTurretDestroyed()
	{
		destroyedTurrets++;
		float num = (float)destroyedTurrets / 4f;
		ShooterBoss.TurretsRotationSpeed = 0.03f * (1f - num) + ((!UserSettings.I.expert) ? 0.04f : 0.03f) * num;
		if (destroyedTurrets >= 4)
		{
			ShooterBoss.Instance.OnTurretsDestroyed();
		}
	}
}
