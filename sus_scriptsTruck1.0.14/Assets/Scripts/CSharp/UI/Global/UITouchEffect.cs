//using HedgehogTeam.EasyTouch;
using System.Collections;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITouchEffect : CUIComponent
{
    public GameObject effectTemplate;
    public int maxEffectNum = 5;

    float[] m_effectPlayTime;
    ParticleSystem[] m_effects;

    int m_effectSpawnNum;
    int m_effectPlayIndex;
    Queue<int> m_useEffects;

    public override void Initialize(CUIForm formScript)
    {
        if (!base.m_isInitialized)
        {
            this.m_customUpdateFlags |= enCustomUpdateFlag.eUpdate;
            m_effectPlayTime = new float[maxEffectNum];
            m_effects = new ParticleSystem[maxEffectNum];
            m_useEffects = new Queue<int>(maxEffectNum);
            m_effectSpawnNum = 0;
            m_effectPlayIndex = 0;
        }
        base.Initialize(formScript);

    }

    void OnEnable()
    {
        EasyTouch.On_TouchStart += OnTouchStart;
    }

    void OnDisable()
    {
        EasyTouch.On_TouchStart -= OnTouchStart;
        while (m_useEffects.Count > 0)
        {
            var idx = m_useEffects.Dequeue();
            m_effects[idx].gameObject.SetActiveEx(false);
        }
    }

    public override void CustomUpdate()
    {
        base.CustomUpdate();
        while(m_useEffects.Count > 0)
        {
            var idx = m_useEffects.Peek();
            if(Time.time - m_effectPlayTime[idx] < 1)
            {
                break;
            }
            m_useEffects.Dequeue();
            m_effects[idx].gameObject.SetActiveEx(false);
        }
    }

    private void OnTouchStart(Gesture gesture)
    {
        Vector3 pos = this.transform.position;
        float z = pos.z;
        pos = CUIManager.Instance.uiCamera.ScreenToWorldPoint(gesture.position);
        pos.z = z;
        //uiParticle.transform.position = pos;
        //uiParticle.uiParticleSystem.Simulate(0.02f, true, true);
        //uiParticle.uiParticleSystem.Play(true);

        //CUIUtility.Screen_To_UGUI_WorldPoint(CUIID)
        var effect = GetEffect();
        var t = effect.transform;
        t.position = pos;
        effect.Simulate(0, true, true);
        effect.Play(true);
    }

    ParticleSystem GetEffect()
    {
        if(m_effectSpawnNum < maxEffectNum)
        {
            var go = GameObject.Instantiate(effectTemplate);
            go.transform.SetParent(this.transform);
            m_effects[m_effectSpawnNum] = go.GetComponent<ParticleSystem>();
            ++m_effectSpawnNum;
        }
        if(m_effectPlayIndex >= m_effectSpawnNum)
        {
            m_effectPlayIndex = 0;
        }

        if(m_useEffects.Count >= m_effectSpawnNum)
        {
            m_useEffects.Dequeue();
        }
        m_useEffects.Enqueue(m_effectPlayIndex);
        m_effectPlayTime[m_effectPlayIndex] = Time.time;
        var effect = m_effects[m_effectPlayIndex++];
        effect.gameObject.SetActiveEx(true);
        return effect;
    }
}
