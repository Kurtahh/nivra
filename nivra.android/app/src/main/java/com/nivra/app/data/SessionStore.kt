package com.nivra.app.data

import android.content.Context
import android.content.SharedPreferences
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow

/**
 * The logged-in user. Kept across app restarts so the user only signs in once.
 */
data class Session(
    val userId: Long,
    val username: String,
)

/**
 * Persisted session, mirroring the web client's `Session` singleton
 * (nivra.ui/src/classes/Session.ts) but stored in SharedPreferences.
 */
class SessionStore(context: Context) {

    private val prefs: SharedPreferences =
        context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)

    private val _session = MutableStateFlow(load())
    val session: StateFlow<Session?> = _session.asStateFlow()

    val current: Session? get() = _session.value

    fun save(session: Session) {
        prefs.edit()
            .putString(KEY_USER_ID, session.userId.toString())
            .putString(KEY_USERNAME, session.username)
            .apply()
        _session.value = session
    }

    fun clear() {
        prefs.edit().clear().apply()
        _session.value = null
    }

    private fun load(): Session? {
        val username = prefs.getString(KEY_USERNAME, null) ?: return null
        val userId = prefs.getString(KEY_USER_ID, null)?.toLongOrNull() ?: return null
        return Session(userId = userId, username = username)
    }

    private companion object {
        const val PREFS_NAME = "nivra_session"
        const val KEY_USER_ID = "user_id"
        const val KEY_USERNAME = "username"
    }
}