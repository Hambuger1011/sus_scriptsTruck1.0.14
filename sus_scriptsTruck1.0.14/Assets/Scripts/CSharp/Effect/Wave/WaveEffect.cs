/*
 * https://blog.csdn.net/puppet_master/article/details/52975666
 * http://www.voidcn.com/article/p-evhphsok-re.html
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEffect : MonoBehaviour
{
    [Header("距离系数")]
    public float distanceFactor = 60.0f;
    [Header("时间系数")]
    public float timeFactor = -30.0f;
    [Header("sin函数结果系数")]
    public float totalFactor = 1.0f;

    [Header("波纹宽度")]
    public float waveWidth = 0.3f;
    [Header("波纹扩散的速度")]
    public float waveSpeed = 0.3f;

    private float waveStartTime;


    Material _material;
    public Material material
    {
        get
        {
            if(_material == null)
            {
                _material = new Material(Shader.Find("Game/UI/Effect/Wave"));
            }
            return _material;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //计算波纹移动的距离，根据enable到目前的时间*速度求解
        float curWaveDistance = (Time.time - waveStartTime) * waveSpeed;
        //设置一系列参数
        material.SetFloat("_distanceFactor", distanceFactor);
        material.SetFloat("_timeFactor", timeFactor);
        material.SetFloat("_totalFactor", totalFactor);
        material.SetFloat("_waveWidth", waveWidth);
        material.SetFloat("_curWaveDis", curWaveDistance);
        material.SetVector("_startPos", startPos);
        Graphics.Blit(source, destination, material);
    }

    void OnEnable()
    {
        //设置startTime
        //waveStartTime = Time.time;

    }



    
    private Vector4 startPos = new Vector4(0.5f, 0.5f, 0, 0);
    

    //void Update()
    //{
    //    if (Input.GetMouseButton(0))
    //    {
    //        Vector2 mousePos = Input.mousePosition;
    //        //将mousePos转化为（0，1）区间
    //        startPos = new Vector4(mousePos.x / Screen.width, mousePos.y / Screen.height, 0, 0);
    //        waveStartTime = Time.time;
    //    }

    //}

    public void Play(float speed)
    {
        waveStartTime = Time.time;
        this.waveSpeed = speed;
    }
}
