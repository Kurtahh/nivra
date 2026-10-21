package com.nivra.app.data

import com.google.gson.JsonParser
import retrofit2.Response
import java.io.IOException

sealed interface ApiResult<out T> {
    data class Success<T>(val data: T) : ApiResult<T>
    data class Failure(val message: String) : ApiResult<Nothing>
}

/** Mirrors the backend's TimePeriod enum. */
enum class TimePeriod(val apiValue: String, val label: String) {
    Daily("Daily", "Today"),
    Weekly("Weekly", "This week"),
    Monthly("Monthly", "This month"),
    AllTime("AllTime", "All time"),
}

class NivraRepository(private val api: NivraApi) {

    suspend fun signUp(username: String, password: String): ApiResult<String> =
        call({ api.signUp(Credentials(username, password)) }) { body ->
            body.string()
        }

    suspend fun login(username: String, password: String): ApiResult<Session> =
        when (val result = call({ api.authenticate(Credentials(username, password)) }) { it }) {
            is ApiResult.Failure -> result
            is ApiResult.Success -> {
                // ["<message>", "true", "<userId>"]
                val parts = result.data
                val userId = parts.getOrNull(2)?.toLongOrNull()
                if (parts.getOrNull(1) == "true" && userId != null) {
                    ApiResult.Success(Session(userId = userId, username = username))
                } else {
                    ApiResult.Failure(parts.getOrNull(0) ?: "Login failed")
                }
            }
        }

    suspend fun todaySteps(userId: Long): ApiResult<Int> =
        call({ api.todaySteps(userId) }) { it }

    suspend fun logSteps(stepCount: Int, userId: Long): ApiResult<String> =
        call({ api.logSteps(CreateStepEntryRequest(stepCount, userId)) }) { it.string() }

    suspend fun leaderboard(period: TimePeriod): ApiResult<List<LeaderboardEntryDto>> =
        call({ api.leaderboard(period.apiValue) }) { it }

    suspend fun healthTip(): ApiResult<HealthTipDto> =
        call({ api.healthTip() }) { it }

    /**
     * Runs an API call, converting transport failures into [ApiResult.Failure] and
     * non-2xx responses into the backend's own error text.
     *
     * The backend is inconsistent about error bodies: the sign-up endpoint returns
     * JSON (`{"message": "..."}`) while the steps endpoint returns plain text, so
     * both shapes are handled here.
     */
    private suspend fun <T, R> call(
        request: suspend () -> Response<T>,
        transform: (T) -> R,
    ): ApiResult<R> = try {
        val response = request()
        val body = response.body()
        if (response.isSuccessful && body != null) {
            ApiResult.Success(transform(body))
        } else {
            ApiResult.Failure(errorMessage(response))
        }
    } catch (e: IOException) {
        ApiResult.Failure("Cannot reach the Nivra server. Check your connection.")
    } catch (e: Exception) {
        ApiResult.Failure(e.message ?: "Unexpected error")
    }

    private suspend fun <T> errorMessage(response: Response<T>): String {
        val raw = runCatching { response.errorBody()?.string() }.getOrNull().orEmpty()
        if (raw.isBlank()) return "Request failed (${response.code()})"

        // Plain-text bodies (e.g. the Lithuanian validation message from the steps
        // endpoint) do not parse as a JSON object, so fall back to the raw text.
        val message = runCatching {
            JsonParser.parseString(raw).asJsonObject.get("message")?.asString
        }.getOrNull()

        return message?.takeIf { it.isNotBlank() } ?: raw
    }
}