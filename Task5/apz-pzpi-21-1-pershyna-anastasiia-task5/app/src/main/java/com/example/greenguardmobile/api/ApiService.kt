package com.example.greenguardmobile.api

import com.example.greenguardmobile.model.Fertilizer
import com.example.greenguardmobile.model.LoginRequest
import com.example.greenguardmobile.model.LoginResponse
import com.example.greenguardmobile.model.Pest
import com.example.greenguardmobile.model.Plant
import com.example.greenguardmobile.model.Task
import com.example.greenguardmobile.model.UpdateWorker
import com.example.greenguardmobile.model.Worker
import com.example.greenguardmobile.model.WorkerSchedule
import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.PUT
import retrofit2.http.Path

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

    @GET("api/Plants/plants")
    fun getPlants(): Call<List<Plant>>

    @PUT("api/Workers/update/{workerId}")
    fun updateWorker(@Path("workerId") workerId: Int, @Body updatedWorker: UpdateWorker): Call<Void>

    @PUT("api/Workers/updateSchedule/{workerId}")
    fun updateWorkingSchedule(@Path("workerId") workerId: Int, @Body updatedSchedule: WorkerSchedule): Call<Void>
}