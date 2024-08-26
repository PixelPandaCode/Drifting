using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class Cannon : Plank
{
    public bool controlMode = false;
    public GameObject space1;
    public GameObject space2;
    public bool isFlipped = false;
    public GameObject shellTemplate;
    public GameObject shooter;

    public float shootRate = 1.0f;
    public float playerShootRate = 0.5f;
    private float shootTimer = 0.0f;
    public float lineWidth = 0.02f;
    public LineRenderer lineRenderer;
    public Vector2 direction;

    void Awake()
    {
        health = maxHealth;
        anim = GetComponentInChildren<Animator>();
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        space1.SetActive(false);
        space2.SetActive(false);
        isFlipped = Random.Range(0f, 1f) > 0.5f;
        if (isFlipped)
        {
            shooter.transform.localScale = new Vector3(-1, 1, 1);
        }
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isConnected)
        {
            return;
        }
        UpdateControlInfo();
        shootTimer += Time.deltaTime;
        if (shootTimer > shootRate && !controlMode)
        {
            shootTimer = 0;
            Shoot();
        }
    }

    public void Shoot()
    {
        Vector3 origin = transform.position;
        if (isFlipped)
        {
            origin += new Vector3(0.15f, 0.07f, 0);
        }
        else
        {
            origin += new Vector3(-0.15f, 0.07f, 0);
        }
        Shell newShell = Instantiate(shellTemplate, origin, Quaternion.identity).GetComponent<Shell>();
        newShell.Init(direction);
    }

    private void UpdateControlInfo()
    {
        if (isOnPlank(PlankManager.Instance.player.transform.position))
        {
            if (Input.GetKey(KeyCode.Space))
            {
                controlMode = true;
                PlankManager.Instance.player.speed = 0;
                PlankManager.Instance.player.canHook = false;
                lineRenderer.enabled = false;
                space1.SetActive(false);
                space2.SetActive(true);
            }
            else
            {
                controlMode = false;
                PlankManager.Instance.player.speed = PlankManager.Instance.player.baseSpeed;
                PlankManager.Instance.player.canHook = true;
                lineRenderer.enabled = false;
                space1.SetActive(true);
                space2.SetActive(false);
            }
        }
        else
        {
            controlMode = false;
            space1.SetActive(false);
            space2.SetActive(false);
        }

        if (controlMode)
        {
            ControlCannon();
        }
        else
        {
            if (isFlipped)
            {
                direction = Vector3.right;
            }
            else
            {
                direction = Vector3.left;
            }
        }
    }

    private void ControlCannon()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePosition.x < transform.position.x)
        {
            isFlipped = false;
        }
        else
        {
            isFlipped = true;
        }
        if (isFlipped)
        {
            shooter.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            shooter.transform.localScale = new Vector3(1, 1, 1);
        }
        Vector3 origin = transform.position;
        if (isFlipped)
        {
            origin += new Vector3(0.15f, 0.07f, -0.5f);
        }
        else
        {
            origin += new Vector3(-0.15f, 0.07f, -0.5f);
        }
        direction = (mousePosition - origin).normalized;
        Vector3[] positions = new Vector3[2];
        positions[0] = origin;
        positions[1] = origin + (Vector3)direction * 10f;
        lineRenderer.SetPositions(positions);

        if (Input.GetMouseButton(0) && shootTimer > playerShootRate)
        {
            shootTimer = 0;
            Shoot();
        }
    }

    private bool playerIsClose()
    {
        Vector3 playerPos = PlankManager.Instance.player.transform.position;
        if (Vector3.Distance(playerPos, transform.position) < 1.03)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
