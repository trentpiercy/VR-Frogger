using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EnvironmentSetup : MonoBehaviour
{
    public int boardWidth;
    public int boardLength;

    public List<int> safeRows;

    public GameObject floorObject;
    public GameObject roadPrefab;

    public bool generate;

    void generateStreets()
    {
        GameObject floorsParent = GameObject.Find("Road Models");
        if (floorsParent != null)
        {
            DestroyImmediate(floorsParent);
        }

        floorsParent = new GameObject("Road Models");
        floorsParent.transform.parent = transform;

        for (int i = 2; i < boardLength; i+=2)
        {
            if (safeRows.Contains(i))
                continue;

            GameObject newRoad = Instantiate(roadPrefab, new Vector3(roadPrefab.transform.position.x, roadPrefab.transform.position.y, i), roadPrefab.transform.rotation);
            newRoad.transform.localScale = new Vector3(roadPrefab.transform.localScale.x * boardWidth, roadPrefab.transform.localScale.y, roadPrefab.transform.localScale.z);
            newRoad.name = "RoadClone-" + (i);
            newRoad.transform.parent = floorsParent.transform;

            //for (int j = 0; j < boardWidth / 2; j++)
            //{
            //    GameObject new_road = Instantiate(roadPrefab, new Vector3(boardWidth / 2 - 1 - 2 * j, 0.003820185f, 2 + 2 * i), roadPrefab.transform.rotation);
            //    new_road.name = "RoadClone-" + (i + 1) + "-" + (j + 1);
            //    new_road.transform.parent = floorsParent.transform;
            //}
        }
    }
    
    void Update()
    {
        #if UNITY_EDITOR
        if (generate){
            Transform floorTransform = floorObject.transform;
            //floorTransform.position = new Vector3(0, 0, boardLength / 2 - 1);
            floorTransform.position = new Vector3();
            //floorTransform.localScale = new Vector3(boardWidth, 0.01f, boardLength);
            floorTransform.localScale = new Vector3(1000, 0.01f, 1000);
            generateStreets();
            print("Regenerated environment");
            generate = false;
            }
        #endif
    }
}
