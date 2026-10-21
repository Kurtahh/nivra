package com.nivra.app.ui.leaderboard

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.nivra.app.ServiceLocator
import com.nivra.app.data.ApiResult
import com.nivra.app.data.LeaderboardEntryDto
import com.nivra.app.data.TimePeriod
import kotlinx.coroutines.launch

data class LeaderboardUiState(
    val period: TimePeriod = TimePeriod.Weekly,
    val entries: List<LeaderboardEntryDto> = emptyList(),
    val currentUserId: Long = 0,
    val isLoading: Boolean = true,
    val error: String? = null,
)

class LeaderboardViewModel : ViewModel() {

    private val repository = ServiceLocator.repository
    private val sessionStore = ServiceLocator.sessionStore

    var uiState by mutableStateOf(
        LeaderboardUiState(currentUserId = sessionStore.current?.userId ?: 0L),
    )
        private set

    init {
        load()
    }

    fun selectPeriod(period: TimePeriod) {
        uiState = uiState.copy(period = period)
        load()
    }

    fun load() {
        uiState = uiState.copy(isLoading = true, error = null)
        viewModelScope.launch {
            when (val result = repository.leaderboard(uiState.period)) {
                is ApiResult.Success -> uiState = uiState.copy(
                    entries = result.data,
                    isLoading = false,
                )

                is ApiResult.Failure ->
                    uiState = uiState.copy(isLoading = false, error = result.message)
            }
        }
    }
}