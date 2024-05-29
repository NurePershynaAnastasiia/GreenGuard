package com.example.greenguardmobile.activities

import android.os.Bundle
import android.util.Log
import android.widget.CheckBox
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.greenguardmobile.R
import com.example.greenguardmobile.adapter.TaskAdapter
import com.example.greenguardmobile.api.ApiService
import com.example.greenguardmobile.api.NetworkModule
import com.example.greenguardmobile.api.TokenManager
import com.example.greenguardmobile.model.Task
import com.example.greenguardmobile.util.NavigationUtils
import com.google.android.material.appbar.MaterialToolbar
import com.google.android.material.bottomnavigation.BottomNavigationView
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class TasksActivity : AppCompatActivity() {

    private lateinit var apiService: ApiService
    private lateinit var tokenManager: TokenManager
    private lateinit var taskAdapter: TaskAdapter
    private var workerId: Int? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_tasks)

        val bottomNavMenu = findViewById<BottomNavigationView>(R.id.bottom_navigation)
        NavigationUtils.setupBottomNavigation(bottomNavMenu, this)
        bottomNavMenu.menu.findItem(R.id.tasks).isChecked = true

        val toolbar = findViewById<MaterialToolbar>(R.id.toolbar)
        NavigationUtils.setupTopMenu(toolbar, this)

        tokenManager = TokenManager(this)
        apiService = NetworkModule.provideApiService(this)

        workerId = tokenManager.getWorkerIdFromToken()
        if (workerId != null) {
            val recyclerView = findViewById<RecyclerView>(R.id.recyclerViewTasks)
            recyclerView.layoutManager = LinearLayoutManager(this)
            taskAdapter = TaskAdapter(mutableListOf(), apiService, workerId!!)
            recyclerView.adapter = taskAdapter

            val checkboxTasksToday = findViewById<CheckBox>(R.id.checkbox_tasks_today)
            checkboxTasksToday.setOnCheckedChangeListener { _, isChecked ->
                if (isChecked) {
                    fetchWorkerTasksToday(workerId!!)
                } else {
                    fetchWorkerTasks(workerId!!)
                }
            }

            fetchWorkerTasks(workerId!!)
        } else {
            Log.d("TasksActivity", "Worker ID not found")
        }
    }

    private fun fetchWorkerTasks(workerId: Int) {
        apiService.getWorkerTasks(workerId).enqueue(object : Callback<List<Task>> {
            override fun onResponse(call: Call<List<Task>>, response: Response<List<Task>>) {
                if (response.isSuccessful) {
                    response.body()?.let { tasks ->
                        taskAdapter.setTasks(tasks)
                    }
                } else {
                    Log.d("TasksActivity", "Error: ${response.errorBody()}")
                }
            }

            override fun onFailure(call: Call<List<Task>>, t: Throwable) {
                Log.d("TasksActivity", "Network error")
                t.printStackTrace()
            }
        })
    }

    private fun fetchWorkerTasksToday(workerId: Int) {
        apiService.getWorkerTasksToday(workerId).enqueue(object : Callback<List<Task>> {
            override fun onResponse(call: Call<List<Task>>, response: Response<List<Task>>) {
                if (response.isSuccessful) {
                    response.body()?.let { tasks ->
                        taskAdapter.setTasks(tasks)
                    }
                } else {
                    Log.d("TasksActivity", "Error: ${response.errorBody()}")
                }
            }

            override fun onFailure(call: Call<List<Task>>, t: Throwable) {
                Log.d("TasksActivity", "Network error")
                t.printStackTrace()
            }
        })
    }
}
