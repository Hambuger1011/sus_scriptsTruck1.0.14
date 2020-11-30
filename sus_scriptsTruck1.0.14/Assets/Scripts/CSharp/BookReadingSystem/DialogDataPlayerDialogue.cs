
#if NOT_USE_LUA
using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogDataPlayerDialogue : BaseDialogData
{
    public DialogDataPlayerDialogue(t_BookDialog data) : base(data) { }

    public override void ShowDialog()
    {
        base.ShowDialog();
        LOG.Warn(typeof(DialogDataPlayerDialogue).Name + dialogID);
    }
}
#endif