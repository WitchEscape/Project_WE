using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firewood : MonoBehaviour
{
    [Header("불꽃 파티클")] public ParticleSystem fireParticle;
    [Header("성냥 태그")] public string matchTag;
    [Header("불 붙이는데 필요한 힘")] public float fireForce;
    [Header("가마솥")] public Cauldron cauldron;

    private void Start()
    {
        if (fireParticle.gameObject.activeSelf)
        {
            fireParticle.gameObject.SetActive(false);
        }

        CheckFireState();
    }

    private void CheckFireState()
    {
        if (PuzzleProgressManager.Instance.GetPuzzleState("Puzzle_2") == PuzzleProgressManager.PuzzleState.InProgress)
        {
            FireAcivate();
        }
    }

    private void FireAcivate()
    {
        cauldron.isFire = true;
        fireParticle.gameObject.SetActive(true);
        fireParticle.Play();
    }


    private void OnEnable()
    {
        SaveLoadManager.OnLoadComplete += CheckFireState;
    }

    private void OnDisable()
    {
        SaveLoadManager.OnLoadComplete -= CheckFireState;
    }


    private void OnTriggerStay(Collider other)
    {
        if (fireParticle.gameObject.activeSelf == true) { return; }

        if (other.CompareTag(matchTag))
        {
            Debug.Log($"other 태그: {other.tag}, matchTag: {matchTag}");

            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (fireForce <= rb.velocity.magnitude)
            {
                FireAcivate();
                PuzzleProgressManager.Instance.SettPuzzleState("Puzzle_2", PuzzleProgressManager.PuzzleState.InProgress);
            }
        }
    }


}
