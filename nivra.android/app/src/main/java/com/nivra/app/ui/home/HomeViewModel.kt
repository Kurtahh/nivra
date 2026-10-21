package com.nivra.app.ui.home

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.nivra.app.ServiceLocator
import com.nivra.app.data.ApiResult
import com.nivra.app.data.HealthConnectAvailability
import com.nivra.app.data.HealthTipDto
import kotlinx.coroutines.launch

data class HomeUiState(
    val username: String = "",
    val availability: HealthConnectAvailability = HealthConnectAvailability.NotSupported,
    val hasPermission: Boolean = false,
    /** Steps for today from Health Connect; null until permissions are granted. */
    val healthConnectSteps: Int? = null,
    /** Steps for today as the Nivra backend has them. */
    val serverSteps: Int = 0,
    val tip: HealthTipDto? = null,
    val isLoading: Boolean = true,
    val isSyncing: Boolean = false,
    val error: String? = null,
    val info: String? = null,
)

class HomeViewModel : ViewModel() {

    private val repository = ServiceLocator.repository
    private val healthManager = ServiceLocator.healthManager
    private val sessionStore = ServiceLocator.sessionStore

    var uiState by mutableStateOf(HomeUiState())
        private set

    init {
        uiState = uiState.copy(username = sessionStore.current?.username.orEmpty())
        refresh()
    }

    fun refresh() {
        val userId = sessionStore.current?.userId ?: return

        val availability = healthManager.availability()

        uiState = uiState.copy(availability = availability, isLoading = true)

        viewModelScope.launch {
            val hasPermission =
                availability == HealthConnectAvailability.Available && healthManager.hasAllPermissions()
            uiState = uiState.copy(hasPermission = hasPermission)

            val healthConnectSteps = if (hasPermission) healthManager.todaySteps() else null
            val tip = when (val result = repository.healthTip()) {
                is ApiResult.Success -> result.data
                is ApiResult.Failure -> null
            }

            when (val serverSteps = repository.todaySteps(userId)) {
                is ApiResult.Success -> uiState = uiState.copy(
                    healthConnectSteps = healthConnectSteps,
                    serverSteps = serverSteps.data,
                    tip = tip,
                    isLoading = false,
                    error = null,
                )

                is ApiResult.Failure -> uiState = uiState.copy(
                    healthConnectSteps = healthConnectSteps,
                    tip = tip,
                    isLoading = false,
                    error = serverSteps.message,
                )
            }
        }
    }

    /**
     * Pushes today's Health Connect step count to the backend.
     *
     * Note the backend is deliberately monotonic here: `StepsService.LogSteps`
     * rejects counts <= 0, and when an entry already exists for today
     * `OverrideEntrySteps` throws unless the new count is *strictly greater*
     * than the stored one. So a sync is only ever sent when Health Connect is
     * ahead of the server; otherwise it is a no-op.
     */
    fun sync() {
        val userId = sessionStore.current?.userId ?: return
        val healthConnectSteps = uiState.healthConnectSteps
        val serverSteps = uiState.serverSteps

        when {
            healthConnectSteps == null ->
                uiState = uiState.copy(info = "Grant Health Connect access to sync steps.")

            healthConnectSteps <= 0 ->
                uiState = uiState.copy(info = "No steps recorded today yet.")

            healthConnectSteps <= serverSteps ->
                uiState = uiState.copy(info = "Nivra is already up to date ($serverSteps steps).")

            uiState.isSyncing -> return

            else -> {
                uiState = uiState.copy(isSyncing = true, info = null, error = null)
                viewModelScope.launch {
                    when (val result = repository.logSteps(healthConnectSteps, userId)) {
                        is ApiResult.Success -> uiState = uiState.copy(
                            isSyncing = false,
                            serverSteps = healthConnectSteps,
                            info = "Synced $healthConnectSteps steps to Nivra.",
                        )

                        is ApiResult.Failure ->
                            uiState = uiState.copy(isSyncing = false, error = result.message)
                    }
                }
            }
        }
    }

    fun clearMessages() {
        uiState = uiState.copy(error = null, info = null)
    }

    fun logout(onLoggedOut: () -> Unit) {
        sessionStore.clear()
        onLoggedOut()
    }
}