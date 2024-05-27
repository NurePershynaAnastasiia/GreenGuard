package com.example.greenguardmobile.activities

import android.os.Bundle
import android.util.Log
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.greenguardmobile.R
import com.example.greenguardmobile.adapter.FertilizerAdapter
import com.example.greenguardmobile.adapter.PestAdapter
import com.example.greenguardmobile.api.ApiService
import com.example.greenguardmobile.api.NetworkModule
import com.example.greenguardmobile.model.Fertilizer
import com.example.greenguardmobile.model.Pest
import com.example.greenguardmobile.util.NavigationUtils
import com.google.android.material.appbar.MaterialToolbar
import com.google.android.material.bottomnavigation.BottomNavigationView
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class PestsActivity : AppCompatActivity() {

    private lateinit var apiService: ApiService
    private lateinit var recyclerView: RecyclerView
    private lateinit var pestAdapter: PestAdapter

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_pests)

        val bottomNavMenu = findViewById<BottomNavigationView>(R.id.bottom_navigation)
        NavigationUtils.setupBottomNavigation(bottomNavMenu, this)
        bottomNavMenu.menu.findItem(R.id.pests).setChecked(true);

        val toolbar = findViewById<MaterialToolbar>(R.id.toolbar)
        NavigationUtils.setupTopMenu(toolbar, this)

        recyclerView = findViewById<RecyclerView>(R.id.recyclerView)
        recyclerView.layoutManager = LinearLayoutManager(this)
        pestAdapter = PestAdapter(mutableListOf())
        recyclerView.adapter = pestAdapter

        apiService = NetworkModule.provideApiService(this)

        fetchPests()
    }

    private fun fetchPests() {
        apiService.getPests().enqueue(object : Callback<List<Pest>> {
            override fun onResponse(call: Call<List<Pest>>, response: Response<List<Pest>>) {
                if (response.isSuccessful) {
                    response.body()?.let { pests ->
                        pestAdapter.setPests(pests)
                    }
                }
            }

            override fun onFailure(call: Call<List<Pest>>, t: Throwable) {
                Log.d("fetchPests", "onFailure")
                t.printStackTrace()
                // Handle error
            }
        })
    }
}