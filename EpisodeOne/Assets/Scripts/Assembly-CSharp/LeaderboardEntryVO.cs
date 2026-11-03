using System;
using Palringo;
using UnityEngine;

public struct LeaderboardEntryVO
{
	public string name;

	public float time;

	public DateTime date;

	public int star;

	public bool expert;

	public Texture2D profileImage;

	public string userID;

	public int position;

	public Palringo.LeaderboardEntry palringoEntry;

	public int level;
}
