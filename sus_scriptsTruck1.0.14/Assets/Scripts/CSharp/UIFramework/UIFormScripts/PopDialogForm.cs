using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopDialogForm : BaseUIForm {
    

    public void SetPopDialogFormStyle(PopDialogFormStyle style)
    {
        switch (style)
        {
            case PopDialogFormStyle.Comfirm:
                break;
            case PopDialogFormStyle.ConfirmAndCancel:
                break;
        }
    }

    public void AddComfirmButtonListener(System.Action call) { }

    public enum PopDialogFormStyle
    {
        Comfirm,
        ConfirmAndCancel,

    }
}
