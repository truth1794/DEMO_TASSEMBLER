using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TagEngine;
using outliner;

public class PositionSensor : MonoBehaviour {
    private Camera _camera;
    private int tiles;
    private int startPosX;
    private int startPosZ;
    public Vector3 hitpt, hitpt1, hitpt2;
    public float brutMousX, brutMousY, brutMousZ;
    public float brutMousX1, brutMousY1, brutMousZ1;
    public float brutMousX2, brutMousY2, brutMousZ2;
    public float adjustedMousX, adjustedMousY, adjustedMousZ;
    public float adjustedMousX1, adjustedMousY1, adjustedMousZ1;
    public float adjustedMousX2, adjustedMousZ2;
    public float adjustedMousX3, adjustedMousZ3;
    public Vector3 mousePositionData, mouseReleasedPos, mousePositionData2;
    public bool isCasted = false;
    public bool isSelected = false;
    public bool buttonClicked = false, finalPos = false, rayTouch = false, onTouchGrid = false, roadTouch, onRoad = false, roadTouch2 = false;
    public bool onGrassSuburb = false;
    public GameObject onGrassObj;
    public GameObject guidata;
    GUIscript guidatascript;
    public Ray ray, ray1, ray2;
    public RaycastHit hit, hit1, hit2;
    public RaycastHit[] hits;
    public GameObject buf0;
    public bool guiSquareTouch = false;

    public GameObject objSelect;

    public bool buildModeToggle = false;

    TileControl tileDataScript;
    public GameObject tileData;

    public float genericPTextureScaleValue;
    public Vector2 genericPlaneTextureScale;

    RoadManager2 roadManagerScript;
    public GameObject roadManagerData;

    public GameObject tObject;

    public string[] tileNames = new string[10]
            {
                "road","house","canal","hill",
                "river","water","building","grass",
                "dirt","sand"
            };

    void Start() {
        guidatascript = guidata.GetComponent<GUIscript>();
        tileDataScript = tileData.GetComponent<TileControl>();
        roadManagerScript = roadManagerData.GetComponent<RoadManager2>();
        hitpt = new Vector3(0f, 0f, 0f);
        hitpt1 = new Vector3(0f, 0f, 0f);
        _camera = GetComponent<Camera>();
    }

    void Update() {
        if (guidatascript.button0) {
            buttonClicked = true;
            finalPos = false;
        }
        if (buttonClicked == true) {
            finalPos = false;
            mousePositionData = Input.mousePosition;
            ray = _camera.ScreenPointToRay(mousePositionData);
            if (Physics.Raycast(ray, out hit)) {
                hitpt = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                brutMousX = hitpt.x;
                brutMousY = hitpt.y;
                brutMousZ = hitpt.z;
                adjustedMousX = TilePosX(buildModeToggle, brutMousX);
                adjustedMousY = TilePosX(buildModeToggle, brutMousY);
                adjustedMousZ = TilePosZ(buildModeToggle, brutMousZ);
            }
        }

        if (Input.GetMouseButtonDown(0) && buttonClicked == true && (Physics.Raycast(ray, out hit))) {
            buttonClicked = false;
            guidatascript.button0 = false;
            finalPos = true;
        }

        if(Input.GetMouseButtonDown(0) && guidatascript.tileOptions[0]) {
            guidatascript.tileOptions[0] = false;
        }
        mousePositionData2 = Input.mousePosition;
        ray1 = _camera.ScreenPointToRay(mousePositionData2);
        if (Physics.Raycast(ray1, out hit2)) {
            if (hit2.collider.tag.Equals("Object")) {
                rayTouch = true;
                hitpt2 = new Vector3(hit2.point.x, hit2.point.y, hit2.point.z);
                brutMousX2 = hitpt2.x;
                brutMousY2 = hitpt2.y;
                brutMousZ2 = hitpt2.z;
                adjustedMousX2 = TilePosX(buildModeToggle, brutMousX2);
                adjustedMousZ2 = TilePosZ(buildModeToggle, brutMousZ2);
            }
            else {
                rayTouch = false;
            }
        }
        hits = Physics.RaycastAll(ray1);
        foreach (RaycastHit hitData in hits) {
            if (hitData.collider.tag.Equals(tileDataScript.touchPlaneTag)) {
                hitpt1 = new Vector3(hitData.point.x, hitData.point.y, hitData.point.z);
                brutMousX1 = hitpt1.x;
                brutMousY1 = hitpt1.y;
                brutMousZ1 = hitpt1.z;
                adjustedMousX1 = Tile_positionAdjustor(buildModeToggle, brutMousX1);
                adjustedMousZ1 = Tile_positionAdjustor(buildModeToggle, brutMousZ1);
                onTouchGrid = true;
            }
            else {
                onTouchGrid = false;
            }
            for(int i = 0; i < 10; i++) {
                if (hitData.collider.tag.Equals(tileNames[i])) {
                    tObject = hitData.collider.gameObject;
                    onRoad = true;
                    break;
                }
                else {
                    onRoad = false;
                }
            }
            //if (hitData.collider.tag.Equals("road")) {
                //tObject = hitData.collider.gameObject;
                //onRoad = true;
            //}
            //else {
               //onRoad = false;
            //}
            if (hitData.collider.tag.Equals("Object") && tileDataScript.tileSpawned == true) {
                guiSquareTouch = true;
            }
            else {
                guiSquareTouch = false;
            }
            if(hitData.collider.tag.Equals("grass")) {
                onGrassSuburb = true;
                onGrassObj = hitData.collider.transform.parent.gameObject;
            }
            else {
                onGrassSuburb = false;
            }
        }
        if (Input.GetMouseButtonDown(0) && (onTouchGrid || roadManagerScript.alreadyClicked)) {
            if (!(guidatascript.cantStart)) {
                roadTouch = !roadTouch;
            }
            if(!(guidatascript.onGUI)) {
            guidatascript.menuBarItems[6] = true;
            }
            else {
                guidatascript.menuBarItems[6] = false;
            }

        }
        else {
            guidatascript.menuBarItems[6] = false;
        }

    }

    public float TilePosX(bool buildMode, float mousX) { ///obsolete
        float resultX = 0;
        if (buildMode) {
            float roundX, difX;
            roundX = Mathf.Round(mousX);
            difX = mousX - roundX;
            if (difX > 0) {
                resultX = roundX + 1;
            }
            else if (difX < 0) {
                resultX = roundX;
            }
            else if (difX == 0) {
                resultX = roundX + 1;
            }
        }
        else {
            float bufX = guidatascript.startPosX;
            for(int i = 0; i < guidatascript.nbTiles; i++) {
                if ((bufX - 8) < mousX && mousX < (bufX + 8)) {
                    resultX = bufX;
                }
                else {
                    bufX += 8;
                }
            }
        }
        return resultX;
    }

    public float TilePosZ(bool buildMode, float mousZ) { ///obsolete
        float resultZ = 0;
        if (buildMode) {
            float roundZ, difZ;
            roundZ = Mathf.Round(mousZ);
            difZ = mousZ - roundZ;
            if (difZ > 0) {
                resultZ = roundZ + 1;
            }
            else if (difZ < 0) {
                resultZ = roundZ;
            }
            else if (difZ == 0) {
                resultZ = roundZ + 1;
            }
        }
        else {
            float bufZ = guidatascript.startPosZ;
            for (int i = 0; i < guidatascript.nbTiles; i++) {
                if ((bufZ - 8) < mousZ && mousZ < (bufZ + 8)) {
                    resultZ = bufZ;
                }
                else {
                    bufZ += 8;
                }
            }
        }
        return resultZ;
    }

    public float Tile_positionAdjustor(bool buildMode, float mousPos) {
        float result = 0, roundPos = Mathf.Round(mousPos);
        if (buildMode) {
            float difPos;
            difPos = mousPos - roundPos;
            if (difPos > 0) {
                result = roundPos + 1;
            }
            else if (difPos < 0) {
                result = roundPos;
            }
            else if (difPos == 0) {
                result = roundPos + 1;
            }
        }
        else {
            float factor = mousPos / 8;
            float factorLimit = Mathf.Round(factor);
            float maxFactor = 0, minFactor = 0;
            if(factorLimit - factor > 0) {
                maxFactor = factorLimit * 8;
                minFactor = (factorLimit - 1) * 8;
            }
            else if(factorLimit - factor < 0) {
                minFactor = factorLimit * 8;
                maxFactor = (factorLimit + 1) * 8;
            }
            else if(factorLimit - factor == 0) {
                minFactor = factorLimit * 8;
            }
            result = minFactor + 4;
        }
        return result;
    }
}
