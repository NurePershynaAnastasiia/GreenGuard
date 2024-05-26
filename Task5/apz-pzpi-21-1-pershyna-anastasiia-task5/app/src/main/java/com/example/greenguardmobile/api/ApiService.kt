package com.example.greenguardmobile.api

import com.example.greenguardmobile.model.Fertilizer
import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST

data class LoginRequest(val email: String, val password: String)
data class LoginResponse(val token: String)

interface ApiService {

    @GET("api/Fertilizers/fertilizers")
    fun getFertilizers(): Call<List<Fertilizer>>

    @POST("api/Workers/login")
    fun login(@Body request: LoginRequest): Call<LoginResponse>

}