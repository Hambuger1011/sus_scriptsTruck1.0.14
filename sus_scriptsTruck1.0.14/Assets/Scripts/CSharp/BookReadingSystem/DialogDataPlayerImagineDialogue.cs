
#if NOT_USE_LUA
using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogDataPlayerImagineDialogue : BaseDialogData
{
    public DialogDataPlayerImagineDialogue(t_BookDialog data) : base(data) { }

    public override void ShowDialog()
    {
        base.ShowDialog();
        LOG.Warn(typeof(DialogDataPlayerImagineDialogue).Name + dialogID);
    }
}
#endif