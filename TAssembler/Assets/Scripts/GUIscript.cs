using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using outliner;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Outline = outliner.Outline;
using UnityEngine.SceneManagement;

public class GUIscript : MonoBehaviour {

    public Texture2D[] tilesMBTexture, tilesLSTexture;
    public Texture2D[] textureLS, textureMB;
    public Texture2D[] uiIcons;
    public string tilesMBIconFolder, tilesLSIconFolder;
    public bool tileSelected = false, button0 = false, button1 = false, button2 = false;
    public int iter = 0, tilesMBSize, tilesLSSize, index, coef0;
    private Rect previewImage, prev2, SaveButton;
    //public RawImage 
    private Rect[] landscapeIconRects;
    private Rect[] modularBuildIconRects;
    private Rect[] menuIconRects;
    private Rect[] toolsIconRects;
    
    private Rect prevButton, nextButton;
    public GUIContent[] prevNextButtons;

    private List<Rect[]> extMenuRects;
    private Rect[] menu0ItemsRect, menu1ItemsRect, menu2ItemsRect;
    private int screenHeight, screenHeightBuffer;
    private int screenWidth, screenWidthBuffer;
    public bool tileTexture = false;
    public Texture2D exportButtonTexture;
    public GameObject expButton;
    public Rect sliderRect;
    public Rect toolsBackgroundPanel;
    public Rect warningMessagePanel;
    public Rect warningSign;
    public Rect menuBarPanel;
    public Rect tilesBarPanel;

    private Rect[] tileOptionsRects;
    public Rect tileOptionsPanel;
    public GUIStyle customTileOptionsBar, customTileOptionsButtons;
    float[] tileOptionsXpos;
    float tileOptionsYpos, tileOptionsPanelXpos, tileOptionsPanelYpos;
    public bool[] tileOptions = new bool[5];

    public Rect settingsPanelRect;
    public GUIStyle customSettingsPanelStyle;

    public Rect settingsCrossRect;
    public GUIStyle customSettingsCrossStyle;

    public Texture2D[] warningImgs;
    private float rectMoveSpeed;

    public GameObject tileInsData;
    TileControl tileData;

    public GameObject positionSensData;
    PositionSensor posData;

    public GameObject cameraData;
    CameraController camData;

    public GameObject roadManagerScript;
    RoadManager2 roadManagerData;

    public GameObject exporterScript;
    Exporter exporterData;

    public bool overUI = false;
    public int coef1;
    public Rect[] rectsObj;

    public Texture2D squareBaseTexture, squareSelectTexture;
    static GameObject squareBaseObject = null, squareSelectObject = null;
    public bool meshRend = true;
    public GameObject[] squareBuffer;
    public bool switch0 = false;
    public float tilePosX, tilePosZ;
    public GameObject gridParent;
    public GameObject squaresParent;
    public GameObject selectSquareParent;
    public bool gridToggle = false;
    public float tileSize;
    public float startPosX;
    public float startPosZ;
    public int nbTiles;
    public bool gameMode = false;
    public string[] fileButtonList = new string[4] {"New","Load","Save","Exit"};
    public GameObject buttonPrefab;
    public GameObject canvasButton;
    public float buttonSpeedMove;
    public bool uPressed = false, buttonHidden = false, trigger0 = false;
    float bPos = 0f;
    float xPosStartTiles, xPosStartTools;
    float[] xPosRestTiles = new float[10], yPosIconRectTiles = new float[3];
    float yPosStartTiles, yPosStartTools, yPosStartTools2, menuItemsYpos;
    float warningPanelxPos, warningPanelyPos;
    public GUIStyle customToolsBarStyle, customMenuItems, customMenuBar, customMenuBar2, warningMsg,
        warningImgSt, customTilesBarPanel, customTileIcons, customPrevIcon, customNextIcon, customToolTipPanel;
    
    public bool roadMode = false, buildMode = false, cityMode = false, cantStart = false, onGUI = false;
    public bool[] menuBarItems, menuOpenedItems, modesEnabled;
    private bool modeEnabled = false, resChanged = false;
    public int indexBuffer;
    private float resMoveAccel = 0f;
    private bool settingsOpened = false;

    public Vector3 prevTilePos;
    public bool outlined = false;

    private void Awake() {
        tilesMBIconFolder = "T5Icons";
        tilesLSIconFolder = "LandscapeIcons";
    }

    private void Start() {
        screenHeight = Screen.height;
        screenWidth = Screen.width;
        xPosStartTiles = (screenWidth - 1000f) / 2f;
        xPosRestTiles[0] = (screenWidth - 1000f) / 2f;
        for (int i = 1; i < 10; i++) {
            xPosRestTiles[i] = xPosRestTiles[i - 1] + 100f;
        }
        yPosIconRectTiles[0] = (screenHeight - 300f) / 2f;
        for(int i = 1; i < 3; i++) {
            yPosIconRectTiles[i] = yPosIconRectTiles[i - 1] + 100f;
        }
        tileOptionsXpos = new float[5];
        tileOptionsXpos[0] = ((screenWidth - 125f) / 2f) - 50f;
        for(int i = 1; i < 5; i++) {
            tileOptionsXpos[i] = tileOptionsXpos[i - 1] + 40f;
        }
        tileOptionsYpos = screenHeight - 130f;
        tileOptionsPanelXpos = tileOptionsXpos[0] - 10f;
        tileOptionsPanelYpos = screenHeight - 130f;
        yPosStartTiles = screenHeight + 5f;
        yPosStartTools = (screenHeight * 4f) / 1080f;
        yPosStartTools2 = (screenHeight - 300f) / 2f;
        warningPanelxPos = (screenWidth - 600f) / 2f;
        warningPanelyPos = (screenHeight - 125f) / 2f;
        menuItemsYpos = -5f;
        buttonSpeedMove = 300f;
        indexBuffer = -1;
        menuBarItems = new bool[7];
        menuOpenedItems = new bool[7];
        camData = cameraData.GetComponent<CameraController>();
        tileData = tileInsData.GetComponent<TileControl>();
        posData = positionSensData.GetComponent<PositionSensor>();
        roadManagerData = roadManagerScript.GetComponent<RoadManager2>();
        exporterData = exporterScript.GetComponent<Exporter>();
        nbTiles = 10;
        startPosX = -14f;
        startPosZ = -14f;
        tileSize = 8f;
        coef0 = 1;
        coef1 = 0;
        gridParent = new GameObject("GRID");
        squaresParent = new GameObject("GeneratedItems");
        selectSquareParent = new GameObject("selectSquare");
        squareBuffer = new GameObject[1];
        rectsObj = new Rect[5];
        rectMoveSpeed = 0f;
        tilesMBSize = tileData.tilesModularBuilding.Length;
        tilesLSSize = tileData.tilesLandscape.Length;
        //AssetPreview.SetPreviewTextureCacheSize(tilesMBSize + 2);
        tilesMBTexture = new Texture2D[tilesMBSize];
        tilesLSTexture = new Texture2D[tilesLSSize];
        textureLS = new Texture2D[tilesLSSize];
        textureMB = new Texture2D[tilesMBSize];
        tilesMBTexture = Texture_loader(tilesMBIconFolder);
        tilesLSTexture = Texture_loader(tilesLSIconFolder);
        previewImage = new Rect(0, 0, 100, 100);
        landscapeIconRects = new Rect[10];
        for(int i = 0; i < 10; i++) {
            landscapeIconRects[i] = new Rect(xPosRestTiles[i], yPosStartTiles, 100f, 100f);
        }
        modularBuildIconRects = new Rect[10];
        for (int i = 0; i < 10; i++) {
            modularBuildIconRects[i] = new Rect(xPosRestTiles[i], yPosStartTiles, 100f, 100f);
        }
        tilesBarPanel = new Rect(xPosRestTiles[0] - 120f, yPosStartTiles, 1240f, 120f);

        prevButton = new Rect(xPosRestTiles[0] - 100f, yPosStartTiles, 100f, 100f);
        nextButton = new Rect(landscapeIconRects[9].position.x + 100f, yPosStartTiles, 100f, 100f);
        prevNextButtons = new GUIContent[4];
        prevNextButtons[0] = new GUIContent(uiIcons[3]);
        prevNextButtons[1] = new GUIContent(uiIcons[4]);
        prevNextButtons[2] = new GUIContent(uiIcons[3]);
        prevNextButtons[3] = new GUIContent(uiIcons[11]);

        menuIconRects = new Rect[3];
        for (int i = 0; i < 3; i++) {
            menuIconRects[i] = new Rect(i * 100f, 0f, 100f, 25f);
        }
        menuBarPanel = new Rect(300f, 0f, screenWidth, 25f);
        toolsIconRects = new Rect[3];
        for (int i = 0; i < 3; i++) {
            toolsIconRects[i] = new Rect(10.56f, yPosIconRectTiles[i], 100f, 100f);
        }
        //extMenuRects = new List<Rect[]>();
        /*for(int i = 0; i < 3; i ++) {
            if(i == 0) {
                extMenuRects.Add(new Rect[4]);
                extMenuRects[0].SetValue(new Rect(0f, menuItemsYpos, 100f, 20f), 0);
                extMenuRects[0].SetValue(new Rect(0f, menuItemsYpos, 100f, 20f), 1);
                extMenuRects[0].SetValue(new Rect(0f, menuItemsYpos, 100f, 20f), 2);
                extMenuRects[0].SetValue(new Rect(0f, menuItemsYpos, 100f, 20f), 3);
            } // 25, 45, 65, 85
        }*/
        menu0ItemsRect = new Rect[4];
        menu1ItemsRect = new Rect[2];
        menu2ItemsRect = new Rect[2];
        for (int i = 0; i < 4; i++) {
            menu0ItemsRect[i] = new Rect(0f, -20f, 100f, 20f);
            if(i < 2) {
                menu1ItemsRect[i] = new Rect(100f, -20f, 100f, 20f);
                menu2ItemsRect[i] = new Rect(200f, -20f, 100f, 20f);
            }
        }
        tileOptionsRects = new Rect[5];
        for(int i = 0; i < 5; i++) {
            tileOptionsRects[i] = new Rect(tileOptionsXpos[i], tileOptionsYpos, 30f, 30f);
        }
        tileOptionsPanel = new Rect(tileOptionsPanelXpos, tileOptionsPanelYpos, 210f, 35f);
        toolsBackgroundPanel = new Rect(0f, yPosIconRectTiles[0] - 15f, 120f, 340f);
        warningMessagePanel = new Rect(warningPanelxPos, warningPanelyPos, 600f, 125f);
        warningSign = new Rect(warningPanelxPos + 32f, warningPanelyPos + 22.5f, 75f, 75f);
        settingsPanelRect = new Rect((screenWidth - 384f) / 2, (screenHeight - 384f) / 2, 384f, 384f);
        settingsCrossRect = new Rect(settingsPanelRect.x + 340f, settingsPanelRect.y + 4f, 20f, 20f);
        sliderRect = new Rect(110f, 10f, 25f, 500f);
        //squareBaseObject = Gen_squareobject(Gen_squaremat(squareBaseTexture),0.9f);
        //squareSelectObject = Gen_squareobject(Gen_squaremat(squareSelectTexture),0.9f);
        //GUI_selectsquare(squareSelectObject);
        //squareSelectObject.SetActive(true);
        //squareSelectObject.transform.parent = selectSquareParent.transform;
        //selectSquareParent.SetActive(false);
        gameMode = posData.buildModeToggle;
        for (int i = 0; i < tilesMBSize; i++) {
            textureMB[i] = (Texture2D)tilesMBTexture[i];
        }
        for (int i = 0; i < tilesLSSize; i++) {
            textureLS[i] = (Texture2D)tilesLSTexture[i];
        }
        modesEnabled = new bool[2];
    }



    private void Update() {
        screenHeightBuffer = Screen.height;
        screenWidthBuffer = Screen.width;
        gridParent.transform.position = camData.camFocusCenter;
        if(screenHeight != screenHeightBuffer || screenWidth != screenWidthBuffer) {
            resChanged = true;
#if !UNITY_EDITOR
    if (Screen.width <= 1280 || Screen.height <= 720){
        Screen.SetResolution(1280, 720, false);
    }
#endif
            screenHeight = Screen.height;
            screenWidth = Screen.width;
            xPosStartTiles = (screenWidth - 1000f) / 2f;
            xPosRestTiles[0] = (screenWidth - 1000f) / 2f;
            yPosIconRectTiles[0] = (screenHeight - 300f) / 2f;
            yPosStartTiles = screenHeight + 5f;
            yPosStartTools = (screenHeight * 4f) / 1080f;
            warningPanelxPos = (screenWidth - 600f) / 2f;
            warningPanelyPos = (screenHeight - 125f) / 2f;
            for(int i = 1; i < 10; i++) {
                xPosRestTiles[i] = xPosRestTiles[i - 1] + 100f;
                if(i < 3) {
                    yPosIconRectTiles[i] = yPosIconRectTiles[i - 1] + 100f;
                }
            }
            for(int i = 0; i < 10; i++) {
                if(i < 3) {
                    toolsIconRects[i].y = yPosIconRectTiles[i];
                }
                landscapeIconRects[i].x = xPosRestTiles[i];
                modularBuildIconRects[i].x = xPosRestTiles[i];
                if (cityMode) {
                    landscapeIconRects[i].y = screenHeight - 100f;
                }
                else {
                    landscapeIconRects[i].y = yPosStartTiles;
                }
                if (buildMode) {
                    modularBuildIconRects[i].y = screenHeight - 100f;
                }
                else {
                    modularBuildIconRects[i].y = yPosStartTiles;
                }
                
            }
            toolsBackgroundPanel.y = yPosIconRectTiles[0] - 15f;
        }
        else {
            resChanged = false;
        }
        if (coef1 > 0) {
            if ((Input.GetAxis("Mouse ScrollWheel") > 0)) {
                if (iter > 0) {
                    iter -= 1;
                }
            }
            else if ((Input.GetAxis("Mouse ScrollWheel")) < 0) {
                int arrSize = 0;
                if (cityMode) {
                    arrSize = tilesLSSize;
                }
                else if (buildMode) {
                    arrSize = tilesMBSize;
                }
                if (iter < arrSize - 10) {
                    iter += 1;
                }
                //Debug.Log(iter);
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if(iter < tilesMBSize - 5) {
                iter += 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (iter > 0) {
                iter -= 1;
            }
        }

        /*if (posData.guiSquareTouch) {
            //Debug.Log("guiS0 " + tileData.tileSpawned);
            //var tileIndex = tileData.Tile_positionSearch(posData.adjustedMousX2, posData.adjustedMousZ2);
            /*Debug.Log(tileIndex);
            Debug.Log(posData.adjustedMousX2);
            Debug.Log(posData.adjustedMousZ2);
            Debug.Log(tileData.tileAdress[0][2]);
            selectSquareParent.transform.position = new Vector3((
                posData.adjustedMousX2),
                (posData.hit2.transform.gameObject.transform.position.y + 0.01f),
                (posData.adjustedMousZ2));
        }
        else {
            if (tileData.buffer0) {
                if (posData.buildModeToggle) {
                    selectSquareParent.transform.position = new Vector3((
                    posData.adjustedMousX1),
                    (posData.hit2.transform.gameObject.transform.position.y + 0.01f),
                    (posData.adjustedMousZ1));
                }
                else {
                    selectSquareParent.transform.position = new Vector3((
                    posData.adjustedMousX1),
                    (posData.hit2.transform.gameObject.transform.position.y - 0.5f),
                    (posData.adjustedMousZ1));
                }
            }
        }*/
            /*if (posData.hit2.collider.tag.Equals("Object")) {
                GUI_outline();
            }*/
            /*tilePosX = posData.adjustedMousX1;
            tilePosZ = posData.adjustedMousZ1;
            if (tilePosX != posData.adjustedMousX1 || tilePosZ != posData.adjustedMousZ) {
               
            }
            else if(tilePosX == posData.adjustedMousX1 && tilePosZ == posData.adjustedMousZ1) {

            }*/
        if (Input.GetKeyDown(KeyCode.G)) {
            gridParent.SetActive(!gridToggle);
        }
        if (resChanged) {
            resMoveAccel = 100f;
        }
        else {
            resMoveAccel = 1f;
        }
        rectMoveSpeed = ((buttonSpeedMove * resMoveAccel) * 1.9f) * Time.deltaTime;
        //Debug.Log(modeEnabled + " " + (!cityMode && !buildMode));
        if(modesEnabled[0] || modesEnabled[1]) {
            modeEnabled = true;
        }
        else if(!modesEnabled[0] && !modesEnabled[1]) {
            modeEnabled = false;
        }
        if (modeEnabled) {
            prevButton.position = Vector2.MoveTowards(prevButton.position, new Vector2(xPosRestTiles[0] - 100f, screenHeight - 100f), rectMoveSpeed);
            nextButton.position = Vector2.MoveTowards(nextButton.position, new Vector2(xPosRestTiles[9] + 100f, screenHeight - 100f), rectMoveSpeed);
            tilesBarPanel.position = Vector2.MoveTowards(tilesBarPanel.position, new Vector2(xPosRestTiles[0] - 120f, screenHeight - 100f), rectMoveSpeed);
        }
        
        else if (!modeEnabled){
            prevButton.position = Vector2.MoveTowards(prevButton.position, new Vector2(xPosRestTiles[0] - 100f, screenHeight + 5f), rectMoveSpeed);
            nextButton.position = Vector2.MoveTowards(nextButton.position, new Vector2(xPosRestTiles[9] + 100f, screenHeight + 5f), rectMoveSpeed);
            tilesBarPanel.position = Vector2.MoveTowards(tilesBarPanel.position, new Vector2(xPosRestTiles[0] - 120f, screenHeight - 5f), rectMoveSpeed);
            //modeEnabled = false;
        }
    }

    private void OnGUI() {
        menuBarPanel.width = screenWidth;
        GUI.Box(menuBarPanel,"", customMenuBar2);
        GUI.Box(toolsBackgroundPanel,"", customToolsBarStyle);
        GUI.Box(tilesBarPanel, "", customTilesBarPanel);
        if (tileData.selectedObj) {
            GUI.Box(tileOptionsPanel, "", customTileOptionsBar);
        }
        if (settingsOpened) {
            GUI.Box(settingsPanelRect, "Settings", customSettingsPanelStyle);
            if (GUI.Button(settingsCrossRect, uiIcons[5], customSettingsCrossStyle)) {
                settingsOpened = false;
            }
        }
        Vector2 mousePosEvent = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y);

        for (int i = 0; i < 10; i++) {
            if (landscapeIconRects[i].Contains(mousePosEvent) || modularBuildIconRects[i].Contains(mousePosEvent)) {
                coef1 = 4;
            }

            /*if (!(landscapeIconRects[i].Contains(mousePosEvent))) {
                coef1 -= 1;
                if(coef1 < 0) {
                    coef1 = 0;
                }
            }*/
            /*if(i < 3) {
                if (menuIconRects[i].Contains(mousePosEvent)) {
                    coef1 = 4;
                }
                if (!(menuIconRects[i].Contains(mousePosEvent))) {
                    coef1 -= 1;
                    if (coef1 < 0) {
                        coef1 = 0;
                    }
                }
            }
            if(i < 4) {
                if (menu0ItemsRect[i].Contains(mousePosEvent)) {
                    coef1 = 4;
                }
                if (!(menu0ItemsRect[i].Contains(mousePosEvent))) {
                    coef1 -= 1;
                    if (coef1 < 0) {
                        coef1 = 0;
                    }
                }
                if(i < 1) {
                    if (menu1ItemsRect[i].Contains(mousePosEvent)) {
                        coef1 = 4;
                    }
                    if (!(menu1ItemsRect[i].Contains(mousePosEvent))) {
                        coef1 -= 1;
                        if (coef1 < 0) {
                            coef1 = 0;
                        }
                    }
                }
                if(i < 2) {
                    if (menu2ItemsRect[i].Contains(mousePosEvent)) {
                        coef1 = 4;
                    }
                    if (!(menu2ItemsRect[i].Contains(mousePosEvent))) {
                        coef1 -= 1;
                        if (coef1 < 0) {
                            coef1 = 0;
                        }
                    }
                }
            }*/
            /*if (!(menu0ItemsRect[i].Contains(mousePosEvent)) && Input.GetMouseButtonDown(0)){
                menuBarItems[3] = true;
            }
            if(i < 1) {
                if (!(menu1ItemsRect[i].Contains(mousePosEvent)) && Input.GetMouseButtonDown(0)) {
                    menuBarItems[4] = true;
                }
            }
            if (i < 2) {
                if (!(menu2ItemsRect[i].Contains(mousePosEvent)) && Input.GetMouseButtonDown(0)) {
                    menuBarItems[5] = true;
                }
            }
            if(menuBarItems[3] && menuBarItems[4] && menuBarItems[5]) {
                menuBarItems[6] = true;
            }
            else {
                menuBarItems[6] = false;
            }*/

        }
        if (cityMode) {
            if (!(landscapeIconRects[0].Contains(mousePosEvent)) && !(landscapeIconRects[1].Contains(mousePosEvent)) && !(landscapeIconRects[2].Contains(mousePosEvent)) &&
            !(landscapeIconRects[3].Contains(mousePosEvent)) && !(landscapeIconRects[4].Contains(mousePosEvent)) && !(landscapeIconRects[5].Contains(mousePosEvent)) &&
            !(landscapeIconRects[6].Contains(mousePosEvent)) && !(landscapeIconRects[7].Contains(mousePosEvent)) && !(landscapeIconRects[8].Contains(mousePosEvent)) &&
            !(landscapeIconRects[9].Contains(mousePosEvent))) {
                coef1 = 0;
            }
        }
        if (buildMode) {
            if (!(modularBuildIconRects[0].Contains(mousePosEvent)) && !(modularBuildIconRects[1].Contains(mousePosEvent)) && !(modularBuildIconRects[2].Contains(mousePosEvent)) &&
            !(modularBuildIconRects[3].Contains(mousePosEvent)) && !(modularBuildIconRects[4].Contains(mousePosEvent)) && !(modularBuildIconRects[5].Contains(mousePosEvent)) &&
            !(modularBuildIconRects[6].Contains(mousePosEvent)) && !(modularBuildIconRects[7].Contains(mousePosEvent)) && !(modularBuildIconRects[8].Contains(mousePosEvent)) &&
            !(modularBuildIconRects[9].Contains(mousePosEvent))) {
                coef1 = 0;
            }
        }
        if(toolsIconRects[0].Contains(mousePosEvent) || toolsIconRects[1].Contains(mousePosEvent) || toolsIconRects[2].Contains(mousePosEvent) ||
            menuIconRects[0].Contains(mousePosEvent) || menuIconRects[1].Contains(mousePosEvent) || menuIconRects[2].Contains(mousePosEvent) ||
            menu0ItemsRect[0].Contains(mousePosEvent) || menu0ItemsRect[1].Contains(mousePosEvent) || menu0ItemsRect[2].Contains(mousePosEvent) || menu0ItemsRect[3].Contains(mousePosEvent)) {

            cantStart = true;
        }
        else {
            cantStart = false;
        }
        for(int i = 0; i < 4; i++) {
            if (menu0ItemsRect[i].Contains(mousePosEvent)) {
                onGUI = true;
                break;
            }
            else {
                onGUI = false;
            }
            
        }

        /*if (menu0ItemsRect[1].Contains(mousePosEvent) ||) {
            onGUI = true;
        }
        else {
            onGUI = false;
        }*/
        //slidVal = GUI.VerticalScrollbar(sliderRect, slidVal, 1f, 0f, 10f);
        //Debug.Log(Mathf.RoundToInt(slidVal));
        if (cityMode) {
            for (int i = iter; i < 10 + iter; i++) {
                if (GUI.Button(landscapeIconRects[i - iter], new GUIContent((Texture)textureLS[i]), customTileIcons)) {
                    if (!tileData.selectedObj) {
                        button0 = true;
                        button1 = true;
                        index = i;
                        indexBuffer = index;
                    }
                }
                
            }
        }
        if (buildMode) {
            for (int i = iter; i < 10 + iter; i++) {
                if (GUI.Button(modularBuildIconRects[i - iter], new GUIContent((Texture)textureMB[i]), customTileIcons)) {
                    button0 = true;
                    button1 = true;
                    index = i;
                }
            }
        }
        /*while(GUI.Button(prevButton, new GUIContent(uiIcons[3]))) {
            if (iter > 0) {
                iter -= 1;
            }
        }*/
        if (prevButton.Contains(mousePosEvent)) {
            if (GUI.Button(prevButton, prevNextButtons[2], customNextIcon)) {
                if (iter > 0) {
                    iter -= 1;
                }
            }
        }
        else {
            if (GUI.Button(prevButton, prevNextButtons[0], customNextIcon)) {
                if (iter > 0) {
                    iter -= 1;
                }
            }
        }
        if (nextButton.Contains(mousePosEvent)) {
            if (GUI.Button(nextButton, prevNextButtons[3], customNextIcon)) {
                int arrSize = 0;
                if (cityMode) {
                    arrSize = tilesLSSize;
                }
                else if (buildMode) {
                    arrSize = tilesMBSize;
                }
                if (iter < arrSize - 10) {
                    iter += 1;
                }
            }
        }
        else {
            if (GUI.Button(nextButton, prevNextButtons[1], customNextIcon)) {
                int arrSize = 0;
                if (cityMode) {
                    arrSize = tilesLSSize;
                }
                else if (buildMode) {
                    arrSize = tilesMBSize;
                }
                if (iter < arrSize - 10) {
                    iter += 1;
                }
            }
        }
        
        /*if (GUI.Button(SaveButton, exportButtonTexture)) {
            button2 = true;
        }*/
        if (exporterData.cantExport) {
            //Debug.Log("cant export");
            GUI.Box(warningMessagePanel, "Objects not exported, make sure to select at least one object !", warningMsg);
            GUI.Box(warningSign, warningImgs[0], warningImgSt);
        }
        if (exporterData.exported) {
            GUI.Box(warningMessagePanel, "Objects exported !", warningMsg);
            GUI.Box(warningSign, warningImgs[1], warningImgSt);
        }
        
        if (Input.GetKeyDown(KeyCode.U)){
            uPressed = true;
            //float xPos = Mathf.Lerp(0, -100f, Time.deltaTime * 5f);

        }

        for(int i = 0; i < 3; i++) {
            string[] menuText = new string[3]
            {
                "File", "Edit", "Help"
            };
            if (GUI.Button(menuIconRects[i], menuText[i], customMenuBar)) {
                roadManagerData.roadBuilder = false;
                if (i == 0) {
                    menuBarItems[0] = true;
                    menuBarItems[1] = false;
                    menuBarItems[2] = false;
                    //SceneManager.LoadScene("version 0.8");
                }
                else if(i == 1) {
                    menuBarItems[0] = false;
                    menuBarItems[1] = true;
                    menuBarItems[2] = false;
                }
                else if(i == 2) {
                    menuBarItems[0] = false;
                    menuBarItems[1] = false;
                    menuBarItems[2] = true;
                    //Application.Quit();
                }
            }
            /*else if(!(GUI.Button(menuIconRects[i], menuText[i], customMenuBar))) {
                menuBarItems[0] = false;
                menuBarItems[1] = false;
                menuBarItems[2] = false;
                menuBarItems[3] = true;
            }*/
        }
        for(int i = 0; i < 3; i++) {
            string[] toolTipText = new string[3]
            {
                "Create roads", "Create custom buildings", "Create the environment"
            };
            if (GUI.Button(toolsIconRects[i], new GUIContent(uiIcons[i],toolTipText[i]), customTileIcons)) {
                if (!settingsOpened) {
                    menuBarItems[6] = true;
                    if (i == 0) {
                        roadMode = true;
                        buildMode = false;
                        cityMode = false;
                    }
                    else if (i == 1) {
                        iter = 0;
                        roadMode = false;
                        buildMode = true;
                        cityMode = false;
                    }
                    else if (i == 2) {
                        iter = 0;
                        roadMode = false;
                        buildMode = false;
                        cityMode = true;
                    }
                }
            }
            if (toolsIconRects[i].Contains(mousePosEvent)) {
                GUI.Label(new Rect(toolsIconRects[i].x + 115f, toolsIconRects[i].y + 25f, 200f, 50f), GUI.tooltip, customToolTipPanel);
            }

        }
        

        if (roadMode) {
            if (modularBuildIconRects[0].position.y == screenHeight + 5f) {
                float genericPTextureScaleValue = 1250f;
                tileData.genericPlane.GetComponent<Renderer>().material.mainTextureScale =
                    new Vector2(genericPTextureScaleValue, genericPTextureScaleValue);
                posData.buildModeToggle = false;
                if(!(menuBarItems[0]) && !(menuBarItems[1]) && !(menuBarItems[2])) {
                    roadManagerData.roadBuilder = true;
                }
                else {
                    roadManagerData.roadBuilder = false;
                }
            }

        }
        if (!roadMode && (buildMode || cityMode)) {
            
        }
        if (cityMode) {
            float genericPTextureScaleValue = 1250f;
            tileData.genericPlane.GetComponent<Renderer>().material.mainTextureScale =
                new Vector2(genericPTextureScaleValue, genericPTextureScaleValue);
            roadManagerData.roadBuilder = false;
            posData.buildModeToggle = false;
            //modeEnabled = true;
            modesEnabled[0] = true;
            if (tileData.selectedObj) {
                for(int i = 0; i < 5; i++) {
                    if (GUI.Button(tileOptionsRects[i], new GUIContent(uiIcons[6 + i]), customTileOptionsButtons)) {
                        if(i == 0) {
                            tileOptions[0] = true;
                        }
                        else if(i == 1) {
                            tileOptions[1] = true;
                        }
                        else if (i == 2) {
                            tileOptions[2] = true;
                        }
                        else if (i == 3) {
                            tileOptions[3] = true;
                        }
                        else if (i == 4) {
                            tileOptions[4] = true;
                        }
                    }
                }
            }
            for (int i = 0; i < 10; i++) {
                
                if (modularBuildIconRects[0].position.y == screenHeight + 5f) {
                    landscapeIconRects[i].position = Vector2.MoveTowards(landscapeIconRects[i].position, new Vector2(xPosRestTiles[i], screenHeight - 100f), (buttonSpeedMove * resMoveAccel) * Time.deltaTime);
                }
            }
            if (modularBuildIconRects[0].position.y == screenHeight + 5f) {
                
            }
            //Debug.Log(previewImages[0].position.x);
            if (landscapeIconRects[0].position.y == screenHeight - 200f) {
                buttonHidden = false;
                uPressed = false;
            }
        }
        if(!cityMode && (roadMode || buildMode)) {            
            for (int i = 0; i < 10; i++) {
                landscapeIconRects[i].position = Vector2.MoveTowards(landscapeIconRects[i].position, new Vector2(xPosRestTiles[i], screenHeight + 5f), (buttonSpeedMove * resMoveAccel) * Time.deltaTime);
            }
            
            bPos = landscapeIconRects[0].position.y;
            if (bPos == screenHeight + 5f) {
                uPressed = false;
                buttonHidden = true;
            }
        }
        if (buildMode) {
            float genericPTextureScaleValue = 5000f;
            tileData.genericPlane.GetComponent<Renderer>().material.mainTextureScale =
                new Vector2(genericPTextureScaleValue, genericPTextureScaleValue);
            roadManagerData.roadBuilder = false;
            posData.buildModeToggle = true;
            //modeEnabled = true;
            modesEnabled[1] = true;
            //prevButton.position = Vector2.MoveTowards(prevButton.position, new Vector2(prevButton.position.x, screenHeight - 200f), rectMoveSpeed);
            //nextButton.position = Vector2.MoveTowards(nextButton.position, new Vector2(nextButton.position.x, screenHeight - 200f), rectMoveSpeed);
            if (tileData.selectedObj) {
                for (int i = 0; i < 5; i++) {
                    if (GUI.Button(tileOptionsRects[i], new GUIContent(uiIcons[6 + i]), customTileOptionsButtons)) {
                        if (i == 0) {
                            tileOptions[0] = true;
                        }
                        else if (i == 1) {
                            tileOptions[1] = true;
                        }
                        else if (i == 2) {
                            tileOptions[2] = true;
                        }
                        else if (i == 3) {
                            tileOptions[3] = true;
                        }
                        else if (i == 4) {
                            tileOptions[4] = true;
                        }
                    }
                }
            }
            for (int i = 0; i < 10; i++) {
                if(landscapeIconRects[0].position.y == screenHeight + 5f) {
                    modularBuildIconRects[i].position = Vector2.MoveTowards(modularBuildIconRects[i].position, new Vector2(xPosRestTiles[i], screenHeight - 100f), (buttonSpeedMove * resMoveAccel) * Time.deltaTime);
                    
                }
            }
            if (landscapeIconRects[0].position.y == screenHeight + 5f) {
                
            }
            //Debug.Log(previewImages[0].position.x);
            if (modularBuildIconRects[0].position.y == screenHeight - 200f) {
                buttonHidden = false;
                uPressed = false;
            }
        }
        if (!buildMode && (roadMode || cityMode)) {
            //prevButton.position = Vector2.MoveTowards(prevButton.position, new Vector2(prevButton.position.x, screenHeight + 5f), rectMoveSpeed);
            //nextButton.position = Vector2.MoveTowards(nextButton.position, new Vector2(nextButton.position.x, screenHeight + 5f), rectMoveSpeed);
            for (int i = 0; i < 10; i++) {
                modularBuildIconRects[i].position = Vector2.MoveTowards(modularBuildIconRects[i].position, new Vector2(xPosRestTiles[i], screenHeight + 5f), (buttonSpeedMove * resMoveAccel) * Time.deltaTime);
            }
            
            bPos = modularBuildIconRects[0].position.y;
            if (bPos == screenHeight + 5f) {
                uPressed = false;
                buttonHidden = true;
            }
        }
        if(!cityMode && !buildMode) {
            modesEnabled[0] = false;
            modesEnabled[1] = false;
        }
        string[] menu0Text = new string[4] { "New", "Open", "Export", "Exit"};
        string[] menu1Text = new string[2] { "Coming soon", "Settings" };
        string[] menu2Text = new string[2] { "About", "Credits" };
        for (int i = 0; i < 4; i++) {
            if (GUI.Button(menu0ItemsRect[i], menu0Text[i], customMenuItems)) {
                if (i == 0) {
                    SceneManager.LoadScene("V0.9");
                }
                if (i == 1) {
                    //Debug.Log("OPENED");
                    menuOpenedItems[i] = true;
                }
                if(i == 2) {
                    //button2 = true;
                    menuOpenedItems[i] = true;
                }
                if (i == 3) {
                    Application.Quit();
                }
            }
            if(i < 2) {
                if (GUI.Button(menu1ItemsRect[i], menu1Text[i], customMenuItems)) {
                    if(i == 1) {
                        settingsOpened = true;
                    }
                    else {
                        settingsOpened = false;
                    }
                }
                if (GUI.Button(menu2ItemsRect[i], menu2Text[i], customMenuItems)) {

                }
            }
        }

        /*for(int i = 0; i < 4; i++) {
            if (GUI.Button(menuItemsRect[i], stb[i], customMenuItems)) {
                if (i == 0) {
                    menuBarItems[0] = true;
                    menuBarItems[1] = false;
                    menuBarItems[2] = false;
                }
            }
            /*for(int j = 0; j < 4; j++) {
                if(GUI.Button((Rect)extMenuRects[i].GetValue(j), stb[j], customMenuItems)) {
                    if(i == 0) {
                        menuBarItems[0] = true;
                        menuBarItems[1] = false;
                        menuBarItems[2] = false;
                    }menuBarItems[6]
                }
                if(!(GUI.Button((Rect)extMenuRects[i].GetValue(j), stb[j], customMenuItems))) {
                    menuBarItems[3] = false;
                }
            }
        }*/
        if (menuBarItems[0]) {
            float rectPos = 0f;
            menuBarItems[3] = false;
            for (int i = 0; i < 4; i++) {
                if(i == 0) {
                    rectPos = 25f;
                }
                if(i == 1) {
                    rectPos = 45f;
                }
                if (i == 2) {
                    rectPos = 65f;
                }
                if (i == 3) {
                    rectPos = 85f;
                }
                menu0ItemsRect[i].position = Vector2.MoveTowards(menu0ItemsRect[i].position, new Vector2(menu0ItemsRect[i].x, rectPos), buttonSpeedMove * Time.deltaTime);
                menuBarItems[3] = false;
            }
        }
        if(!(menuBarItems[0]) && (menuBarItems[1] || menuBarItems[2])){
            for(int i = 0; i < 4; i++) {
                menu0ItemsRect[i].position = Vector2.MoveTowards(menu0ItemsRect[i].position, new Vector2(menu0ItemsRect[i].x, -20f), buttonSpeedMove * Time.deltaTime);
            }
        }
        if (menuBarItems[1]) {
            float rectPos = 0f;
            menuBarItems[4] = false;
            for (int i = 0; i < 2; i++) {
                if (i == 0) {
                    rectPos = 25f; 
                }
                if(i == 1) {
                    rectPos = 45f;
                }
                menu1ItemsRect[i].position = Vector2.MoveTowards(menu1ItemsRect[i].position, new Vector2(menu1ItemsRect[i].x, rectPos), buttonSpeedMove * Time.deltaTime);
            }
        }
        if (!(menuBarItems[1]) && (menuBarItems[0] || menuBarItems[2])) {
            for (int i = 0; i < 2; i++) {
                menu1ItemsRect[i].position = Vector2.MoveTowards(menu1ItemsRect[i].position, new Vector2(menu1ItemsRect[i].x, -20f), buttonSpeedMove * Time.deltaTime);
            }
        }
        if (menuBarItems[2]) {
            menuBarItems[5] = false;
            float rectPos = 0f;
            for (int i = 0; i < 2; i++) {
                if (i == 0) {
                    rectPos = 25f;
                }
                if (i == 1) {
                    rectPos = 45f;
                }
                menu2ItemsRect[i].position = Vector2.MoveTowards(menu2ItemsRect[i].position, new Vector2(menu2ItemsRect[i].x, rectPos), buttonSpeedMove * Time.deltaTime);
            }
        }
        if (!(menuBarItems[2]) && (menuBarItems[0] || menuBarItems[1])) {
            for (int i = 0; i < 2; i++) {
                menu2ItemsRect[i].position = Vector2.MoveTowards(menu2ItemsRect[i].position, new Vector2(menu2ItemsRect[i].x, -20f), buttonSpeedMove * Time.deltaTime);
            }
        }
        if (menuBarItems[6]) {
            //Debug.Log("ALL BACK");
            for(int i = 0; i < 4; i++) {
                menu0ItemsRect[i].position = Vector2.MoveTowards(menu0ItemsRect[i].position, new Vector2(menu0ItemsRect[i].x, -20f), (buttonSpeedMove * 100f) * Time.deltaTime);
                if( i < 2) {
                    menu1ItemsRect[i].position = Vector2.MoveTowards(menu1ItemsRect[i].position, new Vector2(menu1ItemsRect[i].x, -20f), (buttonSpeedMove * 100f) * Time.deltaTime);
                    menu2ItemsRect[i].position = Vector2.MoveTowards(menu2ItemsRect[i].position, new Vector2(menu2ItemsRect[i].x, -20f), (buttonSpeedMove * 100f) * Time.deltaTime);
                }
            }
            if(menu0ItemsRect[0].position.y == -20f && menu1ItemsRect[0].position.y == -20f && menu2ItemsRect[0].position.y == -20f) {
                menuBarItems[6] = false;
                menuBarItems[0] = false;
                menuBarItems[1] = false;
                menuBarItems[2] = false;
            }
            //Debug.Log(menu0ItemsRect[0].position.y);
        }
        /*if (uPressed && buttonHidden == false) {
            for (int i = 0; i < 4; i++) {
                previewImages[i].position = Vector2.MoveTowards(previewImages[i].position, new Vector2(previewImages[i].position.x, screenHeight + 5f), buttonSpeedMove * Time.deltaTime);
            }
            bPos = previewImages[0].position.y;
            if (bPos == screenHeight + 5f) {
                uPressed = false;
                buttonHidden = true;
            }
        }
        if(buttonHidden && uPressed == true) {
            for (int i = 0; i < 4; i++) {
                previewImages[i].position = Vector2.MoveTowards(previewImages[i].position, new Vector2(previewImages[i].position.x, screenHeight - 200f), buttonSpeedMove * Time.deltaTime);
            }
            Debug.Log(previewImages[0].position.x);
            if (previewImages[0].position.y == screenHeight - 200f) {
                buttonHidden = false;
                uPressed = false;
            }
        }*/
        //slidVal = GUI.VerticalScrollbar(sliderRect, slidVal, 1f, 0f, 10f);
        //Debug.Log(Mathf.RoundToInt(slidVal));
        /*else if(buttonHidden){
            for (int i = 0; i < 4; i++) {
                previewImages[i].position = Vector2.MoveTowards(new Vector2(-100f, previewImages[i].position.y), new Vector2(100f, previewImages[i].position.y), buttonSpeedMove * Time.deltaTime);
            }
            buttonHidden = false;
        }*/

        //GUI.Window(0, new Rect(1, 400, 800, 300),)
    }


    /*void GUI_selectsquare(GameObject squareName) {
        if(squareBuffer[0] == null) {
            squareBuffer[0] = (GameObject)Instantiate(squareName);
        }
        else {
            Del_square(0);
            squareBuffer[0] = (GameObject)Instantiate(squareName);
        }
    }*/

    /*void Del_square(int index) {
        Destroy(squareBuffer[index]);
    }*/

    /*Material Gen_squaremat(Texture2D textureName) {
        Material MatName = new Material(Shader.Find("Transparent/Cutout/Bumped Specular"));
        MatName.mainTexture = textureName;
        MatName.SetFloat("_Shininess", 0.01F);
        return MatName;
    }*/

    GameObject Gen_squareobject(Material matName, float scale) {
        //float squareScale = 0.11f;
        meshRend = true;
        GameObject squareObject;
        squareObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        squareObject.transform.parent = squaresParent.transform;
        var squareSettings = squareObject.GetComponent<MeshRenderer>();
        var meshColliderSet = squareObject.GetComponent<MeshCollider>();
        if (meshRend == true) {
            squareSettings.enabled = true;
        }
        else {
            squareSettings.enabled = false;
        }
        meshColliderSet.enabled = false;
        squareSettings.material = matName;
        squareSettings.receiveShadows = false;
        squareSettings.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        squareObject.transform.localScale = new Vector3(scale, scale, scale);
        squareObject.SetActive(false);
        //squareObject.transform.parent = planesParent.transform;
        return squareObject;
    }

    void GUI_grid() {
        for(float i = startPosX; i < startPosX + (nbTiles * 8); i+=8) {
            for (float j = startPosZ; j < startPosZ + (nbTiles * 8); j += 8) {
                var gridGen = (GameObject)Instantiate(squareBaseObject, 
                    new Vector3(i, tileData.touchPlaneHeight, j), 
                    Quaternion.Euler(0, 0, 0)) as GameObject;
                gridGen.SetActive(true);
                gridGen.transform.parent = gridParent.transform;
            }
        }
        gridParent.SetActive(false);
    }

    void GUI_selectedTile() {

    }

    void GUI_outline() {
        Debug.Log("Outline activated");
        Transform objectTrans = posData.hit2.transform.gameObject.transform;
        if (outlined == true && objectTrans.position != prevTilePos) {
            foreach (Transform childParent in tileData.touchPlaneParent.transform) {
                if (childParent.transform.position != objectTrans.position) {
                    foreach (Transform child in childParent) {
                        child.GetComponent<Outline>().color = 0;
                        Debug.Log("HERE0");
                    }
                }
            }
        }
        foreach (Transform child in objectTrans) {
            //child.GetComponent<Outline>().color = 2;
            child.GetComponent<Outline>().color = 1;
            Debug.Log(child.GetComponent<Outline>().eraseRenderer);
        }
        prevTilePos = objectTrans.position;
        outlined = true;
    }

    /// Generateur des icones des objets
    // UnityEditor requis (incompatible avec les Builds). 
    /*
    public static T[] GetAllAssetsAtPath<T>(string path) where T: Object {
        List<T> returnList = new List<T>();
        IEnumerable<string> fullpaths = Directory.GetFiles
            (Application.dataPath + path).Where(x => !x.EndsWith(".meta")).OrderBy(s => s);
        foreach(string fullpath in fullpaths) {
            string assetPath = fullpath.Replace(Application.dataPath, "Assets");
            Object obj = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            var tImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(obj)) as TextureImporter;
            if (tImporter != null) {
                tImporter.mipmapEnabled = false;
            }
            if (obj is T) {
                returnList.Add(obj as T);
            }
        }
        return returnList.ToArray();
    }

    Texture2D[] GUI_iconGen(string texturePath, Object[] textureContainer tileData.tiles) {
        Texture2D[] textureBuffer = new Texture2D[textureContainer.Length];
        string texturePathFixed = string.Concat("/", texturePath);
        int j = 0;
        if (Directory.Exists("Assets/" + texturePath)) {
            Debug.Log("Assets/" + texturePath);
            Debug.Log(texturePathFixed);
            var textArrayFunc = GetAllAssetsAtPath<Texture2D>(texturePathFixed);
            foreach (Object indexedObject in textArrayFunc) {
                textureBuffer[j] = textArrayFunc[j];
                j++;
            }
        }
        //else {
            AssetDatabase.CreateFolder("Assets", texturePath + "2");
            j = 0;
            foreach (Object indexedObj in textureContainer tileData.tiles) {
                var imgPreview = AssetPreview.GetAssetPreview(textureContainer[j]);
                if (imgPreview == null) {
                    while (imgPreview == null) {
                        imgPreview = AssetPreview.GetAssetPreview(textureContainer[j]);
                        System.Threading.Thread.Sleep(1);
                    }
                }
                byte[] bytes = imgPreview.EncodeToPNG();
                File.WriteAllBytes(Application.dataPath + texturePathFixed + "2" + "/prev_" + j.ToString("000") + ".png", bytes);
                j++;
            }
            j = 0;
            AssetDatabase.Refresh();
            var textArrayFunc = GetAllAssetsAtPath<Texture2D>(texturePathFixed + "2");
            foreach (Object indexObj in textArrayFunc) {
                textureBuffer[j] = textArrayFunc[j];
                j++;
            }
        //}
        return textureBuffer;
    }*/
    

    private Texture2D[] Texture_loader(string path)
    {
        Texture2D[] textureArray;
        var textureArrayFunc = Resources.LoadAll(path, typeof(Texture2D));
        textureArray = new Texture2D[textureArrayFunc.Length];
        for (int i = 0; i < textureArrayFunc.Length; i++)
        {
            textureArray[i] = (Texture2D)textureArrayFunc[i];
        }
        return textureArray;
    }

    private void GUI_menuBar(string[] menuList) {
        for(int i = 0; i < menuList.Length; i++) {
            GameObject button = (GameObject)Instantiate(buttonPrefab);
            GameObject canvas = Instantiate(canvasButton) as GameObject;
            button.GetComponentInChildren<Text>().text = fileButtonList[i];
            int index = i;
            button.transform.SetParent(canvasButton.transform, false);
        }
    }

}
