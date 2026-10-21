package com.nivra.app.ui.auth

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.nivra.app.ServiceLocator
import com.nivra.app.data.ApiResult
import kotlinx.coroutines.launch

data class LoginUiState(
    val username: String = "",
    val password: String = "",
    val isSubmitting: Boolean = false,
    val error: String? = null,
)

class LoginViewModel : ViewModel() {

    private val repository = ServiceLocator.repository
    private val sessionStore = ServiceLocator.sessionStore

    var uiState by mutableStateOf(LoginUiState())
        private set

    fun onUsernameChange(value: String) {
        uiState = uiState.copy(username = value, error = null)
    }

    fun onPasswordChange(value: String) {
        uiState = uiState.copy(password = value, error = null)
    }

    fun submit(onSuccess: () -> Unit) {
        val state = uiState
        if (state.username.isBlank() || state.password.isBlank()) {
            uiState = state.copy(error = "Enter your username and password")
            return
        }
        if (state.isSubmitting) return

        uiState = state.copy(isSubmitting = true, error = null)
        viewModelScope.launch {
            when (val result = repository.login(state.username.trim(), state.password)) {
                is ApiResult.Success -> {
                    sessionStore.save(result.data)
                    uiState = uiState.copy(isSubmitting = false, password = "")
                    onSuccess()
                }

                is ApiResult.Failure ->
                    uiState = uiState.copy(isSubmitting = false, error = result.message)
            }
        }
    }
}