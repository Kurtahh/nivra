package com.nivra.app

import android.app.Application
import com.nivra.app.data.ApiClient
import com.nivra.app.data.HealthManager
import com.nivra.app.data.NivraRepository
import com.nivra.app.data.SessionStore

/**
 * Tiny hand-rolled service locator — this app has one API client, one repository,
 * one Health Connect wrapper and one session store, so a DI framework is overkill.
 */
object ServiceLocator {

    lateinit var sessionStore: SessionStore
        private set
    lateinit var repository: NivraRepository
        private set
    lateinit var healthManager: HealthManager
        private set

    fun init(application: Application) {
        sessionStore = SessionStore(application)
        repository = NivraRepository(ApiClient.create(logRequests = BuildConfig.DEBUG))
        healthManager = HealthManager(application)
    }
}