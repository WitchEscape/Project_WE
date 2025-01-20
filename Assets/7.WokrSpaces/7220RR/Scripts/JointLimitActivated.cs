using System.Collections.Generic;
using UnityEngine;

public class JointLimitActivated : Activated
{
    public bool isMinLimit;
    public List<Joint> minLimitLists;
    public List<float> minLimitFloatLists = new List<float>();
    private List<float> minbaseFloatLists = new List<float>();
    public bool isMaxLimit;
    public List<Joint> maxLimitLists;
    public List<float> maxLimitFloatLists = new List<float>();
    private List<float> maxbaseFloatLists = new List<float>();

    protected override void Awake()
    {
        base.Awake();
        if (isMinLimit)
        {
            MinLimitSet();
        }

        if (isMaxLimit)
        {
            print("ismaxlimit");
            MaxLimitSet();
        }
    }

    public override void Activate()
    {
        base.Activate();
        if (isMinLimit)
        {
            MinLimitReSet();
        }

        if (isMaxLimit)
        {
            MaxLimitReSet();
        }
    }

    private void MinLimitSet()
    {
        if (minLimitFloatLists.Count != minLimitLists.Count)
        {
            while (minLimitLists.Count > minLimitFloatLists.Count) minLimitFloatLists.Add(0);
            while (minLimitLists.Count < minLimitFloatLists.Count) minLimitFloatLists.RemoveAt(minLimitFloatLists.Count - 1);
        }
        if (minbaseFloatLists.Count != minLimitLists.Count)
        {
            while (minLimitLists.Count > minbaseFloatLists.Count) minbaseFloatLists.Add(0);
            while (minLimitLists.Count < minbaseFloatLists.Count) minbaseFloatLists.RemoveAt(minbaseFloatLists.Count - 1);
        }

        for (int i = 0; i < minLimitLists.Count; i++)
        {
            if (minLimitLists[i] is ConfigurableJoint con)
            {
                SoftJointLimit limit = con.linearLimit;
                minbaseFloatLists[i] = limit.limit;
                limit.limit = minLimitFloatLists[i];
                con.linearLimit = limit;
            }
            else if (minLimitLists[i] is HingeJoint hin)
            {
                JointLimits limits = hin.limits;
                minbaseFloatLists[i] = limits.min;
                limits.min = minLimitFloatLists[i];
                hin.limits = limits;
            }
        }
    }

    private void MinLimitReSet()
    {
        for (int i = 0; i < minLimitLists.Count; i++)
        {
            if (minLimitLists[i] is ConfigurableJoint con)
            {
                SoftJointLimit limit = con.linearLimit;
                limit.limit = minbaseFloatLists[i];
                con.linearLimit = limit;
            }
            else if (minLimitLists[i] is HingeJoint hin)
            {
                JointLimits limits = hin.limits;
                limits.min = minbaseFloatLists[i];
                hin.limits = limits;
            }
        }
    }

    private void MaxLimitSet()
    {

        if (maxbaseFloatLists.Count != maxLimitLists.Count)
        {
            while (maxLimitLists.Count > maxbaseFloatLists.Count) maxbaseFloatLists.Add(0);
            while (maxLimitLists.Count < maxbaseFloatLists.Count) maxbaseFloatLists.RemoveAt(maxbaseFloatLists.Count - 1);
        }
        if (maxLimitFloatLists.Count != maxLimitLists.Count)
        {
            while (maxLimitLists.Count > maxLimitFloatLists.Count) maxLimitFloatLists.Add(0);
            while (maxLimitLists.Count < maxLimitFloatLists.Count) maxLimitFloatLists.RemoveAt(maxLimitFloatLists.Count - 1);
        }
        for (int i = 0; i < maxLimitLists.Count; i++)
        {
            print("MaxSet");
            if (maxbaseFloatLists.Count <= i)
            {
                maxbaseFloatLists.Add(0);
            }

            if (maxLimitLists[i] is ConfigurableJoint con)
            {
                print("hin");
                SoftJointLimit limit = con.linearLimit;
                maxbaseFloatLists[i] = limit.limit;
                limit.limit = maxLimitFloatLists[i];
                con.linearLimit = limit;
            }
            else if (maxLimitLists[i] is HingeJoint hin)
            {
                print("hin");

                JointLimits limits = hin.limits;
                maxbaseFloatLists[i] = limits.max;
                limits.max = maxLimitFloatLists[i];
                hin.limits = limits;
            }
        }
    }

    private void MaxLimitReSet()
    {
        for (int i = 0; i < maxLimitLists.Count; i++)
        {
            if (maxLimitLists[i] is ConfigurableJoint con)
            {
                SoftJointLimit limit = con.linearLimit;
                limit.limit = maxbaseFloatLists[i];
                con.linearLimit = limit;
            }
            else if (maxLimitLists[i] is HingeJoint hin)
            {
                JointLimits limits = hin.limits;
                limits.min = maxbaseFloatLists[i];
                hin.limits = limits;
            }
        }
    }




}
