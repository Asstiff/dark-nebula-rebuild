using System.Collections.Generic;
using UnityEngine;

public class LeaderboardView : MonoBehaviour
{
	public delegate void HandlerDelegate(bool value);

	private const float ViewHeight = 180f;

	private const float FirstEntryPosition = 54f;

	public GameObject GfxHeaderNormal;

	public GameObject GfxHeaderExpert;

	public GameObject GfxContent;

	public GameObject EntryPrefab;

	private List<LeaderboardEntry> _entries;

	private int _entryCount;

	private bool _expert;

	private float _inputStartY;

	private float _contentYAtMouseDown;

	private bool _isMouseDown;

	private Vector3 _contentOrigin;

	private float _headerOriginY;

	private Camera _viewCamera;

	private ScrollPhysics _scroll;

	private bool _initialied;

	private float origY;

	private bool _isUsingLargeView;

	private float lastMouseDownY;

	public event HandlerDelegate EventDifficultyChanged;

	private void Start()
	{
		_viewCamera = base.transform.parent.gameObject.GetComponentInChildren<Camera>();
        origY = _viewCamera.rect.y;
		FixAspect();
		Init(UserSettings.I.expert);
	}

	private void Init(bool pExpert)
	{
		if (!_initialied)
		{
			_initialied = true;
			_expert = pExpert;
			_scroll = new ScrollPhysics();
			_scroll.position = 54f;
			_contentOrigin = GfxContent.transform.localPosition;
			_contentOrigin.Scale(new Vector3(1f, 0f, 1f));
			_headerOriginY = GfxHeaderNormal.transform.localPosition.y;
			GfxHeaderNormal.GetComponent<ToggleButton>().ButtonUpEvent += HandleButtonNormalUpEvent;
			GfxHeaderExpert.GetComponent<ToggleButton>().ButtonUpEvent += HandleButtonExpertUpEvent;
			UpdateToggles();
			_entries = new List<LeaderboardEntry>();
			List<LeaderboardEntryVO> list = new List<LeaderboardEntryVO>();
			SetList(list, _expert);
		}
	}

	private void OnDisable()
	{
	}

	private void UpdateToggles()
	{
		GfxHeaderNormal.GetComponent<ToggleButton>().SetToggle(!_expert);
		GfxHeaderExpert.GetComponent<ToggleButton>().SetToggle(_expert);
	}

	private void HandleButtonNormalUpEvent(ToggleButton source)
	{
		_expert = false;
		UpdateToggles();
		if (EventDifficultyChanged != null)
		{
			EventDifficultyChanged(_expert);
		}
	}

	private void HandleButtonExpertUpEvent(ToggleButton source)
	{
		_expert = true;
		UpdateToggles();
		if (EventDifficultyChanged != null)
		{
			EventDifficultyChanged(_expert);
		}
	}

	public void SetList(List<LeaderboardEntryVO> list, bool pExpert)
	{
		list = new List<LeaderboardEntryVO>(list);
		Init(pExpert);
		int num = 0;
		int count = _entries.Count;
		_entryCount = list.Count;
		if (_entryCount < 4)
		{
			for (int i = _entryCount; i < 4; i++)
			{
				list.Add(default(LeaderboardEntryVO));
			}
			_entryCount = list.Count;
		}
		foreach (LeaderboardEntryVO item in list)
		{
			LeaderboardEntry leaderboardEntry;
			if (num < count)
			{
				leaderboardEntry = _entries[num];
			}
			else
			{
				GameObject gameObject = Object.Instantiate(EntryPrefab) as GameObject;
				leaderboardEntry = gameObject.GetComponent<LeaderboardEntry>();
				leaderboardEntry.transform.parent = GfxContent.transform;
				_entries.Add(leaderboardEntry);
			}
			leaderboardEntry.SetEntry(item, num);
			num++;
		}
		for (int num2 = count - 1; num2 >= num; num2--)
		{
			Object.Destroy(_entries[num2].gameObject);
			_entries.RemoveAt(num2);
		}
	}

    private Camera _layoutHudCamera;
    private Rect _lastHudRect;
    private bool _lastLargeView;
	private void FixAspect()
    {
        if(_layoutHudCamera==null)_layoutHudCamera=GameObject.Find("Hud Camera").GetComponent<Camera>();
        Camera hud=_layoutHudCamera;
        Rect frame=LegacyScreenFit.ContentRect();
        float nativeHeight=frame.width/LegacyScreenFit.ReferenceAspect;
        frame=new Rect(frame.xMin,frame.center.y-nativeHeight*0.5f,frame.width,nativeHeight);
        if(frame==_lastHudRect && _lastLargeView==_isUsingLargeView)return;
        _lastHudRect=frame;_lastLargeView=_isUsingLargeView;
        float width=frame.width*0.94f;
        float height=width*178f/320f;
        float bottom=frame.yMin+frame.height*(_isUsingLargeView ? 0.36f : origY);
        _viewCamera.rect=new Rect((frame.center.x-width*0.5f)/Screen.width,bottom/Screen.height,width/Screen.width,height/Screen.height);
        _viewCamera.orthographicSize=89f;
        _viewCamera.ResetProjectionMatrix();
        _viewCamera.aspect=320f/178f;
    }

    private void LateUpdate()
    {
        if(_viewCamera!=null)FixAspect();
    }

	public void SetUseLargeView()
	{
		_isUsingLargeView = true;
	}

	private void Update()
	{
		if (_scroll == null)
		{
			return;
		}
		Ray ray = _viewCamera.ScreenPointToRay(Input.mousePosition);
		float num = GfxContent.transform.localPosition.y;
		if (Input.GetMouseButton(0))
		{
			RaycastHit hitInfo;
			if (_isMouseDown)
			{
				float num2 = _inputStartY - Input.mousePosition.y;
				num2 *= 800f / (float)Screen.height;
				num = _contentYAtMouseDown - num2;
			}
			else if (base.GetComponent<Collider>().Raycast(ray, out hitInfo, 100000000f))
			{
				_inputStartY = Input.mousePosition.y;
				_contentYAtMouseDown = GfxContent.transform.localPosition.y;
				_isMouseDown = true;
			}
		}
		else if (_isMouseDown)
		{
			_scroll.velocity += (Input.mousePosition.y - lastMouseDownY) * 10f;
			_isMouseDown = false;
		}
		float num3 = 54f + (float)_entryCount * 36f;
		float num4 = num;
		if (num4 < 54f)
		{
			num4 = 54f;
		}
		else if (num4 > num3 - 180f + 54f)
		{
			num4 = num3 - 180f + 54f;
		}
		if (_isMouseDown)
		{
			if (num4 != num)
			{
				num += (num4 - num) * 0.5f;
			}
			lastMouseDownY = Input.mousePosition.y;
			_scroll.position = num;
		}
		else
		{
			if (num4 != num)
			{
				_scroll.drag = 5f;
				_scroll.SpringToTarget(num4, Time.deltaTime);
			}
			else
			{
				_scroll.drag = 0.5f;
			}
			_scroll.Update(Time.deltaTime);
			num = _scroll.position;
		}
		Vector3 localPosition = GfxHeaderNormal.transform.localPosition;
		localPosition.y = _headerOriginY + Mathf.Min(0f, num - 54f);
		GfxHeaderNormal.transform.localPosition = localPosition;
		localPosition = GfxHeaderExpert.transform.localPosition;
		localPosition.y = _headerOriginY + Mathf.Min(0f, num - 54f);
		GfxHeaderExpert.transform.localPosition = localPosition;
		localPosition = GfxContent.transform.localPosition;
		if (!_isMouseDown && _scroll.velocity < 15f && _scroll.velocity > -15f)
		{
			float num5 = 54f - num;
			if (num5 < 1f && num5 > -1f)
			{
				num = 54f;
				_scroll.position = num;
			}
		}
		localPosition.y = num;
		GfxContent.transform.localPosition = localPosition;
	}
}
