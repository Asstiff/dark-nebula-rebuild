using FLD;
using UnityEngine;

public class CrossPromotionButton : MonoBehaviour
{
	public SimpleButton Button;

	public tk2dTextMesh ButtonText;

	public tk2dTextMesh GetText;

	public MeshRenderer Icon;

	private CrossPromotionManager.Promo _activePromo;

	public bool HasPromo
	{
		get
		{
			return _activePromo != null;
		}
	}

	private void Start()
	{
		gameObject.SetActive(false); // No cross-promotion in the offline build.
	}

	private void OnButtonPress(SimpleButton source)
	{
		if (!(source != Button))
		{
			WebUtility.OpenURL(_activePromo.LinkURL, false);
		}
	}

	public void Refresh()
	{
		_activePromo = null;
		gameObject.SetActive(false);
	}
}
