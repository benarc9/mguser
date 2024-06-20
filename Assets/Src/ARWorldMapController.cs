using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

using Constants;
using UnityEngine.XR.ARKit;
using Uploadcare;
using System.Net.Http;

/// <summary>
/// Demonstrates the saving and loading of an
/// <a href="https://developer.apple.com/documentation/arkit/arworldmap">ARWorldMap</a>
/// </summary>
/// <remarks>
/// ARWorldMaps are only supported by ARKit, so this API is in the
/// <c>UntyEngine.XR.ARKit</c> namespace.
/// </remarks>
public class ARWorldMapController : MonoBehaviour
{
    private readonly static string TAG = "ARWorldMapController";
    

    #region Properties
        [Tooltip("The ARSession component controlling the session from which to generate ARWorldMaps.")]
        [SerializeField]
        ARSession m_ARSession;
        /// <summary>
        /// The ARSession component controlling the session from which to generate ARWorldMaps.
        /// </summary>
        public ARSession arSession
        {
            get { return m_ARSession; }
            set { m_ARSession = value; }
        }

        [Tooltip("A UI button component which will generate an ARWorldMap and save it to disk.")]
        [SerializeField]
        Button m_SaveButton;
        /// <summary>
        /// A UI button component which will generate an ARWorldMap and save it to disk.
        /// </summary>
        public Button saveButton
        {
            get { return m_SaveButton; }
            set { m_SaveButton = value; }
        }

        [Tooltip("A UI button component which will load a previously saved ARWorldMap from disk and apply it to the current session.")]
        [SerializeField]
        Button m_LoadButton;
        /// <summary>
        /// A UI button component which will load a previously saved ARWorldMap from disk and apply it to the current session.
        /// </summary>
        public Button loadButton
        {
            get { return m_LoadButton; }
            set { m_LoadButton = value; }
        }

        [Tooltip("A UI button component which will reset the AR World Map.")]
        [SerializeField]
        Button m_ResetButton;
        /// <summary>
        /// A UI button component which will reset the AR World.
        /// </summary>
        public Button resetButton
        {
            get { return m_ResetButton; }
            set { m_ResetButton = value; }
        }

        private Settings m_Settings;
        public Settings settings
        {
            get { return m_Settings; }
            set { m_Settings = value; }
        }

        private List<string> m_LogMessages;
        public List<string> logMessages
        {
            get { return m_LogMessages; }
            set { m_LogMessages = value; }
        }

        public string path
        {
            get { return Path.Combine(Application.persistentDataPath, "my_session.worldmap"); }
        }

        public ARWorldMap worldMap;
        
    #endregion
    

    #region Unity Lifecycle Methods
        void Awake()
        {
            logMessages = new List<string>();
        }

        void Start()
        {
            saveButton = GameObject.FindGameObjectWithTag(Tag.SaveWorldButton.ToString()).GetComponent<Button>();
            // loadButton = GameObject.FindGameObjectWithTag(Tag.LoadWorldButton.ToString()).GetComponent<Button>();
            // resetButton = GameObject.FindGameObjectWithTag(Tag.ResetWorldButton.ToString()).GetComponent<Button>();
            arSession = GameObject.FindGameObjectWithTag(Tag.Session.ToString()).GetComponent<ARSession>();
        
            
        }

        void Update()
        {
            var sessionSubsystem = arSession.subsystem;

            if (sessionSubsystem == null)
            {
                return;
            }
            var numLogsToShow = 20;
            string msg = "";

            for (int i = Mathf.Max(0, logMessages.Count - numLogsToShow); i < logMessages.Count; ++i)
            {
                msg += logMessages[i];
                msg += "\n";
            }
        }
    #endregion

    #region Methods
        /// <summary>
        /// Create an <c>ARWorldMap</c> and save it to disk.
        /// </summary>
        public void OnSaveButton()
        {
            StartCoroutine(Save());
        }

        /// <summary>
        /// Load an <c>ARWorldMap</c> from disk and apply it
        /// to the current session.
        /// </summary>
        public void OnLoadButton()
        {
            Debug.Log("Attempting to load world mapVV...");
            StartCoroutine(Load());
        }

        /// <summary>
        /// Reset the <c>ARSession</c>, destroying any existing trackables,
        /// such as planes. Upon loading a saved <c>ARWorldMap</c>, saved
        /// trackables will be restored.
        /// </summary>
        public void OnResetButton()
        {
            arSession.Reset();
        }

        private IEnumerator Save()
        {
            var sessionSubsystem = (ARKitSessionSubsystem)arSession.subsystem;
            if (sessionSubsystem == null)
            {
                Debug.Log("No session subsystem available. Could not save.");
                yield break;
            }

            var request = sessionSubsystem.GetARWorldMapAsync();

            while (!request.status.IsDone())
                yield return null;

            if (request.status.IsError())
            {
                Debug.Log(string.Format("Session serialization failed with status {0}", request.status));
                yield break;
            }

            var worldMap = request.GetWorldMap();
            request.Dispose();

            SaveAndDisposeWorldMap(worldMap);
        }

        IEnumerator Load()
        {
            var sessionSubsystem = (ARKitSessionSubsystem)arSession.subsystem;
            if (sessionSubsystem == null)
            {
                Debug.Log("No session subsystem available. Could not load.");
                yield break;
            }

            var file = File.Open(path, FileMode.Open);
            if (file == null)
            {
                Debug.Log(string.Format("File {0} does not exist.", path));
                yield break;
            }

            Debug.Log(string.Format("Reading {0}...", path));
            int bytesPerFrame = 1024 * 10;
            var bytesRemaining = file.Length;
            var binaryReader = new BinaryReader(file);
            var allBytes = new List<byte>();
            
            while (bytesRemaining > 0)
            {
                var bytes = binaryReader.ReadBytes(bytesPerFrame);
                allBytes.AddRange(bytes);
                bytesRemaining -= bytesPerFrame;
                yield return null;
            }
        
            var data = new NativeArray<byte>(allBytes.Count, Allocator.Temp);
            data.CopyFrom(allBytes.ToArray());
            Debug.Log(string.Format("Deserializing to ARWorldMap...", path));
            
            if (ARWorldMap.TryDeserialize(data, out worldMap))
            {
                data.Dispose();
                if (worldMap.valid)
                {
                    Debug.Log("Deserialized successfully.");
                }
                else
                {
                    Debug.Log(TAG + ": Data is not a valid ARWorldMap.");
                    yield break;
                }
                Debug.Log("Apply ARWorldMap to current session.");
                sessionSubsystem.ApplyWorldMap(worldMap);
            }
        }

    void LoadFromResources(){
        Debug.Log("Loading map from resources...");
        var map = Resources.Load("init_map.worldmap");
    }

    void SaveToRemote(string fileName){
        Debug.Log("!!!---[" + TAG + "]----: Attempting to save file named: " + fileName + " to remote Uploadcare server");
        Uploadcare.Configuration config = new Uploadcare.Configuration(UploadCare.PUB_KEY, UploadCare.PRIV_KEY);
        HttpClient httpClient = new HttpClient();
        Uploadcare.UploadClient client = new(config, httpClient);
        FileInfo fileInfo = new FileInfo(fileName);
        client.UploadAsync(fileInfo);
    }

    void SaveAndDisposeWorldMap(ARWorldMap worldMap)
    {
        Debug.Log("Serializing ARWorldMap to byte array...");
        var data = worldMap.Serialize(Allocator.Temp);
        Debug.Log(string.Format("ARWorldMap has {0} bytes.", data.Length));
        var file = File.Open(path, FileMode.Create);
        var writer = new BinaryWriter(file);
        SaveToRemote(file.Name);
        writer.Write(data.ToArray());
        writer.Close();
        data.Dispose();
        worldMap.Dispose();
        Debug.Log(string.Format("ARWorldMap written to {0}", path));
    }
    #endregion
}