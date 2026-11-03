using System.Collections;
using FLD;
using UnityEngine;

public class UnlockAllEffect : MonoBehaviour
{
	public GameObject UnlockParticle;

	public AudioClip UnlockSound;

	private ParticleSystem _unlockParticle;

	public float UnlockDelay
	{
		get
		{
			return 1.6600001f;
		}
	}

	private void Start()
	{
		_unlockParticle = UnlockParticle.GetComponent<ParticleSystem>();
		StartCoroutine("Animate");
	}

	private IEnumerator Animate()
	{
		yield return new WaitForSeconds(1f);
		Audio.Play("Unlock", UnlockSound);
		yield return new WaitForSeconds(0.25f);
		_unlockParticle.Play();
		yield return new WaitForSeconds(2f);
		Object.Destroy(base.gameObject);
	}
}
