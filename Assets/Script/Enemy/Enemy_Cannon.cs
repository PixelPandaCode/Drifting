using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Cannon : Enemy_Hit
{
    public bool isFlipped = false;
    public GameObject shellTemplate;
    public GameObject shooter;
    public GameObject pirate;

    public float shootRate = 1.0f;
    private float shootTimer = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            if (health <= 0)
            {
                anim.SetInteger("Dead", 1);
                StartCoroutine(DestroySelfAfterDelay(0.4f));
            }
        }
        shootTimer += Time.deltaTime;
        if (PlankManager.Instance.player.transform.position.x > transform.position.x)
        {
            isFlipped = true;
        }
        else
        {
            isFlipped = false;
        }
        if (isFlipped)
        {
            shooter.transform.localScale = new Vector3(-1, 1, 1);
            pirate.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            shooter.transform.localScale = new Vector3(1, 1, 1);
            pirate.transform.localScale = new Vector3(-1, 1, 1);
        }
        if (shootTimer > shootRate)
        {
            shootTimer = 0;
            Vector3 origin = transform.position;
            if (isFlipped)
            {
                origin += new Vector3(0.29f, 0.03f, 0);
            }
            else
            {
                origin += new Vector3(-0.29f, 0.03f, 0);
            }
            Shell newShell = Instantiate(shellTemplate, origin, Quaternion.identity).GetComponent<Shell>();
            Vector3 direction = PlankManager.Instance.player.transform.position - origin;
            newShell.Init(direction);
            newShell.isEnemy = true;
        }
    }

    // Coroutine to destroy the object after a delay
    private IEnumerator DestroySelfAfterDelay(float delay)
    {
        // Wait for the specified amount of time
        yield return new WaitForSeconds(delay);

        // Destroy the game object
        Destroy(gameObject);
        PlankManager.Instance.killedEnemy += 1;
    }
}
