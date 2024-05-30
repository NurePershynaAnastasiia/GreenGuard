package com.example.greenguardmobile.network

import com.example.greenguardmobile.models.fertilizer.AddFertilizer
import com.example.greenguardmobile.models.plant.AddPlant
import com.example.greenguardmobile.models.fertilizer.Fertilizer
import com.example.greenguardmobile.models.login.LoginRequest
import com.example.greenguardmobile.models.login.LoginResponse
import com.example.greenguardmobile.models.pest.Pest
import com.example.greenguardmobile.models.plant.Plant
import com.example.greenguardmobile.models.plant.PlantType
import com.example.greenguardmobile.models.task.Task
import com.example.greenguardmobile.models.task.TaskStatus
import com.example.greenguardmobile.models.fertilizer.UpdateFertilizerQuantity
import com.example.greenguardmobile.models.plant.UpdatePlant
import com.example.greenguardmobile.models.worker.UpdateWorker
import com.example.greenguardmobile.models.worker.Worker
import com.example.greenguardmobile.models.worker.WorkerSchedule
import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.DELETE
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.PUT
import retrofit2.http.Path
import retrofit2.http.Query

interface ApiService {

    @GET("api/Fertilizers/fertilizers")
    fun getFertilizers(): Call<List<Fertilizer>>

    @POST("api/Fertilizers/add")
    fun addFertilizer(): Call<List<Fertilizer>>

    @POST("api/Workers/login")
    fun login(@Body request: LoginRequest): Call<LoginResponse>

    @GET("api/Pests/pests")
    fun getPests(): Call<List<Pest>>

    @GET("api/Tasks/tasks-today/{workerId}")
    fun getWorkerTasksToday(@Path("workerId") workerId: Int): Call<List<Task>>

    @GET("api/Tasks/tasks/{workerId}")
    fun getWorkerTasks(@Path("workerId") workerId: Int): Call<List<Task>>

    @GET("api/Workers/workers/{workerId}")
    fun getWorker(@Path("workerId") workerId: Int): Call<Worker>

    @GET("api/WorkingSchedule/workerSchedule/{workerId}")
    fun getWorkerSchedule(@Path("workerId") workerId: Int): Call<WorkerSchedule>

    @GET("api/Plants/plants")
    fun getPlants(): Call<List<Plant>>

    @GET("api/PlantTypes/plantTypes")
    fun getPlantTypes(): Call<List<PlantType>>

    @PUT("api/Workers/update/{workerId}")
    fun updateWorker(@Path("workerId") workerId: Int, @Body updatedWorker: UpdateWorker): Call<Void>

    @PUT("api/WorkingSchedule/update/{workerId}")
    fun updateWorkingSchedule(
        @Path("workerId") workerId: Int,
        @Body updatedSchedule: WorkerSchedule
    ): Call<Void>

    @POST("api/Fertilizers/add")
    fun addFertilizer(@Body addFertilizer: AddFertilizer): Call<Void>

    @POST("api/Plants/add")
    fun addPlant(@Body addPlant: AddPlant): Call<Void>

    @DELETE("api/Fertilizers/delete/{fertilizerId}")
    fun deleteFertilizer(@Path("fertilizerId") fertilizerId: Int): Call<Void>

    @PUT("api/Fertilizers/update-quantity/{fertilizerId}")
    fun updateFertilizerQuantity(
        @Path("fertilizerId") fertilizerId: Int,
        @Body updatedFertilizerQuantity: UpdateFertilizerQuantity
    ): Call<Void>

    @POST("api/Pests/add/{pestId}/{plantId}")
    fun addPestToPlant(@Path("pestId") pestId: Int, @Path("plantId") plantId: Int): Call<Void>

    @GET("api/Salary/{workerId}")
    fun calculateSalary(@Path("workerId") workerId: Int): Call<Double>

    @DELETE("api/Pests/delete/{pestId}/{plantId}")
    fun deletePestFromPlant(@Path("pestId") pestId: Int, @Path("plantId") plantId: Int): Call<Void>

    @GET("api/Tasks/status/{taskId}/{workerId}")
    fun getTaskStatus(
        @Path("taskId") taskId: Int,
        @Path("workerId") workerId: Int
    ): Call<TaskStatus>

    @PUT("api/Tasks/update-status/{taskId}/{workerId}")
    fun updateTaskStatus(
        @Path("taskId") taskId: Int,
        @Path("workerId") workerId: Int,
        @Query("taskStatus") taskStatus: String
    ): Call<Void>

    @PUT("api/Plants/update/{plantId}")
    fun updatePlant(@Path("plantId") plantId: Int, @Body updatedPlant: UpdatePlant): Call<Void>

    @DELETE("api/Plants/delete/{plantId}")
    fun deletePlant(@Path("plantId") plantId: Int): Call<Void>
}