package com.nivra.app

import android.app.Application

class NivraApplication : Application() {
    override fun onCreate() {
        super.onCreate()
        ServiceLocator.init(this)
    }
}