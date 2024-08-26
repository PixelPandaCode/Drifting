using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sail : Plank
{
    public float speed = 1.5f;
    public Vector3 playerInput;
    public bool controlMode = false;
    public GameObject space1;
    public GameObject space2;
    // Start is called before the first frame update

    void Awake()
    {
        health = maxHealth;
        anim = GetComponentInChildren<Animator>();
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        plankType = 1;
        space1.SetActive(false);
        space2.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isConnected)
        {
            SailMovement();
        }
    }

    private void SailMovement(){
        if (isOnPlank(PlankManager.Instance.player.transform.position)){

            if (Input.GetKey(KeyCode.Space))
            {
                controlMode = true;
                space1.SetActive(false);
                space2.SetActive(true);

            }
            else{
                controlMode = false;
                PlankManager.Instance.player.speed = PlankManager.Instance.player.baseSpeed;
                space1.SetActive(true);
                space2.SetActive(false);
            }
            if (controlMode){

                playerInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
                float total_speed = 0;

                foreach (Plank plank in PlankManager.Instance.connectedPlanks)
                {
                    if (plank.plankType == 1){
                        total_speed+=1.0f * speed;
                    }
                }
                PlankManager.Instance.player.speed=total_speed;
                foreach (Plank plank in PlankManager.Instance.connectedPlanks)
                {
                    plank.transform.position += playerInput * total_speed * Time.deltaTime;
                }
            }

        }
        else{
            space1.SetActive(false);
            space2.SetActive(false);
        }
    }

}
