using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 相机跟随
/// </summary>
public class BaseCameraFollow : MonoBehaviour
{
    protected Transform cameraTranform;
    protected Transform targetTranform;
    protected Vector3 offsetPos;
    protected int smoothing = 4;
    protected bool acviteCamera = false;
    void Awake()
    {
        cameraTranform = GameObject.Find("Main Camera").transform;
    }

    public void SetTarget(Transform vTarget)
    {
        targetTranform = vTarget;
        if(cameraTranform != null && targetTranform != null)
        {
            offsetPos = new Vector3(0, 8, -4);
        }
    }

    public bool ActiveCamera
    {
        set
        {
            acviteCamera = value;
        }
        get
        {
            return acviteCamera;
        }
    }

    public Transform MainCameraTrans()
    {
        return cameraTranform;
    }

    //public void OnFixedUpdate()
    //{
    //    cameraFollow();
    //}

    //public void OnStopFixedUpdate(LinkedList<IBattleObjectFixedUpdate> list)
    //{
    //    list.Remove(this);
    //}

    private void cameraFollow()
    {
        if (targetTranform != null && cameraTranform != null && acviteCamera)
        {
            Vector3 curPos = targetTranform.position + offsetPos;
            cameraTranform.position = Vector3.Lerp(cameraTranform.position, curPos, (smoothing * Time.deltaTime));
        }
    }
}
