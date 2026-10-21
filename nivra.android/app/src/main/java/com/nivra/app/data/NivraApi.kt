package com.nivra.app.data

import com.google.gson.annotations.SerializedName
import com.nivra.app.BuildConfig
import okhttp3.OkHttpClient
import okhttp3.ResponseBody
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path
import retrofit2.http.Query
import java.util.concurrent.TimeUnit

/**
 * Mirrors the payloads the Nivra backend already accepts. See nivra.api/Models/Requests
 * and nivra.ui/src/components (the web client sends the exact same shapes).
 */
data class Credentials(
    @SerializedName("username") val username: String,
    @SerializedName("passwordHash") val passwordHash: String,
)

data class CreateStepEntryRequest(
    @SerializedName("stepCount") val stepCount: Int,
    @SerializedName("userId") val userId: Long,
)

data class LeaderboardEntryDto(
    @SerializedName("userId") val userId: Long,
    @SerializedName("username") val username: String,
    @SerializedName("totalSteps") val totalSteps: Int,
    @SerializedName("rank") val rank: Int,
)

data class HealthTipDto(
    @SerializedName("id") val id: Int,
    @SerializedName("content") val content: String,
    @SerializedName("category") val category: String,
    @SerializedName("author") val author: String?,
    @SerializedName("sourceUrl") val sourceUrl: String?,
)

interface NivraApi {

    /** POST api/User — 200 on success, 409 with `{ "message": ... }` on conflict. */
    @POST("api/User")
    suspend fun signUp(@Body body: Credentials): Response<ResponseBody>

    /**
     * POST api/User/Authenticate — returns ["<message>", "true", "<userId>"] on success,
     * or ["<message>", "false"] otherwise.
     */
    @POST("api/User/Authenticate")
    suspend fun authenticate(@Body body: Credentials): Response<List<String>>

    /** GET api/User/TodaySteps/{id} */
    @GET("api/User/TodaySteps/{id}")
    suspend fun todaySteps(@Path("id") userId: Long): Response<Int>

    /** POST api/Steps — 200 on success, 400 with a plain-text reason on rejection. */
    @POST("api/Steps")
    suspend fun logSteps(@Body body: CreateStepEntryRequest): Response<ResponseBody>

    @GET("api/leaderboard")
    suspend fun leaderboard(
        @Query("period") period: String,
        @Query("top") top: Int = 20,
    ): Response<List<LeaderboardEntryDto>>

    @GET("api/healthtip")
    suspend fun healthTip(): Response<HealthTipDto>
}

object ApiClient {

    fun create(logRequests: Boolean): NivraApi {
        val logging = HttpLoggingInterceptor().apply {
            level = if (logRequests) {
                HttpLoggingInterceptor.Level.BODY
            } else {
                HttpLoggingInterceptor.Level.NONE
            }
        }

        val client = OkHttpClient.Builder()
            .addInterceptor(logging)
            .connectTimeout(15, TimeUnit.SECONDS)
            .readTimeout(20, TimeUnit.SECONDS)
            .build()

        return Retrofit.Builder()
            .baseUrl(BuildConfig.API_BASE_URL)
            .client(client)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
            .create(NivraApi::class.java)
    }
}