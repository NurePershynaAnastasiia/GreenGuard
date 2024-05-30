package com.example.greenguardmobile.activities

import android.os.Bundle
import android.util.Log
import android.widget.CheckBox
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.greenguardmobile.R
import com.example.greenguardmobile.adapters.TaskAdapter
import com.example.greenguardmobile.network.NetworkModule
import com.example.greenguardmobile.network.TokenManager
import com.example.greenguardmobile.service.TasksService
import com.example.greenguardmobile.util.NavigationUtils
import com.google.android.material.appbar.MaterialToolbar
import com.google.android.material.bottomnavigation.BottomNavigationView

class TasksActivity : AppCompatActivity() {

    private lateinit var tokenManager: TokenManager
    private lateinit var tasksService: TasksService
    private lateinit var taskAdapter: TaskAdapter
    private var workerId: Int? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_tasks)
    }

    override fun onStart() {
        super.onStart()
        initViews()
        setupNavigation()
        setupListeners()
        initializeServices()

        workerId = tokenManager.getWorkerIdFromToken()
        if (workerId != null) {
            setupRecyclerView(workerId!!)
            fetchWorkerTasks(workerId!!)
        } else {
            Log.d("TasksActivity", "Worker ID not found")
        }
    }

    private fun initViews() {
        findViewById<BottomNavigationView>(R.id.bottom_navigation).apply {
            menu.findItem(R.id.tasks).isChecked = true
        }
        findViewById<MaterialToolbar>(R.id.toolbar)
        findViewById<CheckBox>(R.id.checkbox_tasks_today)
    }

    private fun setupNavigation() {
        val bottomNavMenu = findViewById<BottomNavigationView>(R.id.bottom_navigation)
        NavigationUtils.setupBottomNavigation(bottomNavMenu, this)

        val toolbar = findViewById<MaterialToolbar>(R.id.toolbar)
        NavigationUtils.setupTopMenu(toolbar, this)
    }

    private fun setupListeners() {
        findViewById<CheckBox>(R.id.checkbox_tasks_today).setOnCheckedChangeListener { _, isChecked ->
            if (isChecked) {
                fetchWorkerTasksToday(workerId!!)
            } else {
                fetchWorkerTasks(workerId!!)
            }
        }
    }

    private fun initializeServices() {
        tokenManager = TokenManager(this)
        val apiService = NetworkModule.provideApiService(this)
        tasksService = TasksService(apiService)
    }

    private fun setupRecyclerView(workerId: Int) {
        val recyclerView = findViewById<RecyclerView>(R.id.recyclerViewTasks)
        recyclerView.layoutManager = LinearLayoutManager(this)
        taskAdapter = TaskAdapter(mutableListOf(), tasksService.apiService, workerId)
        recyclerView.adapter = taskAdapter
    }

    private fun fetchWorkerTasks(workerId: Int) {
        tasksService.fetchWorkerTasks(workerId, { tasks ->
            taskAdapter.setTasks(tasks)
        }, { errorMsg ->
            Log.d("TasksActivity", errorMsg)
        })
    }

    private fun fetchWorkerTasksToday(workerId: Int) {
        tasksService.fetchWorkerTasksToday(workerId, { tasks ->
            taskAdapter.setTasks(tasks)
        }, { errorMsg ->
            Log.d("TasksActivity", errorMsg)
        })
    }
}
