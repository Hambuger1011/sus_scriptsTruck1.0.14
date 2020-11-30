namespace pb
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public partial class t_contrast
    {
        int[] _nums;
        public int[] nums
        {
            get
            {
                if (_nums == null)
                {
                    _nums = JsonHelper.JsonToObject<int[]>(num);
                }
                return _nums;
            }
        }

        int[] _frames;
        public int[] frames
        {
            get
            {
                if (_frames == null)
                {
                    _frames = JsonHelper.JsonToObject<int[]>(frame);
                }
                return _frames;
            }
        }
    }
}
