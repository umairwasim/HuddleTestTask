using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingGrid : MonoBehaviour
{
    public int widthX, heightY, depthZ;
    public GameObject cubePrefab;
    public int maxCapacity = 0;

    private List<GameObject> nodes = new List<GameObject>();
    private ArrayList items = new ArrayList();
  
    void Start()
    {
        StartCoroutine(Generate3DGrid());
    }

    IEnumerator Generate3DGrid()
    {
        for (int w = 0; w < widthX; w++)
        {
            for (int h = 0; h < heightY; h++)
            {
                for (int d = 0; d < depthZ; d++)
                {
                    if (nodes.Count < maxCapacity)
                    {
                        GameObject go = Instantiate(cubePrefab, new Vector3(w, h, d), Quaternion.identity, transform);

                        yield return new WaitForSeconds(0.25f);
                        Debug.Log(go);
                        nodes.Add(go);
                    }
                    yield return null;
                }
            }
        }
    }
}
