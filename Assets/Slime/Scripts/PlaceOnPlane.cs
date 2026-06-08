using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceOnPlane : MonoBehaviour
{

    [SerializeField]
    private GameObject cubePrefab;

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

                    Pose hitPose = hitResults[0].pose;
                    Instantiate(cubePrefab,hitPose.position, hitPose.rotation);
                }
            }
        }
    }
}