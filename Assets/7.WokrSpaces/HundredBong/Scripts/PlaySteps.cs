using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlaySteps : MonoBehaviour
{
    private PlayableDirector director;
    public List<Step> steps;
    private void Update()
    {
        director = GetComponent<PlayableDirector>();
    }

    public void PlayStepIndex(int index)
    {
        Step step = steps[index];
        if (step.hasPlayed == false)
        {
            step.hasPlayed = true;

            director.Stop();
            director.time = step.time;
            director.Play();
        }
    }

}

[System.Serializable]
public class Step
{
    public string name;
    public float time;
    public bool hasPlayed;
}