using UnityEngine;
using RollicSDK; // Use the top-level namespace for the facade
using System.Collections.Generic; // Required for using List<T>
using RollicSDK.Core;

/// <summary>
/// A comprehensive and scalable test harness for the Rollic SDK.
/// This script provides an in-game UI to demonstrate and test all core functionalities.
/// The event list is editable in the Inspector, and the UI is scrollable.
///
/// HOW TO USE:
/// 1. Ensure you have a 'RollicSDKConfig.asset' file in a 'Resources' folder.
/// 2. Create a new scene in Unity.
/// 3. Create an empty GameObject and name it "SDK_Tester".
/// 4. Attach this 'SDKTestHarness.cs' script to the "SDK_Tester" GameObject.
/// 5. In the Inspector, you can add or remove event names from the 'Events To Test' list.
/// 6. Press Play.
/// </summary>
public class SDKTestHarness : MonoBehaviour
{
    // --- Public fields for easy editing in the Inspector ---
    [Header("Events to Test")]
    [Tooltip("Add any event names you want to test here. They will appear as buttons.")]
    public List<string> eventNamesToTest = new List<string>();

    // --- Private fields for UI state ---
    private bool _sdkInitialized = false;
    private string _sessionDurationText = "0.00s";
    private int _queuedEventCount = 0;
    private Vector2 _eventListScrollPos;

    // --- Styles for the UI ---
    private GUIStyle _labelStyle, _headerStyle, _subHeaderStyle, _boxStyle;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        // Populate the list with default values if it's empty in the Inspector.
        // This ensures the harness is usable immediately without any setup.
        if (eventNamesToTest.Count == 0)
        {
            eventNamesToTest.Add("level_start");
            eventNamesToTest.Add("level_complete");
            eventNamesToTest.Add("ad_impression_rewarded");
            eventNamesToTest.Add("iap_purchase_small_gem_pack");
            eventNamesToTest.Add("tutorial_step_1_complete");
            eventNamesToTest.Add("social_share_button_click");
            eventNamesToTest.Add("settings_menu_opened");
            eventNamesToTest.Add("player_death");
            eventNamesToTest.Add("item_crafted_sword");
        }

        Debug.Log("[SDKTestHarness] Test script is running. SDK will initialize automatically.");
    }

    /// <summary>
    /// Called every frame. We use this to continuously poll the SDK for the latest data.
    /// </summary>
    void Update()
    {
        if (!_sdkInitialized)
        {
            _sdkInitialized = RollicSDK.RollicSDK.IsInitialized();
        }

        if (_sdkInitialized)
        {
            _sessionDurationText = $"{RollicSDK.RollicSDK.GetSessionDuration():F2}s";
            _queuedEventCount = RollicSDK.RollicSDK.GetQueuedEventCount();
        }
    }

    /// <summary>
    /// Called for rendering and handling GUI events.
    /// </summary>
    void OnGUI()
    {
        if (_labelStyle == null) InitializeStyles();

        // Use a main layout area for consistent padding around the screen edges.
        GUILayout.BeginArea(new Rect(20, 20, Screen.width - 40, Screen.height - 40));

        GUILayout.Label("Rollic SDK Test Harness", _headerStyle);
        GUILayout.Space(15);

        // --- Status Section ---
        GUILayout.BeginVertical(_boxStyle);
        GUILayout.Label("Live Status", _subHeaderStyle);
        string statusText = _sdkInitialized ? "<color=green>Initialized</color>" : "<color=red>Not Initialized</color>";
        GUILayout.Label($"SDK Status: {statusText}", _labelStyle);

        if (_sdkInitialized)
        {
            GUILayout.Label($"Session Duration: {_sessionDurationText}", _labelStyle);
            GUILayout.Label($"Events in Queue: {_queuedEventCount}", _labelStyle);
        }
        GUILayout.EndVertical();

        GUILayout.Space(15);

        // --- Testing Controls Section ---
        GUILayout.BeginVertical(_boxStyle);
        GUILayout.Label("Testing Controls", _subHeaderStyle);
        var sdkConfig = Resources.Load<RollicSDKConfig>("RollicSDKConfig");
        if (sdkConfig != null)
        {
            // This toggle directly reads and WRITES to the ScriptableObject asset at runtime.
            sdkConfig.MockOfflineMode = GUILayout.Toggle(sdkConfig.MockOfflineMode, " Mock Offline Mode", GUILayout.Height(30));
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Log Persistent Data Path", GUILayout.Height(45)))
        {
            Debug.Log("<b>[SDKTestHarness] Persistent Data Path:</b> " + Application.persistentDataPath);
        }
        GUILayout.EndVertical();

        GUILayout.Space(15);

        if (_sdkInitialized)
        {
            // --- SCROLLABLE EVENT TRIGGERS SECTION ---
            GUILayout.BeginVertical(_boxStyle);
            GUILayout.Label("Event Triggers", _subHeaderStyle);

            // Start the scrollable area
            _eventListScrollPos = GUILayout.BeginScrollView(
                _eventListScrollPos,
                false, // Horizontal scrollbar off
                true,  // Vertical scrollbar on
                GUILayout.MaxHeight(250) // Set a max height for the scroll view area
            );

            // Automatically create a button for each event in our public list
            foreach (string eventName in eventNamesToTest)
            {
                if (GUILayout.Button($"Track '{eventName}'", GUILayout.Height(45)))
                {
                    TrackAndLogEvent(eventName);
                }
            }

            // End the scrollable area
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        GUILayout.EndArea();
    }

    /// <summary>
    /// A helper method to track an event and log it to the console.
    /// </summary>
    private void TrackAndLogEvent(string eventName)
    {
        if (!_sdkInitialized)
        {
            Debug.LogError("[SDKTestHarness] Cannot track event because SDK is not initialized.");
            return;
        }

        // Core call to the SDK.
        RollicSDK.RollicSDK.TrackEvent(eventName);

        // Confirmation log.
        Debug.Log($"[SDKTestHarness] Sent '{eventName}' event to the Rollic SDK queue.");
    }

    /// <summary>
    /// Initializes the GUIStyle objects used for the UI.
    /// This is called only once to avoid performance overhead in OnGUI.
    /// </summary>
    private void InitializeStyles()
    {
        _boxStyle = new GUIStyle(GUI.skin.box);
        _boxStyle.padding = new RectOffset(15, 15, 15, 15);

        _labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleLeft,
            wordWrap = true,
            richText = true // Important for color tags
        };
        _headerStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 26,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        _subHeaderStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };
    }
}