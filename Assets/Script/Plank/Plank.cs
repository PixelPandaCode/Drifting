using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Plank : MonoBehaviour
{
    [SerializeField] public float maxHealth = 5;
    public float health = 0;
    public bool isDestroyed = false;
    public Animator anim;
    public BoxCollider2D boxCollider;
    public Rigidbody2D rb2d;
    public bool isConnected = false;
    public bool isHooked = false;
    public List<Plank> neighbors = new List<Plank>();
    // 0: normal plank, 1: sail, 2: cannon
    public int plankType = 0;
    public Animator destroyClip;
    public GameObject normalState;
    public float deadTime = 0;
    public bool isEnemy = false;

    // Start is called before the first frame update
    void Awake()
    {
        health = maxHealth;
        anim = GetComponentInChildren<Animator>();
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public float GetWidth()
    {
        return boxCollider.bounds.size.x;
    }

    public float GetHeight()
    {
        return boxCollider.bounds.size.y;
    }

    public void DamagePlank(float damage)
    {
        health -= damage;
        Player player = PlankManager.Instance.player;
        if (health <= 1)
        {
            if (destroyClip != null)
            {
                destroyClip.gameObject.SetActive(true);
                destroyClip.enabled = false;
            }
            if (normalState != null)
            {
                normalState.SetActive(false);
            }
            if (health == 0)
            {
                if (this == PlankManager.Instance.playerLocatedPlank)
                {
                    player.DamagePlayer(1.0f);
                    if (neighbors.Count > 0)
                    {
                        player.transform.position = neighbors[0].transform.position;
                        PlankManager.Instance.playerLocatedPlank = neighbors[0];
                    }
                    else
                    {
                        player.health = 0.0f;
                        PlankManager.Instance.GameOver();
                    }
                }
                destroyClip.enabled = true;
                // Get the current AnimatorStateInfo from the Animator
                AnimatorStateInfo stateInfo = destroyClip.GetCurrentAnimatorStateInfo(0);

                // Get the current time of the animation in seconds
                deadTime = 0.7f;
                boxCollider.enabled = false;
                StartCoroutine(DestroySelfAfterDelay(deadTime));
            }
        }
    }

    // Coroutine to destroy the object after a delay
    private IEnumerator DestroySelfAfterDelay(float delay)
    {
        // Wait for the specified amount of time
        yield return new WaitForSeconds(delay);

        // Destroy the game object
        PlankManager.Instance.DestroyPlank(this);
    }

    public bool isOnPlank(Vector2 position)
    {
        if (boxCollider == null)
        {
            return false;
        }
        return boxCollider.bounds.Contains(position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Plank connectedPlank = collision.collider.GetComponent<Plank>();
        if (connectedPlank != null)
        {
            if (isHooked && connectedPlank.isConnected)
            {
                PlankManager.Instance.ConnectPlank(connectedPlank, this);
            }
            else if (rb2d.bodyType != RigidbodyType2D.Static)
            {
                rb2d.velocity = (rb2d.position - (Vector2)connectedPlank.transform.position).normalized * PlankManager.Instance.moveSpeed;
            }
        }
    }

    void onDestory()
    {
        
    }
}
