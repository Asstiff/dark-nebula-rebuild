using System;
using System.Collections.Generic;
using System.IO;
using FLD.JSON;
using UnityEngine;

namespace FLD
{
	public class CrossPromotionManager
	{
		public class Promo
		{
			public string Id;

			public DateTime StartTime;

			public DateTime EndTime;

			public string Text;

			public string LinkURL;

			public float Weight;

			public string ImageURL;

			public string Param;

			public Texture2D Image;

			public string ImageCachePath
			{
				get
				{
					return Path.Combine(Application.temporaryCachePath, "crosspromo-" + Id + ".png");
				}
			}

			public Promo()
			{
				StartTime = DateTimeExtensions.UnixEpoch;
				EndTime = new DateTime(2037, 1, 1);
				Weight = 1f;
			}

			public void Serialize(SimpleJSerializer s)
			{
				s.Serialize("id", ref Id).Serialize("startTime", ref StartTime).Serialize("endTime", ref EndTime)
					.Serialize("text", ref Text)
					.Serialize("linkUrl", ref LinkURL)
					.Serialize("imageUrl", ref ImageURL)
					.Serialize("param", ref Param)
					.Serialize("weight", ref Weight);
			}
		}

		private List<Promo> _activePromos = new List<Promo>();

		private static CrossPromotionManager _instance;

		public static CrossPromotionManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CrossPromotionManager();
				}
				return _instance;
			}
		}

		private CrossPromotionManager()
		{
			LoadPromosFromCache();
		}

		private void SavePromosToCache()
		{
			JValue emptyArray = JValue.EmptyArray;
			foreach (Promo activePromo in _activePromos)
			{
				JValue emptyObject = JValue.EmptyObject;
				activePromo.Serialize(new SimpleJSerializer(emptyObject, SimpleJSerializer.SerializationMode.SerializeToJSON));
				emptyArray.Add(emptyObject);
			}
			uFuga.Storage.persistent.Set("crosspromo", emptyArray.Encode());
			uFuga.Storage.persistent.Save();
		}

		private void LoadPromosFromCache()
		{
			if (uFuga.Storage.persistent.Has("crosspromo"))
			{
				LoadPromosFromJSON(uFuga.Storage.persistent.GetString("crosspromo"));
			}
		}

		public void LoadPromosFromURL(string pURL)
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				return;
			}
			WWWUtility.OpenURL(pURL, (bool pOk, WWW pWWW) =>
			{
				if (pOk && !string.IsNullOrEmpty(pWWW.text))
				{
					LoadPromosFromJSON(pWWW.text);
				}
			});
		}

		public void ClearPromos()
		{
			_activePromos.Clear();
		}

		public void LoadPromosFromJSON(string pJsonString)
		{
			LoadPromosFromJSON(JValue.Decode(pJsonString));
		}

		public void LoadPromosFromJSON(JValue pJson)
		{
			if (!pJson.IsArray)
			{
				Debug.LogError("CrossPromotionManager: Invalid JSON");
				return;
			}
			_activePromos.Clear();
			foreach (JValue item in pJson)
			{
				if (item.IsObject)
				{
					Promo promo = new Promo();
					promo.Serialize(new SimpleJSerializer(item, SimpleJSerializer.SerializationMode.SerializeFromJSON));
					LoadPromo(promo);
					continue;
				}
				Debug.LogError("CrossPromotionManager: Got unexpected non-object: " + item.ToString());
				break;
			}
			RemoveExpiredPromos();
			SavePromosToCache();
		}

		public void LoadPromo(Promo pPromo)
		{
			if (string.IsNullOrEmpty(pPromo.Id))
			{
				Debug.LogError("CrossPromotionManager: Promo has no id and can't be loaded");
				return;
			}
			pPromo.Image = GetCachedImage(pPromo);
			if (pPromo.Image != null)
			{
				AddPromo(pPromo);
			}
			else if (!string.IsNullOrEmpty(pPromo.ImageURL))
			{
				WWWUtility.OpenURL(pPromo.ImageURL, (bool pOk, WWW pWWW) =>
				{
					if (pOk)
					{
						pPromo.Image = pWWW.texture;
						SaveCachedImage(pPromo);
						AddPromo(pPromo);
					}
					else
					{
						Debug.LogError("Failed to load image");
					}
				});
			}
			else
			{
				AddPromo(pPromo);
			}
		}

		private void AddPromo(Promo pPromo)
		{
			for (int i = 0; i < _activePromos.Count; i++)
			{
				if (_activePromos[i].Id == pPromo.Id)
				{
					_activePromos[i] = pPromo;
					return;
				}
			}
			_activePromos.Add(pPromo);
		}

		public Promo GetRandomPromo()
		{
			float num = 0f;
			foreach (Promo activePromo in _activePromos)
			{
				num += activePromo.Weight;
			}
			if (num <= 0f)
			{
				return null;
			}
			float num2 = UnityEngine.Random.value * num;
			foreach (Promo activePromo2 in _activePromos)
			{
				num2 -= activePromo2.Weight;
				if (num2 <= 0f)
				{
					return activePromo2;
				}
			}
			return null;
		}

		private void RemoveExpiredPromos()
		{
			DateTime now = DateTime.Now;
			int num = 0;
			while (num < _activePromos.Count)
			{
				if (_activePromos[num].EndTime < now)
				{
					_activePromos.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
		}

		private Texture2D GetCachedImage(Promo pPromo)
		{
			string imageCachePath = pPromo.ImageCachePath;
			if (File.Exists(imageCachePath) && File.GetLastWriteTime(imageCachePath) >= DateTime.Now - TimeSpan.FromDays(3.0))
			{
				byte[] array = File.ReadAllBytes(imageCachePath);
				if (array.Length > 0)
				{
					Texture2D texture2D = new Texture2D(8, 8);
					texture2D.LoadImage(array);
					return texture2D;
				}
			}
			return null;
		}

		private void SaveCachedImage(Promo pPromo)
		{
			if (pPromo != null && pPromo.Image != null)
			{
				File.WriteAllBytes(pPromo.ImageCachePath, pPromo.Image.EncodeToPNG());
				Debug.Log("Saved promo image at " + pPromo.ImageCachePath);
			}
		}
	}
}
