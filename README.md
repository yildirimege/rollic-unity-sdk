# Rollic SDK for Unity

![Test, Build, and Deploy](https://github.com/yildirimege/rollic-unity-sdk/actions/workflows/build-sdk.yml/badge.svg)

This repository contains a simple yet robust SDK designed for the Rollic SDK Engineer case study. It provides functionality for tracking user session time and custom events, with a focus on data integrity, professional engineering practices, and easy integration.

The entire project, from unit testing and package creation to documentation deployment, is fully automated via a GitHub Actions CI/CD pipeline.

---

## Features

-   **Session Tracking:** Automatically tracks the user's total time spent in the application, correctly handling app pause and resume states.
-   **Custom Event Tracking:** Provides a simple and clean `RollicSDK.TrackEvent("your_event_name")` API.
-   **Robust Offline Persistence:** Events are saved to local device storage if the user is offline and are sent automatically when the connection is restored, ensuring no data is ever lost.
-   **Network Optimization:** Events are intelligently batched and sent with an exponential backoff retry strategy to handle API errors or network issues efficiently.
-   **Extensible Architecture:** Built with modern software design patterns (DI, Strategy, Adapters) to easily support new platforms or features in the future.
-   **Automated CI/CD Pipeline:**
    -   **Unit Tested:** A full suite of unit tests are run automatically on every push to the `main` branch.
    -   **Automated Packaging:** The distributable `.unitypackage` is built automatically for legacy support.
    -   **Automated Documentation:** The API documentation is generated and deployed automatically from source code comments.

---

## Documentation

**The full, browsable API documentation is available at the following link:**

**[https://yildirimege.github.io/rollic-unity-sdk/](https://yildirimege.github.io/rollic-unity-sdk/)**

---

## Installation

This SDK can be installed via the Unity Package Manager (UPM) using a Git URL, which is the recommended method. A traditional `.unitypackage` is also provided for fallback.

### Recommended: Install via Unity Package Manager (UPM)

This method keeps the SDK organized in your project's `Packages` folder and makes version management easy.

1.  In your Unity project, open the **Package Manager** by going to **Window > Package Manager**.
2.  Click the **`+`** icon in the top-left corner of the window.
3.  Select **"Add package from git URL..."**.
4.  Paste the following URL into the text box and click **Add**:

    ```
    https://github.com/yildirimege/rollic-unity-sdk.git#v1.0.0
    ```
    *(To use a different version, simply change the `#v1.0.0` part of the URL to the desired version tag from the [Releases Page](https://github.com/yildirimege/rollic-unity-sdk/releases).)*

    ![UPM Git URL Installation](https://user-images.githubusercontent.com/1090646/158942323-83244198-4c3e-42b7-873b-5544627198d2.png)

### Alternative: Install via `.unitypackage`

1.  Navigate to the **[Releases Page](https://github.com/yildirimege/rollic-unity-sdk/releases)**.
2.  Download the `RollicSDK.unitypackage` from the assets of the latest release.
3.  Open your Unity project and import the package via the top menu: **Assets > Import Package > Custom Package...**.

---

## Quick Start After Installation

After installing the SDK via either method, you must create a configuration file.

1.  In your Unity Project window, create a `Resources` folder inside your `Assets` directory if one doesn't already exist.
2.  Right-click inside the `Resources` folder and select **Create > Rollic SDK > Configuration**. This will create the required `RollicSDKConfig.asset` file.
3.  The SDK is now ready to use! It will initialize itself automatically when the game starts.

**Example Usage:**

```csharp
using UnityEngine;
using RollicSDK; // 1. Import the SDK's namespace

public class MyGameController : MonoBehaviour
{
    void Start()
    {
        // 2. You can start tracking events right away.
        RollicSDK.TrackEvent("game_start");
    }

    public void OnPlayerLevelUp()
    {
        // 3. Track any custom event you need.
        RollicSDK.TrackEvent("player_level_up");
    }
}
```

---

## Sample Project

This repository is a complete, working Unity project. To see a live demonstration of all SDK features:

1.  Clone this repository to your local machine.
2.  Open the project in a compatible version of Unity (e.g., `2022.3.21f1`).
3.  Open the scene at **`Assets/Sample/Scenes/SampleScene.unity`**.
4.  Enter Play Mode to use the interactive test harness.
