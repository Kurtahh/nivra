package com.nivra.app.data

import android.Manifest
import android.content.Context
import android.content.Intent
import androidx.activity.result.contract.ActivityResultContract
import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.PermissionController
import androidx.health.connect.client.permission.HealthPermission
import androidx.health.connect.client.records.StepsRecord
import androidx.health.connect.client.request.AggregateRequest
import androidx.health.connect.client.time.TimeRangeFilter
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import java.time.Instant
import java.time.LocalDate
import java.time.ZoneId

enum class HealthConnectAvailability {
    /** Health Connect is installed and usable. */
    Available,

    /** Installed but too old — offer an update. */
    UpdateRequired,

    /** Not available at all, e.g. an emulator image without Health Connect. */
    NotSupported,
}

/**
 * Thin wrapper around the Google Health Connect SDK.
 *
 * Only step count is read, because that is the only metric the Nivra backend
 * currently stores (nivra.api/Models/StepEntry.cs).
 */
class HealthManager(private val context: Context) {

    /**
     * Steps need both the Health Connect read permission and — on Android 10+ —
     * the ACTIVITY_RECOGNITION runtime permission. Health Connect's own permission
     * contract requests the Android one too when it is present in this set.
     */
    val requiredPermissions: Set<String> = setOf(
        HealthPermission.getReadPermission(StepsRecord::class),
        Manifest.permission.ACTIVITY_RECOGNITION,
    )

    fun availability(): HealthConnectAvailability =
        when (HealthConnectClient.getSdkStatus(context)) {
            HealthConnectClient.SDK_AVAILABLE -> HealthConnectAvailability.Available
            HealthConnectClient.SDK_UNAVAILABLE_PROVIDER_UPDATE_REQUIRED ->
                HealthConnectAvailability.UpdateRequired
            else -> HealthConnectAvailability.NotSupported
        }

    suspend fun hasAllPermissions(): Boolean =
        HealthConnectClient.getOrCreate(context)
            .permissionController
            .getGrantedPermissions()
            .containsAll(requiredPermissions)

    /**
     * One contract instance for the process — `rememberLauncherForActivityResult`
     * keys on the contract instance, so building a new one per composition would
     * relaunch the permission sheet.
     */
    val permissionContract: ActivityResultContract<Set<String>, Set<String>> by lazy {
        PermissionController.createRequestPermissionResultContract()
    }

    fun healthConnectSettingsIntent(): Intent =
        HealthConnectClient.getHealthConnectManageDataIntent(context)

    /**
     * Total steps recorded today across all sources, or 0 when there is no data.
     */
    suspend fun todaySteps(): Int = withContext(Dispatchers.IO) {
        val zone = ZoneId.systemDefault()
        val startOfDay = LocalDate.now().atStartOfDay(zone).toInstant()
        val end = Instant.now()

        val result = HealthConnectClient.getOrCreate(context).aggregate(
            AggregateRequest(
                metrics = setOf(StepsRecord.COUNT_TOTAL),
                timeRangeFilter = TimeRangeFilter.between(startOfDay, end),
            ),
        )

        result.get(StepsRecord.COUNT_TOTAL)?.toInt() ?: 0
    }
}