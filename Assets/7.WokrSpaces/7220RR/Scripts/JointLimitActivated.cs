using System.Collections.Generic;
using UnityEngine;

public class JointLimitActivated : Activated
{
    public bool isMinLimit;
    public List<Joint> minLimitLists;
    public List<float> minLimitFloatLists;
    private List<float> minbaseFloatLists;
    public bool isMaxLimit;
    public List<Joint> maxLimitLists;
    public List<float> maxLimitFloatLists;
    private List<float> maxbaseFloatLists;

    protected override void Awake()
    {
        base.Awake();
        if (isMinLimit)
        {
            MinLimitSet();
        }

        if (isMaxLimit)
        {
            MaxLimitSet();
        }
    }

    public override void Activate()
    {
        base.Activate();
        MinLimitReSet();
        MaxLimitReSet();
    }

    private void MinLimitSet()
    {
        for (int i = 0; i < minLimitLists.Count; i++)
        {
            if (minbaseFloatLists.Count <= i)
            {
                minbaseFloatLists.Add(0);
            }

            if (minLimitLists[i] is ConfigurableJoint con)
            {
                minbaseFloatLists[i] = con.linearLimit.limit;
            }
            else if (minLimitLists[i] is HingeJoint hin)
            {
                minbaseFloatLists[i] = hin.limits.min;
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
        for (int i = 0; i < maxLimitLists.Count; i++)
        {
            if (maxbaseFloatLists.Count <= i)
            {
                maxbaseFloatLists.Add(0);
            }

            if (maxLimitLists[i] is ConfigurableJoint con)
            {
                maxbaseFloatLists[i] = con.linearLimit.limit;
            }
            else if (maxLimitLists[i] is HingeJoint hin)
            {
                maxbaseFloatLists[i] = hin.limits.max;
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
            else if (minLimitLists[i] is HingeJoint hin)
            {
                JointLimits limits = hin.limits;
                limits.min = maxbaseFloatLists[i];
                hin.limits = limits;
            }
        }
    }




}
