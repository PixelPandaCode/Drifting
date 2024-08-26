using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Hit : Plank
{
    public float speed;
    public float lerpSpeed = 1f;  // How fast the lerp should be
    // Start is called before the first frame update
    void Awake()
    {
        health = maxHealth;
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        isHooked = true;
        if (anim != null)
        {
            anim.SetLayerWeight(0, 1f);
        }
        isEnemy = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = PlankManager.Instance.player.transform.position - transform.position;
        direction.Normalize();
        rb2d.velocity = Vector2.Lerp(rb2d.velocity, direction * speed, lerpSpeed * Time.deltaTime);
        if (health <= 0)
        {
            anim.SetInteger("Dead", 1);
            StartCoroutine(DestroySelfAfterDelay(0.4f));
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Plank hitPlank = collision.collider.GetComponent<Plank>();
        if (hitPlank != null)
        {
            hitPlank.DamagePlank(1);
            rb2d.velocity = (rb2d.position - (Vector2)hitPlank.transform.position).normalized * PlankManager.Instance.moveSpeed;
        }
    }
}
