
#if NOT_USE_LUA
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pb;

public class DialogDataManualChangeScene : BaseDialogData
{

    public DialogDataManualChangeScene(t_BookDialog data) : base(data) { }

    public override void ShowDialog()
    {
        base.ShowDialog();
        LOG.Warn(typeof(DialogDataManualChangeScene).Name + dialogID);
    }
}
#endif