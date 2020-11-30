namespace pb
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public partial class t_BookDialog
    {
        int[] _SceneParticalsArray;
        public int[] SceneParticalsArray
        {
            get
            {
                if(_SceneParticalsArray == null)
                {
                    _SceneParticalsArray = JsonHelper.JsonToObject<int[]>(SceneParticals);
                }
                return _SceneParticalsArray;
            }
        }
    }
}