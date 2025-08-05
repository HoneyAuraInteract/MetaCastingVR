using System.Collections.Generic;
using QuickOutline;
using UnityEngine;

public class OutlineObject : MonoBehaviour
{
    public Outline.Mode mode;
    public OutlineGroups group;
    public Transform ghostWhenOutline;
    public bool removeGhostAfterOutlineOff;

    private void Start()
    {
        OutlineManager.Instance.AddOutlineScripts(this.transform);
        OutlineGroup outlineGroup = new OutlineGroup();

        if (group != OutlineGroups.None) outlineGroup.group = group;
        if (ghostWhenOutline != null) outlineGroup.ghost = ghostWhenOutline;
        outlineGroup.removeGhost = removeGhostAfterOutlineOff;
        outlineGroup.groupList = new List<Transform>() { this.transform };
        outlineGroup.mode = mode;
        OutlineManager.Instance.AddGroup(outlineGroup);
        ChangeSettings();
    }

    private void ChangeSettings()
    {
        if (GetComponent<Outline>() != null)
        {
            GetComponent<Outline>().OutlineMode = mode;
        }
    }

}