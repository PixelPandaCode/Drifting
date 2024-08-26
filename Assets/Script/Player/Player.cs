using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public float speed;
    public float baseSpeed;
    public float lineWidth = 0.01f;
    private Rigidbody2D rb2d;
    private Vector2 moveVelocity;
    private Vector2 mousePosition;
    private LineRenderer lineRenderer;
    private float baseLength = 0f;
    private float deltaLength = 0.05f;
    private float maxLength = 10.0f;
    private float curLength;
    private Plank hookedPlank = null;
    public GameObject hook;
    public GameObject hand;
    public bool canHook = true;
    public float health = 3.0f;
    public List<GameObject> Aims;
    public int aimIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        curLength = baseLength;
        baseSpeed = speed;
        PlankManager.Instance.player = this;
       
    }

    // Update is called once per frame
    void Update()
    {
        moveVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButton(1))
        {
            aimIndex = 2;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            aimIndex = 1;
        }
        else
        {
            aimIndex = 0;
        }
        for (int i = 0; i < Aims.Count; ++i)
        {
            Aims[i].SetActive(i == aimIndex);
            if (i == aimIndex)
            {
                Aims[i].transform.position = mousePosition;
            }
        }
    }

    private void FixedUpdate()
    {
        if (health <= 0)
        {
            return;
        }
        Movement();
        UpdateLine();
        DestroyPlank();

        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z), 
            PlankManager.Instance.smoothSpeed);
    }

    private void Movement()
    {
        Vector2 newPosition = (Vector2)transform.position + moveVelocity * Time.deltaTime;
        bool isOnPlank = false;
        foreach (Plank plank in PlankManager.Instance.connectedPlanks)
        {
            if (plank.isOnPlank(newPosition))
            {
                isOnPlank = true;
                PlankManager.Instance.playerLocatedPlank = plank;
                break;
            }
        }
        rb2d.velocity = isOnPlank ? moveVelocity : Vector2.zero;
        transform.localScale = new Vector3(mousePosition.x < transform.position.x ? -1 : 1, 1, 1);
    }

    private void DestroyPlank()
    {
        if (Input.GetMouseButton(1))
        {
            Plank DestroyPlank = null;
            foreach (Plank plank in PlankManager.Instance.connectedPlanks)
            {
                if (plank.isOnPlank(mousePosition) && plank != PlankManager.Instance.playerLocatedPlank)
                {
                    DestroyPlank = plank;
                    break;
                }
            }
            if (DestroyPlank != null)
            {
                PlankManager.Instance.DestroyPlank(DestroyPlank);
                DestroyPlank = null;
            }
        }
    }

    private void UpdateLine()
    {
        if (!canHook)
        {
            lineRenderer.enabled = false;
            hook.SetActive(false);
            return;
        }
        lineRenderer.enabled = true;
        Vector2 originalPosition = hand.transform.position;
        Vector2 direction = (mousePosition - originalPosition).normalized;
        Vector3[] positions = new Vector3[2];
        positions[0] = originalPosition;
        if (hookedPlank != null && hookedPlank.isConnected)
        {
            hookedPlank = null;
            curLength = baseLength;
        }
        if (Input.GetMouseButton(0))
        {
            if (curLength < maxLength)
            {
                curLength += deltaLength;
            }
            else
            {
                curLength = maxLength;
            }
            if (hookedPlank == null)
            {
                // detect if line hits unhooked plank
                Vector2 dest = originalPosition + direction * curLength;
                foreach (Plank plank in PlankManager.Instance.spawnedPlanks)
                {
                    if (!plank.isHooked && !plank.isConnected && plank.isOnPlank(dest))
                    {
                        plank.isHooked = true;
                        plank.transform.position = mousePosition;
                        direction = (mousePosition - originalPosition).normalized;
                        curLength = (mousePosition - originalPosition).magnitude;
                        plank.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                        hookedPlank = plank;
                        break;
                    }
                }
            }
            else
            {
                hookedPlank.transform.position = mousePosition;
                curLength = (mousePosition - originalPosition).magnitude;
            }
        }
        else
        {
            if (curLength > baseLength)
            {
                curLength -= deltaLength;
            }
            else
            {
                curLength = baseLength;
            }
            if (hookedPlank != null)
            {
                hookedPlank.transform.position = originalPosition + direction * curLength;
            }
        }
        positions[1] = originalPosition + direction * curLength;
        positions[1] = new Vector3(positions[1].x, positions[1].y, -0.5f);
        lineRenderer.SetPositions(positions);
        hook.SetActive(curLength != baseLength);
        if (hook != null)
        {
            hook.transform.position = positions[1];
            // Calculate the angle in radians
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Apply the rotation to the transform
            hook.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            hook.transform.localScale = new Vector3(mousePosition.x < transform.position.x ? -0.1f : 0.1f, 0.1f, 0.1f);
        }
    }

    public void DamagePlayer(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            PlankManager.Instance.GameOver();
            Time.timeScale = 0.0f;
        }
    }
}
