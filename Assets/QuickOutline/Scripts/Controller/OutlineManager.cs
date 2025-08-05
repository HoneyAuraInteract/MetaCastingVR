using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QuickOutline;

public enum OutlineGroups
{
    FireExtinguisher = 0,
    FirePin = 1,
    FireNozzle = 2,
    Telephone = 3,
    ExitDoorHandle = 4,
    DustPan = 5, BlueBin = 6, RedBin = 7, SkyBlueBin = 8, Cloth = 9, Cotton = 10, Broom = 11, Computer = 12, HeadPhone = 13, Bulb = 14, MetalWaste = 15, OtherDustPan = 16,
    Neils = 17, Gears = 18, Scaled = 19,
    None,

    BenchViceHandle = 101,
    Hammer = 102,
    DotPunch = 103,
    FirstJob = 104,
    Scale = 105,
    LevelingPlate = 106,
    JennyCaliper = 107,

    ArcWeldingCablePlug = 201,
    PositiveSocket = 202,
    NegativeSoket = 203,
    EarthClamp = 204,
    ElectrodeHolder = 205,
    ElektrodeRod = 206,
    ArcWeldingPowerSwitch = 207,
    ArcWeldingKnob = 208,
    AreWeldingJob = 209,
    ArcWeldingMainBoardPowerSwitch = 210,
}

[System.Serializable]
public class OutlineGroup
{
    public Outline.Mode mode = Outline.Mode.OutlineVisible;
    public OutlineGroups group;
    public Transform ghost;
    public bool removeGhost = false;
    public List<Transform> groupList = new List<Transform>();
}

public class OutlineManager : MonoBehaviour
{
    public static OutlineManager Instance;
    public Color outlineColor;
    public float outlineWidth;

    // SerializeField Variables
    [SerializeField] private List<Transform> objectsToEnableOutline = new List<Transform>();
    [SerializeField] private List<OutlineGroup> outlineGroups = new List<OutlineGroup>();

    // Static Variables
    public static event Action OnOutlineAddComplete;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        //  StartCoroutine(CheckAndAddOutline()); // Old Code
    }

    public OutlineManager AddGroup(OutlineGroup og)
    {
        if (og == null) return Instance;
        Instance.outlineGroups.Add(og);
        Instance.objectsToEnableOutline.AddRange(og.groupList);
        return Instance;
    }

    public IEnumerator AddOutlineScripts(Transform ele)
    {
        Debug.Log("AddOutlineScripts");

        Outline outline = ele.gameObject.GetComponent<Outline>() ? ele.gameObject.GetComponent<Outline>() : ele.gameObject.AddComponent<Outline>();

        outline.enabled = false;

        outline.OutlineWidth = outlineWidth;

        outline.OutlineColor = outlineColor;

        yield return null;

        FlashingOutline fo = ele.gameObject.GetComponent<FlashingOutline>() ? ele.gameObject.GetComponent<FlashingOutline>() : ele.gameObject.AddComponent<FlashingOutline>();

        yield return null;
    }


    public void EnableFlashing(OutlineGroups group, float delay = 0)
    {
        //StartCoroutine(FlashingDelay(group, true, delay)); // Old Code
        FlashingDelay(group, true, delay);
    }

    public void DisableFlashing(OutlineGroups group, float delay = 0)
    {
        //StartCoroutine(FlashingDelay(group, false, delay)); // Old Code
        FlashingDelay(group, false, delay);
    }

    void FlashingDelay(OutlineGroups group, bool enable, float delay = 0)
    {
        OutlineGroup outlineGroup = outlineGroups.Find(x => x.group == group);

        if (outlineGroup != null)
        {
            if (outlineGroup.ghost != null)
            {
                Debug.Log($"ghost: {outlineGroup.ghost.name}");
                outlineGroup.ghost.gameObject.SetActive(enable);
                if (outlineGroup.removeGhost && !enable) { outlineGroup.ghost = null; }
            }

            if (outlineGroup.groupList != null)
            {
                // First, ensure all outline components are added **without yielding**
                foreach (var e in outlineGroup.groupList)
                {
                    if (e != null)
                    {
                        Outline outline = e.gameObject.GetComponent<Outline>() ? e.gameObject.GetComponent<Outline>() : e.gameObject.AddComponent<Outline>();
                        outline.enabled = false;
                        outline.OutlineWidth = outlineWidth;
                        outline.OutlineColor = outlineColor;

                        FlashingOutline fo = e.gameObject.GetComponent<FlashingOutline>() ? e.gameObject.GetComponent<FlashingOutline>() : e.gameObject.AddComponent<FlashingOutline>();

                        if (fo != null)
                        {
                            fo.CurrentState = enable ? FlashState.Enable : FlashState.Disable;
                        }
                    }
                }
            }
        }
    }
    public static void SkipOutline(List<OutlineGroups> _outlineGroups)
    {
        foreach (OutlineGroups _item in _outlineGroups)
        {
            Instance.DisableFlashing(_item);
        }
    }
}

#region ... // Old Code
/*private IEnumerator CheckAndAddOutline()
   {
       yield return new WaitForSeconds(2f);
       objectsToEnableOutline = objectsToEnableOutline.Distinct().ToList();
       yield return new WaitForSeconds(0.1f);
       yield return ApplyOutlineComponent();
   }

   private ienumerator applyoutlinecomponent()
   {
       foreach (var ele in objectstoenableoutline)
       {
           yield return addoutlinescripts(ele);
       }

       onoutlineaddcomplete?.invoke();
   }*/

/* IEnumerator FlashingDelay(OutlineGroups group, bool enable, float delay = 0)
     {
         yield return new WaitForSeconds(delay);
         OutlineGroup outlineGroup = outlineGroups.Find(x => x.group == group);

         if (outlineGroup != null)
         {
             if (outlineGroup.ghost != null)
             {
                 Debug.Log($"ghost: {outlineGroup.ghost.name}");
                 outlineGroup.ghost.gameObject.SetActive(enable);
                 if (outlineGroup.removeGhost && !enable) { outlineGroup.ghost = null; }
             }

             if (outlineGroup.groupList != null)
             {
                 // First, ensure all outline components are added **without yielding**
                 foreach (var e in outlineGroup.groupList)
                 {
                     if (e != null)
                     {
                         Outline outline = e.gameObject.GetComponent<Outline>() ? e.gameObject.GetComponent<Outline>() : e.gameObject.AddComponent<Outline>();
                         outline.enabled = false;
                         outline.OutlineWidth = outlineWidth;
                         outline.OutlineColor = outlineColor;

                         FlashingOutline fo = e.gameObject.GetComponent<FlashingOutline>() ? e.gameObject.GetComponent<FlashingOutline>() : e.gameObject.AddComponent<FlashingOutline>();

                         if (fo != null)
                         {
                             fo.CurrentState = enable ? FlashState.Enable : FlashState.Disable;
                         }
                     }
                     //if (e != null && e.GetComponent<FlashingOutline>() != null)
                     //{
                     //    e.GetComponent<FlashingOutline>().CurrentState = enable ? FlashState.Enable : FlashState.Disable;
                     //}
                 }

                 // After all are set, then apply the flash state together
                 *//*foreach (var e in outlineGroup.groupList)
                 {
                     if (e != null && e.GetComponent<FlashingOutline>() != null)
                     {
                         e.GetComponent<FlashingOutline>().CurrentState = enable ? FlashState.Enable : FlashState.Disable;
                     }
                 }*//*
             }
         }

         yield return null;
     }*/

/*IEnumerator FlashingDelay(OutlineGroups group, bool enable, float delay = 0)
{
    yield return new WaitForSeconds(delay);
    OutlineGroup outlineGroup = outlineGroups.Find(x => x.group == group);

    if (outlineGroup != null)
    {
        if (outlineGroup.ghost != null)
        {
            Debug.Log($"ghost: {outlineGroup.ghost.name}");
            outlineGroup.ghost.gameObject.SetActive(enable);
            if (outlineGroup.removeGhost && !enable) { outlineGroup.ghost = null; }
        }

        if (outlineGroup.groupList != null)
        {
            foreach (var e in outlineGroup.groupList)
            {
                //add outline if not
                if (e != null)
                {
                    yield return AddOutlineScripts(e);
                }

                if (e != null && e.GetComponent<FlashingOutline>() != null)
                {
                    e.GetComponent<FlashingOutline>().CurrentState = enable ? FlashState.Enable : FlashState.Disable;
                }
            }
        }
    }
}*/
#endregion