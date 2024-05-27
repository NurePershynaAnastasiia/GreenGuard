package com.example.greenguardmobile.activities

import android.app.TimePickerDialog
import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.widget.CheckBox
import android.widget.EditText
import android.widget.ImageButton
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import com.example.greenguardmobile.R
import com.example.greenguardmobile.api.ApiService
import com.example.greenguardmobile.api.NetworkModule
import com.example.greenguardmobile.api.TokenManager
import com.example.greenguardmobile.model.Worker
import com.example.greenguardmobile.model.WorkerSchedule
import com.example.greenguardmobile.util.NavigationUtils
import com.google.android.material.bottomnavigation.BottomNavigationView
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import java.util.*

class ProfileActivity : AppCompatActivity() {

    private lateinit var workStartTime: TextView
    private lateinit var workEndTime: TextView
    private lateinit var apiService: ApiService
    private lateinit var tokenManager: TokenManager

    private lateinit var fullName: EditText
    private lateinit var phoneNumber: EditText
    private lateinit var email: EditText

    private lateinit var monday: CheckBox
    private lateinit var tuesday: CheckBox
    private lateinit var wednesday: CheckBox
    private lateinit var thursday: CheckBox
    private lateinit var friday: CheckBox
    private lateinit var saturday: CheckBox
    private lateinit var sunday: CheckBox

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_profile)

        workStartTime = findViewById(R.id.work_start_time)
        workEndTime = findViewById(R.id.work_end_time)
        fullName = findViewById(R.id.full_name)
        phoneNumber = findViewById(R.id.phone_number)
        email = findViewById(R.id.email)

        monday = findViewById(R.id.monday)
        tuesday = findViewById(R.id.tuesday)
        wednesday = findViewById(R.id.wednesday)
        thursday = findViewById(R.id.thursday)
        friday = findViewById(R.id.friday)
        saturday = findViewById(R.id.saturday)
        sunday = findViewById(R.id.sunday)

        findViewById<ImageButton>(R.id.exit_btn).setOnClickListener {
            val myIntent = Intent(this, LoginActivity::class.java)
            startActivity(myIntent)
            finish()
        }

        val bottomNavMenu = findViewById<BottomNavigationView>(R.id.bottom_navigation)
        NavigationUtils.setupBottomNavigation(bottomNavMenu, this)

        tokenManager = TokenManager(this)
        apiService = NetworkModule.provideApiService(this)

        val workerId = tokenManager.getWorkerIdFromToken()
        if (workerId != null) {
            fetchWorkerProfile(workerId)
            fetchWorkerSchedule(workerId)
        } else {
            Log.d("ProfileActivity", "Worker ID not found")
        }
    }

    private fun fetchWorkerProfile(workerId: Int) {
        apiService.getWorker(workerId).enqueue(object : Callback<Worker> {
            override fun onResponse(call: Call<Worker>, response: Response<Worker>) {
                if (response.isSuccessful) {
                    response.body()?.let { worker ->
                        fullName.setText(worker.workerName)
                        phoneNumber.setText(worker.phoneNumber)
                        email.setText(worker.email)
                        workStartTime.text = worker.startWorkTime
                        workEndTime.text = worker.endWorkTime
                    }
                } else {
                    Log.e("ProfileActivity", "Error: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<Worker>, t: Throwable) {
                Log.e("ProfileActivity", "Network error")
                t.printStackTrace()
            }
        })
    }

    private fun fetchWorkerSchedule(workerId: Int) {
        apiService.getWorkerSchedule(workerId).enqueue(object : Callback<WorkerSchedule> {
            override fun onResponse(call: Call<WorkerSchedule>, response: Response<WorkerSchedule>) {
                if (response.isSuccessful) {
                    response.body()?.let { schedule ->
                        monday.isChecked = schedule.monday
                        tuesday.isChecked = schedule.tuesday
                        wednesday.isChecked = schedule.wednesday
                        thursday.isChecked = schedule.thursday
                        friday.isChecked = schedule.friday
                        saturday.isChecked = schedule.saturday
                        sunday.isChecked = schedule.sunday
                    }
                } else {
                    Log.e("ProfileActivity", "Error: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<WorkerSchedule>, t: Throwable) {
                Log.e("ProfileActivity", "Network error")
                t.printStackTrace()
            }
        })
    }

    fun showTimePickerStart(view: android.view.View) {
        showTimePickerDialog(workStartTime)
    }

    fun showTimePickerEnd(view: android.view.View) {
        showTimePickerDialog(workEndTime)
    }

    private fun showTimePickerDialog(timeTextView: TextView) {
        val calendar = Calendar.getInstance()
        val hour = calendar.get(Calendar.HOUR_OF_DAY)
        val minute = calendar.get(Calendar.MINUTE)

        val timePickerDialog = TimePickerDialog(this,
            { _, hourOfDay, minuteOfHour ->
                timeTextView.text = String.format("%02d:%02d", hourOfDay, minuteOfHour)
            }, hour, minute, true)
        timePickerDialog.show()
    }
}
