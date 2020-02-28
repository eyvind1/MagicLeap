using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.UI;

[RequireComponent(typeof(ControllerConnectionHandler))]
public class ImageTracking : MonoBehaviour
{
    [SerializeField]
    private Text debugLog;
    [SerializeField]
    private Text imageTrackingStatus;
    [SerializeField]
    private MLImageTrackerBehavior imageTrackerBehaviour;

    [SerializeField]
    private GameObject targetPrefab;

    private GameObject targetObject;

    private ControllerConnectionHandler connectionHandler;
    private PrivilegeRequester privilegeRequester;
    
    private bool startTracking = false;
    void Awake()
    {
        connectionHandler = GetComponent<ControllerConnectionHandler>();
        if(connectionHandler == null)
        {
            Debug.Log("Error ");
            enabled = false;
            return;
        }

        // camera editor
        privilegeRequester = GetComponent<PrivilegeRequester>();
        debugLog.text = "Setting up Onprivilegesdone binding    ";
        //antes de iniciar los objetos mltracker la escena deberia esperar
        privilegeRequester.OnPrivilegesDone += HandlePrivilegesDone; 
    }
    // Start is called before the first frame update
    void Start()
    {
        MLInput.OnTriggerDown += HandleOnTriggerDown;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HandlePrivilegesDone(MLResult result)
    {
        if(!result.IsOk)
        {
            if(result.Code == MLResultCode.PrivilegeDenied)
            {
                Instantiate(Resources.Load("PrivilegeDeniedError"));
            }
            debugLog.text = $"Error: ImageTrackingExample failed to get requested privileges, disabling script. Reason{result}";
            enabled = false;
            return;
        }
        debugLog.text = $"Succeeded in requesting all privileges";
        StartCapture();
    }

    private void StartCapture()
    {
        imageTrackerBehaviour.gameObject.SetActive(true);
        imageTrackerBehaviour.enabled = true;
        imageTrackerBehaviour.OnTargetFound += OnTargetFound;
        imageTrackerBehaviour.OnTargetLost += OnTargetLost;
        imageTrackerBehaviour.OnTargetUpdated += OnTargetUpdated;
    }

    private void OnTargetFound(bool isReliable)
    {
        debugLog.text = isReliable ? "the target is reliable":  "the trget is not reliable";
    }
    private void OnTargetLost()
    {
        debugLog.text = "the target was lost";
    }
    private void OnTargetUpdated(MLImageTargetResult imageTargetResult)
    {
        if(!startTracking)
        {
            debugLog.text = "start capturing  is currently disabled";
            targetObject.SetActive(startTracking);
            return;
        }
        targetObject.SetActive(startTracking);
        imageTrackingStatus.text = imageTargetResult.Status.ToString();

        if(targetObject == null)
        {
            targetObject = Instantiate(targetPrefab,imageTargetResult.Position,imageTargetResult.Rotation);
        }
        else
        {
            targetObject.transform.SetPositionAndRotation(imageTargetResult.Position,imageTargetResult.Rotation);
        }
       
    }

    private void HandleOnTriggerDown(byte controllerId,  float value)
    {
        MLInputController controller = connectionHandler.ConnectedController;
        if(controller != null && controller.Id == controllerId)
        {
            MLInputControllerFeedbackIntensity intensity = (MLInputControllerFeedbackIntensity)((int)value*2.0f);
            controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Buzz,intensity);
            debugLog.text = "Start capturing has been enables by the trigger button action";
            startTracking = true;
        }
    }
    void OnDestroy()
    {
        MLInput.OnTriggerDown -= HandleOnTriggerDown;
        imageTrackerBehaviour.OnTargetFound -= OnTargetFound;
        imageTrackerBehaviour.OnTargetLost -= OnTargetLost;
        imageTrackerBehaviour.OnTargetUpdated -= OnTargetUpdated;
    }
}
