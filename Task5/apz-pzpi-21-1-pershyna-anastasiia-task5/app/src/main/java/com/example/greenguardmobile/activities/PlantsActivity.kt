package com.example.greenguardmobile.activities

import android.os.Bundle
import android.util.Log
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.greenguardmobile.R
import com.example.greenguardmobile.adapter.PlantAdapter
import com.example.greenguardmobile.api.ApiService
import com.example.greenguardmobile.api.NetworkModule
import com.example.greenguardmobile.model.Plant
import com.example.greenguardmobile.util.NavigationUtils
import com.google.android.material.appbar.MaterialToolbar
import com.google.android.material.bottomnavigation.BottomNavigationView
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class PlantsActivity : AppCompatActivity() {

    private lateinit var apiService: ApiService
    private lateinit var plantsRecyclerView: RecyclerView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_plants)

        val bottomNavMenu = findViewById<BottomNavigationView>(R.id.bottom_navigation)
        NavigationUtils.setupBottomNavigation(bottomNavMenu, this)
        bottomNavMenu.menu.findItem(R.id.plants).setChecked(true)

        val toolbar = findViewById<MaterialToolbar>(R.id.toolbar)
        NavigationUtils.setupTopMenu(toolbar, this)

        plantsRecyclerView = findViewById(R.id.plants_recycler_view)
        plantsRecyclerView.layoutManager = LinearLayoutManager(this)

        apiService = NetworkModule.provideApiService(this)

        fetchPlants()
    }

    private fun fetchPlants() {
        apiService.getPlants().enqueue(object : Callback<List<Plant>> {
            override fun onResponse(call: Call<List<Plant>>, response: Response<List<Plant>>) {
                if (response.isSuccessful) {
                    response.body()?.let { plants ->
                        plantsRecyclerView.adapter = PlantAdapter(plants)
                    }
                } else {
                    Log.e("PlantsActivity", "Error: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<List<Plant>>, t: Throwable) {
                Log.e("PlantsActivity", "Network error")
                t.printStackTrace()
            }
        })
    }
}
