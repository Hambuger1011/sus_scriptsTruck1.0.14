using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewPageBar : MonoBehaviour
{
    public bool playOnAwake = true;
    public bool loop = true;
    public float playInterval = 3;
    public RectTransform tipsRoot;
    public GameObject tipsItemPfb;

    private float playTimer;
    private ScrollViewPage page;
    public bool isPlaying { get; private set; }
    
    List<Toggle> _useItems = new List<Toggle>(8);
    Stack<Toggle> _unseItems = new Stack<Toggle>(8);

    private void Awake()
    {
        tipsItemPfb.SetActive(false);
        page = this.GetComponent<ScrollViewPage>();
        //SetTipsCount(page.content.childCount);
        page.onPageIndexChange += OnPageIndexChange;
        if (playOnAwake)
        {
            Play();
        }
    }

    public void SetTipsCount(int count)
    {
        ClearItems();
        for (int i=0;i<count;++i)
        {
            var item = GetItem();
            _useItems.Add(item);
        }
        if(_useItems.Count > 0)
        {
            _useItems[0].isOn = true;
        }
    }


    void ClearItems()
    {
        foreach (var item in _useItems)
        {
            RemoveItem(item);
        }
        _useItems.Clear();
    }

    Toggle GetItem()
    {
        Toggle toggle = null;
        if (_unseItems.Count > 0)
        {
            toggle = _unseItems.Pop();
            toggle.gameObject.SetActiveEx(true);
        }
        else
        {
            var go = GameObject.Instantiate(tipsItemPfb, tipsRoot);
            toggle = go.GetComponent<Toggle>();
            go.SetActiveEx(true);
            var t = go.transform;
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;
        }
        toggle.isOn = false;
        return toggle;
    }

    void RemoveItem(Toggle item)
    {
        item.gameObject.SetActiveEx(false);
        _unseItems.Push(item);
    }

    private void Update()
    {
        if(!isPlaying)
        {
            return;
        }

        if(!page.canAbsorb)
        {
            playTimer = 0;
            return;
        }
        playTimer += Time.deltaTime;
        if(playTimer >= playInterval)
        {
            playTimer = 0;
            if(!loop)
            {
                isPlaying = false;
            }
            page.MoveToNext();
        }
    }

    public void Play()
    {
        isPlaying = true;
        playTimer = 0;
    }

    private void OnPageIndexChange(int idx)
    {
        if(idx < 0 || idx >= _useItems.Count){
            return;
        }
        _useItems[idx].isOn = true;
    }
}
