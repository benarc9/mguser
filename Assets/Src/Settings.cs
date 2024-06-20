
using UnityEngine;
using Constants;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    private static readonly string TAG = "Settings";
    
    [Space]
    [Header("Object References")]
    public TrackableManager trackableManager;
    public Dropdown trackingModeSelector;

    [Space]
    public AnchorType anchorType;

    private void Start()
    {
        trackableManager = GameObject.FindGameObjectWithTag(Tag.SessionOrigin.ToString()).GetComponent<TrackableManager>();
        trackingModeSelector = GameObject.FindGameObjectWithTag(Tag.TrackingModeSelector.ToString()).GetComponent<Dropdown>();

        StartPlaneManager();

        Debug.Log("Build GUID" + Application.buildGUID);
        Debug.Log("Console Log Path" + Application.consoleLogPath);
        Debug.Log("Data Path" + Application.dataPath);
        Debug.Log("Bundle Identifier" + Application.identifier);
        Debug.Log("Persistent Data Path" + Application.persistentDataPath);
        Debug.Log("Streaming Assets Path" + Application.streamingAssetsPath);
    }

    public void AnchorTypeChanged()
    {
        StopManagers();
        Debug.Log(TAG + ": Tracking mode changed to " + trackingModeSelector.value.ToString());
        switch(trackingModeSelector.value.ToString())
        {
            case "Plane":
                anchorType = AnchorType.Plane;
                Debug.Log(TAG + ": Plane Tracking selected");
                StartPlaneManager();
                break;

            case "Object":
                anchorType = AnchorType.Object;
                Debug.Log(TAG + ": Object tracking selected");
                StartObjectManager();
                break;

            case "Image":
                anchorType = AnchorType.Image;
                Debug.Log(TAG + ": Image tracking selected");
                StartImageManager();
                break;

            case "Point":
                anchorType = AnchorType.Point;
                Debug.Log(TAG + ": Point tracking selected");
                StartPointManager();
                break;

            default:
                return;
        }
    }

    private void StartPlaneManager()
    {
        trackableManager.StartManager(Constants.AnchorType.Plane);
    }

    private void StartImageManager()
    {
        trackableManager.StartManager(Constants.AnchorType.Image);
    }
    
    private void StartObjectManager()
    {
        trackableManager.StartManager(Constants.AnchorType.Object);
    }

    private void StartPointManager()
    {
        trackableManager.StartManager(Constants.AnchorType.Point);
    }

    private void StopManagers()
    {
        Debug.Log(TAG + ": Stopping all trackable managers");
        trackableManager.StopAllManagers();
    }
}
