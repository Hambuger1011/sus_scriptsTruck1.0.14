
#if NOT_USE_LUA
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pb;

public class DialogDataSceneInteraction : BaseDialogData
{

    public DialogDataSceneInteraction(t_BookDialog data) : base(data) { }

    public override void ShowDialog()
    {
        base.ShowDialog();
        LOG.Warn(typeof(DialogDataPlayerImagineDialogue).Name + dialogID);
    }
}
#endif