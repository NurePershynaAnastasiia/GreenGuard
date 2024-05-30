package com.example.greenguardmobile.service

import android.content.Context
import android.widget.Toast
import com.example.greenguardmobile.models.worker.UpdateWorker
import com.example.greenguardmobile.models.worker.Worker
import com.example.greenguardmobile.models.worker.WorkerSchedule
import com.example.greenguardmobile.network.ApiService
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class ProfileService(private val apiService: ApiService, private val context: Context) {

    fun fetchWorkerProfile(workerId: Int, onSuccess: (Worker) -> Unit, onError: (String) -> Unit) {
        apiService.getWorker(workerId).enqueue(object : Callback<Worker> {
            override fun onResponse(call: Call<Worker>, response: Response<Worker>) {
                if (response.isSuccessful) {
                    response.body()?.let(onSuccess) ?: onError("Failed to parse worker profile")
                } else {
                    onError("Error: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<Worker>, t: Throwable) {
                onError("Network error")
                t.printStackTrace()
            }
        })
    }

    fun fetchWorkerSchedule(workerId: Int, onSuccess: (WorkerSchedule) -> Unit, onError: (String) -> Unit) {
        apiService.getWorkerSchedule(workerId).enqueue(object : Callback<WorkerSchedule> {
            override fun onResponse(call: Call<WorkerSchedule>, response: Response<WorkerSchedule>) {
                if (response.isSuccessful) {
                    response.body()?.let(onSuccess) ?: onError("Failed to parse worker schedule")
                } else {
                    onError("Error: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<WorkerSchedule>, t: Throwable) {
                onError("Network error")
                t.printStackTrace()
            }
        })
    }

    fun updateWorkerProfile(workerId: Int, updatedWorker: UpdateWorker, onSuccess: () -> Unit, onError: (String) -> Unit) {
        apiService.updateWorker(workerId, updatedWorker).enqueue(object : Callback<Void> {
            override fun onResponse(call: Call<Void>, response: Response<Void>) {
                if (response.isSuccessful) {
                    onSuccess()
                } else {
                    onError("Error: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                onError("Network error")
                t.printStackTrace()
            }
        })
    }

    fun updateWorkerSchedule(workerId: Int, updatedSchedule: WorkerSchedule, onSuccess: () -> Unit, onError: (String) -> Unit) {
        apiService.updateWorkingSchedule(workerId, updatedSchedule).enqueue(object : Callback<Void> {
            override fun onResponse(call: Call<Void>, response: Response<Void>) {
                if (response.isSuccessful) {
                    onSuccess()
                } else {
                    onError("updateWorkerSchedule: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                onError("Network error")
                t.printStackTrace()
            }
        })
    }

    fun calculateSalary(workerId: Int, onSuccess: (Double) -> Unit, onError: (String) -> Unit) {
        apiService.calculateSalary(workerId).enqueue(object : Callback<Double> {
            override fun onResponse(call: Call<Double>, response: Response<Double>) {
                if (response.isSuccessful) {
                    response.body()?.let(onSuccess) ?: onError("Failed to parse salary")
                } else {
                    onError("Error: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<Double>, t: Throwable) {
                onError("Network error")
                t.printStackTrace()
            }
        })
    }
}
