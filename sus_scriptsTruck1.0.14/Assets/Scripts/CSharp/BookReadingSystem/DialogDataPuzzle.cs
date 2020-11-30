﻿
#if NOT_USE_LUA
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pb;

public class DialogDataPuzzle  : BaseDialogData
{

    public DialogDataPuzzle(t_BookDialog data) : base(data) { }

    public override void ShowDialog()
    {
        base.ShowDialog();
        LOG.Warn(typeof(DialogDataPuzzle).Name + dialogID);
    }
}
#endif