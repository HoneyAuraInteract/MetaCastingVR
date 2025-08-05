using System.Collections.Generic;
using UnityEngine;
using QuickOutline;

[CreateAssetMenu(fileName = "HighlightData", menuName = "Highlighting/Highlight Data", order = 1)]
public class HighlightData : ScriptableObject
{
    public Outline.Mode mode = Outline.Mode.OutlineVisible;
    public OutlineGroups group;
    public Transform ghost;
    public bool removeGhost = false;
    public List<Transform> groupList = new List<Transform>();
}
