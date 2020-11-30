using Framework;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolItem
{
    void SetPoolHolder(IPool holder);
    uint GetUsingSeq();
    void SetUsingSeq(uint seq);
    void OnActive();
    void OnDeactive();
}
