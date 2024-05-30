package com.example.greenguardmobile.service

import com.example.greenguardmobile.models.task.Task
import com.example.greenguardmobile.network.ApiService
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class TasksService(val apiService: ApiService) {

    fun fetchWorkerTasks(workerId: Int, onSuccess: (List<Task>) -> Unit, onError: (String) -> Unit) {
        apiService.getWorkerTasks(workerId).enqueue(object : Callback<List<Task>> {
            override fun onResponse(call: Call<List<Task>>, response: Response<List<Task>>) {
                if (response.isSuccessful) {
                    response.body()?.let(onSuccess) ?: onError("Failed to parse tasks")
                } else {
                    onError("Error: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<List<Task>>, t: Throwable) {
                onError("Network error")
                t.printStackTrace()
            }
        })
    }

    fun fetchWorkerTasksToday(workerId: Int, onSuccess: (List<Task>) -> Unit, onError: (String) -> Unit) {
        apiService.getWorkerTasksToday(workerId).enqueue(object : Callback<List<Task>> {
            override fun onResponse(call: Call<List<Task>>, response: Response<List<Task>>) {
                if (response.isSuccessful) {
                    response.body()?.let(onSuccess) ?: onError("Failed to parse tasks")
                } else {
                    onError("Error: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<List<Task>>, t: Throwable) {
                onError("Network error")
                t.printStackTrace()
            }
        })
    }
}
