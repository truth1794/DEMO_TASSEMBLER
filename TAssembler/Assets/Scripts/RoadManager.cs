using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TagEngine;
using Object = UnityEngine.Object;

public class RoadManager : MonoBehaviour {

    public Object[] roadObjects;
    public GameObject[] basicRoads;
    public Vector2 targetPosition;
    public string roadTag = "road";
    public string roadObjPath = "road";
    public List<RaycastHit[]> neighbourData;
    public bool onTargetClick, onRoadClick;
    public GameObject genObj, genObj2;
    public int[] directAdjacentTiles = new int[4] { 1, 7, 5, 3 };
    public int[,] directAdjacentTilesCombo = new int[6, 2] { { 1, 7 }, { 5, 3 }, { 1, 5 }, { 1, 3 }, { 7, 5 }, { 7, 3 } };
    public int[,] adjacentTilesCombo = new int[8, 2] { { 1, 2 }, { 1, 0 }, { 7, 8 }, { 7, 6 }, { 5, 2 }, { 5, 8 }, { 3, 0 }, { 3, 6 } };
    public List<Object> spawnedObjs;
    public GameObject[] spOb;
    public List<Vector3> occupiedTilesPosition = new List<Vector3>();
    public List<string> occupiedTilesType = new List<string>();
    public List<string> roadTypes;
    public GameObject[] spObjs;
    public GameObject spawnedObjsParent;
    public GameObject buffer;
    public GameObject RoadPrefabs;
    List<string> extendedNeighbours = new List<string>();
    public string eNeigh;
    public int neightbourRotation;


    PositionSensor posDataScript;
    public GameObject posData;

    private void Awake() {
        roadObjects = Road_ObjLoader(roadObjPath);
        basicRoads = new GameObject[5];
        int iter0 = 0;
        foreach (Transform child in RoadPrefabs.transform) {
            foreach (Transform subChild in child.transform) {
                subChild.gameObject.AddComponent<TagSystem>();
            }
            basicRoads[iter0] = child.gameObject;
            iter0 += 1;
        }
        /*for(int i = 0; i < roadObjects.Length; i++) {
            GameObject tempObj = (GameObject)roadObjects[i];
            roadTypes.Add(tempObj.GetComponent<TagSystem>().basicType);
        }*/
    }

    private void Start() {
        posDataScript = posData.GetComponent<PositionSensor>();
        onTargetClick = false;
        onRoadClick = false;
    }

    private void Update() {
        onTargetClick = posDataScript.roadTouch;
        onRoadClick = posDataScript.roadTouch2;

        if (onTargetClick) {
            //Debug.Log(onTargetClick);
            targetPosition = new Vector2(posDataScript.adjustedMousX1, posDataScript.adjustedMousZ1);
            //Debug.Log(targetPosition);
            //neighbourData = Target_neighbours(targetPosition);
            neighbourData = Target_neighboursB(targetPosition);
            //Target_neighboursB(targetPosition);
            //Road_DeadEnd(neighbourData, road_end, road_staight);
            Road_Instantiator(neighbourData, targetPosition);
        }
    }

    public List<RaycastHit[]> Target_neighbours(Vector2 position) {
        RaycastHit[] hitArray;
        List<RaycastHit[]> hitList = new List<RaycastHit[]>();
        Vector3[] rayOrigins = new Vector3[9];
        Vector2[,] neighbourPos = new Vector2[3, 3];
        foreach (Transform child in spawnedObjsParent.transform) {
            foreach (Transform subchild in child.transform) {
                subchild.gameObject.GetComponent<TagSystem>().gridId = -1;
            }
        }
        float xCursor = 0, zCursor = 0;
        int it = 0;
        for (int x = 0; x < 3; x++) {
            for (int z = 0; z < 3; z++) {
                if (x == 0) {
                    xCursor = -8;
                }
                else if (x == 1) {
                    xCursor = 0;
                }
                else if (x == 2) {
                    xCursor = 8;
                }
                if (z == 0) {
                    zCursor = -8;
                }
                else if (z == 1) {
                    zCursor = 0;
                }
                else if (z == 2) {
                    zCursor = 8;
                }
                neighbourPos[x, z] =
                    new Vector2(
                        position.x + (xCursor),
                        position.y + (zCursor)); // y = z
                rayOrigins[it] = new Vector3(neighbourPos[x, z].x, -1f, neighbourPos[x, z].y);
                hitArray = Physics.RaycastAll(rayOrigins[it], Vector3.up, 3.0f);
                hitList.Add(hitArray);
                foreach (RaycastHit hits in hitList[it]) {
                    if (hits.collider.gameObject.tag == roadTag) {
                        //Debug.Log(it);
                        hits.collider.gameObject.GetComponent<TagSystem>().gridId = it;
                        //Debug.Log(hits.collider.gameObject.GetComponent<TagSystem>().gridId);
                        //Debug.Log(hits.collider.gameObject.name + " " + hits.collider.gameObject.transform.position);
                    }
                }
                it += 1;
                //Debug.Log(obj.transform.position.x + " " + obj.transform.position.z);
                //Debug.Log("[" + x + "," + z + "]" + neighbourPos[x, z]);
            }
        }
        return hitList;
    }


    public List<RaycastHit[]> Target_neighboursB(Vector2 position) {
        RaycastHit[] hitArray;
        List<RaycastHit[]> hitList = new List<RaycastHit[]>();
        Vector3[] rayOrigins = new Vector3[13];
        Vector2[,] neighbourPos = new Vector2[4, 4];
        foreach (Transform child in spawnedObjsParent.transform) {
            foreach (Transform subchild in child.transform) {
                subchild.gameObject.GetComponent<TagSystem>().gridId = -1;
            }
        }
        float xCursor = 0, zCursor = 0;
        int iteratorA = 0, iteratorB = 9;
        for (int x = 0; x < 3; x++) {
            for (int z = 0; z < 3; z++) {
                //Debug.Log(x + " " + z);
                if (x == 0) {
                    xCursor = -8f;
                }
                else if (x == 1) {
                    xCursor = 0f;
                }
                else if (x == 2) {
                    xCursor = 8f;
                }
                if (z == 0) {
                    zCursor = -8f;
                }
                else if (z == 1) {
                    zCursor = 0f;
                }
                else if (z == 2) {
                    zCursor = 8f;
                }
                if (x < 3 && z < 3) {
                    neighbourPos[x, z] =
                    new Vector2(
                        position.x + (xCursor),
                        position.y + (zCursor)); // y = z
                    rayOrigins[iteratorA] = new Vector3(neighbourPos[x, z].x, -1f, neighbourPos[x, z].y);
                }
                iteratorA += 1;
            }
        }
        for (int i = 0; i < 4; i++) {
            if (i == 0) {
                xCursor = -16f;
                zCursor = 0f;
            }
            else if (i == 1) {
                xCursor = 16;
                zCursor = 0f;
            }
            else if (i == 2) {
                xCursor = 0f;
                zCursor = 16f; ;
            }
            else if (i == 3) {
                xCursor = 0f;
                zCursor = -16f;
            }
            neighbourPos[i, 0] =
                    new Vector2(
                        position.x + (xCursor),
                        position.y + (zCursor)); // y = z
                                                 //Debug.Log(iteratorB + " " + neighbourPos[x, z].x + " " + neighbourPos[x, z].y);
            rayOrigins[iteratorB] = new Vector3(neighbourPos[i, 0].x, -1f, neighbourPos[i, 0].y);
            iteratorB += 1;
        }
        for (int i = 0; i < 13; i++) {
            hitArray = Physics.RaycastAll(rayOrigins[i], Vector3.up, 3.0f);
            hitList.Add(hitArray);
            foreach (RaycastHit hits in hitList[i]) {
                if (hits.collider.gameObject.tag == roadTag) {
                    //Debug.Log(iteratorA);
                    hits.collider.gameObject.GetComponent<TagSystem>().gridId = i;
                    //Debug.Log(hits.collider.gameObject.GetComponent<TagSystem>().gridId);
                    //Debug.Log(hits.collider.gameObject.name + " " + hits.collider.gameObject.transform.position);
                }
            }
        }
        return hitList;
    }

    public void Road_Instantiator(List<RaycastHit[]> neighbours, Vector3 targetPosition) {
        int[] takenPos = new int[9];
        spOb = new GameObject[9];

        int iter = 0;
        List<int> occupiedTiles = new List<int>();
        for (int i = 0; i < 13; i++) {
            foreach (RaycastHit hits in neighbours[i]) {
                if (hits.collider.gameObject.tag == roadTag) {
                    //Debug.Log(i);
                    occupiedTiles.Add(hits.collider.gameObject.GetComponent<TagSystem>().gridId);
                    occupiedTilesPosition.Add(hits.collider.gameObject.transform.position);
                    //occupiedTilesType.Add(hits.collider.gameObject.GetComponent<TagSystem>().basicType);
                    //spawnedObjs.Add(hits.collider.gameObject);
                    //spOb[iter] = hits.collider.gameObject;
                    //spOb[iter].GetComponent<TagSystem>().gridId = i;
                    //iter += 1;
                    //Debug.Log (spawnedObjs[iter].name);
                    //iter += 1;
                    //occupiedTilesName.Add(hits.collider.gameObject.get
                }
            }
        }
        for (int i = 0; i < occupiedTiles.Count; i++) {
            //Debug.Log(occupiedTiles[i]);
        }
        /*for(int i = 0; i< spawnedObjs.Count; i++) {
            Debug.Log("before " + spawnedObjs[i].name);
            spObjs = new GameObject[spawnedObjs.Count];
            spObjs[i] = (GameObject)spawnedObjs[i];
            spObjs[i].GetComponent<TagSystem>().gridId = occupiedTiles[i];
        }
        for(int i = 0; i < occupiedTiles.Count; i++) {
            //Debug.Log("arrPos= " + occupiedTiles[i] + " roadPos= " + occupiedTilesPosition[i] + " type= " + occupiedTilesType[i]);
        }*/
        Neighbour_DataProcessorC(NeighboursCases(occupiedTiles), basicRoads, targetPosition);
        /*for(int i = 0; i < spawnedObjs.Count; i++) {
            Debug.Log("after " + spawnedObjs[i].name);
        }*/
    }


    public void Road_InstantiatorB(List<RaycastHit[]> neighbours, Vector3 targetPosition) {
        int[] takenPos = new int[9];
        spOb = new GameObject[9];

        int iter = 0;
        List<int> occupiedTiles = new List<int>();
        for (int i = 0; i < 13; i++) {
            foreach (RaycastHit hits in neighbours[i]) {
                if (hits.collider.gameObject.tag == roadTag) {
                    occupiedTiles.Add(hits.collider.gameObject.GetComponent<TagSystem>().gridId);
                    occupiedTilesPosition.Add(hits.collider.gameObject.transform.position);
                }
            }
        }
        for (int i = 0; i < occupiedTiles.Count; i++) {
            Debug.Log(occupiedTiles[i]);
        }
        Neighbour_DataProcessorC(NeighboursCasesB(occupiedTiles), basicRoads, targetPosition);
    }


    private bool[] NeighbourBoolArray(List<int> data, int posA, int posB, int posC, int posD) {
        bool[] cases = new bool[5];
        int nbC0 = 0;
        if (data.Count > 0) {
            for (int i = 0; i < data.Count; i++) {
                for (int j = 0; j < data.Count; j++) {
                    if (data[i] == posA) {
                        if (nbC0 > 0) {
                            cases[0] = false;
                            //nbC0 = 1;
                        }
                        else {
                            cases[0] = true;
                        }
                        if (data[j] == posD) {
                            cases[4] = true;
                        }
                        if (data[j] == posB) {
                            cases[1] = true;
                            nbC0 = 1;
                            cases[0] = false;
                        }
                        if (data[j] == posC) {
                            cases[2] = true;
                            nbC0 = 1;
                            cases[0] = false;
                        }
                        if (cases[1] && cases[2]) {
                            cases[3] = true;
                        }
                    }
                }
            }
        }
        return cases;
    }

    private bool[] NeighbourBoolArrayB(List<int> data, int posA, int posB, int posC, int posD, int posE) {
        bool[] cases = new bool[6];
        int nbC0 = 0;
        if (data.Count > 0) {
            for (int i = 0; i < data.Count; i++) {
                for (int j = 0; j < data.Count; j++) {
                    if (data[i] == posA) {
                        if (nbC0 > 0) {
                            cases[0] = false;
                            //nbC0 = 1;
                        }
                        else {
                            cases[0] = true;
                        }
                        if (data[j] == posE) {
                            cases[5] = true;
                        }
                        if (data[j] == posD) {
                            cases[4] = true;
                        }
                        if (data[j] == posB) {
                            cases[1] = true;
                            nbC0 = 1;
                            cases[0] = false;
                        }
                        if (data[j] == posC) {
                            cases[2] = true;
                            nbC0 = 1;
                            cases[0] = false;
                        }
                        if (cases[1] && cases[2]) {
                            cases[3] = true;
                        }
                    }
                }
            }
        }
        return cases;
    }

    public bool[,] NeighboursCases(List<int> neighboursData) {
        bool caseData = false;
        int[] cases = new int[9] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        int[,] adresses = new int[4, 4]
        {
                {1,0,2,7},
                {7,6,8,1},
                {5,2,8,3},
                {3,0,6,5}

        };
        bool[,] globalCases = new bool[4, 5]; //exemple : [0,0] = case0 , [0,1] = case1, [0,2] = case2, [0,3] = case3, [0,4] = case4

        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 5; j++) {
                //NeighbourBoolArrayB(neighboursData, adresses[i, 0], adresses[i, 1], adresses[i, 2], adresses[i, 3]).CopyTo(globalCases, i);
                globalCases[i, j] = (bool)NeighbourBoolArray(neighboursData, adresses[i, 0], adresses[i, 1], adresses[i, 2], adresses[i, 3]).GetValue(j);
            }
        }
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 5; j++) {
                if (globalCases[i, j]) {
                    //Debug.Log("i= " + i + " j= " + j + globalCases[i, j]);
                }
            }
        }
        return globalCases;
    }

    public bool[,] NeighboursCasesB(List<int> neighboursData) {
        int[] cases = new int[9] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        int[,] adresses = new int[4, 5]
        {
                {1,0,2,7,9},
                {7,6,8,1,10},
                {5,2,8,3,11},
                {3,0,6,5,12}

        };
        bool[,] globalCases = new bool[4, 6]; //exemple : [0,0] = case0 , [0,1] = case1, [0,2] = case2, [0,3] = case3, [0,4] = case4

        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 6; j++) {
                //NeighbourBoolArrayB(neighboursData, adresses[i, 0], adresses[i, 1], adresses[i, 2], adresses[i, 3]).CopyTo(globalCases, i);
                globalCases[i, j] = (bool)NeighbourBoolArrayB(neighboursData, adresses[i, 0], adresses[i, 1], adresses[i, 2], adresses[i, 3], adresses[i, 4]).GetValue(j);
            }
        }
        return globalCases;
    }

    public void Road_Fixer(List<RaycastHit[]> neighbours, GameObject[] roadPrefabArray, Vector3 targetPosition, int arrayPosition) {
        foreach (RaycastHit hits in neighbours[arrayPosition]) {
            if ((arrayPosition == 3 || arrayPosition == 5) && hits.collider.gameObject.tag == roadTag) {
                if (arrayPosition == 3) {

                }
            }
        }
    }

    public void Road_DeadEnd(List<RaycastHit[]> neighbours, GameObject road_end, GameObject road_st) {
        int nbNeighbours = neighbours.Count;
        float xRot = road_end.transform.localEulerAngles.x;
        float yRot = road_end.transform.localEulerAngles.y;
        float zRot = road_end.transform.localEulerAngles.z;
        int cnt = 0;
        //zRot = road_end.transform.localEulerAngles.y
        //Debug.Log(nbNeighbours);
        if (nbNeighbours > 0) {
            for (int i = 0; i < nbNeighbours; i++) {

                foreach (RaycastHit hits in neighbours[i]) {
                    if ((i == 3 || i == 5) && (hits.collider.gameObject.tag == roadTag)) {
                        cnt += 1;
                        //Debug.Log(i);
                        if (i == 5) {
                            //Debug.Log(i);
                            //Debug.Log("tile detected");
                            Vector3 neighbourRotation = hits.collider.gameObject.transform.localEulerAngles;
                            Vector3 neighbourPosition = hits.collider.gameObject.transform.position;
                            var deadEndPrefab = (GameObject)Instantiate(road_end, new Vector3(targetPosition.x, 1f, targetPosition.y), Quaternion.Euler(xRot, yRot, zRot)) as GameObject;
                            genObj = deadEndPrefab;
                            foreach (Transform child in genObj.transform) {
                                child.gameObject.AddComponent<BoxCollider>();
                                child.gameObject.transform.tag = roadTag;
                                //child.gameObject.AddComponent<TagSystem>();
                            }
                            Destroy(hits.collider.gameObject);
                            var roadStraightPrefab = (GameObject)Instantiate(road_st, neighbourPosition, Quaternion.Euler(neighbourRotation)) as GameObject;
                            genObj2 = roadStraightPrefab;
                            foreach (Transform child in genObj2.transform) {
                                child.gameObject.AddComponent<BoxCollider>();
                                child.gameObject.transform.tag = roadTag;
                                //child.gameObject.AddComponent<TagSystem>();
                            }
                            //hits.collider.gameObject.transform.localEulerAngles = new Vector3(neighbourXrot, neighbourYrot + 180f, neighbourZrot);
                        }
                    }
                }
            }
        }
        if (cnt == 0) {
            //Debug.Log("we are here");
            var deadEndPrefab = (GameObject)Instantiate(road_end, new Vector3(targetPosition.x, 1f, targetPosition.y), Quaternion.Euler(xRot, yRot, zRot)) as GameObject;
            genObj = deadEndPrefab;
            foreach (Transform child in genObj.transform) {
                child.gameObject.AddComponent<BoxCollider>();
                child.gameObject.transform.tag = roadTag;
                //child.gameObject.AddComponent<TagSystem>();
            }
        }
    }


    public int Id_Converter(int rawId, bool[,] neighbourData) {
        int convertedId = 0;
        if (rawId == 0) {
            if (neighbourData[rawId, 1]) {
                convertedId = 1;
            }
            else if (neighbourData[rawId, 2]) {
                convertedId = 1;
            }
            else {
                convertedId = 1;
            }
        }
        if (rawId == 1) {
            if (neighbourData[rawId, 1]) {
                convertedId = 7;
            }
            else if (neighbourData[rawId, 2]) {
                convertedId = 7;
            }
            else {
                convertedId = 7;
            }
        }
        if (rawId == 2) {
            if (neighbourData[rawId, 1]) {
                convertedId = 5;
            }
            else if (neighbourData[rawId, 2]) {
                convertedId = 5;
            }
            else {
                convertedId = 5;
            }
        }
        if (rawId == 3) {
            if (neighbourData[rawId, 1]) {
                convertedId = 3;
            }
            else if (neighbourData[rawId, 2]) {
                convertedId = 3;
            }
            else {
                convertedId = 3;
            }
        }
        return convertedId;
    }

    public int Id_NeighbourConverter(int id) {
        int convertedId = 0;
        if (id == 1) {
            convertedId = 9;

        }
        if (id == 7) {
            convertedId = 10;

        }
        if (id == 5) {
            convertedId = 11;

        }
        if (id == 3) {
            convertedId = 12;

        }
        return convertedId;
    }



    public List<string> Road_ExtendedNeighbours(GameObject targetObj) {
        //Debug.Log("extendedEXEC");
        List<string> neighboursResult = new List<string>();
        RaycastHit[] hitArray;
        List<RaycastHit[]> hitList = new List<RaycastHit[]>();
        Vector3 targetPos = targetObj.transform.position;
        List<Vector3> rayOrigins = new List<Vector3>();
        int id = targetObj.GetComponent<TagSystem>().gridId;
        //GameObject tranF = new GameObject();
        //Debug.Log("tgetID= " + id);
        //Debug.Log("targetPos= " + targetPos);
        switch (id) {
            case 0:
                rayOrigins.Add(new Vector3(targetPos.x - 8, -1.0f, targetPos.z));
                rayOrigins.Add(new Vector3(targetPos.x, -1.0f, targetPos.z - 8));
                break;
            case 1:
                //Debug.Log("case1EXEC");
                rayOrigins.Add(new Vector3(targetPos.x - 8, -1.0f, targetPos.z));

                //var obSt = (GameObject)Instantiate(tranF, new Vector3(targetPos.x - 8, -1.0f, targetPos.z), Quaternion.identity);
                break;
            case 2:
                rayOrigins.Add(new Vector3(targetPos.x + 8, -1.0f, targetPos.z));
                rayOrigins.Add(new Vector3(targetPos.x, -1.0f, targetPos.z + 8));
                break;
            case 3:
                rayOrigins.Add(new Vector3(targetPos.x, -1.0f, targetPos.z - 8));
                break;
            case 5:
                rayOrigins.Add(new Vector3(targetPos.x, -1.0f, targetPos.z + 8));
                break;
            case 6:
                rayOrigins.Add(new Vector3(targetPos.x + 8, -1.0f, targetPos.z));
                rayOrigins.Add(new Vector3(targetPos.x, -1.0f, targetPos.z - 8));
                break;
            case 7:
                rayOrigins.Add(new Vector3(targetPos.x + 8, -1.0f, targetPos.z));
                break;
            case 8:
                rayOrigins.Add(new Vector3(targetPos.x + 8, -1.0f, targetPos.z));
                rayOrigins.Add(new Vector3(targetPos.x, -1.0f, targetPos.z + 8));
                break;
            default:
                break;
        }
        for (int i = 0; i < rayOrigins.Count; i++) {
            //Debug.Log("nPos= " + rayOrigins[i]);
            hitArray = Physics.RaycastAll(rayOrigins[i], Vector3.up, 3.0f);
            hitList.Add(hitArray);
        }
        for (int i = 0; i < hitList.Count; i++) {
            foreach (RaycastHit hits in hitList[i]) {
                if (hits.collider.gameObject.tag == roadTag) {
                    neighboursResult.Add(hits.collider.gameObject.GetComponent<TagSystem>().basicType);
                }
            }
        }
        foreach (Transform child in spawnedObjsParent.transform) {
            foreach (Transform subChild in child.transform) {
                if (subChild.gameObject.GetComponent<TagSystem>().gridId > -1) {
                    //Debug.Log("id= " + subChild.gameObject.GetComponent<TagSystem>().gridId + " obj= " + subChild.gameObject.GetComponent<TagSystem>().basicType);
                }
            }
        }
        return neighboursResult;
    }


    public string Road_ExtendedNeighboursB(GameObject targetObj) {
        string neighboursResult = null;
        RaycastHit[] hitArray;
        RaycastHit[] hitList;
        Vector3 targetPos = targetObj.transform.position;
        Vector3 rayOrigins = new Vector3();
        int id = targetObj.GetComponent<TagSystem>().gridId;
        switch (id) {
            case 1:
                rayOrigins = new Vector3(targetPos.x - 8, -1.0f, targetPos.z);
                break;
            case 3:
                rayOrigins = new Vector3(targetPos.x, -1.0f, targetPos.z - 8);
                break;
            case 5:
                rayOrigins = new Vector3(targetPos.x, -1.0f, targetPos.z + 8);
                break;
            case 7:
                rayOrigins = new Vector3(targetPos.x + 8, -1.0f, targetPos.z);
                break;
            default:
                break;
        }
        hitArray = Physics.RaycastAll(rayOrigins, Vector3.up, 3.0f);

        for (int i = 0; i < hitArray.Length; i++) {
            foreach (RaycastHit hits in hitArray) {
                if (hits.collider.gameObject.tag == roadTag) {
                    //Debug.Log("detected");
                    neighboursResult = hits.collider.gameObject.GetComponent<TagSystem>().basicType;
                    Debug.Log(neighboursResult);
                }
            }
        }
        return neighboursResult;
    }


    public void Neighbour_DataProcessorC(bool[,] neighbours, Object[] roadArray, Vector3 targetPostion) {
        int notAr = 0;
        int idConverter;
        int neighbourScen = -1;
        int side = -1;
        string neighbourName;
        for (int i = 0; i < 4; i++) {

            if (neighbours[i, 0] && !neighbours[i, 4] && !neighbours[i, 1] && !neighbours[i, 2]) {
                //Debug.Log("only " + i);
                //idConverter = Id_Converter(i, neighbours);
                //Debug.Log(idConverter);
                //if (neighbours[i, 5]) {
                // neighbourName =
                // }
                Road_Spawner((GameObject)roadArray[0], targetPostion, i);
                Road_Replacer((GameObject)roadArray[1], i, Id_Converter(i, neighbours));


            }
            else if (!neighbours[i, 4] && neighbours[i, 1] && !neighbours[i, 2]) {
                //Debug.Log(i + " and 1");
                side = 1;
                neighbourScen = 0;
                idConverter = Id_Converter(i, neighbours);
                //Debug.Log(idConverter);

                Road_Spawner((GameObject)roadArray[0], targetPostion, i);
                //Road_Replacer((GameObject)roadArray[4], i, Id_Converter(i, neighbours));
                Road_ReplacerB(i, idConverter, neighbourScen, side);

            }
            else if (!neighbours[i, 4] && !neighbours[i, 1] && neighbours[i, 2]) {
                Debug.Log(i + " and 2");
                neighbourScen = 1;
            }
            else if (!neighbours[i, 4] && neighbours[i, 3]) {
                Debug.Log(i + " and 1+2");
                neighbourScen = 2;
            }
            else if (neighbours[i, 0] && neighbours[i, 4] && !neighbours[i, 1] && !neighbours[i, 2]) {
                Debug.Log(i + " and opposite");
            }
            else if (neighbours[i, 4] && neighbours[i, 1] && !neighbours[i, 2]) {
                Debug.Log(i + " and 1 and opposite");
            }
            else if (neighbours[i, 4] && !neighbours[i, 1] && neighbours[i, 2]) {
                Debug.Log(i + " and 2 and opposite");
            }
            else if (neighbours[i, 4] && neighbours[i, 3]) {
                Debug.Log(i + " and 1+2 and opposite");
            }
            else if (!neighbours[i, 0] && !neighbours[i, 4] && !neighbours[i, 1] && !neighbours[i, 2]) {
                notAr += 1;
                if (notAr == 4) {
                    Road_Spawner((GameObject)roadArray[0], targetPostion, i);
                }
            }
        }
    }

    public GameObject Road_TypeProcessor(List<string> extendedN, GameObject obj, int neighbourPos, int scen, GameObject[] rArray) {
        GameObject objResult = null;
        int nbExtendedNs = extendedN.Count;
        int id = obj.GetComponent<TagSystem>().gridId;
        string nBasicType = obj.GetComponent<TagSystem>().basicType;
        string extN = extendedN[0];
        //Debug.Log("HERE " + neighbourPos + " " + scen + " " + extendedN[0] + " " + nBasicType);
        if ((neighbourPos == 0 && scen == 0) || (neighbourPos == 0 && scen == 1) ||
                (neighbourPos == 1 && scen == 0) || (neighbourPos == 1 && scen == 1) ||
                (neighbourPos == 2 && scen == 0) || (neighbourPos == 2 && scen == 1) ||
                (neighbourPos == 3 && scen == 0) || (neighbourPos == 3 && scen == 1)) {
            //Debug.Log("sc0");
            //Debug.Log(extendedN[0] + " " + nBasicType);
            /*if ((extendedN[0] == "straight" || extendedN[0] == "corner" || extendedN[0] == "crossing" || extendedN[0] == "end") && (nBasicType == " straight" || nBasicType == " corner" || nBasicType == " tjunction" || nBasicType == "crossing" || nBasicType == "end")) {
                Debug.Log("sc1");
                objResult = rArray[3];
            }*/
            //Debug.Log(nbExtendedNs);
            if (extendedN[0] == "straight" && nBasicType == "end") {
                objResult = rArray[4];
            }
            else if (extendedN[0] == "straight" && nBasicType == "corner") {
                objResult = rArray[3];
            }
            //else if()
            //else if(extendedN[0] == "straight" && nBasicType == "corner") {
            // objResult = rArray[3];
            // }
            //if()
        }
        else if ((neighbourPos == 0 && scen == 2) ||
                (neighbourPos == 1 && scen == 2) ||
                (neighbourPos == 2 && scen == 2) ||
                (neighbourPos == 3 && scen == 3)) {
            //Debug.Log("sc2");
            if ((extendedN[0] == "straight" || extendedN[0] == "corner" || extendedN[0] == "crossing" || extendedN[0] == "end") &&
                (nBasicType == " straight" || nBasicType == " corner" || nBasicType == " tjunction" || nBasicType == "crossing" || nBasicType == " end")) {
                //Debug.Log("sc3");
                objResult = rArray[2];
            }
        }


        /*foreach (Transform child in spawnedObjsParent.transform) {
            foreach (Transform subChild in child.transform) {
                if (subChild.gameObject.GetComponent<TagSystem>().gridId > -1) {
                    Debug.Log("id= " + subChild.gameObject.GetComponent<TagSystem>().gridId + " obj= " + subChild.gameObject.GetComponent<TagSystem>().basicType);
                }
            }
        }*/
        return objResult;
    }
    //0 = end, 1 = straight, 2 = crossing, 3 = tjunction 4 = corner
    public int NONAME0(int pos, int scenario, string extendedN, string objType, float objRotation) {
        int objIndex = 0;
        if (extendedN != null) {
            Debug.Log(pos + " " + scenario + " " + extendedN + " " + objType + " " + objRotation);
        }
        else {
            Debug.Log(pos + " " + scenario + " " + " " + objType + " " + objRotation);
        }

        if (pos == 0) {
            if (scenario == 0) {
                if (extendedN != null) {
                    if (extendedN == "straight" && objType == "end") {
                        objIndex = 4;
                    }
                    else if (extendedN == "straight" && objType == "corner") {
                        objIndex = 3;
                    }
                    else if (extendedN == "corner" && objType == "tjunction") {

                    }
                    else if (extendedN == "tjunction" && objType == "end") {
                        objIndex = 1;
                    }
                    else if (extendedN == "tjunction" && objType == "corner") {
                        if (objRotation == 0f || objRotation == 270f) {
                            objIndex = 3;
                        }
                        else if (objRotation == 90f || objRotation == 180f) {
                            objIndex = 1;
                        }
                    }
                    else if (extendedN == "tjunction" && objType == "corner") {
                        objIndex = 3;
                    }
                    else if (extendedN == "end" && objType == "corner") {
                        objIndex = 3;
                    }
                }
                else {
                    if (objType == "end") {
                        objIndex = 4;
                    }
                    else if (objType == "corner") {
                        objIndex = 3;
                    }
                }
            }
            else if (scenario == 1) {

            }
            else if (scenario == 2) {

            }
            else if (scenario == 3) {

            }
        }
        else if (pos == 1) {
            if (scenario == 0) {
                if (extendedN != null) {
                    if (extendedN == "straight" && objType == "end") {
                        objIndex = 4;
                    }
                    else if (extendedN == "straight" && objType == "corner") {
                        objIndex = 3;
                    }
                    else if (extendedN == "corner" && objType == "tjunction") {

                    }
                    else if (extendedN == "tjunction" && objType == "end") {
                        objIndex = 1;
                    }
                    else if (extendedN == "tjunction" && objType == "corner") {
                        if (objRotation == 0f || objRotation == 270f) {
                            objIndex = 3;
                        }
                        else if (objRotation == 90f || objRotation == 180f) {
                            objIndex = 1;
                        }
                    }
                    else if (extendedN == "tjunction" && objType == "corner") {
                        objIndex = 3;
                    }
                    else if (extendedN == "end" && objType == "corner") {
                        objIndex = 1;
                    }
                }
                else {
                    if (objType == "end") {
                        objIndex = 4;
                    }
                    else if (objType == "corner") {
                        objIndex = 3;
                    }
                }
            }
            else if (scenario == 1) {

            }
            else if (scenario == 2) {

            }
            else if (scenario == 3) {

            }
        }
        else if (pos == 2) {
            if (scenario == 0) {
                if (extendedN != null) {
                    if (extendedN == "straight" && objType == "end") {
                        objIndex = 4;
                    }
                    else if (extendedN == "straight" && objType == "corner") {
                        objIndex = 3;
                    }
                    else if (extendedN == "corner" && objType == "tjunction") {

                    }
                    else if (extendedN == "tjunction" && objType == "end") {
                        objIndex = 1;
                    }
                    else if (extendedN == "tjunction" && objType == "corner") {
                        if (objRotation == 0f || objRotation == 270f) {
                            objIndex = 3;
                        }
                        else if (objRotation == 90f || objRotation == 180f) {
                            objIndex = 1;
                        }
                    }
                    else if (extendedN == "end" && objType == "corner") {
                        objIndex = 1;
                    }
                }
                else {
                    if (objType == "end") {
                        objIndex = 4;
                    }
                    else if (objType == "corner") {
                        objIndex = 3;
                    }
                }
            }
            else if (scenario == 1) {

            }
            else if (scenario == 2) {

            }
            else if (scenario == 3) {

            }
        }
        else if (pos == 3) {
            if (scenario == 0) {
                if (extendedN != null) {
                    if (extendedN == "straight" && objType == "end") {
                        objIndex = 4;
                    }
                    else if (extendedN == "straight" && objType == "corner") {
                        objIndex = 3;
                    }
                    else if (extendedN == "corner" && objType == "tjunction") {

                    }
                    else if (extendedN == "tjunction" && objType == "end") {
                        objIndex = 1;
                    }
                    else if (extendedN == "tjunction" && objType == "corner") {
                        if (objRotation == 0f || objRotation == 270f) {
                            objIndex = 1;
                        }
                        else if (objRotation == 90f || objRotation == 180f) {
                            objIndex = 3;
                        }
                    }
                    else if (extendedN == "tjunction" && objType == "corner") {
                        objIndex = 3;
                    }
                    else if (extendedN == "end" && objType == "corner") {
                        objIndex = 1;
                    }
                }
                else {
                    if (objType == "end") {
                        objIndex = 4;
                    }
                    else if (objType == "corner") {
                        objIndex = 3;
                    }
                }
            }
            else if (scenario == 1) {

            }
            else if (scenario == 2) {

            }
            else if (scenario == 3) {

            }
        }
        return objIndex;
    }

    public GameObject Road_TypeProcessorB(string extendedN, GameObject obj, int neighbourPos, int scen, GameObject[] rArray) {
        GameObject objResult = null;
        if (extendedN != null) {
            Debug.Log(extendedN);
        }
        else {
            //Debug.Log("no neighbour");
        }
        int id = obj.GetComponent<TagSystem>().gridId;
        float nRotation = 0;
        nRotation = obj.transform.parent.localEulerAngles.y;
        string nBasicType = obj.GetComponent<TagSystem>().basicType;
        Debug.Log(obj.transform.transform.position);

        objResult = rArray[NONAME0(neighbourPos, scen, extendedN, nBasicType, nRotation)];


        /*if ((neighbourPos == 0 && scen == 0) || (neighbourPos == 0 && scen == 1) ||
                (neighbourPos == 1 && scen == 0) || (neighbourPos == 1 && scen == 1) ||
                (neighbourPos == 2 && scen == 0) || (neighbourPos == 2 && scen == 1) ||
                (neighbourPos == 3 && scen == 0) || (neighbourPos == 3 && scen == 1)) {
            //0 = end, 1 = straight, 2 = crossing, 3 = tjunctionm 4 = corner
            if(extendedN != null) {
                if (extendedN == "straight" && nBasicType == "end") {
                    objResult = rArray[4];
                }
                else if (extendedN == "straight" && nBasicType == "corner") {
                    objResult = rArray[3];
                }
                else if (extendedN == "corner" && nBasicType == "tjunction") {

                }

            }
            else {
                if(nBasicType == "end") {
                    objResult = rArray[4];
                }
                else if (nBasicType == "corner") {
                    objResult = rArray[3];
                }
            }
        }*/


        /*else if ((neighbourPos == 0 && scen == 2) ||
                (neighbourPos == 1 && scen == 2) ||
                (neighbourPos == 2 && scen == 2) ||
                (neighbourPos == 3 && scen == 3)) {
            //Debug.Log("sc2");
            if ((extendedN == "straight" || extendedN == "corner" || extendedN == "crossing" || extendedN == "end") &&
                (nBasicType == " straight" || nBasicType == " corner" || nBasicType == " tjunction" || nBasicType == "crossing" || nBasicType == " end")) {
                //Debug.Log("sc3");
                objResult = rArray[2];
            }
        }


        /*foreach (Transform child in spawnedObjsParent.transform) {
            foreach (Transform subChild in child.transform) {
                if (subChild.gameObject.GetComponent<TagSystem>().gridId > -1) {
                    Debug.Log("id= " + subChild.gameObject.GetComponent<TagSystem>().gridId + " obj= " + subChild.gameObject.GetComponent<TagSystem>().basicType);
                }
            }
        }*/
        return objResult;
    }


    public GameObject Road_TypeProcessorC(string extendedN, GameObject obj, int neighbourPos, int scen, GameObject[] rArray) {
        GameObject objResult = null;
        if (extendedN != null) {
            Debug.Log(extendedN);
        }
        else {
            //Debug.Log("no neighbour");
        }
        int id = obj.GetComponent<TagSystem>().gridId;
        float nRotation = 0;
        nRotation = obj.transform.parent.localEulerAngles.y;
        string nBasicType = obj.GetComponent<TagSystem>().basicType;
        Debug.Log(obj.transform.transform.position);

        objResult = rArray[NONAME0(neighbourPos, scen, extendedN, nBasicType, nRotation)];


        /*if ((neighbourPos == 0 && scen == 0) || (neighbourPos == 0 && scen == 1) ||
                (neighbourPos == 1 && scen == 0) || (neighbourPos == 1 && scen == 1) ||
                (neighbourPos == 2 && scen == 0) || (neighbourPos == 2 && scen == 1) ||
                (neighbourPos == 3 && scen == 0) || (neighbourPos == 3 && scen == 1)) {
            //0 = end, 1 = straight, 2 = crossing, 3 = tjunctionm 4 = corner
            if(extendedN != null) {
                if (extendedN == "straight" && nBasicType == "end") {
                    objResult = rArray[4];
                }
                else if (extendedN == "straight" && nBasicType == "corner") {
                    objResult = rArray[3];
                }
                else if (extendedN == "corner" && nBasicType == "tjunction") {

                }

            }
            else {
                if(nBasicType == "end") {
                    objResult = rArray[4];
                }
                else if (nBasicType == "corner") {
                    objResult = rArray[3];
                }
            }
        }*/


        /*else if ((neighbourPos == 0 && scen == 2) ||
                (neighbourPos == 1 && scen == 2) ||
                (neighbourPos == 2 && scen == 2) ||
                (neighbourPos == 3 && scen == 3)) {
            //Debug.Log("sc2");
            if ((extendedN == "straight" || extendedN == "corner" || extendedN == "crossing" || extendedN == "end") &&
                (nBasicType == " straight" || nBasicType == " corner" || nBasicType == " tjunction" || nBasicType == "crossing" || nBasicType == " end")) {
                //Debug.Log("sc3");
                objResult = rArray[2];
            }
        }


        /*foreach (Transform child in spawnedObjsParent.transform) {
            foreach (Transform subChild in child.transform) {
                if (subChild.gameObject.GetComponent<TagSystem>().gridId > -1) {
                    Debug.Log("id= " + subChild.gameObject.GetComponent<TagSystem>().gridId + " obj= " + subChild.gameObject.GetComponent<TagSystem>().basicType);
                }
            }
        }*/
        return objResult;
    }


    public GameObject Road_Replacer(GameObject newObj, int rotator, int id) {
        //GameObject targetObj = new GameObject();
        //targetObj.name = "targetobj";
        Vector3 nPos = new Vector3();
        Vector3 nRot = new Vector3();
        GameObject objReturn;
        GameObject targObj = null;
        foreach (Transform child in spawnedObjsParent.transform) {
            foreach (Transform subChild in child.transform) {
                if (subChild.transform.gameObject.GetComponent<TagSystem>().gridId == id) {
                    //Debug.Log(subChild.gameObject.name + " " + subChild.transform.position);
                    nPos = subChild.transform.position;
                    targObj = subChild.gameObject;

                    string newObjBasicType = null;
                    nRot = subChild.transform.localEulerAngles;
                    foreach (Transform newObjChild in newObj.transform) {
                        newObjBasicType = newObjChild.gameObject.GetComponent<TagSystem>().basicType;
                    }
                    if (rotator == 0) {
                        nRot.y += 90f;
                    }

                    else if (rotator == 1 && newObjBasicType == "corner") {
                        nRot.y += 180f;
                    }
                    else if (rotator == 1 && newObjBasicType == "straight") {
                        nRot.y += 90f;
                    }
                    else if (rotator == 2 || rotator == 3 && newObjBasicType == "corner") {
                        nRot.y += 180f;
                    }
                    else if (rotator == 2 || rotator == 3 && newObjBasicType == "straight") {
                        nRot.y += 180f;
                    }

                    //targetObj = subChild.gameObject;
                }
            }
        }
        eNeigh = Road_ExtendedNeighboursB(targObj);
        for (int i = 0; i < extendedNeighbours.Count; i++) {
            //Debug.Log(extendedNeighbours[i]);
        }
        foreach (Transform child in spawnedObjsParent.transform) {
            foreach (Transform subChild in child.transform) {
                if (subChild.transform.gameObject.GetComponent<TagSystem>().gridId == id) {
                    //Debug.Log(subChild.gameObject.GetComponent<TagSystem>().basicType);
                    Destroy(child.gameObject);
                }
            }
        }
        var spawnedObj = (GameObject)Instantiate(newObj, nPos, Quaternion.Euler(nRot)) as GameObject;
        genObj2 = spawnedObj;
        genObj2.transform.parent = spawnedObjsParent.transform;

        foreach (Transform child in genObj2.transform) {
            child.gameObject.AddComponent<BoxCollider>();
            child.gameObject.transform.tag = roadTag;
            //child.gameObject.AddComponent<TagSystem>();
        }
        objReturn = genObj2;
        return objReturn;
    }

    public float Road_SpawnedRotator(int index, string objType, int side) {
        float addedAngle = 0;
        if (index == 0) {
            if (objType == "corner") {
                addedAngle = 90f;
            }
            else if (objType == "straight") {
                addedAngle = 90f;
            }
            else if (objType == "tjunction") {
                addedAngle = 180f;
            }
            else if (objType == "crossing") {
                addedAngle = 0f;
            }
            else if (objType == "end") {
                addedAngle = 270f;
            }
        }
        else if (index == 1) {
            if (objType == "corner") {
                addedAngle = 180f;
            }
            else if (objType == "straight") {
                addedAngle = 90f;
            }
            else if (objType == "tjunction") {
                addedAngle = 180f;
            }
            else if (objType == "crossing") {
                addedAngle = 0f;
            }
            else if (objType == "end") {
                addedAngle = 90f;
            }
        }
        else if (index == 2) {
            if (objType == "corner") {
                addedAngle = 180f;
            }
            else if (objType == "straight") {
                addedAngle = 0f;
            }
            else if (objType == "tjunction") {
                if (side == 1) {
                    addedAngle = 270f;
                }
                else if (side == 2) {
                    addedAngle = 90f;
                }
            }
            else if (objType == "crossing") {
                addedAngle = 0f;
            }
            else if (objType == "end") {
                addedAngle = 0f;
            }
        }
        else if (index == 3) {
            if (objType == "corner") {
                addedAngle = 270f;
            }
            else if (objType == "straight") {
                addedAngle = 0f;
            }
            else if (objType == "tjunction") {
                addedAngle = 270f;
            }
            else if (objType == "crossing") {
                addedAngle = 0f;
            }
            else if (objType == "end") {
                addedAngle = 180f;
            }
        }
        return addedAngle;
    }

    public void Road_ReplacerB(int rotator, int id, int scenario, int side) {
        //GameObject targetObj = new GameObject();
        //targetObj.name = "targetobj";
        Debug.Log("id= " + id);
        Vector3 nPos = new Vector3();
        Vector3 nRot = new Vector3();
        GameObject objReturn;
        GameObject newOb = null;
        GameObject targObj = null;
        foreach (Transform child in spawnedObjsParent.transform) {
            foreach (Transform subChild in child.transform) {
                if (subChild.transform.gameObject.GetComponent<TagSystem>().gridId == id) {
                    //Debug.Log(subChild.gameObject.name + " " + subChild.transform.position);
                    nPos = subChild.transform.position;
                    targObj = subChild.gameObject;
                    //Debug.Log(targObj.gameObject.name);
                    eNeigh = null;
                    eNeigh = Road_ExtendedNeighboursB(targObj);
                    newOb = Road_TypeProcessorB(eNeigh, targObj, rotator, scenario, basicRoads);
                    string newObjBasicType = null;
                    nRot = subChild.transform.localEulerAngles;
                    foreach (Transform newObjChild in newOb.transform) {
                        newObjBasicType = newObjChild.gameObject.GetComponent<TagSystem>().basicType;
                    }
                    //Debug.Log(rotator + " " + newObjBasicType);
                    nRot.y += Road_SpawnedRotator(rotator, newObjBasicType, side);
                    //targetObj = subChild.gameObject;
                }
            }
        }
        //extendedNeighbours = Road_ExtendedNeighbours(targObj);

        foreach (Transform child in spawnedObjsParent.transform) {
            foreach (Transform subChild in child.transform) {
                if (subChild.transform.gameObject.GetComponent<TagSystem>().gridId == id) {
                    //Debug.Log(subChild.gameObject.GetComponent<TagSystem>().basicType);
                    Destroy(child.gameObject);
                }
            }
        }
        var spawnedObj = (GameObject)Instantiate(newOb, nPos, Quaternion.Euler(nRot)) as GameObject;
        genObj2 = spawnedObj;
        genObj2.transform.parent = spawnedObjsParent.transform;

        foreach (Transform child in genObj2.transform) {
            child.gameObject.AddComponent<BoxCollider>();
            child.gameObject.transform.tag = roadTag;
            //child.gameObject.AddComponent<TagSystem>();
        }
        objReturn = genObj2;
    }

    public List<int> NeighbourScenarioConv(int scenario) {
        List<int> nPositions = new List<int>();
        switch (scenario) {
            case 0:
                nPositions.Add(0);
                break;
            case 1:
                nPositions.Add(2);
                break;
            case 2:
                nPositions.Add(0);
                nPositions.Add(2);
                break;
            case 3:

                break;
            default:
                break;
        }
        return nPositions;
    }

    public void Road_ReplacerC(int rotator, int id, int scenario, int side) {
        //GameObject targetObj = new GameObject();
        //targetObj.name = "targetobj";
        Vector3 nPos = new Vector3();
        Vector3 nRot = new Vector3();
        int itj = 0;
        GameObject objReturn;
        GameObject newOb = null;
        GameObject targObj = null;
        string newObjBasicType = null;
        float neighbourRot = 0;
        int neigbourId = 0;
        string neighbourType;
        List<int> basicNeighboursPos = new List<int>();
        basicNeighboursPos = NeighbourScenarioConv(scenario);
        foreach (Transform child in spawnedObjsParent.transform) {
            foreach (Transform subChild in child.transform) {
                if (subChild.transform.gameObject.GetComponent<TagSystem>().gridId == id) {
                    nPos = subChild.transform.position;
                    targObj = subChild.gameObject;
                    nRot = subChild.transform.localEulerAngles;
                }
                if (subChild.transform.gameObject.GetComponent<TagSystem>().gridId == neigbourId) {
                    Id_NeighbourConverter(id);
                    neighbourRot = subChild.transform.localEulerAngles.y;
                    neighbourType = subChild.transform.gameObject.GetComponent<TagSystem>().basicType;
                }
            }
        }
        foreach (Transform newObjChild in newOb.transform) {
            newObjBasicType = newObjChild.gameObject.GetComponent<TagSystem>().basicType;
        }
        newOb = Road_TypeProcessorB(eNeigh, targObj, rotator, scenario, basicRoads);
        nRot.y += Road_SpawnedRotator(rotator, newObjBasicType, side);
        foreach (Transform child in spawnedObjsParent.transform) {
            foreach (Transform subChild in child.transform) {
                if (subChild.transform.gameObject.GetComponent<TagSystem>().gridId == id) {
                    Destroy(child.gameObject);
                }
            }
        }
        var spawnedObj = (GameObject)Instantiate(newOb, nPos, Quaternion.Euler(nRot)) as GameObject;
        genObj2 = spawnedObj;
        genObj2.transform.parent = spawnedObjsParent.transform;

        foreach (Transform child in genObj2.transform) {
            child.gameObject.AddComponent<BoxCollider>();
            child.gameObject.transform.tag = roadTag;
            //child.gameObject.AddComponent<TagSystem>();
        }
        objReturn = genObj2;
    }

    public void Road_Spawner(GameObject objToSpawn, Vector3 targetPos, int rotator) {
        float yRot = 0;
        //Debug.Log(rotator);
        if (rotator == 0) {
            yRot = objToSpawn.transform.localEulerAngles.y - 90f;
        }
        else if (rotator == 1) {
            yRot = objToSpawn.transform.localEulerAngles.y + 90f;
        }
        else if (rotator == 3) {
            yRot = objToSpawn.transform.localEulerAngles.y + 180f;
        }

        else {
            yRot = objToSpawn.transform.localEulerAngles.y;
        }
        float xRot = objToSpawn.transform.localEulerAngles.x;
        float zRot = objToSpawn.transform.localEulerAngles.z;
        var spawnedObj = (GameObject)Instantiate(objToSpawn, new Vector3(targetPosition.x, 1f, targetPosition.y), Quaternion.Euler(xRot, yRot, zRot)) as GameObject;
        genObj = spawnedObj;
        genObj.transform.parent = spawnedObjsParent.transform;
        foreach (Transform child in genObj.transform) {
            child.gameObject.AddComponent<BoxCollider>();
            child.gameObject.transform.tag = roadTag;
            //child.gameObject.AddComponent<TagSystem>();
            //child.gameObject.GetComponent<TagSystem>().gridId = null;
        }
    }


    private Object[] Road_ObjLoader(string path) {
        Object[] objArray;
        var tileArrayFunc = Resources.LoadAll(path, typeof(GameObject));
        objArray = new Object[tileArrayFunc.Length];
        for (int i = 0; i < tileArrayFunc.Length; i++) {
            objArray[i] = tileArrayFunc[i];
        }
        return objArray;
    }
}
