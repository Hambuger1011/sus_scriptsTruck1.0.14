
#if NOT_USE_LUA
using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogDataPhoneCallDialogue : BaseDialogData
{
    public DialogDataPhoneCallDialogue(t_BookDialog data) : base(data) { }

    public override void ShowDialog()
    {
        base.ShowDialog();
        LOG.Warn(typeof(DialogDataPhoneCallDialogue).Name + dialogID);
    }

    public override void StopDialog()
    {
        base.StopDialog();

    }
}
#endif