package com.example.greenguardmobile.activities

import android.app.AlertDialog
import android.app.TimePickerDialog
import android.content.Intent
import android.content.SharedPreferences
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

    private lateinit var sharedPreferences: SharedPreferences

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_profile)

        initViews()
        setupNavigation()
        setupListeners()
        initializeServices()

        val workerId = tokenManager.getWorkerIdFromToken()
        if (workerId != null) {
            fetchWorkerProfile(workerId)
            fetchWorkerSchedule(workerId)
        } else {
            Log.d("ProfileActivity", "Worker ID not found")
        }
    }

    override fun onSaveInstanceState(outState: Bundle) {
        super.onSaveInstanceState(outState)
        saveInstanceState(outState)
    }

    override fun onRestoreInstanceState(outState: Bundle) {
        super.onRestoreInstanceState(outState)
        saveInstanceState(outState)
    }

    private fun initViews() {
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

        sharedPreferences = getPreferences(MODE_PRIVATE)
    }

    private fun setupNavigation() {
        val bottomNavMenu = findViewById<BottomNavigationView>(R.id.bottom_navigation)
        NavigationUtils.setupBottomNavigation(bottomNavMenu, this)
    }

    private fun setupListeners() {
        findViewById<ImageButton>(R.id.exit_btn).setOnClickListener {
            navigateToLogin()
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

        findViewById<Button>(R.id.edit_start_time).setOnClickListener {
            showTimePickerDialog(workStartTime)
        }

        findViewById<Button>(R.id.edit_end_time).setOnClickListener {
            showTimePickerDialog(workEndTime)
        }
    }

    private fun initializeServices() {
        tokenManager = TokenManager(this)
        val apiService = NetworkModule.provideApiService(this)
        profileService = ProfileService(apiService, this)
    }

    private fun navigateToLogin() {
        val myIntent = Intent(this, LoginActivity::class.java)
        startActivity(myIntent)
        finish()
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
            Log.d("ProfileActivity", getResources().getString(R.string.information_updated))
            Toast.makeText(this@ProfileActivity, getResources().getString(R.string.information_updated), Toast.LENGTH_SHORT).show()
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
            Log.d("ProfileActivity", getResources().getString(R.string.information_updated))
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
        builder.setTitle(getResources().getString(R.string.calculated_salary))
        builder.setMessage(getResources().getString(R.string.your_salary) + ": $salary " + getResources().getString(R.string.grn))
        builder.setPositiveButton("OK") { dialog, _ ->
            dialog.dismiss()
        }
        val dialog = builder.create()
        dialog.show()
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

    private fun saveInstanceState(outState: Bundle) {
        outState.putString("fullName", fullName.text.toString())
        outState.putString("phoneNumber", phoneNumber.text.toString())
        outState.putString("email", email.text.toString())
        outState.putString("workStartTime", workStartTime.text.toString())
        outState.putString("workEndTime", workEndTime.text.toString())
        outState.putBoolean("monday", monday.isChecked)
        outState.putBoolean("tuesday", tuesday.isChecked)
        outState.putBoolean("wednesday", wednesday.isChecked)
        outState.putBoolean("thursday", thursday.isChecked)
        outState.putBoolean("friday", friday.isChecked)
        outState.putBoolean("saturday", saturday.isChecked)
        outState.putBoolean("sunday", sunday.isChecked)
    }

    private fun restoreSavedInstanceState(savedInstanceState: Bundle) {
        fullName.setText(savedInstanceState.getString("fullName"))
        phoneNumber.setText(savedInstanceState.getString("phoneNumber"))
        email.setText(savedInstanceState.getString("email"))
        workStartTime.text = savedInstanceState.getString("workStartTime")
        workEndTime.text = savedInstanceState.getString("workEndTime")
        monday.isChecked = savedInstanceState.getBoolean("monday")
        tuesday.isChecked = savedInstanceState.getBoolean("tuesday")
        wednesday.isChecked = savedInstanceState.getBoolean("wednesday")
        thursday.isChecked = savedInstanceState.getBoolean("thursday")
        friday.isChecked = savedInstanceState.getBoolean("friday")
        saturday.isChecked = savedInstanceState.getBoolean("saturday")
        sunday.isChecked = savedInstanceState.getBoolean("sunday")
    }

}
