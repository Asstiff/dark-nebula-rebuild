using FLD;
using UnityEngine;

public class PushButtonEntity : Entity
{
	public tk2dSprite ButtonSprite;

	public tk2dSprite LightSprite;

	public AudioClip Ticker;

	public AudioClip Ticker2;

	public AudioClip StartSound;

	public AudioClip EndSound;

	private float duration;

	private float originalDuration;

	private int whenActivatedFlag;

	private int whenDeactivatedFlag;

	private float countdownFrame;

	private float prevCountdownFrame;

	private float framesSinceActivation;

	private bool activated;

	protected override void Awake()
	{
		base.Awake();
		base.keepalive = true;
	}

	private void Start()
	{
		whenActivatedFlag = GetParameterInt("whenActivatedFlag");
		whenDeactivatedFlag = GetParameterInt("whenDeactivatedFlag");
		duration = GetParameterFloat("duration");
		countdownFrame = 10f;
		originalDuration = duration;
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.CompareTag("Player"))
		{
			Activate();
		}
	}

	private void Deactivate()
	{
		countdownFrame = 10f;
		activated = false;
		ButtonSprite.spriteId = ButtonSprite.GetSpriteIdByName("entity_pushbutton/8");
		World.TriggerFlag(whenDeactivatedFlag);
		Audio.Play("pushbutton_end " + Random.value, EndSound, base.transform.position);
	}

	private void Update()
	{
		if (activated && duration != 0f)
		{
			framesSinceActivation += Time.deltaTime * 30f;
			float num = duration * 30f;
			float num2 = 1f - (num - framesSinceActivation) / num;
			if (num2 > 1f)
			{
				Deactivate();
			}
			else
			{
				countdownFrame = 8f * num2;
				if (countdownFrame < 0f)
				{
					countdownFrame = 0f;
				}
				else if (countdownFrame > 8f)
				{
					countdownFrame = 8f;
				}
				if ((int)countdownFrame != (int)prevCountdownFrame)
				{
					if (countdownFrame < 6f)
					{
						Audio.Play("pushbutton_ticker " + Random.value, Ticker, base.transform.position);
					}
					else
					{
						Audio.Play("pushbutton_ticker2 " + Random.value, Ticker2, base.transform.position);
					}
					prevCountdownFrame = countdownFrame;
				}
			}
		}
		if (World.Difficulty <= 1f)
		{
			duration = 1f / World.Difficulty * originalDuration;
		}
		if (duration != 0f)
		{
			LightSprite.spriteId = LightSprite.GetSpriteIdByName("entity_pushbutton/" + (int)countdownFrame);
			LightSprite.gameObject.SetActive(true);
		}
		else
		{
			LightSprite.gameObject.SetActive(false);
		}
	}

	private void Activate()
	{
		if (!activated)
		{
			World.TriggerFlag(whenActivatedFlag);
			Audio.Play("pushbutton_start " + Random.value, StartSound, base.transform.position);
			ButtonSprite.spriteId = ButtonSprite.GetSpriteIdByName("entity_pushbutton/9");
		}
		countdownFrame = 0f;
		activated = true;
		framesSinceActivation = 0f;
	}
}
