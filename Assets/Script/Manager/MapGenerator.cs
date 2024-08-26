using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject seaBlock;
    public int rows = 10;
    public int cols = 10;

    // generate from these lists, make sure their numbers match
    public List<GameObject> templates = new List<GameObject>();
    public List<float> templateFreqs = new List<float>();
    public int decorateNum = 1000;
    public float maxLength;
    public Vector3 lastGeneratePosition = Vector3.zero;

    public List<GameObject> generatedSeaBlocks = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        rows = 0; cols = 0;
        GenerateSea();
        GenerateDecorate();
        lastGeneratePosition = transform.position;
    }

    public void GenerateSea()
    {
        Vector3 leftDownPosition = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 rightUpPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        // Adjust rows and cols calculation to use 0, 0 as origin
        int newRows = Mathf.CeilToInt(rightUpPosition.x - leftDownPosition.x) + 1;
        int newCols = Mathf.CeilToInt(rightUpPosition.y - leftDownPosition.y) + 1;

        if (newRows <= rows && newCols <= cols)
        {
            return;
        }
        Vector2Int basePosition = new Vector2Int(Mathf.FloorToInt(leftDownPosition.x), Mathf.FloorToInt(leftDownPosition.y));
        int generatedNum = 0;
        for (int i = 0; i < newRows; i++)
        {
            for (int j = 0; j < newCols; j++)
            {
                if (generatedNum == generatedSeaBlocks.Count)
                {
                    // Generate the block at the appropriate position
                    GameObject newBlock = Instantiate(seaBlock, Vector3.zero, Quaternion.identity);
                    newBlock.transform.parent = transform;

                    // 1.01 to fill the gap
                    newBlock.transform.localScale = Vector3.one * 1.01f;
                    newBlock.transform.position = new Vector3(basePosition.x + i, basePosition.y + j, transform.position.z);
                    // make sure the animation is in the same position
                    if (generatedNum != 0)
                    {
                        // Synchronize the animator state and time
                        Animator newAnimator = newBlock.GetComponent<Animator>();
                        Animator referenceAnimator = generatedSeaBlocks[0].GetComponent<Animator>();

                        if (newAnimator != null && referenceAnimator != null)
                        {
                            AnimatorStateInfo currentState = referenceAnimator.GetCurrentAnimatorStateInfo(0);
                            newAnimator.Play(currentState.fullPathHash, -1, currentState.normalizedTime);
                        }
                    }
                    generatedSeaBlocks.Add(newBlock);
                }
                else
                {
                    generatedSeaBlocks[generatedNum].transform.position = new Vector3(basePosition.x + i, basePosition.y + j, transform.position.z);
                }
                generatedNum++;
            }
        }

        rows = newRows;
        cols = newCols;
    }
    private void FixedUpdate()
    {
        GenerateSea();
        if ((transform.position - lastGeneratePosition).magnitude > 0.8f * maxLength)
        {
            GenerateDecorate();
            lastGeneratePosition = transform.position;
        }
    }

    public Vector2 GenerateRandomVector()
    {
        // Generate a random angle in degrees
        float randomAngle = Random.Range(0f, 360f);

        // Convert the angle to radians
        float radians = randomAngle * Mathf.Deg2Rad;

        // Create a direction vector using the angle
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

        // Generate a random length between minLength and maxLength
        float randomLength = Mathf.Sqrt(Random.Range(0f, 1f)) * maxLength;

        // Scale the direction vector by the random length
        Vector2 randomVector = direction * randomLength;

        return randomVector;
    }

    public void GenerateDecorate()
    {
        for (int i = 0; i < decorateNum; ++i)
        {
            GameObject template = null;
            float genFreq = Random.Range(0f, 1f);
            float accumulatedFreq = 0;
            Vector2 spawnPosition = GenerateRandomVector();
            for (int j = 0; j < templates.Count; j++)
            {
                if (genFreq < accumulatedFreq + templateFreqs[j])
                {
                    template = templates[j];
                    break;
                }
                accumulatedFreq += templateFreqs[j];
            }
            if (template == null)
            {
                Debug.LogError("No Template to generate");
                return;
            }
            GameObject newPlank = Instantiate(template, spawnPosition, Quaternion.identity);
        }
    }
}
