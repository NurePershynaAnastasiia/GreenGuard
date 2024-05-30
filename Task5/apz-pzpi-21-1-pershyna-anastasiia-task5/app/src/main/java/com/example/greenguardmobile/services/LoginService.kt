package com.example.greenguardmobile.service

import android.content.Context
import android.util.Log
import com.example.greenguardmobile.models.login.LoginRequest
import com.example.greenguardmobile.models.login.LoginResponse
import com.example.greenguardmobile.network.ApiService
import com.example.greenguardmobile.network.TokenManager
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class LoginService(private val apiService: ApiService, private val context: Context) {

    fun login(email: String, password: String, onSuccess: (String) -> Unit, onError: (String) -> Unit) {
        val loginRequest = LoginRequest(email, password)

        apiService.login(loginRequest).enqueue(object : Callback<LoginResponse> {
            override fun onResponse(call: Call<LoginResponse>, response: Response<LoginResponse>) {
                Log.d("LoginService", "Response code: ${response.code()}")
                if (response.isSuccessful) {
                    val loginResponse = response.body()
                    Log.d("LoginService", "Login successful: $loginResponse")
                    loginResponse?.let {
                        val tokenManager = TokenManager(context)
                        tokenManager.saveJwtToken(it.token)
                        onSuccess(it.token)
                    }
                } else {
                    val errorMsg = response.errorBody()?.string() ?: "Unknown error"
                    Log.e("LoginService", "Login failed with response: $errorMsg")
                    onError("Login failed")
                }
            }

            override fun onFailure(call: Call<LoginResponse>, t: Throwable) {
                Log.e("LoginService", "Network error", t)
                onError("Network error")
                t.printStackTrace()
            }
        })
    }
}
