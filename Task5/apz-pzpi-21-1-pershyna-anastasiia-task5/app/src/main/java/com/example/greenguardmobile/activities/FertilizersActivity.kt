package com.example.greenguardmobile.activities

import android.os.Bundle
import android.util.Log
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.greenguardmobile.R
import com.example.greenguardmobile.adapter.FertilizerAdapter
import com.example.greenguardmobile.api.ApiService
import com.example.greenguardmobile.api.NetworkModule
import com.example.greenguardmobile.databinding.ActivityFertilizersBinding
import com.example.greenguardmobile.model.Fertilizer
import com.example.greenguardmobile.util.NavigationUtils
import com.google.android.material.appbar.MaterialToolbar
import com.google.android.material.bottomnavigation.BottomNavigationView
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class FertilizersActivity : AppCompatActivity() {

    private lateinit var apiService: ApiService
    private lateinit var recyclerView: RecyclerView
    private lateinit var fertilizerAdapter: FertilizerAdapter

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_fertilizers)

        val bottomNavMenu = findViewById<BottomNavigationView>(R.id.bottom_navigation)
        NavigationUtils.setupBottomNavigation(bottomNavMenu, this)
        bottomNavMenu.menu.findItem(R.id.fertilizers).setChecked(true);

        val toolbar = findViewById<MaterialToolbar>(R.id.toolbar)
        NavigationUtils.setupTopMenu(toolbar, this)

        recyclerView = findViewById<RecyclerView>(R.id.recyclerView)
        recyclerView.layoutManager = LinearLayoutManager(this)
        fertilizerAdapter = FertilizerAdapter(mutableListOf())
        recyclerView.adapter = fertilizerAdapter

        apiService = NetworkModule.provideApiService(this)

        fetchFertilizers()
    }
    private fun fetchFertilizers() {
        apiService.getFertilizers().enqueue(object : Callback<List<Fertilizer>> {
            override fun onResponse(call: Call<List<Fertilizer>>, response: Response<List<Fertilizer>>) {
                if (response.isSuccessful) {
                    response.body()?.let { fertilizers ->
                        fertilizerAdapter.setFertilizers(fertilizers)
                    }
                }
            }

            override fun onFailure(call: Call<List<Fertilizer>>, t: Throwable) {
                //Log.d("fetchFertilizers", "onFailure")
                t.printStackTrace()
                // Handle error
            }
        })
    }
}