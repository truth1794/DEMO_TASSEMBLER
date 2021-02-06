using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public Camera _camera;

    GUIscript guiData;
    public GameObject guiData0;

    PositionSensor posData;
    public GameObject posData0;

    TileControl tileData;
    public GameObject data2;
    public string touchTag;

    Ray ray;
    RaycastHit hitpoint;

    int dir;
    public float camSpeed;
    public float rotSpeed;
    bool up;
    bool down;
    bool left;
    bool right;
    bool freeCam = false;
    public bool cameraRotating = false;
    public float fovMin;
    public float fovMax;
    public Vector3 posTrans;
    public Vector3 camFocusCenter;

    public float movSpeed;
    public float zoomMinCity;
    public float zoomMaxCity;
    public float zoomMinBuilder;
    public float zoomMaxBuilder;
    public float zoomSpeed;
    public float dampening;

    public float zoomPos = 0;

    public enum RotationAxe { MouseXAndY = 0 }
    public RotationAxe axe = RotationAxe.MouseXAndY;
    public float sensitivityX = 1f;
    public float sensitivityY = 0.25f;
    public float minX = 0f, maxX = 0f;
    public float minY = -90f, maxY = 0f;
    public float rotationY = 0F;

    void Start() {

        
        posData = posData0.GetComponent<PositionSensor>();
        guiData = guiData0.GetComponent<GUIscript>();
        tileData = data2.GetComponent<TileControl>();
        camSpeed = 60.0f;
        rotSpeed = 120f;
        fovMin = 30;
        fovMax = 40;
        zoomSpeed = 50f;
        zoomMinCity = 120f;
        zoomMaxCity = 50f;
        zoomMinBuilder = 20f;
        zoomMaxBuilder = 10f;
        dampening = 4f;
        _camera = GetComponent<Camera>();
        touchTag = tileData.touchPlaneTag;
        //_camera.transform.LookAt(guiData.gridParent.transform.position,Vector3.up);
    }
    void Update() {
        ray = new Ray(_camera.transform.position, _camera.transform.forward);
        if (Physics.Raycast(ray, out hitpoint)) {
            if (hitpoint.collider.tag.Equals(touchTag)) {
                camFocusCenter = new Vector3(hitpoint.point.x, 0, hitpoint.point.z);
            }
        }
        up = Input.GetKey(KeyCode.W);
        down = Input.GetKey(KeyCode.S);
        left = Input.GetKey(KeyCode.A);
        right = Input.GetKey(KeyCode.D);
        if (guiData.buildMode) {
            camSpeed = 20f;
        }
        else {
            camSpeed = 60f;
        }
        if (up) {
            dir = 8;
            //Debug.Log ("UP");
        }
        else if (down) {
            dir = 2;
            //Debug.Log ("DOWN");
        }
        else if (left) {
            dir = 4;
            //Debug.Log ("LEFT");
        }
        else if (right) {
            dir = 6;
            //Debug.Log ("RIGHT");
        }
        else if (up && right) {
            dir = 9;
            //Debug.Log ("UPRIGHT");
        }
        else if (up && left) {
            dir = 7;
            //Debug.Log ("UPLEFT");
        }
        else if (down && left) {
            dir = 1;
            //Debug.Log ("DOWNLEFT");
        }
        else if (down && right) {
            dir = 3;
            //Debug.Log ("DOWNRIGHT");
        }
        else {
            dir = 0;
        }

        if (dir == 8) { //FORWARD
            transform.Translate(0, camSpeed * Time.deltaTime, camSpeed * Time.deltaTime, Space.Self);
        }
        else if (dir == 2) { //BACKWARD
            transform.Translate(0, -camSpeed * Time.deltaTime, -camSpeed * Time.deltaTime, Space.Self);
        }
        else if (dir == 4) { //LEFT
            transform.Translate(-camSpeed * Time.deltaTime, 0, 0, Space.Self);
        }
        else if (dir == 6) { //RIGHT  
            transform.Translate(camSpeed * Time.deltaTime, 0, 0, Space.Self);
        }
        else if (dir == 9) { //FORWARD
            transform.Translate(camSpeed * Time.deltaTime, camSpeed * Time.deltaTime, camSpeed * Time.deltaTime);
        }
        else if (dir == 7) { //FORWARD
            transform.Translate(-camSpeed * Time.deltaTime, camSpeed * Time.deltaTime, camSpeed * Time.deltaTime);
        }
        else if (dir == 1) { //FORWARD
            transform.Translate(-camSpeed * Time.deltaTime, -camSpeed * Time.deltaTime, -camSpeed * Time.deltaTime);
        }
        else if (dir == 3) { //FORWARD
            transform.Translate(camSpeed * Time.deltaTime, -camSpeed * Time.deltaTime, -camSpeed * Time.deltaTime);
        }

        if (guiData.coef1 == 0) {
            Camera_zoom();
        }
        if (Input.GetKeyDown(KeyCode.V)) {
            freeCam = !freeCam;
        }
    
        /*if (_camera.transform.position.y < 10) {
            Vector3 tmp = transform.eulerAngles;
            tmp.x = 40;
            float xRot = _camera.transform.eulerAngles.x;

            if (xRot != tmp.x) {
                //Debug.Log("<10 EXECUTED");
                transform.rotation = Quaternion.Euler(tmp);
            }
        }
        else if (_camera.transform.position.y > 10 && _camera.transform.position.y < 50) {
            Vector3 tmp = transform.eulerAngles;
            tmp.x = 45;
            float xRot = _camera.transform.eulerAngles.x;

            if (xRot != tmp.x) {
                //Debug.Log (">10 EXECUTED");
                transform.rotation = Quaternion.Euler(tmp);
            }
        }
        else if (_camera.transform.position.y > 50) {
            Vector3 tmp = transform.eulerAngles;
            tmp.x = 50;
            float xRot = _camera.transform.eulerAngles.x;

            if (xRot != tmp.x) {
                //Debug.Log(">50 EXECUTED");
                transform.rotation = Quaternion.Euler(tmp);
            }
        }*/

        if (Input.GetMouseButton(1) && !(tileData.selectedObj)) {
            float _rotationX = 0f;
            float _RotationY = 0f;
            _rotationX = Input.GetAxis("Mouse X");
            _RotationY = Input.GetAxis("Mouse Y");

            if (_rotationX > 0) {
                transform.RotateAround(camFocusCenter, Vector3.up, (rotSpeed * 1.2f) * Time.deltaTime);
            }
            else if (_rotationX < 0) {
                transform.RotateAround(camFocusCenter, Vector3.up, -(rotSpeed * 1.2f) * Time.deltaTime);
            }
            if(axe == RotationAxe.MouseXAndY) {
                rotationY = Input.GetAxis("Mouse Y");
                rotationY = Mathf.Clamp(rotationY, minY, maxY);
                if(rotationY > 0) {
                    if(transform.localEulerAngles.x > 27) {
                        transform.Rotate(Vector3.left, (rotSpeed * 0.30f) * Time.deltaTime);
                    }
                }
                else if(rotationY < 0) {
                    if(transform.localEulerAngles.x < 75) {
                        transform.Rotate(Vector3.left, -(rotSpeed * 0.30f) * Time.deltaTime);
                    }
                }
            }
        }
        if (Input.GetMouseButton(1) && tileData.selectedObj) {
            cameraRotating = true;
            float _rotationX = 0f;
            float _RotationY = 0f;
            _rotationX = Input.GetAxis("Mouse X");
            _RotationY = Input.GetAxis("Mouse Y");

            if (_rotationX > 0) {
                transform.RotateAround(tileData.buffer0.transform.position, Vector3.up, (rotSpeed * 1.2f) * Time.deltaTime);
            }
            else if (_rotationX < 0) {
                transform.RotateAround(tileData.buffer0.transform.position, Vector3.up, -(rotSpeed * 1.2f) * Time.deltaTime);
            }
            if (axe == RotationAxe.MouseXAndY) {
                rotationY = Input.GetAxis("Mouse Y");
                rotationY = Mathf.Clamp(rotationY, minY, maxY);
                if (rotationY > 0) {
                    if (transform.localEulerAngles.x > 27) {
                        transform.Rotate(Vector3.left, (rotSpeed * 0.30f) * Time.deltaTime);
                    }
                }
                else if (rotationY < 0) {
                    if (transform.localEulerAngles.x < 75) {
                        transform.Rotate(Vector3.left, -(rotSpeed * 0.30f) * Time.deltaTime);
                    }
                }
            }
        }
        else {
            cameraRotating = false;
        }



    }


    private float Camera_groundDistance() {
        Ray ray = new Ray(_camera.transform.position, Vector3.down);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)) {
            return (hit.point - _camera.transform.position).magnitude;
        }
        return 0f;
    }

    private float ScrollMouse {
        get {
            return Input.GetAxis("Mouse ScrollWheel");
                }
    }

    void Camera_zoom() {
        float groundDistance = Camera_groundDistance();
        float minZoom = 0, maxZoom = 0;
        if (posData.buildModeToggle) {
            minZoom = zoomMinBuilder;
            maxZoom = zoomMaxBuilder;
        }
        else {
            minZoom = zoomMinCity;
            maxZoom = zoomMaxCity;
        }
        zoomPos += ScrollMouse * Time.deltaTime * zoomSpeed;
        zoomPos = Mathf.Clamp01(zoomPos);
        float height = Mathf.Lerp(minZoom, maxZoom, zoomPos);
        float dif = 0;

        if(groundDistance != height) {
            dif = height - groundDistance;
        }

        _camera.transform.position = Vector3.Lerp(_camera.transform.position,
            new Vector3(_camera.transform.position.x, height + dif, _camera.transform.position.z), Time.deltaTime * dampening);
    }

    void Camera_speed() {

    }
}
