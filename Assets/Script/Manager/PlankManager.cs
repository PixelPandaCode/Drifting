using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlankManager : MonoBehaviour
{
    private static PlankManager _instance;
    private float targetOrthographicSize = 1.0f;
    public float baseOrghographicSize = 2.0f;
    public float smoothSpeed = 2.0f; // Adjust this value to control the speed of the transition

    public static PlankManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlankManager();
            }
            return _instance;
        }
    }

    // generate from these lists, make sure their numbers match
    public List<GameObject> templates = new List<GameObject>();
    public List<float> templateFreqs = new List<float>();

    public List<GameObject> enemyTemplates = new List<GameObject>();
    public List<float> enemyTemplateFreqs = new List<float>();

    public float moveSpeed = .1f;
    public float minSpawnRate = 3;
    public float maxSpawnRate = 7;
    public float minEnemySpawnRate = 3;
    public float maxEnemySpawnRate = 7;

    private Vector3 targetPosition = new Vector3(0, 0, 0); //World Center
    private float spawnRate = 0;
    private float enemySpawnRate = 0;
    private float timer = 0;
    private float enemySpawnTimer = 0;

    public List<Plank> connectedPlanks = new List<Plank>();
    public List<Plank> spawnedPlanks = new List<Plank>();
    public List<Plank> spawnedEnemies = new List<Plank>();
    public int maxPlankNum = 1;

    public Plank playerLocatedPlank = null;
    public Player player = null;
    public float liveTime = 0;
    public int killedEnemy = 0;

    public EndMenu endMenu;
    public GameUI gameUI;

    public void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnRate = UnityEngine.Random.Range(minSpawnRate, maxSpawnRate);
        enemySpawnRate = UnityEngine.Random.Range(minEnemySpawnRate, maxEnemySpawnRate);
        // templates[0] is the initial one
        Plank initPlank = Instantiate(templates[0], new Vector3(0, 0, 0f), Quaternion.identity).GetComponent<Plank>();
        initPlank.isConnected = true;
        initPlank.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        connectedPlanks.Add(initPlank);
        playerLocatedPlank = initPlank;
        targetOrthographicSize = baseOrghographicSize;
        SpawnPlank();
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < spawnRate)
        {
            timer += Time.deltaTime;
        }
        else
        {
            SpawnPlank();
            timer = 0;
            spawnRate = Random.Range(minSpawnRate, maxSpawnRate);
        }

        if (enemySpawnTimer < enemySpawnRate)
        {
            enemySpawnTimer += Time.deltaTime;
        }
        else
        {
            SpawnEnemy();
            enemySpawnTimer = 0;
            enemySpawnRate = Random.Range(minEnemySpawnRate, maxEnemySpawnRate);
        }

        // Smoothly interpolate the camera's orthographic size towards the target size
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetOrthographicSize, Time.deltaTime * smoothSpeed);
        liveTime += Time.deltaTime;
    }

    public void GameOver()
    {
        int minutes = Mathf.FloorToInt(liveTime / 60);
        int seconds = Mathf.FloorToInt(liveTime % 60);
        endMenu.Setup((int)gameUI.Score, string.Format("{0:00}:{1:00}", minutes, seconds), maxPlankNum);
    }

    private void SpawnPlank()
    {
        int spawnEdge = 0;
        Vector3 spawnPosition = GetRandomPositionOnEdge(-1, out spawnEdge);
        GameObject template = null;
        float genFreq = Random.Range(0f, 1f);
        float accumulatedFreq = 0;
        for (int i = 0; i < templates.Count; i++)
        {
            if (genFreq < accumulatedFreq + templateFreqs[i])
            {
                template = templates[i];
                break;
            }
            accumulatedFreq += templateFreqs[i];
        }
        if (template == null)
        {
            Debug.Log("No Template to generate");
            return;
        }
        GameObject newPlank = Instantiate(template, spawnPosition, Quaternion.identity);

        // Move towards the other side of screen
        int newEdge = (spawnEdge + 2) % 4;
        targetPosition = GetRandomPositionOnEdge(newEdge, out spawnEdge);
        Vector2 direction = (targetPosition - newPlank.transform.position).normalized;
        newPlank.GetComponent<Rigidbody2D>().velocity = direction * moveSpeed;
        if (newPlank.GetComponent<Plank>())
        {
            spawnedPlanks.Add(newPlank.GetComponent<Plank>());
        }
    }

    private void SpawnEnemy()
    {
        int spawnEdge = 0;
        Vector3 spawnPosition = GetRandomPositionOnEdge(-1, out spawnEdge);
        GameObject template = null;
        float genFreq = Random.Range(0f, 1f);
        float accumulatedFreq = 0;
        for (int i = 0; i < enemyTemplates.Count; i++)
        {
            if (genFreq < accumulatedFreq + enemyTemplateFreqs[i])
            {
                template = enemyTemplates[i];
                break;
            }
            accumulatedFreq += enemyTemplateFreqs[i];
        }
        if (template == null)
        {
            Debug.LogError("No Template to generate");
            return;
        }
        GameObject newPlank = Instantiate(template, spawnPosition, Quaternion.identity);

        // Move towards the other side of screen
        int newEdge = (spawnEdge + 2) % 4;
        targetPosition = GetRandomPositionOnEdge(newEdge, out spawnEdge);
        Vector2 direction = (targetPosition - newPlank.transform.position).normalized;
        newPlank.GetComponent<Rigidbody2D>().velocity = direction * moveSpeed;
        if (newPlank.GetComponent<Plank>())
        {
            spawnedEnemies.Add(newPlank.GetComponent<Plank>());
        }
    }

    // when edge == -1 it will choose random edge to generate
    private Vector3 GetRandomPositionOnEdge(int edge, out int spawnEdge)
    {
        if (edge == -1)
        {
            edge = Random.Range(0, 4);
        }
        spawnEdge = edge;

        Vector2 randomScreenPosition = Vector2.zero;
        switch (edge)
        {
            case 0: // Top
                randomScreenPosition = new Vector2(Random.Range(0f, Screen.width), Screen.height);
                break;
            case 1: // Left
                randomScreenPosition = new Vector2(0, Random.Range(0f, Screen.height));
                break;
            case 2: // Bottom
                randomScreenPosition = new Vector2(Random.Range(0f, Screen.width), 0);
                break;
            case 3: // Right
                randomScreenPosition = new Vector2(Screen.width, Random.Range(0f, Screen.height));
                break;
        }

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(randomScreenPosition.x, randomScreenPosition.y, 0));
        worldPosition.z = 0f; // Or another appropriate value for your game's coordinate system

        return worldPosition;
    }

    public void ConnectPlank(Plank oldPlank, Plank newPlank)
    {
        if (newPlank == null || oldPlank == null)
        {
            return;
        }
        newPlank.isConnected = true;
        newPlank.isHooked = false;
        newPlank.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        connectedPlanks.Add(newPlank);
        maxPlankNum = Math.Max(maxPlankNum, connectedPlanks.Count);
        if (spawnedPlanks.Contains(newPlank))
        {
            spawnedPlanks.Remove(newPlank);
        }

        Vector2 direction = newPlank.transform.position - oldPlank.transform .position;
        int directionSwitch = DetermineDirection(direction);
        float width = newPlank.GetWidth();
        float height = newPlank.GetHeight();
        switch (directionSwitch)
        {
            // top
            case 0:
                newPlank.transform.position = new Vector3(oldPlank.transform.position.x, oldPlank.transform.position.y + height, 0);
                break;
            // left
            case 1:
                newPlank.transform.position = new Vector3(oldPlank.transform.position.x - width, oldPlank.transform.position.y, 0);
                break;
            // down
            case 2:
                newPlank.transform.position = new Vector3(oldPlank.transform.position.x, oldPlank.transform.position.y - height, 0);
                break;
            // right
            case 3:
                newPlank.transform.position = new Vector3(oldPlank.transform.position.x + width, oldPlank.transform.position.y, 0);
                break;
        }
        oldPlank.neighbors.Add(newPlank);
        newPlank.neighbors.Add(oldPlank);
        newPlank.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        AdjustCameraToFitPlanks();
    }

    public void AdjustCameraToFitPlanks()
    {
        if (connectedPlanks.Count == 0)
        {
            return;
        }
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        foreach (Plank plank in connectedPlanks)
        {
            Vector3 plankPosition = plank.transform.position;
            float plankWidth = plank.GetWidth();
            float plankHeight = plank.GetHeight();

            // Calculate boundaries considering the plank's size
            minX = Mathf.Min(minX, plankPosition.x - plankWidth / 2);
            maxX = Mathf.Max(maxX, plankPosition.x + plankWidth / 2);
            minY = Mathf.Min(minY, plankPosition.y - plankHeight / 2);
            maxY = Mathf.Max(maxY, plankPosition.y + plankHeight / 2);
        }

        float totalWidth = maxX - minX;
        float totalHeight = maxY - minY;

        // Adjust the Orthographic Size
        targetOrthographicSize = Mathf.Max(baseOrghographicSize, totalHeight, totalWidth / Camera.main.aspect);
    }

    public void DestroyPlank(Plank plank)
    {
        PlankManager.Instance.connectedPlanks.Remove(plank);

        // clear neighbors
        foreach (Plank neighbor in plank.neighbors)
        {
            neighbor.neighbors.Remove(plank);
        }
        plank.neighbors.Clear();

        // Perform BFS to find all planks that are still connected
        HashSet<Plank> visited = new HashSet<Plank>();
        Queue<Plank> queue = new Queue<Plank>();

        // Start BFS from any remaining connected plank (if there is one)
        if (connectedPlanks.Count > 0)
        {
            Plank startPlank = playerLocatedPlank;
            queue.Enqueue(startPlank);
            visited.Add(startPlank);
        }

        while (queue.Count > 0)
        {
            Plank current = queue.Dequeue();
            // Iterate through potential connections (you'll need to implement this logic)
            foreach (Plank neighbor in current.neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        // Identify unconnected planks (planks in connectedPlanks but not visited)
        List<Plank> unconnectedPlanks = new List<Plank>();
        foreach (Plank p in connectedPlanks)
        {
            if (!visited.Contains(p))
            {
                unconnectedPlanks.Add(p);
            }
        }

        // Remove the unconnected planks from the connected list and store them
        foreach (Plank unconnectedPlank in unconnectedPlanks)
        {
            connectedPlanks.Remove(unconnectedPlank);
            spawnedPlanks.Add(unconnectedPlank);
            unconnectedPlank.isConnected = false;
            unconnectedPlank.neighbors.Clear();
            unconnectedPlank.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            unconnectedPlank.GetComponent<Rigidbody2D>().velocity = (unconnectedPlank.transform.position - playerLocatedPlank.transform.position).normalized * moveSpeed;
        }
        Destroy(plank.gameObject);
        AdjustCameraToFitPlanks();
    }


    public int DetermineDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                return 3; // right
            }
            else
            {
                return 1; // left
            }
        }
        else
        {
            if (direction.y > 0)
            {
                return 0; // top
            }
            else
            {
                return 2; // above
            }
        }
    }

}
