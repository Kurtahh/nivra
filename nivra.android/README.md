# Nivra for Android

Native Android client for the Nivra backend. It signs in with the **same username/password
account as the web app**, reads your step count from **Google Health Connect**, and pushes it
to the Nivra backend.

- Kotlin + Jetpack Compose (Material 3)
- Retrofit + OkHttp + Gson
- `androidx.health.connect:connect-client`
- `minSdk` 26 (Android 9), `compileSdk`/`targetSdk` 35

## Screens

| Screen | What it does |
| --- | --- |
| Log in | `POST api/User/Authenticate`, stores the session in `SharedPreferences` |
| Sign up | `POST api/User` — shares the web app's user table |
| Home | Today's steps from Health Connect, current total on Nivra, one-tap sync, daily health tip |
| Leaderboard | `GET api/leaderboard` with a Daily / Weekly / Monthly / All time selector |

No backend changes are required. The app uses the existing endpoints as-is, with the same
payloads `nivra.ui` sends.

## Setup

Requires **JDK 17** and the **Android SDK** (compileSdk 35). Neither is required to be installed
globally — Android Studio bundles its own JDK.

The Gradle wrapper JAR is not checked in. Generate it once (with a local Gradle 8.9+ install,
or let Android Studio do it on first open):

```bash
cd nivra.android
gradle wrapper --gradle-version 8.9
```

Then point `local.properties` at your SDK if Android Studio has not already:

```properties
sdk.dir=/home/<you>/Android/Sdk
```

Build and test:

```bash
./gradlew test          # unit tests
./gradlew assembleDebug # APK at app/build/outputs/apk/debug/
```

## Pointing the app at the backend

The base URL is baked in as `BuildConfig.API_BASE_URL` from the `nivra.apiBaseUrl` Gradle
property (see `gradle.properties`), defaulting to `http://10.0.2.2:5207/`.

- **Emulator** — `10.0.2.2` is your host machine's loopback, so the default works as long as
  the backend is running on this machine.
- **Physical device** — override per build:

  ```bash
  ./gradlew assembleDebug -Pnivra.apiBaseUrl=http://192.168.1.20:5207/
  ```

  Cleartext HTTP is permitted for `10.0.2.2`, `localhost` and `127.0.0.1` via
  `res/xml/network_security_config.xml`, and for all hosts in debug builds only. Release
  builds are HTTPS-only. **Add your LAN IP to the network security config** if you point a
  release build at a plain-HTTP server.

Start the backend first:

```bash
dotnet run --project ../nivra.api/backend.csproj
```

## Health Connect

Reading step count needs:

1. The **Health Connect provider** installed. On Android 14+ it ships in the system; on
   Android 9–13 it is a separate Play Store app. The app shows a banner with a link to
   install/update it when it is missing.
2. Both the Health Connect `Steps` read permission **and** `ACTIVITY_RECOGNITION`.
   `HealthManager.requiredPermissions` contains both and Health Connect's own permission
   contract requests them together.

On an emulator, use a **Google APIs** (or Google Play) system image — Health Connect is absent
from plain AOSP images, and the app will tell you so rather than crashing.

## Behaviour worth knowing

**Step syncing is monotonic by backend design.** `StepsService.LogSteps` rejects a count of 0,
and when an entry already exists for today `OverrideEntrySteps` throws unless the new count is
*strictly greater* than the stored one. So `HomeViewModel.sync()` only sends a value when
Health Connect is ahead of the server, and otherwise reports "already up to date". This means:

- A day's total can go up on Nivra but never down. Correcting your Health Connect data
  (deleting steps) will not lower the Nivra figure — clear the day's entry in the database if
  you need to reset it.
- Steps are logged for the **server's** current date (`DateTime.Now`), so syncing late in the
  evening attributes the day's total to that day, which matches Health Connect's local-day
  boundaries closely enough. Sending yesterday's steps is not possible with the current API.

**Authentication is the existing scheme.** The backend returns a bare `["message","true",id]`
array and issues no token, so the app just stores the user id. There is no bearer token on
requests, matching the web client. If the backend later moves to JWT, `SessionStore` is the
single place to add it.

**`UseHttpsRedirection` is enabled in `Program.cs`.** It only kicks in when an HTTPS port is
configured; the checked-in `launchSettings.json` defines none, so plain-HTTP development
requests are not redirected. Configure an HTTPS port before deploying the backend.