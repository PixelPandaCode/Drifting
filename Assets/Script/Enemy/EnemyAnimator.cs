using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyAnimator : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animator;
    private bool death = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetInteger("Dead", 0);
        if (death!=true){
            animator.SetInteger("Dead", 2);
            death=true;
        }
    }
}
