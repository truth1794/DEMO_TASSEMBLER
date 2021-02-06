/*
 * Copyright (c) 2015 José Guerreiro. All rights reserved.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace outliner {
    [RequireComponent(typeof(Renderer))]
    /* [ExecuteInEditMode] */
    public class Outline : MonoBehaviour {

        public bool buildMode =  false;
        public bool casted = false;
        public bool selected = false;
        public bool alreadySelected = false;

        public bool mousOver = false;
        public bool mousDown = false;
        public GameObject selectedTile;
        public GameObject childObj;
        public GameObject targetObject;

        public GameObject posDataScript;
        PositionSensor posData;

        TileControl tileDataScript;
        public GameObject tileData;

        GUIscript guiScript;
        public GameObject guiData;


        public Renderer Renderer { get; private set; }
        public SpriteRenderer SpriteRenderer { get; private set; }
        public SkinnedMeshRenderer SkinnedMeshRenderer { get; private set; }
        public MeshFilter MeshFilter { get; private set; }

        public int color;
        public bool eraseRenderer = false;

        private void Awake() {
            Renderer = GetComponent<Renderer>();
            SkinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            MeshFilter = GetComponent<MeshFilter>();
        }

        private void Start() {
            posDataScript = GameObject.Find("Camera").gameObject;
            tileData = GameObject.Find("GridGenerator").gameObject;
            guiData = GameObject.Find("GUI").gameObject;
            posData = posDataScript.GetComponent<PositionSensor>();
            tileDataScript = tileData.GetComponent<TileControl>();
            guiScript = guiData.GetComponent<GUIscript>();
            buildMode = GameObject.Find("Camera").GetComponent<PositionSensor>().buildModeToggle;
        }


        void OnEnable() {
            OutlineEffect.Instance?.AddOutline(this);
        }

        void OnDisable() {
            OutlineEffect.Instance?.RemoveOutline(this);
        }

        private Material[] _SharedMaterials;
        public Material[] SharedMaterials {
            get {
                if (_SharedMaterials == null)
                    _SharedMaterials = Renderer.sharedMaterials;

                return _SharedMaterials;
            }
        }


        private void Update() {


            if (guiScript.buildMode) {
                if (mousOver || posData.buttonClicked) {
                    selectedTile = this.transform.parent.gameObject;
                    childObj = this.transform.gameObject;
                    childObj.GetComponent<Outline>().color = 1;
                    casted = true;
                }
                else if (casted) {
                    childObj.GetComponent<Outline>().color = 0;
                    casted = false;
                    selectedTile = null;
                }
                if (mousDown) {
                    selected = true;
                    if (!(posData.buttonClicked)) {
                        selectedTile = this.transform.parent.gameObject;
                        tileDataScript.buffer0 = selectedTile;
                    }
                    tileDataScript.selectedObj = true;
                }
                if (selected && guiScript.tileOptions[3]) {
                    Debug.Log("CLICK HERE");
                    tileDataScript.selectedObj = false;
                    
                    guiScript.tileOptions[0] = false;
                    guiScript.tileOptions[3] = false;
                    foreach(Transform child in tileDataScript.buffer0.transform) {
                        child.GetComponent<Outline>().selected = false;
                        child.GetComponent<Outline>().mousDown = false;
                    }
                    //selected = false;
                    //mousDown = false;
                }
            }
            else if(guiScript.cityMode) {
                if (mousOver || posData.buttonClicked) {
                    //Debug.Log("Over");
                    if (posData.buttonClicked) {
                        if (tileDataScript.buffer0) {
                            foreach (Transform child in tileDataScript.buffer0.transform) {
                                if (!(child.GetComponent<Outline>() == null)) {
                                    //Debug.Log(child.name);
                                    selectedTile = child.gameObject;
                                    selectedTile.GetComponent<Outline>().eraseRenderer = false;
                                }
                            }
                        }
                    }
                    else if (targetObject) {
                        //Debug.Log(targetObject.name);
                        targetObject.GetComponent<Outline>().eraseRenderer = false;
                    }
                    else {
                        //targetObject.GetComponent<Outline>().eraseRenderer = true;
                    }
                }
                else if (!mousOver && !(posData.buttonClicked) && !(selected)) {
                    //Debug.Log("notOver");
                    if (targetObject) {
                        targetObject.GetComponent<Outline>().eraseRenderer = true;
                    }
                    if (tileDataScript.buffer0) {
                        foreach (Transform child in tileDataScript.buffer0.transform) {
                            if (!(child.GetComponent<Outline>() == null)) {
                                selectedTile = child.gameObject;
                                selectedTile.GetComponent<Outline>().eraseRenderer = true;
                            }
                        }
                    }
                }
                /*else if(casted && !(this.gameObject.tag == "house")) {
                    selectedTile.GetComponent<Outline>().color = 0;
                    casted = false;
                    selectedTile = null;
                }*/
                //Debug.Log("mouse " + mousDown);
                //Debug.Log(this.gameObject.name);
                //Debug.Log("sele " + selected);
                //Debug.Log("but " + guiScript.tileOptions[3]);
                if (mousDown) {
                    selected = true;
                    //selectedTile = this.transform.gameObject;
                    tileDataScript.selectedObj = true;
                    tileDataScript.buffer0 = selectedTile;
                    tileDataScript.buffer0.GetComponent<Outline>().eraseRenderer = false;
                    tileDataScript.buffer0.GetComponent<Outline>().color = 0;
                    foreach (Transform objs in tileDataScript.generatedTiles.transform) {
                        if (objs.transform.position != tileDataScript.buffer0.transform.parent.position) {
                            foreach(Transform objChild in objs.transform) {
                                if(!(objChild.GetComponent<Outline>() == null)) {
                                    objChild.GetComponent<Outline>().eraseRenderer = true;
                                }
                            }
                        }
                    }
                }
                if (selected && (/*!mousDown || */guiScript.tileOptions[3])) {
                    Debug.Log("selected validated");
                    tileDataScript.selectedObj = false;
                    selected = false;
                    guiScript.tileOptions[0] = false;
                    guiScript.tileOptions[3] = false;
                    mousDown = false;
                }
                //if(selected && )
            }
        }
        private void OnMouseDown() {
            //if (posData.buttonClicked == false) {
            //mousDown = !mousDown;
            //}
            bool alreadySelected = false;
            foreach (Transform objs in tileDataScript.generatedTiles.transform) {
                if (objs.transform.position != this.transform.parent.position) {
                    foreach (Transform objChild in objs.transform) {
                        if (!(objChild.GetComponent<Outline>() == null)) {
                            if (objChild.GetComponent<Outline>().selected == true) {
                                alreadySelected = true;
                                break;
                            }
                        }
                    }
                }
            }
            //Debug.Log(alreadySelected);
            if (!alreadySelected) {
                selectedTile = this.gameObject;
                mousDown = true;
            }
            
        }
        private void OnMouseOver() {
            bool alreadySelected = false;
            foreach(Transform objs in tileDataScript.generatedTiles.transform) {
                if (objs.transform.position != this.transform.parent.position) {
                    foreach (Transform objChild in objs.transform) {
                        if (!(objChild.GetComponent<Outline>() == null)) {
                            if(objChild.GetComponent<Outline>().selected == true) {
                                alreadySelected = true;
                                break;
                            }
                        }
                    }
                }
            }
            //Debug.Log(alreadySelected);
            if (!alreadySelected) {
                if (!mousOver) {
                    mousOver = true;
                }
                targetObject = this.gameObject;
            }
            
        }
        private void OnMouseExit() {
            if (mousOver) {
                mousOver = false;
            }
        }
    }
}