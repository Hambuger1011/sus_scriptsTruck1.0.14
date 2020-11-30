
#if NOT_USE_LUA
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pb;
public class DialogDataEnterNpcName : BaseDialogData
{
    public DialogDataEnterNpcName(t_BookDialog data) : base(data) { }

    public override void ShowDialog()
    {
        base.ShowDialog();
        LOG.Warn(typeof(DialogDataEnterNpcName).Name + dialogID);
    }
}
#endif