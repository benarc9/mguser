using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

using Constants;

public class TrackableManager : MonoBehaviour
{
    private readonly string TAG = "TrackableManager";
    public ARAnchorManager anchorManager;
    public ARPlaneManager planeManager;
    public ARPointCloudManager pointCloudManager;
    public ARTrackedImageManager imageManager;
    public ARTrackedObjectManager objectManager;
    public RegisteredAnchor[] existingAnchors;
    public int index = 0;
    public Object soundAnchorPrefab;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        GameObject sessionOrigin = GameObject.FindGameObjectWithTag(Tag.SessionOrigin.ToString());
        anchorManager = sessionOrigin.GetComponent<ARAnchorManager>();
        planeManager = sessionOrigin.GetComponent<ARPlaneManager>();
        pointCloudManager = sessionOrigin.GetComponent<ARPointCloudManager>();
        imageManager = sessionOrigin.GetComponent<ARTrackedImageManager>();
        objectManager = sessionOrigin.GetComponent<ARTrackedObjectManager>();
    }

    public void StopManager(Constants.AnchorType type)
    {
        switch(type)
        {
            case Constants.AnchorType.Plane:
                planeManager.SetTrackablesActive(false);
                planeManager.gameObject.SetActive(false);
                break;

            case Constants.AnchorType.Point:
                pointCloudManager.SetTrackablesActive(false);
                pointCloudManager.gameObject.SetActive(false);
                break;

            case Constants.AnchorType.Image:
                imageManager.SetTrackablesActive(false);
                imageManager.gameObject.SetActive(false);
                break;

            case Constants.AnchorType.Object:
                objectManager.SetTrackablesActive(false);
                objectManager.gameObject.SetActive(false);
                break;
        }
    }

    public void StartManager(Constants.AnchorType type)
    {
        switch (type)
        {
            case Constants.AnchorType.Plane:
                planeManager.SetTrackablesActive(true);
                planeManager.gameObject.SetActive(true);
                break;

            case Constants.AnchorType.Point:
                pointCloudManager.SetTrackablesActive(true);
                pointCloudManager.gameObject.SetActive(true);
                break;

            case Constants.AnchorType.Image:
                imageManager.SetTrackablesActive(true);
                imageManager.gameObject.SetActive(true);
                break;

            case Constants.AnchorType.Object:
                objectManager.SetTrackablesActive(true);
                objectManager.gameObject.SetActive(true);
                break;
        }
    }

    public void StopAllManagers()
    {
        planeManager.SetTrackablesActive(false);
        planeManager.gameObject.SetActive(false);
        pointCloudManager.SetTrackablesActive(false);
        pointCloudManager.gameObject.SetActive(false);
        imageManager.SetTrackablesActive(false);
        imageManager.gameObject.SetActive(false);
        objectManager.SetTrackablesActive(false);
        objectManager.gameObject.SetActive(false);
    }

    public void AddAnchorPayload(Object soundAnchorPrefab)
    {
        this.soundAnchorPrefab = soundAnchorPrefab;
    }

    public void AnchorContentToPlane(TrackableId planeId, Pose pose)
    {
        if (anchorManager == null)
        {
            Debug.Log(TAG + ": Unable to anchor content, Anchor Manager reference is null");
        }
        else
        {
            if (soundAnchorPrefab != null)
            {
                ARPlane plane = GetPlane(planeId);

                if(plane != null)
                {
                    ARAnchor anchor = anchorManager.AttachAnchor(plane, pose);
                    GameObject soundAnchor = Instantiate(soundAnchorPrefab, pose.position, pose.rotation) as GameObject;
                    soundAnchor.transform.SetParent(anchor.transform);
                    
                }
                else
                {
                    Debug.Log(TAG + ": Failed to get the ARPlane object from ARPlaneManager based on the trackable id given");
                }
                
            }
            else
            {
                Debug.Log(TAG + ": SoundAnchor prefab was not set. Cannot attach empty sound anchor to plane!");
            }
        }
    }

    public bool IsAnchorLoaded()
    {
        return !(soundAnchorPrefab == null);
    }

    public ARPlane GetPlane(TrackableId planeId)
    {
        return planeManager.GetPlane(planeId);
    }

    public void RegisterAnchor(TrackableId trackable, GameObject anchor)
    {
        RegisteredAnchor newAnchor = new RegisteredAnchor(trackable, anchor);
        existingAnchors[index] = newAnchor;
    }

    public class RegisteredAnchor
    {   
        public TrackableId trackable;
        public GameObject anchor;


        public RegisteredAnchor(TrackableId trackable, GameObject anchor)
        {
            this.trackable = trackable;
            this.anchor = anchor;
        }
    }
}
