using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoldingLeaf : MonoBehaviour, IActivatable
{
    private bool unfolded = false;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Activate()
    {
        if (!unfolded)
            animator.Play("Activate");
        else
            animator.Play("Deactivate");
        unfolded = !unfolded;
    }

    public bool IsActive()
    {
        return animator.IsInTransition(0);
    }
}
