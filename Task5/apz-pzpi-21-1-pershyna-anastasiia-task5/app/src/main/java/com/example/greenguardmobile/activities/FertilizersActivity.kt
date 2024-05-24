package com.example.greenguardmobile.activities

import android.os.Bundle
import android.view.Menu
import android.view.MenuItem
import android.widget.Toolbar
import androidx.appcompat.app.AppCompatActivity
import com.example.greenguardmobile.R
import com.example.greenguardmobile.util.NavigationUtils
import com.google.android.material.appbar.MaterialToolbar
import com.google.android.material.bottomnavigation.BottomNavigationView

class FertilizersActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_fertilizers)

        val bottomNavMenu = findViewById<BottomNavigationView>(R.id.bottom_navigation)
        NavigationUtils.setupBottomNavigation(bottomNavMenu, this)
        bottomNavMenu.menu.findItem(R.id.fertilizers).setChecked(true);

        val toolbar = findViewById<MaterialToolbar>(R.id.toolbar)
        NavigationUtils.setupTopMenu(toolbar, this)
    }
}