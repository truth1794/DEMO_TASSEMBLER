using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using outliner;
using TagEngine;

 

public class TileControl : MonoBehaviour {
    
    public Object[] tilesModularBuilding;
    public Object[] tilesLandscape;
    public string tilesMBFolder, tilesLSFolder;
    public GameObject buffer0;

    static GameObject touchPlaneObj = null;
    public GameObject touchPlaneParent;
    public string touchPlaneTag;
    public float touchPlaneHeight;

    public bool tileSpawned = false;
    public bool finalClick = false;
    public bool alreadySpawned = false;
    public bool selectedObj = false;
    public bool mouseOverTile = false;
    public bool toto = false;

    GUIscript guiData;
    public GameObject data0;

    PositionSensor posData;
    public GameObject data1;

    CameraController camData;
    public GameObject data2;

    Exporter exportDataScript;
    public GameObject exportData;

    public GameObject tileCenter;
    Ray ray0;

    public string objectsTag;
    public float sphereRad;
    public float sphereDist;
    public int rotIt;
    public float tileXSize, tileYSize, tileZSize;
    public List<Vector3> proxTilePos;
    public Vector2 previousPos;
    public Vector2 bufferPos;
    public bool posChanged = false;
    public int nbPos;
    public Vector3 bufferP;
    public bool selectedDefault = false;
    public Vector2[] proxPosition;
    float tilePosBuffer;
    public Vector2 tileBuffer;
    public bool spawned = false, onGras = false;

    Ray verticalRay;


    public GameObject bufGenTilesParent;
    public GameObject generatedTiles;

    public GameObject tileBufferObj;
    public GameObject genericPlane;


    private void Awake() {
        tilesMBFolder = "T5";
        tilesLSFolder = "landscape";
        tilesModularBuilding = Tile_loader(tilesMBFolder);
        tilesLandscape = Tile_loader(tilesLSFolder);
        touchPlaneTag = "touchPlane";
    }

    private void Start() {
        tilePosBuffer = 0;
        proxPosition = new Vector2[5];
        proxTilePos = new List<Vector3>();
        nbPos = 0;
        rotIt = 0;
        sphereRad = 1f;
        sphereDist = 1f;
        guiData = data0.GetComponent<GUIscript>();
        posData = data1.GetComponent<PositionSensor>();
        camData = data2.GetComponent<CameraController>();
        exportDataScript = exportData.GetComponent<Exporter>();
        touchPlaneParent = new GameObject("touchPlaneParent");
        objectsTag = "modulartile";
        genericPlane.GetComponent<Transform>().tag = touchPlaneTag;
    }

    private void Update() {
        if (guiData.button1) {
            Tile_spawner(guiData.index);
        }
        if ((Input.GetKeyDown(KeyCode.Escape) || guiData.tileOptions[4]) && (posData.buttonClicked || selectedObj)) {
            posData.buttonClicked = false;
            guiData.button0 = false;
            Destroy(buffer0);
            guiData.tileOptions[4] = false;
            selectedObj = false;
        }
        if ((Input.GetKeyDown(KeyCode.R) || guiData.tileOptions[1] || guiData.tileOptions[2]) && (posData.buttonClicked || selectedObj)) {
            Tile_rotator(buffer0, selectedObj, guiData.buildMode, guiData.tileOptions[1], guiData.tileOptions[2], Input.GetKeyDown(KeyCode.R));
            guiData.tileOptions[1] = false;
            guiData.tileOptions[2] = false;
        }
        if (posData.buttonClicked) {
            if (guiData.buildMode) {
                foreach(Transform child in buffer0.transform) {
                    child.GetComponent<Outline>().mousDown = true;
                }
            }
            Tile_spawnMover(buffer0, selectedDefault, posData.buildModeToggle);
        }
        if (posData.finalPos) {
            foreach(Transform child in buffer0.transform) {
                child.gameObject.AddComponent<BoxCollider>();
            }
            buffer0.transform.parent = generatedTiles.transform;
            posData.finalPos = false;
        }
        if (toto) {
            Tile_neighbours(buffer0);
            if (/*posData.buildModeToggle && */!alreadySpawned) {
                foreach (Transform child in buffer0.transform) {
                    child.gameObject.AddComponent<BoxCollider>();
                }
            }
            if (tileSpawned == false) {
                tileSpawned = true;
            }

            posData.finalPos = false;
            bufferPos = new Vector2();
            posChanged = false;

        }
        if (exportDataScript.exporting) {
            foreach (Transform child in bufGenTilesParent.transform) {
                child.transform.parent = null;
                child.transform.parent = generatedTiles.transform;
            }
        }

        if (selectedObj) {
            if (guiData.tileOptions[0]) {
                Tile_spawnMover(buffer0, selectedObj, posData.buildModeToggle);
            }
        }
    }

    private Object[] Tile_loader(string path) {
        Object[] objArray;
        var tileArrayFunc = Resources.LoadAll(path, typeof(GameObject));
        objArray = new Object[tileArrayFunc.Length];
        for (int i = 0; i < tileArrayFunc.Length; i++) {
            objArray[i] = tileArrayFunc[i];
        }
        return objArray;
    }
    
    private void TouchPlane_gen() {
        touchPlaneObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        touchPlaneObj.transform.parent = touchPlaneParent.transform;
        touchPlaneObj.GetComponentInChildren<Transform>().tag = touchPlaneTag;
        touchPlaneParent.GetComponent<Transform>().tag = touchPlaneTag;
        touchPlaneObj.GetComponent<MeshRenderer>().enabled = false;
        touchPlaneObj.GetComponent<Transform>().localScale = new Vector3(1000f, 1000f, 1000f);
    }

    private void Tile_spawner(int index) {
        foreach(Transform child in generatedTiles.transform) {
            foreach(Transform subChild in child.transform) {
                if (!(subChild.GetComponent<Outline>())) {
                    Debug.Log(child.name);
                    subChild.GetComponent<Outline>().selected = false;
                    subChild.GetComponent<Outline>().mousDown = false;
                }
            }
        }
        touchPlaneHeight = genericPlane.transform.position.y;
        float targetHeight = 0f;
        Object[] tileMode;
        if (posData.buildModeToggle) {
            tileMode = tilesModularBuilding;
        }
        else {
            tileMode = tilesLandscape;
        }
        int chilPres = 0;
        GameObject bufferObject = (GameObject)tileMode[index];
        float xAng = bufferObject.transform.localEulerAngles.x;
        float yAng = bufferObject.transform.localEulerAngles.y;
        float zAng = bufferObject.transform.localEulerAngles.z;
        bufferObject = null;
        var insta = (GameObject)Instantiate(tileMode[index]);
        /*int l = 0;
        if (guiData.buildMode) {
            int k = 0;
            MeshFilter[] meshF = insta.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] comb = new CombineInstance[meshF.Length];
            while (k < meshF.Length){
                comb[k].mesh = meshF[k].sharedMesh;
                comb[k].transform = meshF[k].transform.localToWorldMatrix;
                meshF[k].gameObject.SetActive(false);
                k++;
            }
            foreach(Transform child in insta.transform) {
                if(l == 0) {
                    child.transform.GetComponent<MeshFilter>().mesh = new Mesh();
                    child.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(comb, false);
                    child.transform.gameObject.SetActive(true);
                }
                l++;
            }
        }*/
        foreach (Transform child in insta.transform) {
            if(child.name == "housePos" || child.name == "path_tilesLong" || child.name == "path_tilesShort") {
                
            }
            else {
                child.gameObject.AddComponent<Outline>();
                child.GetComponent<Outline>().color = 0;
                if (guiData.cityMode) {
                    child.gameObject.AddComponent<TagSystem>();
                }
                if (guiData.buildMode) {
                    child.tag = objectsTag;
                }
                chilPres += 1;
            }
        }
        if (chilPres == 0) {
            insta.gameObject.AddComponent<Outline>();
            insta.GetComponent<Outline>().color = 0;
        }
        buffer0 = insta;
        if (guiData.cityMode) {
            targetHeight = touchPlaneHeight;
        }
        else if (guiData.buildMode) {
            targetHeight = Tile_height(buffer0, guiData.buildMode);
        }
        
        buffer0.transform.position = new Vector3(posData.brutMousX + 1, targetHeight, posData.brutMousZ + 1);
        buffer0.transform.localEulerAngles = new Vector3(xAng, yAng, zAng);
        guiData.button1 = false;
    }

    private void Tile_Buffer(int index) {
        Object[] tileMode;
        if (posData.buildModeToggle) {
            tileMode = tilesModularBuilding;
        }
        else {
            tileMode = tilesLandscape;
        }
        tileBufferObj = (GameObject)tileMode[index];
    }

    //mode: true: Road builder false: All tiles builder
    //buildMode: true: modular builder false: landscape builder
    private void Tile_moverV2(bool buildMode, bool mode) {
        if (buildMode) {

        }
        else {

        }
        if (mode) {

        }
    }

    private void Tile_spawnMover(GameObject obj, bool selected, bool mode) {
        if (!(camData.cameraRotating)) {
            if (obj) {
                if (!selected) {
                    if (mode) {
                        bufferPos = new Vector2(obj.transform.position.x, obj.transform.position.z);
                        obj.transform.position = new Vector3(
                                (posData.adjustedMousX1),
                                Tile_height(buffer0, mode),
                                (posData.adjustedMousZ1));

                        Vector2 currentPos = new Vector2(obj.transform.position.x, obj.transform.position.z);
                        if (currentPos != bufferPos) {
                            previousPos = bufferPos;
                        }
                        else {
                            previousPos = currentPos;
                        }
                    }
                    else {
                        foreach (Transform ch in obj.transform) {
                            //Debug.Log(ch.gameObject.tag);
                            if (ch.gameObject.tag == "house") {
                                //Debug.Log("0");
                                if (posData.onGrassSuburb) {
                                    //Debug.Log("1");
                                    //Debug.Log(posData.onGrassObj.name);
                                    foreach (Transform targetTrans in posData.onGrassObj.transform) {
                                        //Debug.Log(targetTrans.gameObject.name);
                                        if (targetTrans.gameObject.name == "housePos") {
                                            onGras = true;
                                            //Debug.Log("onT");
                                            //Vector3 tPos = new Vector3(targetTrans.transform.position.x, targetTrans.transform.position.y, targetTrans.transform.position.z);
                                            ch.gameObject.GetComponent<Outline>().color = 1;
                                            obj.transform.position = targetTrans.transform.position;
                                            //Debug.Log(ch.gameObject.GetComponent<Outline>().color);
                                        }
                                    }
                                    break;
                                }
                                else {
                                    spawned = true;
                                    onGras = false;
                                    ch.gameObject.GetComponent<Outline>().color = 2;
                                    //Debug.Log(ch.gameObject.GetComponent<Outline>().color);
                                    obj.transform.position = new Vector3(
                                    (posData.adjustedMousX1),
                                    (guiData.gridParent.transform.position.y + 1f),
                                    (posData.adjustedMousZ1));
                                    break;
                                }
                            }
                            else {
                                spawned = false;
                                onGras = false;
                                obj.transform.position = new Vector3(
                                    (posData.adjustedMousX1),
                                    (guiData.gridParent.transform.position.y + 1f),
                                    (posData.adjustedMousZ1));
                                if (!(ch.gameObject.GetComponent<Outline>() == null)) {
                                    if (posData.onRoad) {
                                        /*foreach (Transform otherObj in generatedTiles.transform) {
                                            Vector3 worldPos = transform.TransformPoint(otherObj.transform.position);
                                            Debug.Log("objPos= " + obj.transform.position);
                                            Debug.Log("GenPos= " + otherObj.transform.position);
                                            if (obj.transform.position == worldPos) {
                                                ch.gameObject.GetComponent<Outline>().color = 2;
                                                break;
                                            }
                                        }*/
                                        if (obj.transform.position == posData.tObject.transform.position) {
                                            ch.gameObject.GetComponent<Outline>().color = 2;
                                            break;
                                        }
                                    }
                                    else {
                                        ch.gameObject.GetComponent<Outline>().color = 1;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (selected) {
                    if (mode) {
                        bufferPos = new Vector2(obj.transform.position.x, obj.transform.position.z);
                        obj.transform.position = new Vector3(
                                (posData.adjustedMousX1),
                                Tile_height(buffer0, mode),
                                (posData.adjustedMousZ1));
                        //Debug.Log(Tile_height(buffer0));
                        Vector2 currentPos = new Vector2(obj.transform.position.x, obj.transform.position.z);
                        if (currentPos != bufferPos) {
                            previousPos = bufferPos;
                        }
                        else {
                            previousPos = currentPos;
                        }
                    }
                    else {
                        obj.transform.parent.position = new Vector3(
                                (posData.adjustedMousX1),
                                (guiData.gridParent.transform.position.y + 1f),
                                (posData.adjustedMousZ1));
                    }
                    /*if (mode) {
                        bufferPos = new Vector2(obj.transform.position.x, obj.transform.position.z);
                        obj.transform.position = new Vector3(
                                (posData.adjustedMousX1 - 0.5f),
                                Tile_height(buffer0),
                                (posData.adjustedMousZ1 - 0.5f));
                        Vector2 currentPos = new Vector2(obj.transform.position.x, obj.transform.position.z);
                        if (currentPos != bufferPos) {
                            previousPos = bufferPos;
                        }
                        else {
                            previousPos = currentPos;
                        }
                        obj.transform.position = new Vector3(
                                (posData.adjustedMousX1 - 0.5f),
                                (guiData.gridParent.transform.position.y),
                                (posData.adjustedMousZ1 - 0.5f));
                    }
                    else {
                        obj.transform.position = new Vector3(
                              (posData.adjustedMousX1),
                              (guiData.gridParent.transform.position.y + 1f),
                              (posData.adjustedMousZ1));
                    }*/
                }
                //Debug.Log(obj.transform.position);
            }
        }
        
    }

    private void Tile_rotator(GameObject obj, bool selected, bool mode, bool rotateLeft, bool rotateRight, bool keyPressed) {
        float rotationDirection = 0f;
        if (rotateLeft) {
            rotationDirection = -90f;
        }
        if (rotateRight || keyPressed) {
            rotationDirection = 90f;
        }
        if (selected) {
            if (mode) {
                obj.transform.Rotate(0f, rotationDirection, 0f, Space.World);
            }
            else {
                obj.transform.parent.Rotate(0f, rotationDirection, 0f, Space.World);
            }
        }
        else {
            obj.transform.Rotate(0f, rotationDirection, 0f, Space.World);
        }
    }

    private float Tile_height(GameObject tileObj, bool mode) {
        float heightTile = 0;
        RaycastHit mousHit;
        RaycastHit[] tilesColumn;
        Vector3 rayStart;
        Ray ray = camData._camera.ScreenPointToRay(Input.mousePosition);
        float ySize = 0f, ySize2 = 0f;
        float tileSize = 0f;
        float bottomTileYpos = 0f;
        tileSize = tileObj.GetComponentInChildren<Renderer>().bounds.size.y;
        Debug.Log(tileSize);
        bottomTileYpos = genericPlane.transform.position.y;
        ySize = tileObj.GetComponentInChildren<Renderer>().bounds.size.y / 2;
        ySize2 = (tileObj.GetComponentInChildren<Renderer>().bounds.size.y / 2) - ySize;
        int it = 0;
        string tagBuffer = null;
        if (mode) {
            //Debug.Log("buidmode");
            foreach(Transform child in tileObj.transform) {
                tagBuffer = child.tag;
                child.tag = "buffertag";
                //Debug.Log(child.tag);
            }
        }
        if (Physics.Raycast(ray, out mousHit)) {
            
            if (mousHit.collider.gameObject.tag.Equals(objectsTag)) {
                //Debug.Log(mousHit.collider.gameObject.tag);
                rayStart = new Vector3(tileObj.transform.position.x,
                    -1f, tileObj.transform.position.z);
                tilesColumn = Physics.RaycastAll(rayStart, Vector3.up);
                heightTile = ySize2;
                foreach (RaycastHit hits in tilesColumn) {
                    if (hits.collider.gameObject.tag.Equals(objectsTag)) {
                        it += 1;
                        float adjustedHeight = 0f;
                        adjustedHeight = hits.collider.gameObject.GetComponent<BoxCollider>().bounds.size.y;
                        heightTile += adjustedHeight;
                        //Debug.Log()
                    }
                }
            }
            else {
                heightTile = bottomTileYpos;
            }
        }
        if (mode) {
            foreach (Transform child in tileObj.transform) {
                child.tag = tagBuffer;
                //tileObj.tag = tagBuffer;
            }
        }
        return heightTile;
    }
    
    private void Tile_neighbours(GameObject obj) {
        RaycastHit[] hitArray;
        List<RaycastHit[]> hitList = new List<RaycastHit[]>();
        Vector3[] rayOrigins = new Vector3[9];
        Vector2[,] neighbourPos = new Vector2[3,3];
        float xCursor = 0, zCursor = 0;
        int it = 0;
        for (int x = 0; x < 3; x++) {
            for (int z = 0; z < 3; z++) {
                if (x == 0) {
                    xCursor = -8;
                }
                else if(x == 1) {
                    xCursor = 0;
                }
                else if(x == 2) {
                    xCursor = 8;
                }
                if(z == 0) {
                    zCursor = -8;
                }
                else if(z == 1) {
                    zCursor = 0;
                }
                else if(z == 2) {
                    zCursor = 8;
                }
                neighbourPos[x, z] = 
                    new Vector2(
                        obj.transform.position.x + (xCursor),
                        obj.transform.position.z + (zCursor));
                rayOrigins[it] = new Vector3(neighbourPos[x, z].x, -1f, neighbourPos[x, z].y);
                hitArray = Physics.RaycastAll(rayOrigins[it], Vector3.up, 3.0f);
                hitList.Add(hitArray);
                foreach (RaycastHit hits in hitList[it]) {
                    if(hits.collider.gameObject.tag == objectsTag) {
                        Debug.Log(hits.collider.gameObject.name + " " + hits.collider.gameObject.transform.position);
                    }
                }
                it += 1;
            }
        }
    }
}
