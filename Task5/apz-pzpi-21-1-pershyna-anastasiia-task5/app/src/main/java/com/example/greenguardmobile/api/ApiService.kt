package com.example.greenguardmobile.api

import com.example.greenguardmobile.model.Fertilizer
import com.example.greenguardmobile.model.Pest
import com.example.greenguardmobile.model.Task
import com.example.greenguardmobile.model.Worker
import com.example.greenguardmobile.model.WorkerSchedule
import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path

data class LoginRequest(val email: String, val password: String)
data class LoginResponse(val token: String)

interface ApiService {

    @GET("api/Fertilizers/fertilizers")
    fun getFertilizers(): Call<List<Fertilizer>>

    @POST("api/Fertilizers/add")
    fun addFertilizer(): Call<List<Fertilizer>>

    @POST("api/Workers/login")
    fun login(@Body request: LoginRequest): Call<LoginResponse>

    @GET("api/Pests/pests")
    fun getPests(): Call<List<Pest>>

    @GET("api/Tasks/tasks/{workerId}")
    fun getWorkerTasks(@Path("workerId") workerId: Int): Call<List<Task>>

    @GET("api/Workers/workers/{workerId}")
    fun getWorker(@Path("workerId") workerId: Int): Call<Worker>

    @GET("api/WorkingSchedule/workerSchedule/{workerId}")
    fun getWorkerSchedule(@Path("workerId") workerId: Int): Call<WorkerSchedule>
}