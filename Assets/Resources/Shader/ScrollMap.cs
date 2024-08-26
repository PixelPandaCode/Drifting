using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollMap : MonoBehaviour
{
    public Material MapRenderer;
    public float ScrollSpeed = 1.0f;
    public GameObject Player;
    private float xTile = 0.0f;
    private float yTile = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = Player.GetComponent<Rigidbody2D>().position;
        xTile = playerPosition.x * ScrollSpeed % 1.0f;
        yTile = playerPosition.y * ScrollSpeed % 1.0f;
        if (xTile < 0.0f)
        {
            xTile += 1.0f;
        }
        if (yTile < 0.0f)
        {
            yTile += 1.0f;
        }
        MapRenderer.SetFloat("_Xtile", xTile);
        MapRenderer.SetFloat("_Ytile", yTile);
    }
}
