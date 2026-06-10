using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceOnPlane : MonoBehaviour
{

    [SerializeField]
    //private GameObject cubePrefab;
    private GameObject[] _spawnedObjects;

    private int _spawnedObjectIndex = 0;

    private ARRaycastManager arRaycastManager;

    private List<ARRaycastHit> hitResults = new List<ARRaycastHit>();


    void Start()
    {

        arRaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {

        if (Input.touchCount > 0)
        {

            Touch touch = Input.GetTouch(0);


            if (touch.phase == TouchPhase.Began)
            {

                if (arRaycastManager.Raycast(touch.position, hitResults, TrackableType.PlaneWithinPolygon))
                {
                    _spawnedObjectIndex = (_spawnedObjectIndex + 1) % _spawnedObjects.Length;

                    Pose hitPose = hitResults[0].pose;
                    Instantiate(_spawnedObjects[_spawnedObjectIndex],hitPose.position, hitPose.rotation);
                }
            }
        }
    }
}