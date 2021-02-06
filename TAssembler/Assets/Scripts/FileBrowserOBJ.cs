using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System.IO;
using SFB;
using Dummiesman;

public class FileBrowserOBJ : MonoBehaviour {
    public GameObject importedObj;
    public Texture2D importedTexture;
    public string importedTextureName;
    public string objPath = string.Empty;
    public List<string> objsPath;
    private UnityWebRequest webReq;
    public string[] objectsPaths;
    GUIscript guiScript;
    public GameObject guiData;
    public string targetFolder;
    public string fileName;
    public string shortTargetFolder;
    public bool exp1 = false, cancelled = false;

    public void OnPointerDown(PointerEventData eventData) { }

    private void Start() {
        guiScript = guiData.GetComponent<GUIscript>();
        //var button = GetComponent<Button>();
        //button.onClick.AddListener(OnClick);

    }

    private void Update() {
        if (guiScript.menuOpenedItems[1]) {
            Importer();
        }
        if (guiScript.menuOpenedItems[2]){
            Exporter();
            exp1 = true;
            guiScript.menuOpenedItems[2] = false;
        }
    }

    private void OnClick() {

        Importer();
    }

    private void Importer() {

        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "", true);
        objectsPaths = paths;
        if (paths.Length > 0) {
            objsPath = new List<string>(paths.Length);
            for (int i = 0; i < paths.Length; i++){
                objsPath.Add(new System.Uri(paths[i]).OriginalString);
            }
        }
        for(int i = 0; i < objsPath.Count; i++) {
            string objsExtension = objsPath[i].Substring(objsPath[i].Length - 4);
            Debug.Log(objsExtension);
            switch (objsExtension) {
                case ".obj":
                    //objPath = new System.Uri(paths[0]).OriginalString;
                    Debug.Log(objsPath[i]);
                    importedObj = new OBJLoader().Load(objsPath[i]);
                    break;
                case ".jpg":
                    StartCoroutine(LoadTexture(objsPath[i]));
                    //SaveTexture(importedTexture, Application.streamingAssetsPath);
                    break;
                default:
                    break;
            }
        }
        guiScript.menuOpenedItems[1] = false;
        /*objPath = new System.Uri(paths[0]).OriginalString;
        Debug.Log(objPath);
        importedObj = new OBJLoader().Load(objPath);*/
    }
    // C:\Users\truth\Documents\Dev\New folder (3)\TCREAT\Assets\Resources\Textures"
    private void Exporter() {
        fileName = null;
        shortTargetFolder = null;
        var windowVar = StandaloneFileBrowser.SaveFilePanel("Title", "", "", "obj");
        if (!(string.IsNullOrEmpty(windowVar))) {
            int nbSpl0 = CountNumberSplits(windowVar, '\\');
            targetFolder = null;
            for (int i = 0; i < nbSpl0; i++) {
                targetFolder += windowVar.Split('\\')[i];
                targetFolder += '\\';
            }
            for (int i = nbSpl0; i > nbSpl0 - 2; i--) {
                shortTargetFolder += targetFolder.Split('\\')[i];
            }
            fileName = windowVar.Split('\\')[nbSpl0];
            int nbSpl1 = CountNumberSplits(fileName, '.');
            if (nbSpl1 == 1) {
                fileName = fileName.Split('.')[0];
            }
        }
    }

    public static string GetFilePath(string filePath) {
        return "file://" + Path.Combine(filePath);
    }

    System.Collections.IEnumerator LoadTexture(string filePath) {
        using (webReq = UnityWebRequestTexture.GetTexture(GetFilePath(filePath))) {
            yield return webReq.SendWebRequest();
            importedTexture = DownloadHandlerTexture.GetContent(webReq);
        }
    }

    private int CountNumberSplits(string objName, char separator) {
        int nbSplits = objName.Split(separator).Length - 1;
        return nbSplits;
    }

    //Ne fonctionne pas, acces au dossier StreamingAssets refuse (acces en Read-only uniquement)
    public static void SaveTexture(Texture2D textureImage, string path) {
        byte[] textureData = textureImage.EncodeToJPG();
        System.IO.File.WriteAllBytes(path, textureData);
        //Debug.Log("Texture saved in" + path);
    }
}