
#if NOT_USE_LUA
using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogDataChangeScene : BaseDialogData
{
    public DialogDataChangeScene(t_BookDialog data) : base(data) { }

    public override void ShowDialog()
    {
        base.ShowDialog();
        LOG.Warn(typeof(DialogDataChangeScene).Name + dialogID);
    }
}
#endif