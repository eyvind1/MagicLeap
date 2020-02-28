using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class PlaceObject : MonoBehaviour
{
    public GameObject objectToPlace;
    private MLInputController controller;
    // Start is called before the first frame update
    void Start()
    {
        MLInput.Start();
        MLInput.OnControllerButtonDown += OnButtonDown;
        controller = MLInput.GetController(MLInput.Hand.Left);
    }

    void OnButtonDown(byte controller_id, MLInputControllerButton button)
    {
        if(button == MLInputControllerButton.Bumper)
        {
            RaycastHit hit;
            if(Physics.Raycast(controller.Position,transform.forward, out hit))
            {
                GameObject PlaceObject = Instantiate(objectToPlace, hit.point, Quaternion.Euler(hit.normal));
            }
        }
    }

    private void OnDestroy()
    {
        MLInput.Stop();
        MLInput.OnControllerButtonDown -= OnButtonDown;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
