﻿
#if NOT_USE_LUA
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pb;

public class DialogDataSceneTap : BaseDialogData
{

    public DialogDataSceneTap(t_BookDialog data) : base(data) { }

    public override void ShowDialog()
    {
        base.ShowDialog();
        LOG.Warn(typeof(DialogDataPlayerImagineDialogue).Name + dialogID);
    }
}
#endif