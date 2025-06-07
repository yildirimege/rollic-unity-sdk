# Rollic SDK for Unity

![Test, Build, and Deploy](https://github.com/yildirimege/rollic-unity-sdk/actions/workflows/build-sdk.yml/badge.svg)

This repository contains a simple yet robust SDK designed for the Rollic SDK Engineer case study. It provides functionality for tracking user session time and custom events, with a focus on data integrity, professional engineering practices, and easy integration.

The entire project, from unit testing and package creation to documentation deployment, is fully automated via a GitHub Actions CI/CD pipeline.

---

## Features

-   **Session Tracking:** Automatically tracks the user's total time spent in the application, correctly handling app pause and resume states.
-   **Custom Event Tracking:** Provides a simple and clean `RollicSDK.TrackEvent("your_event_name")` API.
-   **Robust Offline Persistence:** Events are saved to local device storage if the user is offline and are sent automatically when the connection is restored, ensuring no data is ever lost.
-   **Network Optimization:** Events are intelligently batched and sent with an exponential backoff retry strategy to handle API errors or network issues efficiently without spamming the server.
-   **Extensible Architecture:** Built with modern software design patterns (DI,Strategy, Adapters) to easily support new platforms or features in the future.
-   **Automated CI/CD Pipeline:**
    -   **Unit Tested:** A full suite of unit tests are run automatically on every push to the `main` branch, ensuring code quality and preventing regressions.
    -   **Automated Packaging:** The distributable `.unitypackage` is built automatically by a GitHub Action.
    -   **Automated Documentation:** The full API documentation is generated from source code comments using Doxygen and deployed automatically.

---

## Documentation

**The full, browsable API documentation is available at the following link:**

**[https://yildirimege.github.io/rollic-unity-sdk/](https://yildirimege.github.io/rollic-unity-sdk/)**

---

## Installation & Quick Start

Follow these steps to integrate the Rollic SDK into your Unity project.

#### 1. Download the Latest Release

Navigate to the **[Releases Page](https://github.com/yildirimege/rollic-unity-sdk/releases)** of this repository and download the `RollicSDK.unitypackage` from the latest release.

#### 2. Import the Package

Open your Unity project and import the package via the top menu: **Assets > Import Package > Custom Package...**. Select the `RollicSDK.unitypackage` file you just downloaded.

#### 3. Create the Configuration File

The SDK requires a configuration asset to function.

1.  In your Unity Project window, create a `Resources` folder inside your `Assets` directory if one doesn't already exist.
2.  Right-click inside the `Resources` folder and select **Create > Rollic SDK > Configuration**. This will create the required `RollicSDKConfig.asset` file.

#### 4. Use the SDK

The SDK initializes itself automatically. You can now track events from any of your game scripts.

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
4.  Enter Play Mode to use the interactive test harness, which allows you to send events and test offline functionality in real-time.
