
#if NOT_USE_LUA
using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogDataStoryItems : BaseDialogData
{
    public DialogDataStoryItems(t_BookDialog data) : base(data) { }

    public override void ShowDialog()
    {
        base.ShowDialog();
        LOG.Warn(typeof(DialogDataStoryItems).Name + dialogID);
    }
}
#endif
