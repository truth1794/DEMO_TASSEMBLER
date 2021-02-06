using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TagEngine;
using outliner;
using Object = UnityEngine.Object;

public class RoadManager2 : MonoBehaviour {

    public Object[] roadObjects;
    public GameObject[] basicRoads;
    public Vector2 targetPosition;
    public Vector2 bufferPos0, bufferPos1, bufferPos2, bufferPos3, onRoadBuffer0, onRoadBuffer1, onRoadBuffer2;
    public string roadTag = "road";
    public string roadObjPath = "road";
    public List<RaycastHit[]> neighbourData;
    public bool onTargetClick = false, onRoadClick = false, alreadyClicked = false, onRoad = false, onRoadPassed = false;
    public GameObject genObj, genObj2, genObj3, genObj4, genObj5;
    public GameObject spawnedObjsParent;
    public GameObject buffer;
    public GameObject RoadPrefabs;
    public int iterator0, iterator1, iterator2, iterator3;
    public bool onR = false, roadBuilder = false;



    PositionSensor posDataScript;
    public GameObject posData;

    GUIscript guiDataScript;
    public GameObject guiData;

    private void Awake() {
        roadObjects = Road_ObjLoader(roadObjPath);
        basicRoads = new GameObject[5];
        iterator2 = 0;
        for (int i = 0; i < 5; i++) {
            basicRoads[i] = (GameObject)roadObjects[i];
        }
        foreach (Transform child in RoadPrefabs.transform) {
            basicRoads[iterator2] = child.gameObject;
            iterator2 += 1;
        }
    }

    private void Start() {
        iterator0 = 0;
        iterator1 = 0;
        iterator3 = 0;
        posDataScript = posData.GetComponent<PositionSensor>();
        guiDataScript = guiData.GetComponent<GUIscript>();
    }

    private void Update() {

        onTargetClick = posDataScript.roadTouch;
        onRoad = posDataScript.onRoad;
        targetPosition = new Vector2(posDataScript.adjustedMousX1, posDataScript.adjustedMousZ1);
        if (onTargetClick && roadBuilder) {
            alreadyClicked = true;
            Road_Generator(targetPosition, onRoad);
        }
        else if (alreadyClicked) {
            if (iterator1 == 0) {
                bufferPos0 = new Vector3();
                bufferPos1 = new Vector3();
                iterator0 = 0;
                iterator1 = 1;
            }
            if (iterator0 > 0) {
                bufferPos0 = new Vector3();
                bufferPos1 = new Vector3();
                iterator0 = 0;
            }
            alreadyClicked = false;
        }
    }


    public void Road_Generator(Vector2 targetPos, bool onRoadData) {
        Vector3 objRot = new Vector3();
        Vector3 spawnPos = new Vector3(targetPos.x, 1f, targetPos.y);
        
        if (iterator0 == 0) {
            objRot = basicRoads[0].transform.localEulerAngles;
            var newObj = (GameObject)Instantiate(basicRoads[0], spawnPos, Quaternion.Euler(objRot.x, objRot.y, objRot.z)) as GameObject;
            genObj = newObj;
            foreach (Transform child in genObj.transform) {
                child.gameObject.AddComponent<TagSystem>();
            }
            genObj.transform.parent = spawnedObjsParent.transform;
        }
        else if (iterator0 < 2 && bufferPos0 != targetPos) {
            if (targetPos.x == bufferPos0.x && targetPos.y != bufferPos0.y) {
                float addRot = 0f;
                if (targetPos.y > bufferPos0.y) {
                    addRot += 180f;
                }
                var newObj = (GameObject)Instantiate(basicRoads[0], spawnPos, Quaternion.Euler(objRot.x, objRot.y + addRot, objRot.z)) as GameObject;
                genObj2 = newObj;
                foreach (Transform child in genObj2.transform) {
                    child.gameObject.AddComponent<TagSystem>();
                    child.gameObject.AddComponent<Outline>();
                }
                genObj2.transform.parent = spawnedObjsParent.transform;
                foreach (Transform previousObj in spawnedObjsParent.transform) {
                    Vector2 previousPos = new Vector2(previousObj.transform.position.x, previousObj.transform.position.z);
                    if (previousPos == bufferPos0) {
                        Destroy(previousObj.gameObject);
                    }
                }
                objRot = basicRoads[1].transform.localEulerAngles;
                Vector3 newObj2Pos = new Vector3(bufferPos0.x, 1f, bufferPos0.y);
                var newObj2 = (GameObject)Instantiate(basicRoads[1], newObj2Pos, Quaternion.Euler(objRot.x, objRot.y, objRot.z)) as GameObject;
                genObj2 = newObj2;
                genObj2.transform.parent = spawnedObjsParent.transform;
                foreach (Transform child in genObj2.transform) {
                    Vector3 bufferChildPos = new Vector3(bufferPos0.x, 1f, bufferPos0.y);
                    if (child.transform.position == bufferChildPos) {
                        child.gameObject.AddComponent<TagSystem>();
                        child.gameObject.AddComponent<BoxCollider>();
                        child.gameObject.AddComponent<Outline>();
                    }
                }
            }
        }
        else if (iterator0 == 2 && bufferPos1 != targetPos) {
            if (targetPos.x == bufferPos1.x + 8 && targetPos.y == bufferPos1.y) {
                int mIndex = -1;
                bool onR2 = false;
                foreach (Transform child in spawnedObjsParent.transform) {
                    if (child.transform.position == spawnPos) {
                        foreach (Transform subChild in child.transform) {
                            if (subChild.GetComponent<BoxCollider>() != null) {
                                onR2 = true;
                                break;
                            }
                            else {
                                onR2 = false;
                            }
                        }
                    }
                }
                if (onR2) {
                    Debug.Log("ON ROAD");
                    mIndex = Road_proc1(targetPos);
                }
                else {
                    float addRotObj0 = 0f, addRotObj1 = 0f;
                    addRotObj0 += 270f;
                    var newObj = (GameObject)Instantiate(basicRoads[0], spawnPos, Quaternion.Euler(objRot.x, objRot.y + addRotObj0, objRot.z)) as GameObject;
                    genObj2 = newObj;
                    genObj2.transform.parent = spawnedObjsParent.transform;
                    foreach (Transform child in genObj2.transform) {
                        child.gameObject.AddComponent<TagSystem>();
                        child.gameObject.AddComponent<Outline>();
                    }
                    genObj2.transform.parent = spawnedObjsParent.transform;
                    string prevObjName = null;
                    foreach (Transform previousObj in spawnedObjsParent.transform) {
                        Vector2 previousPos = new Vector2(previousObj.transform.position.x, previousObj.transform.position.z);
                        Vector2 posComp = new Vector2();
                        posComp = bufferPos1;
                        if (previousPos == posComp) {
                            foreach (Transform subChild in previousObj.transform) {
                                prevObjName = subChild.GetComponent<TagSystem>().basicType;
                            }
                            Destroy(previousObj.gameObject);
                        }
                    }
                    int scen = -1;
                    if (targetPos.x != bufferPos0.x && targetPos.y == bufferPos0.y && !(prevObjName == "tjunction") && !(prevObjName == "corner")) {
                        scen = 0;
                    }
                    else if (targetPos.x != bufferPos0.x && targetPos.y != bufferPos0.y && !(prevObjName == "tjunction") && !(prevObjName == "corner")) {
                        scen = 1;
                    }
                    else if (targetPos.x == bufferPos0.x && targetPos.y != bufferPos0.y && !(prevObjName == "tjunction") && !(prevObjName == "corner")) {
                        scen = 2;
                    }
                    else if (prevObjName == "tjunction") {
                        scen = 3;
                    }
                    else if (prevObjName == "corner") {
                        scen = 4;
                    }
                    Vector3 newObj3Pos = new Vector3(bufferPos1.x, 1f, bufferPos1.y);
                    int index = -1;
                    //Debug.Log(scen);
                    if (scen == 0) {
                        index = 1;
                        addRotObj1 += 90f;
                    }
                    else if (scen == 1) {
                        index = 4;
                        if (targetPos.x > bufferPos0.x && targetPos.y > bufferPos0.y) {
                            addRotObj1 += 90f;
                        }
                        else if (targetPos.x > bufferPos0.x && targetPos.y < bufferPos0.y) {
                            addRotObj1 += 0f;
                        }
                        else if (targetPos.x < bufferPos0.x && targetPos.y > bufferPos0.y) {
                            addRotObj1 += 0f;
                        }
                        else if (targetPos.x < bufferPos0.x && targetPos.y < bufferPos0.y) {
                            addRotObj1 += 90f;
                        }
                    }
                    else if (scen == 2) {
                        index = 1;
                        addRotObj1 += 0f;
                    }
                    else if (scen == 3) {
                        index = 2;
                    }
                    else if (scen == 4) {
                        index = 3;
                    }

                    Vector3 objRot2 = basicRoads[index].transform.localEulerAngles;
                    objRot2.y += addRotObj1;
                    var newObj3 = (GameObject)Instantiate(basicRoads[index], newObj3Pos, Quaternion.Euler(objRot2)) as GameObject;
                    genObj3 = newObj3;
                    genObj3.transform.parent = spawnedObjsParent.transform;
                    foreach (Transform child in genObj3.transform) {
                        Vector3 bufferChildPos = new Vector3(bufferPos1.x, 1f, bufferPos1.y + 8);
                        if (child.transform.position == newObj3Pos) {
                            child.gameObject.AddComponent<TagSystem>();
                            child.gameObject.AddComponent<BoxCollider>();
                            child.gameObject.AddComponent<Outline>();
                        }
                    }
                }
            }
                

            else if ((targetPos.x == bufferPos1.x - 8 && targetPos.y == bufferPos1.y) /*&& !onRoadData*/) {
                //Debug.Log("REPL");
                int mIndex = -1;
                bool onR2 = false;
                foreach(Transform child in spawnedObjsParent.transform) {
                    if (child.transform.position == spawnPos) {
                        foreach (Transform subChild in child.transform) {
                            if (subChild.GetComponent<BoxCollider>() != null) {
                                onR2 = true;
                                break;
                            }
                            else {
                                onR2 = false;
                            }
                        }
                    }
                }
                if (onR2) {
                    //Debug.Log("ON ROAD");
                    mIndex = Road_proc1(targetPos);
                    //Debug.Log(mIndex);
                }
                else {
                    //Debug.Log("NOT ON ROAD");
                    int indx = 0;
                    float addRotObj0 = 0f, addRotObj1 = 0f;
                    addRotObj0 += 90f;

                    var newObj = (GameObject)Instantiate(basicRoads[indx], spawnPos, Quaternion.Euler(objRot.x, objRot.y + addRotObj0, objRot.z)) as GameObject;
                    genObj2 = newObj;
                    foreach (Transform child in genObj2.transform) {
                        child.gameObject.AddComponent<TagSystem>();
                        child.gameObject.AddComponent<Outline>();
                    }
                    genObj2.transform.parent = spawnedObjsParent.transform;
                    string prevObjName = null;
                    foreach (Transform previousObj in spawnedObjsParent.transform) {
                        Vector2 previousPos = new Vector2(previousObj.transform.position.x, previousObj.transform.position.z);
                        Vector2 posComp = new Vector2();
                        posComp = bufferPos1;
                        if (previousPos == posComp) {
                            foreach(Transform subChild in previousObj.transform) {
                                prevObjName = subChild.GetComponent<TagSystem>().basicType;
                            }
                            Destroy(previousObj.gameObject);
                        }
                    }
                    int scen = -1;
                    if (targetPos.x != bufferPos0.x && targetPos.y == bufferPos0.y && !(prevObjName == "tjunction") && !(prevObjName == "corner")) {
                        scen = 0;
                    }
                    else if (targetPos.x != bufferPos0.x && targetPos.y != bufferPos0.y && !(prevObjName == "tjunction") && !(prevObjName == "corner")) {
                        scen = 1;
                    }
                    else if (targetPos.x == bufferPos0.x && targetPos.y != bufferPos0.y && !(prevObjName == "tjunction") && !(prevObjName == "corner")) {
                        scen = 2;
                    }
                    else if(prevObjName == "tjunction") {
                        scen = 3;
                    }
                    else if(prevObjName == "corner") {
                        scen = 4;
                    }
                    Vector3 newObj3Pos = new Vector3(bufferPos1.x, 1f, bufferPos1.y);
                    int index = -1;
                    if (onR) {
                    }
                    else {
                        if (scen == 0) {
                            index = 1;
                            addRotObj1 += 90f;
                        }
                        else if (scen == 1) {
                            index = 4;
                            if (targetPos.x > bufferPos0.x && targetPos.y > bufferPos0.y) {
                                addRotObj1 += 270f;
                            }
                            else if (targetPos.x > bufferPos0.x && targetPos.y < bufferPos0.y) {
                                addRotObj1 += 0f;
                            }
                            else if (targetPos.x < bufferPos0.x && targetPos.y > bufferPos0.y) {
                                addRotObj1 += 180f;
                            }
                            else if (targetPos.x < bufferPos0.x && targetPos.y < bufferPos0.y) {
                                addRotObj1 += 270f;
                            }
                        }
                        else if (scen == 2) {
                            index = 1;
                            addRotObj1 += 0f;
                        }
                        else if(scen == 3) {
                            index = 2;
                        }
                        else if(scen == 4) {
                            index = 3;
                        }
                    }
                    //Debug.Log("ind= " + index);
                    Vector3 objRot2 = basicRoads[index].transform.localEulerAngles;
                    objRot2.y += addRotObj1;
                    var newObj3 = (GameObject)Instantiate(basicRoads[index], newObj3Pos, Quaternion.Euler(objRot2)) as GameObject;
                    genObj3 = newObj3;
                    genObj3.transform.parent = spawnedObjsParent.transform;
                    foreach (Transform child in genObj3.transform) {
                        Vector3 bufferChildPos = new Vector3(bufferPos1.x, 1f, bufferPos1.y + 8);
                        if (child.transform.position == newObj3Pos) {
                            child.gameObject.AddComponent<TagSystem>();
                            child.gameObject.AddComponent<BoxCollider>();
                            child.gameObject.AddComponent<Outline>();
                        }
                    }
                }
                
            }
            else if (targetPos.x == bufferPos1.x && targetPos.y == bufferPos1.y + 8) {
                int mIndex = -1;
                bool onR2 = false;
                foreach (Transform child in spawnedObjsParent.transform) {
                    if (child.transform.position == spawnPos) {
                        foreach (Transform subChild in child.transform) {
                            if (subChild.GetComponent<BoxCollider>() != null) {
                                onR2 = true;
                                break;
                            }
                            else {
                                onR2 = false;
                            }
                        }
                    }
                }
                if (onR2) {
                    Debug.Log("ON ROAD");
                    mIndex = Road_proc1(targetPos);
                }
                else {
                    float addRotObj0 = 0f, addRotObj1 = 0f;
                    addRotObj0 += 180f;

                    var newObj = (GameObject)Instantiate(basicRoads[0], spawnPos, Quaternion.Euler(objRot.x, objRot.y + addRotObj0, objRot.z)) as GameObject;
                    genObj2 = newObj;
                    foreach (Transform child in genObj2.transform) {
                        child.gameObject.AddComponent<TagSystem>();
                        child.gameObject.AddComponent<Outline>();
                    }
                    genObj2.transform.parent = spawnedObjsParent.transform;
                    string prevObjName = null;
                    foreach (Transform previousObj in spawnedObjsParent.transform) {
                        Vector2 previousPos = new Vector2(previousObj.transform.position.x, previousObj.transform.position.z);
                        Vector2 posComp = new Vector2();
                        posComp = bufferPos1;
                        if (previousPos == posComp) {
                            foreach (Transform subChild in previousObj.transform) {
                                prevObjName = subChild.GetComponent<TagSystem>().basicType;
                            }
                            Destroy(previousObj.gameObject);
                        }
                    }
                    int scen = -1;
                    if (targetPos.x != bufferPos0.x && targetPos.y == bufferPos0.y && !(prevObjName == "tjunction") && !(prevObjName == "corner")) {
                        scen = 0;
                    }
                    else if (targetPos.x != bufferPos0.x && targetPos.y != bufferPos0.y && !(prevObjName == "tjunction") && !(prevObjName == "corner")) {
                        scen = 1;
                    }
                    else if (targetPos.x == bufferPos0.x && targetPos.y != bufferPos0.y && !(prevObjName == "tjunction") && !(prevObjName == "corner")) {
                        scen = 2;
                    }
                    else if (prevObjName == "tjunction") {
                        scen = 3;
                    }
                    else if (prevObjName == "corner") {
                        scen = 4;
                    }
                    //Debug.Log("scenario= " + scen);
                    Vector3 newObj3Pos = new Vector3(bufferPos1.x, 1f, bufferPos1.y);
                    int index = -1;

                    if (scen == 0) {
                        index = 1;
                        addRotObj1 += 90f;
                    }

                    else if (scen == 1) {
                        index = 4;
                        if (targetPos.x > bufferPos0.x && targetPos.y > bufferPos0.y) {
                            addRotObj1 += 270f;
                        }
                        else if (targetPos.x > bufferPos0.x && targetPos.y < bufferPos0.y) {
                            addRotObj1 += 180f;
                        }
                        else if (targetPos.x < bufferPos0.x && targetPos.y > bufferPos0.y) {
                            addRotObj1 += 0f;
                        }
                        else if (targetPos.x < bufferPos0.x && targetPos.y < bufferPos0.y) {
                            addRotObj1 += 90f;
                        }
                    }
                    else if (scen == 2) {
                        index = 1;
                        addRotObj1 += 0f;
                    }
                    else if(scen == 3) {
                        index = 2;
                    }
                    else if(scen == 4) {
                        index = 3;
                    }
                    Vector3 objRot2 = basicRoads[index].transform.localEulerAngles;
                    objRot2.y += addRotObj1;
                    var newObj3 = (GameObject)Instantiate(basicRoads[index], newObj3Pos, Quaternion.Euler(objRot2)) as GameObject;
                    genObj3 = newObj3;
                    genObj3.transform.parent = spawnedObjsParent.transform;
                    foreach (Transform child in genObj3.transform) {
                        Vector3 bufferChildPos = new Vector3(bufferPos1.x, 1f, bufferPos1.y + 8);
                        if (child.transform.position == newObj3Pos) {
                            child.gameObject.AddComponent<TagSystem>();
                            child.gameObject.AddComponent<BoxCollider>();
                            child.gameObject.AddComponent<Outline>();
                        }
                    }
                }
                
            }
            else if (targetPos.x == bufferPos1.x && targetPos.y == bufferPos1.y - 8) {
                int mIndex = -1;
                bool onR2 = false;
                foreach (Transform child in spawnedObjsParent.transform) {
                    if (child.transform.position == spawnPos) {
                        foreach (Transform subChild in child.transform) {
                            if (subChild.GetComponent<BoxCollider>() != null) {
                                onR2 = true;
                                break;
                            }
                            else {
                                onR2 = false;
                            }
                        }
                    }
                }
                if (onR2) {
                    Debug.Log("ON ROAD");
                    mIndex = Road_proc1(targetPos);
                }
                else {
                    float addRotObj0 = 0f, addRotObj1 = 0f;

                    var newObj = (GameObject)Instantiate(basicRoads[0], spawnPos, Quaternion.Euler(objRot.x, objRot.y + addRotObj0, objRot.z)) as GameObject;
                    genObj2 = newObj;
                    foreach (Transform child in genObj2.transform) {
                        child.gameObject.AddComponent<TagSystem>();
                        child.gameObject.AddComponent<Outline>();
                    }
                    genObj2.transform.parent = spawnedObjsParent.transform;
                    string prevObjName = null;
                    foreach (Transform previousObj in spawnedObjsParent.transform) {
                        Vector2 previousPos = new Vector2(previousObj.transform.position.x, previousObj.transform.position.z);
                        Vector2 posComp = new Vector2();
                        posComp = bufferPos1;
                        if (previousPos == posComp) {
                            foreach (Transform subChild in previousObj.transform) {
                                prevObjName = subChild.GetComponent<TagSystem>().basicType;
                            }
                            Destroy(previousObj.gameObject);
                        }
                    }
                    int scen = -1;
                    if (targetPos.x != bufferPos0.x && targetPos.y == bufferPos0.y && !(prevObjName == "tjunction") && !(prevObjName == "corner")) {
                        scen = 0;
                    }
                    else if (targetPos.x != bufferPos0.x && targetPos.y != bufferPos0.y && !(prevObjName == "tjunction") && !(prevObjName == "corner")) {
                        scen = 1;
                    }
                    else if (targetPos.x == bufferPos0.x && targetPos.y != bufferPos0.y && !(prevObjName == "tjunction") && !(prevObjName == "corner")) {
                        scen = 2;
                    }
                    else if (prevObjName == "tjunction") {
                        scen = 3;
                    }
                    else if (prevObjName == "corner") {
                        scen = 4;
                    }
                    Vector3 newObj3Pos = new Vector3(bufferPos1.x, 1f, bufferPos1.y);
                    int index = -1;
                    if (scen == 0) {
                        index = 1;
                        addRotObj1 += 90f;
                    }
                    else if (scen == 1) {
                        index = 4;
                        if (targetPos.x > bufferPos0.x && targetPos.y > bufferPos0.y) {
                            addRotObj1 += 270f;
                        }
                        else if (targetPos.x > bufferPos0.x && targetPos.y < bufferPos0.y) {
                            addRotObj1 += 180f;
                        }
                        else if (targetPos.x < bufferPos0.x && targetPos.y > bufferPos0.y) {
                            addRotObj1 += 0f;
                        }
                        else if (targetPos.x < bufferPos0.x && targetPos.y < bufferPos0.y) {
                            addRotObj1 += 90f;
                        }
                    }
                    else if (scen == 2) {
                        index = 1;
                        addRotObj1 += 0f;
                    }
                    else if (scen == 3) {
                        index = 2;
                    }
                    else if (scen == 4) {
                        index = 3;
                    }
                    //Debug.Log(index);
                    Vector3 objRot2 = basicRoads[index].transform.localEulerAngles;
                    objRot2.y += addRotObj1;
                    var newObj3 = (GameObject)Instantiate(basicRoads[index], newObj3Pos, Quaternion.Euler(objRot2)) as GameObject;
                    genObj3 = newObj3;
                    genObj3.transform.parent = spawnedObjsParent.transform;
                    foreach (Transform child in genObj3.transform) {
                        Vector3 bufferChildPos = new Vector3(bufferPos1.x, 1f, bufferPos1.y + 8);
                        if (child.transform.position == newObj3Pos) {
                            child.gameObject.AddComponent<TagSystem>();
                            child.gameObject.AddComponent<BoxCollider>();
                            child.gameObject.AddComponent<Outline>();
                        }
                    }
                }
            }
        }

        if (bufferPos0 != targetPos && iterator0 < 1) {
            bufferPos0 = targetPos;
            iterator0 += 1;
        }
        else if (bufferPos0 != targetPos && iterator0 == 1) {
            bufferPos1 = targetPos;
            iterator0 += 1;
        }
        else if (bufferPos1 != targetPos && iterator0 == 2) {
            bufferPos3 = bufferPos2;
            bufferPos2 = bufferPos0;
            bufferPos0 = bufferPos1;
            bufferPos1 = targetPos;
        }
    }

    public int Road_proc1(Vector2 targetPos) {
        int scen1 = -1;
        int index1 = -1;
        float onRoadYrot = 0f;
        bool onRoadbuf0 = false, onRoadbuf1 = false, onRoadbuf2 = false;
        if (bufferPos1.x == targetPos.x + 8f && bufferPos1.y == targetPos.y) { //going left
            scen1 = 0;
            onRoadBuffer0 = new Vector2(targetPos.x, targetPos.y + 8f);
            onRoadBuffer1 = new Vector2(targetPos.x, targetPos.y - 8f);
            onRoadBuffer2 = new Vector2(targetPos.x - 8f, targetPos.y);
        }
        else if (bufferPos1.x == targetPos.x - 8f && bufferPos1.y == targetPos.y) { //going right
            scen1 = 1;
            onRoadBuffer0 = new Vector2(targetPos.x, targetPos.y + 8f);
            onRoadBuffer1 = new Vector2(targetPos.x, targetPos.y - 8f);
            onRoadBuffer2 = new Vector2(targetPos.x + 8f, targetPos.y);
        }
        else if (bufferPos1.x == targetPos.x && bufferPos1.y == targetPos.y + 8f) { //going down
            scen1 = 2;
            onRoadBuffer0 = new Vector2(targetPos.x + 8f, targetPos.y);
            onRoadBuffer1 = new Vector2(targetPos.x - 8f, targetPos.y);
            onRoadBuffer2 = new Vector2(targetPos.x, targetPos.y - 8f);
        }
        else if (bufferPos1.x == targetPos.x && bufferPos1.y == targetPos.y - 8f) { //going up
            scen1 = 3;
            onRoadBuffer0 = new Vector2(targetPos.x + 8f, targetPos.y);
            onRoadBuffer1 = new Vector2(targetPos.x - 8f, targetPos.y);
            onRoadBuffer2 = new Vector2(targetPos.x, targetPos.y + 8f);
        }
        foreach (Transform child in spawnedObjsParent.transform) {
            Vector2 childPos = new Vector2(child.transform.position.x, child.transform.position.z);
            if (scen1 >= 0) {
                if (scen1 == 0) {
                    if (childPos == onRoadBuffer0) {
                        onRoadbuf0 = true;
                    }
                    if (childPos == onRoadBuffer1) {
                        onRoadbuf1 = true;
                    }
                    if (childPos == onRoadBuffer2) {
                        onRoadbuf2 = true;
                    }
                }
                else if (scen1 == 1) {
                    if (childPos == onRoadBuffer0) {
                        onRoadbuf0 = true;
                    }
                    if (childPos == onRoadBuffer1) {
                        onRoadbuf1 = true;
                    }
                    if (childPos == onRoadBuffer2) {
                        onRoadbuf2 = true;
                    }
                }
                else if (scen1 == 2) {
                    if (childPos == onRoadBuffer0) {
                        onRoadbuf0 = true;
                    }
                    if (childPos == onRoadBuffer1) {
                        onRoadbuf1 = true;
                    }
                    if (childPos == onRoadBuffer2) {
                        onRoadbuf2 = true;
                    }
                }
                else if (scen1 == 3) {
                    if (childPos == onRoadBuffer0) {
                        onRoadbuf0 = true;
                    }
                    if (childPos == onRoadBuffer1) {
                        onRoadbuf1 = true;
                    }
                    if (childPos == onRoadBuffer2) {
                        onRoadbuf2 = true;
                    }
                }
                switch (scen1) {
                    case 0:
                        if (onRoadbuf0 && !onRoadbuf1 && !onRoadbuf2) {
                            index1 = 4;
                        }
                        else if (!onRoadbuf0 && onRoadbuf1 && !onRoadbuf2) {
                            index1 = 4;
                            onRoadYrot = 90f;
                        }
                        else if (!onRoadbuf0 && !onRoadbuf1 && onRoadbuf2) {
                            index1 = 1;
                            onRoadYrot = 90f;
                        }
                        else if (onRoadbuf0 && onRoadbuf1 && !onRoadbuf2) {
                            index1 = 3;
                            onRoadYrot = 90f;
                        }
                        else if (onRoadbuf0 && !onRoadbuf1 && onRoadbuf2) {
                            index1 = 3;
                        }
                        else if (!onRoadbuf0 && onRoadbuf1 && onRoadbuf2) {
                            index1 = 3;
                            onRoadYrot = 180f;
                        }
                        else if (onRoadbuf0 && onRoadbuf1 && onRoadbuf2) {
                            index1 = 2;
                        }
                        break;
                    case 1:
                        if (onRoadbuf0 && !onRoadbuf1 && !onRoadbuf2) {
                            index1 = 4;
                            onRoadYrot = 270f;
                        }
                        else if (!onRoadbuf0 && onRoadbuf1 && !onRoadbuf2) {
                            index1 = 4;
                            onRoadYrot = 180f;
                        }
                        else if (!onRoadbuf0 && !onRoadbuf1 && onRoadbuf2) {
                            index1 = 1;
                            onRoadYrot = 90f;
                        }
                        else if (onRoadbuf0 && onRoadbuf1 && !onRoadbuf2) {
                            index1 = 3;
                            onRoadYrot = 270f;
                        }
                        else if (onRoadbuf0 && !onRoadbuf1 && onRoadbuf2) {
                            index1 = 3;
                        }
                        else if (!onRoadbuf0 && onRoadbuf1 && onRoadbuf2) {
                            index1 = 3;
                            onRoadYrot = 180f;
                        }
                        else if (onRoadbuf0 && onRoadbuf1 && onRoadbuf2) {
                            index1 = 2;
                        }
                        break;
                    case 2:
                        if (onRoadbuf0 && !onRoadbuf1 && !onRoadbuf2) {
                            index1 = 4;
                            onRoadYrot = 0f;
                        }
                        else if (!onRoadbuf0 && onRoadbuf1 && !onRoadbuf2) {
                            index1 = 4;
                            onRoadYrot = 270f;
                        }
                        else if (!onRoadbuf0 && !onRoadbuf1 && onRoadbuf2) {
                            index1 = 1;
                            onRoadYrot = 0f;
                        }
                        else if (onRoadbuf0 && onRoadbuf1 && !onRoadbuf2) {
                            index1 = 3;
                            onRoadYrot = 0f;
                        }
                        else if (onRoadbuf0 && !onRoadbuf1 && onRoadbuf2) {
                            index1 = 3;
                            onRoadYrot = 90f;
                        }
                        else if (!onRoadbuf0 && onRoadbuf1 && onRoadbuf2) {
                            index1 = 3;
                            onRoadYrot = 270f;
                        }
                        else if (onRoadbuf0 && onRoadbuf1 && onRoadbuf2) {
                            index1 = 2;
                        }
                        break;
                    case 3:
                        if (onRoadbuf0 && !onRoadbuf1 && !onRoadbuf2) {
                            index1 = 4;
                            onRoadYrot = 90f;
                        }
                        else if (!onRoadbuf0 && onRoadbuf1 && !onRoadbuf2) {
                            index1 = 4;
                            onRoadYrot = 180f;
                        }
                        else if (!onRoadbuf0 && !onRoadbuf1 && onRoadbuf2) {
                            index1 = 1;
                            onRoadYrot = 0f;
                        }
                        else if (onRoadbuf0 && onRoadbuf1 && !onRoadbuf2) {
                            index1 = 3;
                            onRoadYrot = 180f;
                        }
                        else if (onRoadbuf0 && !onRoadbuf1 && onRoadbuf2) {
                            index1 = 3;
                            onRoadYrot = 90f;
                        }
                        else if (!onRoadbuf0 && onRoadbuf1 && onRoadbuf2) {
                            index1 = 3;
                            onRoadYrot = 270f;
                        }
                        else if (onRoadbuf0 && onRoadbuf1 && onRoadbuf2) {
                            index1 = 2;
                        }
                        break;
                    default:
                        break;
                }
            }

        }
        if (index1 >= 0) {
            string objName = null;
            foreach (Transform child in spawnedObjsParent.transform) {
                Vector2 cPos = new Vector2(child.transform.position.x, child.transform.position.z);
                if (cPos == targetPos) {
                    Vector3 newObjPos = new Vector3(targetPos.x, 1f, targetPos.y);
                    Vector3 newObjRot = basicRoads[index1].transform.localEulerAngles;
                    //Debug.Log(basicRoads[index1].name);
                    newObjRot.y += onRoadYrot;
                    Destroy(child.gameObject);
                    var onRoadNewObj = (GameObject)Instantiate(basicRoads[index1], newObjPos, Quaternion.Euler(newObjRot)) as GameObject;
                    genObj4 = onRoadNewObj;

                    foreach (Transform otherChild in genObj4.transform) {
                        otherChild.gameObject.AddComponent<TagSystem>();
                        otherChild.gameObject.AddComponent<BoxCollider>();
                        otherChild.gameObject.AddComponent<Outline>();
                    }
                }
                if (cPos == bufferPos1) {
                    //Debug.Log("buf0= " + bufferPos0);
                    int index = 0;
                    float objRot = child.transform.localEulerAngles.y;
                    float newYrot = 0f;
                    foreach (Transform subChild in child.transform) {
                        objName = subChild.GetComponent<TagSystem>().basicType;
                    }
                    if (scen1 == 0) {
                        if (bufferPos0.x == bufferPos1.x + 8f && bufferPos0.y == bufferPos1.y) {
                            index = 1;
                            newYrot = 90f;
                        }
                        else if (bufferPos0.x == bufferPos1.x && bufferPos0.y == bufferPos1.y + 8f) {
                            index = 4;
                            newYrot = 270f;
                        }
                        else if (bufferPos0.x == bufferPos1.x && bufferPos0.y == bufferPos1.y - 8f) {
                            index = 4;
                            newYrot = 180f;
                        }
                    }
                    else if (scen1 == 1) {
                        if (bufferPos0.x == bufferPos1.x - 8f && bufferPos0.y == bufferPos1.y) {
                            index = 1;
                            newYrot = 90f;
                        }
                        else if (bufferPos0.x == bufferPos1.x && bufferPos0.y == bufferPos1.y + 8f) {
                            index = 4;
                            newYrot = 0f;
                        }
                        else if (bufferPos0.x == bufferPos1.x && bufferPos0.y == bufferPos1.y - 8f) {
                            index = 4;
                            newYrot = 90f;
                        }
                    }
                    else if (scen1 == 2) {
                        if (bufferPos0.x == bufferPos1.x + 8f && bufferPos0.y == bufferPos1.y) {
                            index = 4;
                            newYrot = 90f;
                        }
                        else if (bufferPos0.x == bufferPos1.x - 8f && bufferPos0.y == bufferPos1.y) {
                            index = 4;
                            newYrot = 180f;
                        }
                        else if (bufferPos0.x == bufferPos1.x && bufferPos0.y == bufferPos1.y + 8f) {
                            index = 1;
                            newYrot = 0f;
                        }
                    }
                    else if (scen1 == 3) {
                        if (bufferPos0.x == bufferPos1.x + 8f && bufferPos0.y == bufferPos1.y) {
                            index = 4;
                            newYrot = 0f;
                        }
                        else if (bufferPos0.x == bufferPos1.x - 8f && bufferPos0.y == bufferPos1.y) {
                            index = 4;
                            newYrot = 270f;
                        }
                        else if (bufferPos0.x == bufferPos1.x && bufferPos0.y == bufferPos1.y - 8f) {
                            index = 1;
                            newYrot = 0f;
                        }
                    }
                    Destroy(child.gameObject);
                    Vector3 newObjPos = new Vector3(bufferPos1.x, 1f, bufferPos1.y);
                    Vector3 newObjRot = basicRoads[index].transform.localEulerAngles;
                    newObjRot.y += newYrot;
                    var onRoadNewObj = (GameObject)Instantiate(basicRoads[index], newObjPos, Quaternion.Euler(newObjRot)) as GameObject;
                    genObj5 = onRoadNewObj;

                    foreach (Transform otherChild in genObj5.transform) {
                        otherChild.gameObject.AddComponent<TagSystem>();
                        otherChild.gameObject.AddComponent<BoxCollider>();
                        otherChild.gameObject.AddComponent<Outline>();
                    }

                }
            }
            onRoadPassed = true;
            genObj4.transform.parent = spawnedObjsParent.transform;
            genObj5.transform.parent = spawnedObjsParent.transform;
        }
        return index1;
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
