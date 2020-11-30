using Framework;

using Object= UnityEngine.Object;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UGUI;

public class SGameGraphicRaycaster : GraphicRaycaster
{
	public enum RaycastMode
	{
		Unity,
		Sgame,
		Sgame_tile
	}

	private struct Coord
	{
		public int x;

		public int y;

		public int numX;

		public int numY;

		public static Coord Invalid = new Coord
		{
			x = -1,
			y = -1
		};

		public bool IsValid()
		{
			return x >= 0 && y >= 0;
		}

		public bool Equals(ref Coord r)
		{
			return r.x == x && r.y == y && r.numX == numX && r.numY == numY;
		}
	}

	private class Item
	{
		public Image m_image;

		public RectTransform m_rectTransform;

		public Coord m_coord = Coord.Invalid;

		public static Item Create(Image image)
		{
			if (image == null)
			{
				return null;
			}
			Item item = new Item();
			item.m_image = image;
			item.m_rectTransform = (image.gameObject.transform as RectTransform);
			return item;
		}

		public void Raycast(List<Graphic> raycastResults, Vector2 pointerPosition, Camera eventCamera)
		{
			if (m_image.enabled && m_rectTransform.gameObject.activeInHierarchy && RectTransformUtility.RectangleContainsScreenPoint(m_rectTransform, pointerPosition, eventCamera))
			{
				raycastResults.Add(m_image);
			}
		}
	}

	private class Tile
	{
		public List<Item> items = new List<Item>();
	}

	private const int TileCount = 4;

	public bool ignoreScrollRect = true;

	private Canvas canvas_;

	private List<Item> m_allItems = new List<Item>();

	private Tile[] m_tiles;

	private int m_tileSizeX;

	private int m_tileSizeY;

	private int m_screenWidth;

	private int m_screenHeight;

	private Vector3[] corners = new Vector3[4];

	[NonSerialized]
	[HideInInspector]
	public bool tilesDirty;

	public RaycastMode raycastMode = RaycastMode.Sgame;

	private bool m_isInitialized;

	[NonSerialized]
	private List<Graphic> m_RaycastResults = new List<Graphic>();

	private Canvas canvas
	{
		get
		{
			if (canvas_ != null)
			{
				return canvas_;
			}
			canvas_ = base.GetComponent<Canvas>();
			return canvas_;
		}
	}

	public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		switch (raycastMode)
		{
		case RaycastMode.Unity:
			base.Raycast(eventData, resultAppendList);
			break;
		case RaycastMode.Sgame:
			Raycast2(eventData, resultAppendList, false);
			break;
		case RaycastMode.Sgame_tile:
			Raycast2(eventData, resultAppendList, true);
			break;
		}
	}

	private void CalcItemCoord(ref Coord coord, Item item)
	{
		item.m_rectTransform.GetWorldCorners(corners);
		int num = 2147483647;
		int num2 = -2147483648;
		int num3 = 2147483647;
		int num4 = -2147483648;
		Camera worldCamera = canvas.worldCamera;
		for (int i = 0; i < corners.Length; i++)
		{
			Vector3 vector = CUIUtility.WorldToScreenPoint(worldCamera, corners[i]);
			num = Mathf.Min((int)vector.x, num);
			num2 = Mathf.Max((int)vector.x, num2);
			num3 = Mathf.Min((int)vector.y, num3);
			num4 = Mathf.Max((int)vector.y, num4);
		}
		coord.x = Mathf.Clamp(num / m_tileSizeX, 0, 3);
		coord.numX = Mathf.Clamp(num2 / m_tileSizeX, 0, 3) - coord.x + 1;
		coord.y = Mathf.Clamp(num3 / m_tileSizeY, 0, 3);
		coord.numY = Mathf.Clamp(num4 / m_tileSizeY, 0, 3) - coord.y + 1;
	}

	private void AddToTileList(Item item)
	{
		int num = item.m_coord.x + item.m_coord.y * 4;
		for (int i = 0; i < item.m_coord.numX; i++)
		{
			for (int j = 0; j < item.m_coord.numY; j++)
			{
				int num2 = j * 4 + i + num;
				m_tiles[num2].items.Add(item);
			}
		}
	}

	private void RemoveFromTileList(Item item)
	{
		if (item.m_coord.IsValid())
		{
			int num = item.m_coord.x + item.m_coord.y * 4;
			for (int i = 0; i < item.m_coord.numX; i++)
			{
				for (int j = 0; j < item.m_coord.numY; j++)
				{
					int num2 = j * 4 + i + num;
					m_tiles[num2].items.Remove(item);
				}
			}
			item.m_coord = Coord.Invalid;
		}
	}

	public void InitTiles()
	{
		if (m_tiles == null)
		{
			m_tiles = new Tile[16];
			for (int i = 0; i < m_tiles.Length; i++)
			{
				m_tiles[i] = new Tile();
			}
			m_screenWidth = Screen.width;
			m_screenHeight = Screen.height;
			m_tileSizeX = m_screenWidth / 4;
			m_tileSizeY = m_screenHeight / 4;
		}
	}

	public void UpdateTiles()
	{
		if (raycastMode == RaycastMode.Sgame_tile)
		{
			Coord invalid = Coord.Invalid;
			for (int i = 0; i < m_allItems.Count; i++)
			{
				Item item = m_allItems[i];
				CalcItemCoord(ref invalid, item);
				if (!invalid.Equals(ref item.m_coord))
				{
					RemoveFromTileList(item);
					item.m_coord = invalid;
					AddToTileList(item);
				}
			}
		}
	}

	public bool IsInitialized()
	{
		return m_isInitialized;
	}

	public bool IsUseSGameMode()
	{
		return raycastMode == RaycastMode.Sgame || raycastMode == RaycastMode.Sgame_tile;
	}

	protected override void Start()
	{
		base.Start();
		InitializeAllItems();
		m_isInitialized = true;
	}

	private void InitializeAllItems()
	{
		if (raycastMode != RaycastMode.Sgame && raycastMode != RaycastMode.Sgame_tile)
		{
			return;
		}
		m_allItems.Clear();
		Image[] componentsInChildren = base.gameObject.GetComponentsInChildren<Image>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (IsGameObjectHandleInput(componentsInChildren[i].gameObject))
			{
				Item item = Item.Create(componentsInChildren[i]);
				if (item != null)
				{
					m_allItems.Add(item);
				}
			}
		}
		if (raycastMode == RaycastMode.Sgame_tile)
		{
			InitTiles();
			for (int j = 0; j < m_allItems.Count; j++)
			{
				Item item2 = m_allItems[j];
				CalcItemCoord(ref item2.m_coord, item2);
				AddToTileList(item2);
			}
		}
	}

	public void RemoveGameObject(GameObject go)
	{
		if (!(go == null) && m_allItems != null)
		{
			Image component = go.GetComponent<Image>();
			if (component != null && IsGameObjectHandleInput(go))
			{
				RemoveItem(component);
			}
		}
	}

	public void RemoveItem(Image image)
	{
		if (!(image == null) && m_allItems != null)
		{
			int num = 0;
			while (true)
			{
				if (num < m_allItems.Count)
				{
					if (!(m_allItems[num].m_image == image))
					{
						num++;
						continue;
					}
					break;
				}
				return;
			}
			if (raycastMode == RaycastMode.Sgame_tile)
			{
				RemoveFromTileList(m_allItems[num]);
			}
			m_allItems.RemoveAt(num);
		}
	}

	public void RefreshGameObject(GameObject go)
	{
		if (raycastMode != RaycastMode.Sgame && raycastMode != RaycastMode.Sgame_tile)
		{
			return;
		}
		if (!(go == null) && m_allItems != null)
		{
			Image[] componentsInChildren = go.GetComponentsInChildren<Image>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (IsGameObjectHandleInput(componentsInChildren[i].gameObject))
				{
					RefreshItem(componentsInChildren[i]);
				}
			}
		}
	}

	public void RefreshItem(Image image)
	{
		Item item;
		if (!(image == null) && m_allItems != null)
		{
			item = null;
			int num = 0;
			while (num < m_allItems.Count)
			{
				if (!(m_allItems[num].m_image == image))
				{
					num++;
					continue;
				}
				item = m_allItems[num];
				break;
			}
			if (item != null)
			{
				if (raycastMode == RaycastMode.Sgame_tile)
				{
					RemoveFromTileList(item);
				}
				goto IL_009c;
			}
			item = Item.Create(image);
			if (item != null)
			{
				m_allItems.Add(item);
				goto IL_009c;
			}
		}
		return;
		IL_009c:
		if (raycastMode == RaycastMode.Sgame_tile)
		{
			CalcItemCoord(ref item.m_coord, item);
			AddToTileList(item);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_allItems.Clear();
		m_RaycastResults.Clear();
		if (m_tiles != null)
		{
			for (int i = 0; i < m_tiles.Length; i++)
			{
				m_tiles[i].items.Clear();
			}
			m_tiles = null;
		}
	}

	private void Raycast2(PointerEventData eventData, List<RaycastResult> resultAppendList, bool useTiles)
	{
		if (!(canvas == null))
		{
			Vector2 vector = default(Vector2);
			if (eventCamera == null)
			{
				Vector2 position = eventData.position;
				float x = position.x / (float)Screen.width;
				Vector2 position2 = eventData.position;
				vector = new Vector2(x, position2.y / (float)Screen.height);
			}
			else
			{
				vector = eventCamera.ScreenToViewportPoint(eventData.position);
			}
			if (!(vector.x < 0f) && !(vector.x > 1f) && !(vector.y < 0f) && !(vector.y > 1f))
			{
				float hitDistance = 3.40282347E+38f;
				Ray ray = default(Ray);
				if (eventCamera != null)
				{
					ray = eventCamera.ScreenPointToRay(eventData.position);
				}
				m_RaycastResults.Clear();
				Vector2 position3 = eventData.position;
				List<Item> listView = null;
				if (useTiles && m_tiles != null)
				{
					int num = Mathf.Clamp((int)position3.x / m_tileSizeX, 0, 3);
					int num2 = Mathf.Clamp((int)position3.y / m_tileSizeY, 0, 3);
					int num3 = num2 * 4 + num;
					listView = m_tiles[num3].items;
				}
				else
				{
					listView = m_allItems;
				}
				for (int i = 0; i < listView.Count; i++)
				{
					listView[i].Raycast(m_RaycastResults, position3, eventCamera);
				}
				m_RaycastResults.Sort((Graphic g1, Graphic g2) => g2.depth.CompareTo(g1.depth));
				AppendResultList(ref ray, hitDistance, resultAppendList, m_RaycastResults);
			}
		}
	}

	private bool IsGameObjectHandleInput(GameObject go)
	{
		return go.GetComponent<CUIEventScript>() != null || 
            //go.GetComponent<CUIMiniEventScript>() != null || 
            //go.GetComponent<CUIMiniEventWithDragScript>() != null ||
            //go.GetComponent<CUIJoystickScript>() != null || 
            (!ignoreScrollRect && go.GetComponent<ScrollRect>() != null);
	}

	private void AppendResultList(ref Ray ray, float hitDistance, List<RaycastResult> resultAppendList, List<Graphic> raycastResults)
	{
		for (int i = 0; i < raycastResults.Count; i++)
		{
			GameObject gameObject = raycastResults[i].gameObject;
			bool flag = true;
			if (base.ignoreReversedGraphics)
			{
				if (eventCamera == null)
				{
					Vector3 rhs = gameObject.transform.rotation * Vector3.forward;
					flag = (Vector3.Dot(Vector3.forward, rhs) > 0f);
				}
				else
				{
					Vector3 lhs = eventCamera.transform.rotation * Vector3.forward;
					Vector3 rhs2 = gameObject.transform.rotation * Vector3.forward;
					flag = (Vector3.Dot(lhs, rhs2) > 0f);
				}
			}
			float num;
			if (flag)
			{
				num = 0f;
				if (!(eventCamera == null) && canvas.renderMode != 0)
				{
					num = Vector3.Dot(gameObject.transform.forward, gameObject.transform.position - ray.origin) / Vector3.Dot(gameObject.transform.forward, ray.direction);
					if (!(num < 0f))
					{
						goto IL_012f;
					}
					continue;
				}
				num = 0f;
				goto IL_012f;
			}
			continue;
			IL_012f:
			if (!(num >= hitDistance))
			{
				RaycastResult raycastResult = default(RaycastResult);
				raycastResult.gameObject = gameObject;
				raycastResult.module = this;
				raycastResult.distance = num;
				raycastResult.index = (float)resultAppendList.Count;
				raycastResult.depth = raycastResults[i].depth;
				raycastResult.sortingLayer = canvas.sortingLayerID;
				raycastResult.sortingOrder = canvas.sortingOrder;
				RaycastResult item = raycastResult;
				resultAppendList.Add(item);
			}
		}
	}
}
