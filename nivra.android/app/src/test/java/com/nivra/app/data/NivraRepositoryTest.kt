package com.nivra.app.data

import kotlinx.coroutines.test.runTest
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.ResponseBody
import okhttp3.ResponseBody.Companion.toResponseBody
import org.junit.Assert.assertEquals
import org.junit.Test
import retrofit2.Response

/**
 * The backend returns a positional string array from /api/User/Authenticate and mixes
 * JSON and plain-text error bodies, so the parsing rules are worth pinning down.
 */
class NivraRepositoryTest {

    // JUnit creates a fresh test instance (and therefore a fresh fake) per method.
    private val api = FakeApi()
    private val repository = NivraRepository(api)

    @Test
    fun `login parses the positional auth response`() = runTest {
        val session = when (val result = repository.login("vincent", "secret")) {
            is ApiResult.Success -> result.data
            is ApiResult.Failure -> error("Expected success but got: ${result.message}")
        }

        assertEquals(42L, session.userId)
        assertEquals("vincent", session.username)
    }

    @Test
    fun `login surfaces the backend message when it returns false`() = runTest {
        api.authenticateResponse = Response.success(listOf("Username doesn't exist!", "false"))

        val result = repository.login("nobody", "secret")

        assertEquals("Username doesn't exist!", (result as ApiResult.Failure).message)
    }

    @Test
    fun `sign up surfaces the json error body`() = runTest {
        api.signUpResponse = Response.error(
            409,
            """{"message":"Username already exist or you entered not secure password!"}"""
                .toResponseBody("application/json".toMediaType()),
        )

        val result = repository.signUp("vincent", "secret")

        assertEquals(
            "Username already exist or you entered not secure password!",
            (result as ApiResult.Failure).message,
        )
    }

    @Test
    fun `log steps surfaces the plain text error body`() = runTest {
        api.logStepsResponse = Response.error(
            400,
            "Jūsų žingsnių kiekis privalo viršyti 0".toResponseBody("text/plain".toMediaType()),
        )

        val result = repository.logSteps(0, userId = 42)

        assertEquals(
            "Jūsų žingsnių kiekis privalo viršyti 0",
            (result as ApiResult.Failure).message,
        )
    }
}

/** Hand-rolled stand-in so the tests stay free of mocking dependencies. */
private class FakeApi : NivraApi {

    var authenticateResponse: Response<List<String>> =
        Response.success(listOf("Login successful!", "true", "42"))

    var signUpResponse: Response<ResponseBody> =
        Response.success("\"Sign up was successful\"".toResponseBody())

    var logStepsResponse: Response<ResponseBody> = Response.success("".toResponseBody())

    override suspend fun signUp(body: Credentials): Response<ResponseBody> = signUpResponse

    override suspend fun authenticate(body: Credentials): Response<List<String>> = authenticateResponse

    override suspend fun todaySteps(userId: Long): Response<Int> = Response.success(1234)

    override suspend fun logSteps(body: CreateStepEntryRequest): Response<ResponseBody> = logStepsResponse

    override suspend fun leaderboard(period: String, top: Int): Response<List<LeaderboardEntryDto>> =
        Response.success(emptyList())

    override suspend fun healthTip(): Response<HealthTipDto> =
        Response.success(HealthTipDto(1, "Walk more", "Activity", null, null))
}