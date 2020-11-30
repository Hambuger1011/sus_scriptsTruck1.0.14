using Object= UnityEngine.Object;


using Framework;
using System;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameGraphicRaycaster : GraphicRaycaster
{

	private const int TileCount = 4;

	public bool ignoreScrollRect = true;

	private int raycast_mask = 4;

	private Canvas canvas_;

	private List<GameGraphicRaycaster.Item> m_allItems = new List<GameGraphicRaycaster.Item>();

	private GameGraphicRaycaster.Tile[] m_tiles;

	private int m_tileSizeX;

	private int m_tileSizeY;

	private int m_screenWidth;

	private int m_screenHeight;

	private Vector3[] corners = new Vector3[4];

	[HideInInspector]
	[NonSerialized]
	public bool tilesDirty;

	public GameGraphicRaycaster.RaycastMode raycastMode = GameGraphicRaycaster.RaycastMode.Game;

	[NonSerialized]
	private List<Graphic> m_RaycastResults = new List<Graphic>();

	private Canvas canvas
	{
		get
		{
			if (this.canvas_ != null)
			{
				return this.canvas_;
			}
			this.canvas_ = base.GetComponent<Canvas>();
			return this.canvas_;
		}
	}

    /// <summary>
    /// 重写点击事件
    /// </summary>
	public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		switch (this.raycastMode)
		{
		case GameGraphicRaycaster.RaycastMode.Unity:
			base.Raycast(eventData, resultAppendList);//unity内置点击系统
			break;
		case GameGraphicRaycaster.RaycastMode.Game:
			this.CustomRaycast(eventData, resultAppendList, false);
			break;
		case GameGraphicRaycaster.RaycastMode.Game_tile:
			this.CustomRaycast(eventData, resultAppendList, true);
			break;
		}
	}


    private void CustomRaycast(PointerEventData eventData, List<RaycastResult> resultAppendList, bool useTiles)
    {
        if (this.canvas == null)
        {
            return;
        }
        Vector2 viewport;
        if (this.eventCamera == null)
        {
            viewport = new Vector2(eventData.position.x / (float)Screen.width, eventData.position.y / (float)Screen.height);
        }
        else
        {
            viewport = this.eventCamera.ScreenToViewportPoint(eventData.position);
        }
        if (viewport.x < 0f || viewport.x > 1f || viewport.y < 0f || viewport.y > 1f)
        {
            return;
        }
        float hitDistance = float.MinValue;
        Ray ray = default(Ray);
        if (this.eventCamera != null)
        {
            ray = this.eventCamera.ScreenPointToRay(eventData.position);
        }
        this.m_RaycastResults.Clear();
        Vector2 screenPos = eventData.position;
        List<GameGraphicRaycaster.Item> List;
        if (useTiles && this.m_tiles != null)
        {
            int x = Mathf.Clamp((int)screenPos.x / this.m_tileSizeX, 0, 3);
            int y = Mathf.Clamp((int)screenPos.y / this.m_tileSizeY, 0, 3);
            int idx = y * 4 + x;
            List = this.m_tiles[idx].items;
        }
        else
        {
            List = this.m_allItems;
        }
        for (int i = 0; i < List.Count; i++)
        {
            List[i].Raycast(this.m_RaycastResults, screenPos, this.eventCamera);
        }
        this.m_RaycastResults.Sort((Graphic g1, Graphic g2) => g2.depth.CompareTo(g1.depth));
        this.AppendResultList(ref ray, hitDistance, resultAppendList, this.m_RaycastResults);
    }


    private void AppendResultList(ref Ray ray, float hitDistance, List<RaycastResult> resultAppendList, List<Graphic> raycastResults)
    {
        for (int i = 0; i < raycastResults.Count; i++)
        {
            GameObject gameObject = raycastResults[i].gameObject;
            bool flag = true;
            if (base.ignoreReversedGraphics)
            {
                if (this.eventCamera == null)
                {
                    Vector3 forward = gameObject.transform.rotation * Vector3.forward;
                    flag = (Vector3.Dot(Vector3.forward, forward) > 0f);
                }
                else
                {
                    Vector3 camForward = this.eventCamera.transform.rotation * Vector3.forward;
                    Vector3 forward = gameObject.transform.rotation * Vector3.forward;
                    flag = (Vector3.Dot(camForward, forward) > 0f);
                }
            }
            if (flag)
            {
                float dist;
                if (this.eventCamera == null || this.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    dist = 0f;
                }
                else
                {
                    dist = Vector3.Dot(gameObject.transform.forward, gameObject.transform.position - ray.origin) / Vector3.Dot(gameObject.transform.forward, ray.direction);
                    if (dist < 0f)
                    {
                        continue;
                    }
                }
                if (dist < hitDistance)
                {
                    RaycastResult raycastResult = default(RaycastResult);
                    raycastResult.gameObject = gameObject;
                    raycastResult.module = this;
                    raycastResult.distance = dist;
                    raycastResult.index = (float)resultAppendList.Count;
                    raycastResult.depth = raycastResults[i].depth;
                    raycastResult.sortingLayer = this.canvas.sortingLayerID;
                    raycastResult.sortingOrder = this.canvas.sortingOrder;
                    RaycastResult raycastResult2 = raycastResult;
                    resultAppendList.Add(raycastResult2);
                }
            }
        }
    }

    private void CalcItemCoord(ref GameGraphicRaycaster.Coord coord, GameGraphicRaycaster.Item item)
	{
		item.m_rectTransform.GetWorldCorners(this.corners);
		int x = int.MaxValue;
		int y = int.MinValue;
		int numX = int.MaxValue;
		int numY = int.MinValue;
		Camera worldCamera = this.canvas.worldCamera;
		for (int i = 0; i < this.corners.Length; i++)
		{
			Vector3 vector = CUIUtility.WorldToScreenPoint(worldCamera, this.corners[i]);
			x = Mathf.Min((int)vector.x, x);
			y = Mathf.Max((int)vector.x, y);
			numX = Mathf.Min((int)vector.y, numX);
			numY = Mathf.Max((int)vector.y, numY);
		}
		coord.x = Mathf.Clamp(x / this.m_tileSizeX, 0, 3);
		coord.numX = Mathf.Clamp(y / this.m_tileSizeX, 0, 3) - coord.x + 1;
		coord.y = Mathf.Clamp(numX / this.m_tileSizeY, 0, 3);
		coord.numY = Mathf.Clamp(numY / this.m_tileSizeY, 0, 3) - coord.y + 1;
	}

	private void AddToTileList(GameGraphicRaycaster.Item item)
	{
		int num = item.m_coord.x + item.m_coord.y * 4;
		for (int i = 0; i < item.m_coord.numX; i++)
		{
			for (int j = 0; j < item.m_coord.numY; j++)
			{
				int num2 = j * 4 + i + num;
				this.m_tiles[num2].items.Add(item);
			}
		}
	}

	private void RemoveFromTileList(GameGraphicRaycaster.Item item)
	{
		if (item.m_coord.IsValid())
		{
			int num = item.m_coord.x + item.m_coord.y * 4;
			for (int i = 0; i < item.m_coord.numX; i++)
			{
				for (int j = 0; j < item.m_coord.numY; j++)
				{
					int num2 = j * 4 + i + num;
					this.m_tiles[num2].items.Remove(item);
				}
			}
			item.m_coord = GameGraphicRaycaster.Coord.Invalid;
		}
	}

	public void InitTiles()
	{
		if (this.m_tiles != null)
		{
			return;
		}
		this.m_tiles = new GameGraphicRaycaster.Tile[16];
		for (int i = 0; i < this.m_tiles.Length; i++)
		{
			this.m_tiles[i] = new GameGraphicRaycaster.Tile();
		}
		this.m_screenWidth = Screen.width;
		this.m_screenHeight = Screen.height;
		this.m_tileSizeX = this.m_screenWidth / 4;
		this.m_tileSizeY = this.m_screenHeight / 4;
	}

	private void UpdateTiles_Editor()
	{
		if ((this.m_screenWidth == Screen.width && this.m_screenHeight == Screen.height) || this.m_tiles == null || this.raycastMode != GameGraphicRaycaster.RaycastMode.Game_tile)
		{
			return;
		}
		this.m_screenWidth = Screen.width;
		this.m_screenHeight = Screen.height;
		this.m_tileSizeX = this.m_screenWidth / 4;
		this.m_tileSizeY = this.m_screenHeight / 4;
		for (int i = 0; i < this.m_tiles.Length; i++)
		{
			this.m_tiles[i].items.Clear();
		}
		for (int j = 0; j < this.m_allItems.Count; j++)
		{
			GameGraphicRaycaster.Item item = this.m_allItems[j];
			this.CalcItemCoord(ref item.m_coord, item);
			this.AddToTileList(item);
		}
	}

	public void UpdateTiles()
	{
		if (this.raycastMode != GameGraphicRaycaster.RaycastMode.Game_tile)
		{
			return;
		}
		GameGraphicRaycaster.Coord invalid = GameGraphicRaycaster.Coord.Invalid;
		for (int i = 0; i < this.m_allItems.Count; i++)
		{
			GameGraphicRaycaster.Item item = this.m_allItems[i];
			this.CalcItemCoord(ref invalid, item);
			if (!invalid.Equals(ref item.m_coord))
			{
				this.RemoveFromTileList(item);
				item.m_coord = invalid;
				this.AddToTileList(item);
			}
		}
	}

	private void Update()
	{
		this.UpdateTiles_Editor();
	}

	protected override void Start()
	{
		base.Start();
		this.InitializeAllItems();
	}

	private void InitializeAllItems()
	{
		if (this.raycastMode != GameGraphicRaycaster.RaycastMode.Game && this.raycastMode != GameGraphicRaycaster.RaycastMode.Game_tile)
		{
			return;
		}
		this.m_allItems.Clear();
		Image[] componentsInChildren = base.gameObject.GetComponentsInChildren<Image>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (this.IsGameObjectHandleInput(componentsInChildren[i].gameObject))
			{
				GameGraphicRaycaster.Item item = GameGraphicRaycaster.Item.Create(componentsInChildren[i]);
				if (item != null)
				{
					this.m_allItems.Add(item);
				}
			}
		}
		if (this.raycastMode == GameGraphicRaycaster.RaycastMode.Game_tile)
		{
			this.InitTiles();
			for (int j = 0; j < this.m_allItems.Count; j++)
			{
				GameGraphicRaycaster.Item item2 = this.m_allItems[j];
				this.CalcItemCoord(ref item2.m_coord, item2);
				this.AddToTileList(item2);
			}
		}
	}

	public void RemoveGameObject(GameObject go)
	{
		if (go == null || this.m_allItems == null)
		{
			return;
		}
		Image component = go.GetComponent<Image>();
		if (component != null && this.IsGameObjectHandleInput(go))
		{
			this.RemoveItem(component);
		}
	}

	public void RemoveItem(Image image)
	{
		if (image == null || this.m_allItems == null)
		{
			return;
		}
		for (int i = 0; i < this.m_allItems.Count; i++)
		{
			if (this.m_allItems[i].m_image == image)
			{
				if (this.raycastMode == GameGraphicRaycaster.RaycastMode.Game_tile)
				{
					this.RemoveFromTileList(this.m_allItems[i]);
				}
				this.m_allItems.RemoveAt(i);
				break;
			}
		}
	}

	public void RefreshGameObject(GameObject go)
	{
		if (this.raycastMode != GameGraphicRaycaster.RaycastMode.Game && this.raycastMode != GameGraphicRaycaster.RaycastMode.Game_tile)
		{
			return;
		}
		if (go == null || this.m_allItems == null)
		{
			return;
		}
		Image[] componentsInChildren = go.GetComponentsInChildren<Image>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (this.IsGameObjectHandleInput(componentsInChildren[i].gameObject))
			{
				this.RefreshItem(componentsInChildren[i]);
			}
		}
	}

	public void RefreshItem(Image image)
	{
		if (image == null || this.m_allItems == null)
		{
			return;
		}
		GameGraphicRaycaster.Item item = null;
		for (int i = 0; i < this.m_allItems.Count; i++)
		{
			if (this.m_allItems[i].m_image == image)
			{
				item = this.m_allItems[i];
				break;
			}
		}
		if (item != null)
		{
			if (this.raycastMode == GameGraphicRaycaster.RaycastMode.Game_tile)
			{
				this.RemoveFromTileList(item);
			}
		}
		else
		{
			item = GameGraphicRaycaster.Item.Create(image);
			if (item == null)
			{
				return;
			}
			this.m_allItems.Add(item);
		}
		if (this.raycastMode == GameGraphicRaycaster.RaycastMode.Game_tile)
		{
			this.CalcItemCoord(ref item.m_coord, item);
			this.AddToTileList(item);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.m_allItems.Clear();
		this.m_RaycastResults.Clear();
		if (this.m_tiles != null)
		{
			for (int i = 0; i < this.m_tiles.Length; i++)
			{
				this.m_tiles[i].items.Clear();
			}
			this.m_tiles = null;
		}
	}


	private bool IsGameObjectHandleInput(GameObject go)
	{
		return go.GetComponent<CUIEventScript>() != null || (!this.ignoreScrollRect && go.GetComponent<ScrollRect>() != null);
	}



    #region declaration

    public enum RaycastMode
    {
        Unity,
        Game,
        Game_tile
    }

    private struct Coord
    {
        public int x;

        public int y;

        public int numX;

        public int numY;

        public static GameGraphicRaycaster.Coord Invalid = new GameGraphicRaycaster.Coord
        {
            x = -1,
            y = -1
        };

        public bool IsValid()
        {
            return this.x >= 0 && this.y >= 0;
        }

        public bool Equals(ref GameGraphicRaycaster.Coord r)
        {
            return r.x == this.x && r.y == this.y && r.numX == this.numX && r.numY == this.numY;
        }
    }

    private class Item
    {
        public Image m_image;

        public RectTransform m_rectTransform;

        public GameGraphicRaycaster.Coord m_coord = GameGraphicRaycaster.Coord.Invalid;

        public static GameGraphicRaycaster.Item Create(Image image)
        {
            if (image == null)
            {
                return null;
            }
            return new GameGraphicRaycaster.Item
            {
                m_image = image,
                m_rectTransform = (image.gameObject.transform as RectTransform)
            };
        }

        public void Raycast(List<Graphic> raycastResults, Vector2 pointerPosition, Camera eventCamera)
        {
            if (this.m_image.enabled && this.m_rectTransform.gameObject.activeInHierarchy && RectTransformUtility.RectangleContainsScreenPoint(this.m_rectTransform, pointerPosition, eventCamera))
            {
                raycastResults.Add(this.m_image);
            }
        }
    }

    private class Tile
    {
        public List<GameGraphicRaycaster.Item> items = new List<GameGraphicRaycaster.Item>();
    }
    #endregion
}
