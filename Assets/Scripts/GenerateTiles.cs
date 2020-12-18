using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GenerateTiles : MonoBehaviour
{
    public int num_rows = 1;
    public int total_length = 10;
    // Start is called before the first frame update
    List<GameObject> tiles = new List<GameObject>();
    void Start()
    {
        for (int row = 0; row < num_rows; row++)
        {
            for (int i = 0; i < total_length; i++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(row+0.5f, 0, i + 0.5f);
                float col = 1f - 0.6f * (i % 2);
                cube.GetComponent<Renderer>().material.color = new Vector4(col, col, col, 1);
                cube.name = "Tile Row" + row.ToString() + " Col" + i.ToString();
                cube.transform.localScale = new Vector3(1, 0.01f, 1);
                cube.transform.SetParent(transform);
                tiles.Add(cube);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
