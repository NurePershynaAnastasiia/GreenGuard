package com.example.greenguardmobile.api

import android.content.Context
import android.util.Log

class TokenManager(private val context: Context) {

    private val sharedPreferences = context.getSharedPreferences("app_prefs", Context.MODE_PRIVATE)

    fun saveJwtToken(token: String) {
        val editor = sharedPreferences.edit()
        editor.putString("jwt_token", token)
        editor.apply()
    }

    fun getJwtToken(): String? {
        Log.d("getJwtToken", sharedPreferences.getString("jwt_token", null).toString())
        return sharedPreferences.getString("jwt_token", null)
    }
}