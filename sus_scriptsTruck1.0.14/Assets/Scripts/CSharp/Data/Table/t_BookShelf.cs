namespace pb
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public partial class t_BookShelf
    {
        int[] _BookIDs;
        public int[] BookIDs
        {
            get
            {
                if (_BookIDs == null)
                {
                    _BookIDs = JsonHelper.JsonToObject<int[]>(BookIDList);
                }
                return _BookIDs;
            }
        }
    }
}