using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public GameObject source;
    public BoxCollider2D boxCollider;
    public Rigidbody2D rb2d;
    public float speed;
    public float damage;
    public float liveTime = 3.0f;
    public bool isEnemy = false;

    // Start is called before the first frame update
    void Awake()
    {
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector3 direction)
    {
        rb2d.velocity = direction.normalized * speed;
    }

    // Update is called once per frame
    void Update()
    {
        liveTime -= Time.deltaTime;
        if (liveTime < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Plank connectedPlank = collision.GetComponent<Plank>();
        if (connectedPlank != null)
        {
            bool canDamage = isEnemy && connectedPlank.isConnected;
            canDamage = canDamage || (!isEnemy && connectedPlank.isEnemy);
            if (canDamage)
            {
                connectedPlank.DamagePlank(damage);
                Destroy(gameObject);
            }
        }
    }
}
