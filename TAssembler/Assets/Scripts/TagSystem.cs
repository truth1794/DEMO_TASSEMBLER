using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TagEngine {

    public class TagSystem : MonoBehaviour {

        //public static Dictionary<string, List<GameObject>> specificTags = new Dictionary<string, List<GameObject>>();
        //public static Dictionary<string, List<GameObject>> heightTags = new Dictionary<string, List<GameObject>>();
        public string parentName;
        public string mainTag;
        public string basicType;
        public string advancedType;
        public string heightTag;
        public int id;
        public int gridId;
        public int objNbTags = -1;
        public string[] tileNames = new string[10]
            {
                "road","house","canal","hill",
                "river","water","building","grass",
                "dirt","sand"
            };

        private void Awake() {

            gridId = -1;

            parentName = this.transform.parent.name;

            objNbTags = CountNumberTags(parentName, '-');

            if (objNbTags == 0) {
                mainTag = parentName.Split('-')[0];
                this.transform.tag = mainTag;
            }
            else if (objNbTags == 1) {
                mainTag = parentName.Split('-')[0];
                this.transform.tag = mainTag;
                if(parentName.Split('-')[1] == "end" || parentName.Split('-')[1] == "straight" || parentName.Split('-')[1] == "crossing" ||
                    parentName.Split('-')[1] == "tjunction" || parentName.Split('-')[1] == "corner" || parentName.Split('-')[1] == "suburb") {
                    basicType = parentName.Split('-')[1];
                }
                else {
                    advancedType = parentName.Split('-')[1];
                }
            }
            else if (objNbTags == 2) {
                mainTag = parentName.Split('-')[0];
                this.transform.tag = mainTag;
                if(mainTag == "house") {

                }
                else {
                    if (parentName.Split('-')[1] == "corner" || parentName.Split('-')[1] == "straight" || parentName.Split('-')[1] == "end" ||
                    parentName.Split('-')[1] == "crossing" || parentName.Split('-')[1] == "tjunction") {
                        basicType = parentName.Split('-')[1];
                    }
                    else {
                        advancedType = parentName.Split('-')[1];
                    }
                    if (parentName.EndsWith("(Clone)")) {
                        //Debug.Log("clone found");
                        string correctedString = parentName.Replace("(Clone)", "");
                        heightTag = correctedString.Split('-')[2];
                    }
                    else {
                        heightTag = parentName.Split('-')[2];
                    }
                }
            }
            else {
                mainTag = parentName.Split('-')[0];
                this.transform.tag = mainTag;
            }
            //Debug.Log(mainTag + "\n" + specificTag + "\n");
            //Debug.Log(heightTag);

            /*if (!specificTags.ContainsKey(specificTag)) specificTags[specificTag] = new List<GameObject>();
            specificTags[specificTag].Add(gameObject);
        }

        private void OnDestroy() {
            specificTags[specificTag].Remove(gameObject);
        }*/
        }

        private int CountNumberTags(string objName, char separator) {
            int nbTags = objName.Split(separator).Length - 1;
            return nbTags;
        }

        private void OnMouseOver() {
            
        }
    }

}