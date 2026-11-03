using UnityEngine;

public class FlipFloorEntity : Entity
{
	public tk2dSprite Sprite;

	public int startFrame;

	private bool activate;

	private bool activated;

	private bool deactivate;

	private FlagConditionChanged whenActivated;

	private FlagConditionChanged whenDeactivated;

	private int spriteFrame;

	private float delay;

	private float maxDelay;

	private int currentSpriteFrame = 7;

	private float _currFrame;

	private bool vesselIsOn;

	private bool isTriggerStay;

	private int currFrame
	{
		get
		{
			return (int)_currFrame;
		}
	}

	public override void InitParametersInEditor()
	{
		base.InitParametersInEditor();
		InitParameters();
	}

	public override void InitParametersInRuntime()
	{
		base.InitParametersInRuntime();
		InitParameters();
	}

	protected void InitParameters()
	{
		switch (GetParameterInt("type"))
		{
		case 0:
			startFrame = 0;
			break;
		case 1:
			startFrame = 8;
			break;
		case 2:
			startFrame = 16;
			break;
		case 3:
			startFrame = 24;
			break;
		}
		spriteFrame = 7;
		updateSprite();
	}

	private void updateSprite()
	{
		if (spriteFrame != currentSpriteFrame)
		{
			currentSpriteFrame = spriteFrame;
			if (spriteFrame == -1)
			{
				Sprite.GetComponent<Renderer>().enabled = false;
				return;
			}
			Sprite.GetComponent<Renderer>().enabled = true;
			Sprite.spriteId = Sprite.GetSpriteIdByName("flipfloor/" + (startFrame + spriteFrame));
		}
	}

	protected override void Awake()
	{
		base.Awake();
		base.keepalive = true;
	}

	private void Start()
	{
		delay = GetParameterFloat("delay");
		maxDelay = GetParameterFloat("maxDelay");
		whenActivated = new FlagConditionChanged(GetParameterInt("whenActivatedFlag"));
		whenActivated.Callback += OnActivated;
		whenDeactivated = new FlagConditionChanged(GetParameterInt("whenDeactivatedFlag"));
		whenDeactivated.Callback += OnDeactivated;
		spriteFrame = -1;
		updateSprite();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (whenActivated != null)
		{
			whenActivated.Callback -= OnActivated;
			whenActivated.Dispose();
			whenActivated = null;
		}
		if (whenDeactivated != null)
		{
			whenDeactivated.Callback -= OnDeactivated;
			whenDeactivated.Dispose();
			whenDeactivated = null;
		}
	}

	private void OnActivated()
	{
		deactivate = false;
		activate = true;
		if (spriteFrame != -1)
		{
			_currFrame = spriteFrame;
		}
		else
		{
			_currFrame = (int)((0f - delay) * 30f);
		}
	}

	private void OnDeactivated()
	{
		deactivate = true;
		activate = false;
		_currFrame = (int)(0f - maxDelay * 30f + delay * 30f);
		VesselLeave();
	}

	private void Update()
	{
		if (deactivate)
		{
			_currFrame += Time.deltaTime * 30f;
			float num = 1f - (float)currFrame / 7f;
			if (num > 1f)
			{
				num = 7f;
			}
			else if (num < 0f)
			{
				spriteFrame = -1;
			}
			else
			{
				activated = false;
				spriteFrame = (int)(num * 7f);
			}
			if ((float)currFrame >= 7f)
			{
				deactivate = false;
				spriteFrame = -1;
			}
		}
		else if (activate)
		{
			_currFrame += Time.deltaTime * 30f;
			if (currFrame >= 7)
			{
				activated = true;
				activate = false;
			}
			float num2 = (float)currFrame / 7f;
			if (num2 > 1f)
			{
				num2 = 7f;
			}
			else if (num2 < 0f)
			{
				spriteFrame = -1;
			}
			else
			{
				spriteFrame = (int)(num2 * 7f);
			}
		}
		updateSprite();
	}

	private void FixedUpdate()
	{
		if (isTriggerStay)
		{
			OnManualTriggerStay();
		}
	}

	private void OnManualTriggerStay()
	{
		if (!activated || vesselIsOn)
		{
			if (!activated && vesselIsOn)
			{
				VesselLeave();
			}
		}
		else
		{
			vesselIsOn = true;
			(World.vessel as StandardVessel).FlipFloor(true);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.CompareTag("Player"))
		{
			isTriggerStay = true;
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.CompareTag("Player"))
		{
			isTriggerStay = false;
			VesselLeave();
		}
	}

	private void VesselLeave()
	{
		if (vesselIsOn)
		{
			vesselIsOn = false;
			(World.vessel as StandardVessel).FlipFloor(false);
		}
	}
}
