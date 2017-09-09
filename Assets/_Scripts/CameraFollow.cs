using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メインカメラ関連
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour {

    [SerializeField]
    private GameObject ball = null;

    [SerializeField]
    private float initSize = 10f;

    [SerializeField]
    private float zoomScale = 10f;

    private float zOffset = -10f;
    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _camera.orthographicSize = initSize;
    }

    // Update is called once per frame
    void Update () {
        float x = ball.transform.position.x;
        float y = ball.transform.position.y;
        transform.position = new Vector3(x, y, zOffset);
	}

    public void ZoomOut(float ballSize){
        _camera.orthographicSize += ballSize * zoomScale;
    }
}
