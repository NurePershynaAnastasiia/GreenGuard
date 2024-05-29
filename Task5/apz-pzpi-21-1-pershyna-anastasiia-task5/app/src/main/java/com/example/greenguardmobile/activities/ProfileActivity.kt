package com.example.greenguardmobile.activities

import android.app.AlertDialog
import android.app.TimePickerDialog
import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.widget.Button
import android.widget.CheckBox
import android.widget.EditText
import android.widget.ImageButton
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.greenguardmobile.R
import com.example.greenguardmobile.network.NetworkModule
import com.example.greenguardmobile.network.TokenManager
import com.example.greenguardmobile.models.worker.UpdateWorker
import com.example.greenguardmobile.models.worker.Worker
import com.example.greenguardmobile.models.worker.WorkerSchedule
import com.example.greenguardmobile.service.ProfileService
import com.example.greenguardmobile.util.NavigationUtils
import com.google.android.material.bottomnavigation.BottomNavigationView
import java.util.*

class ProfileActivity : AppCompatActivity() {

    private lateinit var workStartTime: TextView
    private lateinit var workEndTime: TextView
    private lateinit var tokenManager: TokenManager
    private lateinit var profileService: ProfileService

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

        findViewById<Button>(R.id.save_button).setOnClickListener {
            val workerId = tokenManager.getWorkerIdFromToken()
            if (workerId != null) {
                updateWorkerProfile(workerId)
                updateWorkerSchedule(workerId)
            } else {
                Log.d("ProfileActivity", "Worker ID not found")
            }
        }

        findViewById<Button>(R.id.calculate_salary_button).setOnClickListener {
            val workerId = tokenManager.getWorkerIdFromToken()
            if (workerId != null) {
                calculateSalary(workerId)
            } else {
                Log.d("ProfileActivity", "Worker ID not found")
            }
        }

        val bottomNavMenu = findViewById<BottomNavigationView>(R.id.bottom_navigation)
        NavigationUtils.setupBottomNavigation(bottomNavMenu, this)

        tokenManager = TokenManager(this)
        val apiService = NetworkModule.provideApiService(this)
        profileService = ProfileService(apiService, this)

        val workerId = tokenManager.getWorkerIdFromToken()
        if (workerId != null) {
            fetchWorkerProfile(workerId)
            fetchWorkerSchedule(workerId)
        } else {
            Log.d("ProfileActivity", "Worker ID not found")
        }
    }

    private fun updateWorkerProfile(workerId: Int) {
        val updatedWorker = UpdateWorker(
            workerName = fullName.text.toString(),
            phoneNumber = phoneNumber.text.toString(),
            email = email.text.toString(),
            startWorkTime = workStartTime.text.toString(),
            endWorkTime = workEndTime.text.toString()
        )

        profileService.updateWorkerProfile(workerId, updatedWorker, {
            Log.d("ProfileActivity", "Worker information updated successfully")
        }, { errorMsg ->
            Log.e("ProfileActivity", errorMsg)
        })
    }

    private fun updateWorkerSchedule(workerId: Int) {
        val updatedSchedule = WorkerSchedule(
            monday = monday.isChecked,
            tuesday = tuesday.isChecked,
            wednesday = wednesday.isChecked,
            thursday = thursday.isChecked,
            friday = friday.isChecked,
            saturday = saturday.isChecked,
            sunday = sunday.isChecked
        )

        profileService.updateWorkerSchedule(workerId, updatedSchedule, {
            Log.d("ProfileActivity", "Working schedule updated successfully")
        }, { errorMsg ->
            Log.e("ProfileActivity", errorMsg)
        })
    }

    private fun fetchWorkerProfile(workerId: Int) {
        profileService.fetchWorkerProfile(workerId, { worker ->
            fullName.setText(worker.workerName)
            phoneNumber.setText(worker.phoneNumber)
            email.setText(worker.email)
            workStartTime.text = worker.startWorkTime
            workEndTime.text = worker.endWorkTime
        }, { errorMsg ->
            Log.e("ProfileActivity", errorMsg)
        })
    }

    private fun fetchWorkerSchedule(workerId: Int) {
        profileService.fetchWorkerSchedule(workerId, { schedule ->
            monday.isChecked = schedule.monday
            tuesday.isChecked = schedule.tuesday
            wednesday.isChecked = schedule.wednesday
            thursday.isChecked = schedule.thursday
            friday.isChecked = schedule.friday
            saturday.isChecked = schedule.saturday
            sunday.isChecked = schedule.sunday
        }, { errorMsg ->
            Log.e("ProfileActivity", errorMsg)
        })
    }

    private fun calculateSalary(workerId: Int) {
        profileService.calculateSalary(workerId, { salary ->
            showSalaryPopup(salary)
        }, { errorMsg ->
            Log.e("ProfileActivity", errorMsg)
        })
    }

    private fun showSalaryPopup(salary: Double) {
        val builder = AlertDialog.Builder(this)
        builder.setTitle("Розрахована зарплатня")
        builder.setMessage("Ваша зарплатня: $salary грн")
        builder.setPositiveButton("OK") { dialog, _ ->
            dialog.dismiss()
        }
        val dialog = builder.create()
        dialog.show()
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
