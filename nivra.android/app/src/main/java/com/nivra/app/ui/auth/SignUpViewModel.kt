package com.nivra.app.ui.auth

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.nivra.app.ServiceLocator
import com.nivra.app.data.ApiResult
import kotlinx.coroutines.launch

data class SignUpUiState(
    val username: String = "",
    val password: String = "",
    val confirmPassword: String = "",
    val isSubmitting: Boolean = false,
    val error: String? = null,
    val success: String? = null,
)

class SignUpViewModel : ViewModel() {

    private val repository = ServiceLocator.repository

    var uiState by mutableStateOf(SignUpUiState())
        private set

    fun onUsernameChange(value: String) {
        uiState = uiState.copy(username = value, error = null)
    }

    fun onPasswordChange(value: String) {
        uiState = uiState.copy(password = value, error = null)
    }

    fun onConfirmPasswordChange(value: String) {
        uiState = uiState.copy(confirmPassword = value, error = null)
    }

    fun submit() {
        val state = uiState
        if (state.isSubmitting) return

        val validationError = when {
            state.username.isBlank() || state.password.isBlank() ->
                "Enter a username and password"

            state.password != state.confirmPassword -> "Passwords do not match"

            else -> null
        }
        if (validationError != null) {
            uiState = state.copy(error = validationError)
            return
        }

        uiState = state.copy(isSubmitting = true, error = null, success = null)
        viewModelScope.launch {
            when (val result = repository.signUp(state.username.trim(), state.password)) {
                is ApiResult.Success -> uiState = uiState.copy(
                    isSubmitting = false,
                    success = result.data.trim('"'),
                    password = "",
                    confirmPassword = "",
                )

                is ApiResult.Failure ->
                    uiState = uiState.copy(isSubmitting = false, error = result.message)
            }
        }
    }
}