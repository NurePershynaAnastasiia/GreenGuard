package com.example.greenguardmobile.activities

import android.app.TimePickerDialog
import android.content.Intent
import android.os.Bundle
import android.view.MenuItem
import android.widget.ImageButton
import android.widget.TextView
import androidx.annotation.NonNull
import androidx.appcompat.app.AppCompatActivity
import com.example.greenguardmobile.R
import com.example.greenguardmobile.util.NavigationUtils
import com.google.android.material.bottomnavigation.BottomNavigationView
import java.util.*

class ProfileActivity : AppCompatActivity() {

    private lateinit var workStartTime: TextView
    private lateinit var workEndTime: TextView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_profile)

        workStartTime = findViewById(R.id.work_start_time)
        workEndTime = findViewById(R.id.work_end_time)

        findViewById<ImageButton>(R.id.exit_btn).setOnClickListener {
            val myIntent = Intent(this, LoginActivity::class.java)
            startActivity(myIntent)
            finish()
        }

        val bottomNavMenu = findViewById<BottomNavigationView>(R.id.bottom_navigation)
        NavigationUtils.setupBottomNavigation(bottomNavMenu, this)
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