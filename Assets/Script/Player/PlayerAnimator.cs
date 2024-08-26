using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    public Player player;
    private Vector2 mousePosition;
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
        if (player.health <= 0 && death != true){
            Vector2 direction = (Vector2)player.transform.position - mousePosition;
            if (direction.y < 0 && Math.Abs(direction.x) < Math.Abs(direction.y)){
                animator.SetInteger("Dead", 2);
            }
            else{
                animator.SetInteger("Dead", 1);
            }
            death=true;
        }

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        animator.SetInteger("Walk", 0);
        if (Input.GetKey(KeyCode.W)){
            animator.SetInteger("Walk", 2);
        }
        else if (Input.GetKey(KeyCode.S)){
            animator.SetInteger("Walk", 1);
        }
        else if (Input.GetKey(KeyCode.A)){
            animator.SetInteger("Walk", 3);
        }
        else if (Input.GetKey(KeyCode.D)){
            animator.SetInteger("Walk", 4);
        }
        
        animator.SetInteger("Drag", 0);
        if (Input.GetMouseButton(0)){
            Vector2 direction = (Vector2)player.transform.position - mousePosition;
            if (direction.y > 0 && Math.Abs(direction.x) < Math.Abs(direction.y)){
                animator.SetInteger("Drag", 1);
            }
            else if (direction.y < 0 && Math.Abs(direction.x) < Math.Abs(direction.y)){
                animator.SetInteger("Drag", 2);
            }
            else{
                animator.SetInteger("Drag", 3);
            }
        }

    }
}
