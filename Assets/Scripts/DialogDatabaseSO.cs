using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DislogDatabase", menuName = "Dialog System/Database")]

public class DialogDatabaseSO : ScriptableObject
{
    public List<DialogSo> dialogs = new List<DialogSo>();

    private Dictionary<int, DialogSo> dialogsById;

    public void Initailize()
    {
        dialogsById = new Dictionary<int, DialogSo>();

        foreach (var dialog in dialogs)
        {
            if (dialog != null)
            {
                dialogsById[dialog.id] = dialog;
            }
        }
    }

    public DialogSo GetDialogById(int id)
    {
        if (dialogsById == null)
            Initailize();
        if(dialogsById.TryGetValue(id, out DialogSo dialog))
            return dialog;

        return null;
    }
}
