//  ExportOBJ BY DaveA, KeliHlodversson, tgraupmann, drobe available at 
//  https://wiki.unity3d.com/index.php/ExportOBJ
//  under a Creative Common Attribution-ShareAlike 3.0

//  Modifications found in forums:
//  -No more Editor requirement (possibility to use the script in a build now)

//  Modifications applied (by Patrick1794): 
//  -Fixed the MaterialtoFile function which wouldnt export specific material data
//  -Fixed mf.renderer.sharedMaterials (which is outdated in more recent Unity versions) to mf.GetComponent<Renderer>().sharedMaterials
//  -Custom file/folder/ system specific to the needs of the software
//  -Custom objectsToExport system specific to the needs of the software

//  Modded version of ObjExporter
//  Version modifiee du script ObjExporter

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

public class Exporter : MonoBehaviour {

    struct ObjMaterial {
        public string name;
        public string textureName;
    }

    public int vertexOffset = 0;
    public int normalOffset = 0;
    public int uvOffset = 0;
    public string targetFolder = "ExportedObj";
    public Transform[] objectTransformToExport;
    public GameObject objectToExport;
    public bool exporting, exported, cantExport;
    private Material mt0;

    GUIscript guiDataScript;
    public GameObject guiData;

    PositionSensor posDataScript;
    public GameObject posData;

    FileBrowserOBJ fileBrowserScript;
    public GameObject fileBrowserData;

    public Texture2D txt;

    public static string[] materialData_brick = new string[3]
    {   "Ka 0.000000 0.000000 0.000000\n",
        "Kd 0.917647 0.898039 0.811765\n",
        "Ks 0.330000 0.330000 0.330000\n" },
    materialData_darkBrick = new string[3]
    {   "Ka 0.000000 0.000000 0.000000\n",
        "Kd 0.800000 0.780392 0.705882\n",
        "Ks 0.330000 0.330000 0.330000\n" },
    materialData_frontColor = new string[3]
    {   "Ka 0.000000 0.000000 0.000000\n",
        "Kd 1.000000 1.000000 1.000000\n",
        "Ks 0.330000 0.330000 0.330000\n" },
    materialData_glass = new string[3]
    {   "Ka 0.000000 0.000000 0.000000\n",
        "Kd 0.611765 0.831373 0.886275\n",
        "Ks 0.330000 0.330000 0.330000\n" },
    materialData_green = new string[3]
    {   "Ka 0.000000 0.000000 0.000000\n",
        "Kd 0.541176 0.709804 0.286275\n",
        "Ks 0.330000 0.330000 0.330000\n" },
    materialData_metal = new string[3]
    {   "Ka 0.000000 0.000000 0.000000\n",
        "Kd 0.882353 0.886275 0.901961\n",
        "Ks 0.330000 0.330000 0.330000\n" },
    materialData_roof = new string[3]
    {   "Ka 0.000000 0.000000 0.000000\n",
        "Kd 0.266667 0.266667 0.266667\n",
        "Ks 0.330000 0.330000 0.330000\n" },
    materialData_wood = new string[3]
    {   "Ka 0.000000 0.000000 0.000000\n",
        "Kd 0.615686 0.454902 0.282353\n",
        "Ks 0.330000 0.330000 0.330000\n" };

    string MeshToString(MeshFilter mf, Dictionary<string, ObjMaterial> materialList) {
        Mesh m = mf.sharedMesh;
        Material[] mats = mf.GetComponent<Renderer>().sharedMaterials;

        StringBuilder sb = new StringBuilder();

        sb.Append("g ").Append(mf.name).Append("\n");
        foreach (Vector3 lv in m.vertices) {
            Vector3 wv = mf.transform.TransformPoint(lv);

            //This is sort of ugly - inverting x-component since we're in
            //a different coordinate system than "everyone" is "used to".
            sb.Append(string.Format("v {0} {1} {2}\n", -wv.x, wv.y, wv.z));
        }
        sb.Append("\n");

        foreach (Vector3 lv in m.normals) {
            Vector3 wv = mf.transform.TransformDirection(lv);

            sb.Append(string.Format("vn {0} {1} {2}\n", -wv.x, wv.y, wv.z));
        }
        sb.Append("\n");

        foreach (Vector3 v in m.uv) {
            sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }

        for (int material = 0; material < m.subMeshCount; material++) {
            sb.Append("\n");
            sb.Append("usemtl ").Append(mats[material].name).Append("\n");
            sb.Append("usemap ").Append(mats[material].name).Append("\n");

            //See if this material is already in the materiallist.
            try {
                ObjMaterial objMaterial = new ObjMaterial();

                objMaterial.name = mats[material].name;
                if (posDataScript.buildModeToggle) {

                }
                else {
                    if (mats[material].mainTexture) {
                        objMaterial.textureName = Application.dataPath + "/Resources" + "/Textures" + "/basetexture.jpg";
                        //Debug.Log(objMaterial.textureName);
                        //Debug.Log(objMaterial.textureName);
                    }
                    else {
                        objMaterial.textureName = null;
                    }
                }
                materialList.Add(objMaterial.name, objMaterial);
            }
            catch (ArgumentException) {
                //Already in the dictionary
            }


            int[] triangles = m.GetTriangles(material);
            for (int i = 0; i < triangles.Length; i += 3) {
                //Because we inverted the x-component, we also needed to alter the triangle winding.
                sb.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n",
                                       triangles[i] + 1 + vertexOffset, triangles[i + 1] + 1 + normalOffset, triangles[i + 2] + 1 + uvOffset));
            }
        }

        vertexOffset += m.vertices.Length;
        normalOffset += m.normals.Length;
        uvOffset += m.uv.Length;


        return sb.ToString();
    }
    void Clear() {
        vertexOffset = 0;
        normalOffset = 0;
        uvOffset = 0;
    }

    Dictionary<string, ObjMaterial> PrepareFileWrite() {
        Clear();

        return new Dictionary<string, ObjMaterial>();
    }

    private static void MaterialsToFile(Dictionary<string, ObjMaterial> materialList, string folder, string filename, bool buildMode, string texturePath) {
        using (StreamWriter sw = new StreamWriter(folder + filename + ".mtl")) {
            foreach (KeyValuePair<string, ObjMaterial> kvp in materialList) {
                sw.Write("\n");
                sw.Write("newmtl {0}\n", kvp.Key);
                if (buildMode) {
                    switch (kvp.Key) {
                        case "Brick":
                            sw.Write(materialData_brick[0]);
                            sw.Write(materialData_brick[1]);
                            sw.Write(materialData_brick[2]);
                            break;
                        case "Glass":
                            sw.Write(materialData_glass[0]);
                            sw.Write(materialData_glass[1]);
                            sw.Write(materialData_glass[2]);
                            break;
                        case "Green":
                            sw.Write(materialData_green[0]);
                            sw.Write(materialData_green[1]);
                            sw.Write(materialData_green[2]);
                            break;
                        case "Dark_brick":
                            sw.Write(materialData_darkBrick[0]);
                            sw.Write(materialData_darkBrick[1]);
                            sw.Write(materialData_darkBrick[2]);
                            break;
                        case "FrontColor":
                            sw.Write(materialData_frontColor[0]);
                            sw.Write(materialData_frontColor[1]);
                            sw.Write(materialData_frontColor[2]);
                            break;
                        case "Metal":
                            sw.Write(materialData_metal[0]);
                            sw.Write(materialData_metal[1]);
                            sw.Write(materialData_metal[2]);
                            break;
                        case "Roof":
                            sw.Write(materialData_roof[0]);
                            sw.Write(materialData_roof[1]);
                            sw.Write(materialData_roof[2]);
                            break;
                        case "Wood":
                            sw.Write(materialData_wood[0]);
                            sw.Write(materialData_wood[1]);
                            sw.Write(materialData_wood[2]);
                            break;
                        default:
                            break;
                    }
                }
                else {
                    sw.Write("Ka 0.6 0.0 0.6\n");
                    sw.Write("Kd 0.6 0.6 0.6\n");
                    sw.Write("Ks 0.9 0.9 0.9\n");
                    sw.Write("d  1.0\n");
                    sw.Write("Ns 0.0\n");
                    sw.Write("illum 2\n");
                }

                if (kvp.Value.textureName != null) {
                    string destinationFile = kvp.Value.textureName;


                    int stripIndex = destinationFile.LastIndexOf(Path.PathSeparator);


                    if (stripIndex >= 0)
                        destinationFile = destinationFile.Substring(stripIndex + 1).Trim();


                    string relativeFile = destinationFile;

                    destinationFile = folder + Path.PathSeparator + destinationFile;

                    //Debug.Log("Copying texture from " + kvp.Value.textureName + " to " + destinationFile);

                    try {
                        //Copy the source file
                        File.Copy(kvp.Value.textureName, destinationFile);
                    }
                    catch {

                    }
                    string textureP = " " + texturePath + "/basetexture.jpg";
                    sw.Write("map_Kd" + textureP);
                    //sw.Write("map_Kd {0}", relativeFile);
                }

                sw.Write("\n\n\n");
            }
        }
    }

    void MeshToFile(MeshFilter mf, string folder, string filename) {
        Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();
        string mtlName = "Exported_Objects";
        using (StreamWriter sw = new StreamWriter(folder + "/" + filename + ".obj")) {
            sw.Write("mtllib ./" + mtlName + ".mtl\n");

            sw.Write(MeshToString(mf, materialList));
        }
        MaterialsToFile(materialList, folder, filename, posDataScript.buildModeToggle, fileBrowserScript.shortTargetFolder + "/Textures");
    }

    void MeshesToFile(MeshFilter[] mf, string folder, string filename) {
        Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();
        string mtlName = "Exported_Objects";
        using (StreamWriter sw = new StreamWriter(folder + filename + ".obj")) {
            sw.Write("mtllib ./" + mtlName + ".mtl\n");

            for (int i = 0; i < mf.Length; i++) {
                sw.Write(MeshToString(mf[i], materialList));
            }
        }
        MaterialsToFile(materialList, folder, filename, posDataScript.buildModeToggle, fileBrowserScript.shortTargetFolder + "/Textures");
    }


    bool CreateTargetFolder() {
        string destFolder = fileBrowserScript.targetFolder + "/Textures";
        try {
            System.IO.Directory.CreateDirectory(destFolder);
        }
        catch {
            //Debug.Log("ERROR");
            //EditorUtility.DisplayDialog("Error!", "Failed to create target folder!", "");
            return false;
        }
        //Debug.Log("SUCCESS");
        return true;
    }

    void Start() {

        

        guiDataScript = guiData.GetComponent<GUIscript>();
        posDataScript = posData.GetComponent<PositionSensor>();

        fileBrowserScript = fileBrowserData.GetComponent<FileBrowserOBJ>();
        objectTransformToExport = new Transform[1];
        exporting = false;
        exported = false;
        cantExport = false;
        //objectToExport = GameObject.Find("GeneratedTiles");
    }

    private void Update() {
        if (fileBrowserScript.exp1) {
            exporting = true;
        }
    }

    private void LateUpdate() {
        if (exporting) {
            objectToExport = GameObject.Find("generatedTiles");
            //Debug.Log(objectToExport.transform.position);
            objectTransformToExport[0] = objectToExport.transform;
            CreateTargetFolder();
            SaveModels(objectTransformToExport);
            guiDataScript.button2 = false;
            fileBrowserScript.exp1 = false;
            StartCoroutine(DisableExport());
            exporting = false;
            //exported = false;
            //cantExport = false;
        }
        //Debug.Log(exporting);
    }

    public void SaveModels(Transform[] models) {
        if (models.Length == 0) return;

        int exportedObjects = 0;

        ArrayList mfList = new ArrayList();

        for (int i = 0; i < models.Length; i++) {
            Component[] meshfilter = models[i].GetComponentsInChildren(typeof(MeshFilter));
            //Debug.Log(models[i].name);

            for (int m = 0; m < meshfilter.Length; m++) {
                exportedObjects++;
                mfList.Add(meshfilter[m]);
            }
        }

        if (exportedObjects > 0) {
            MeshFilter[] mf = new MeshFilter[mfList.Count];

            for (int i = 0; i < mfList.Count; i++) {
                mf[i] = (MeshFilter)mfList[i];
            }

            //string filename = "Exported_Objects";
            //string destFolder = targetFolder + "/";
            string filename = fileBrowserScript.fileName;
            string destFolder = fileBrowserScript.targetFolder;
            //Debug.Log(filename);
            //Debug.Log(destFolder);
            MeshesToFile(mf, destFolder, filename);
            exported = true;
            //Application.dataPath + "/Ressources" + "/Textures" + "/basetexture.jpg";
            string texturePath = Application.dataPath + "/Resources" + "/Textures";
            string textureFullPath = Path.Combine(Directory.GetCurrentDirectory(), texturePath);
            //Debug.Log(textureFullPath);
            textureFullPath = textureFullPath.Replace('/', '\\');
            //Debug.Log(textureFullPath);
            string pt = "Textures/basetexture";
            //Debug.Log(pt);
            TextAsset textureAsset = Resources.Load(pt, typeof(TextAsset)) as TextAsset;
            txt = Resources.Load(pt, typeof(Texture2D)) as Texture2D;
            byte[] bytes = txt.EncodeToJPG();
            string targetPath = fileBrowserScript.targetFolder + "Textures" + "/basetexture.jpg" ;
            targetPath = targetPath.Replace('/', '\\');
            //Debug.Log(targetPath);
            System.IO.File.WriteAllBytes(targetPath, bytes);
            //string fileName = "basetexture.jpg";
            //string sourcePath = textureFullPath;
            //string targetPath = fileBrowserScript.targetFolder + "/Textures";
            //string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            //string destFile = System.IO.Path.Combine(targetPath, fileName);
            //System.IO.File.Copy(sourceFile, destFile, true);
            //Debug.Log("Objects exported" + "Exported " + exportedObjects + " objects to " + filename + "");
        }
        else {
            cantExport = true;
        }


        //Debug.Log("Objects not exported " + "Make sure at least some of your selected objects have mesh filters!" + "");
    }

    IEnumerator DisableExport() {
        yield return new WaitForSeconds(2.5f);
        cantExport = false;
        exported = false;
        //exporting = false;
    }
}

