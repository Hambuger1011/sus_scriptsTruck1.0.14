
#if NOT_USE_LUA
using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogDataOtherDialogue : BaseDialogData
{
    public DialogDataOtherDialogue(t_BookDialog data) : base(data) { }

    public override void ShowDialog()
    {
        base.ShowDialog();
        LOG.Warn(typeof(DialogDataOtherDialogue).Name + dialogID);
    }
}
#endif