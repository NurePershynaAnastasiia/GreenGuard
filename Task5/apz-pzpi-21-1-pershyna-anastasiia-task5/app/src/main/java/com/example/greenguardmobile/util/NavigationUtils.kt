package com.example.greenguardmobile.util

import android.content.Context
import android.content.Intent
import com.example.greenguardmobile.R
import com.example.greenguardmobile.activities.FertilizersActivity
import com.example.greenguardmobile.activities.PestsActivity
import com.example.greenguardmobile.activities.PlantsActivity
import com.example.greenguardmobile.activities.ProfileActivity
import com.example.greenguardmobile.activities.TasksActivity
import com.google.android.material.appbar.MaterialToolbar
import com.google.android.material.bottomnavigation.BottomNavigationView

object NavigationUtils {
    fun setupBottomNavigation(bottomNavMenu: BottomNavigationView, context: Context) {
        bottomNavMenu.setOnItemSelectedListener { item ->
            val activityClass = when (item.itemId) {
                R.id.plants -> PlantsActivity::class.java
                R.id.tasks -> TasksActivity::class.java
                R.id.pests -> PestsActivity::class.java
                R.id.fertilizers -> FertilizersActivity::class.java
                else -> null
            }
            // Перевірка, чи не знаходитеся ви вже в необхідній активності
            if (activityClass != null && context::class.java != activityClass) {
                context.startActivity(Intent(context, activityClass))
                item.isChecked = true
            }
            false
        }
    }

    fun setupTopMenu(toolbar: MaterialToolbar, context: Context) {
        toolbar.setOnMenuItemClickListener { item ->
            when (item.itemId) {
                R.id.profile -> {
                    context.startActivity(Intent(context, ProfileActivity::class.java))
                    true
                }
                else -> false
            }
        }
    }
}

