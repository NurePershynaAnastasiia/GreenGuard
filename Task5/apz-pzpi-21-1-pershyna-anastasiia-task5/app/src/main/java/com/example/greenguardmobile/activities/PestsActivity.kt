package com.example.greenguardmobile.activities

import android.os.Bundle
import android.util.Log
import android.view.Gravity
import android.view.LayoutInflater
import android.view.View
import android.widget.*
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.greenguardmobile.R
import com.example.greenguardmobile.adapter.PestAdapter
import com.example.greenguardmobile.api.ApiService
import com.example.greenguardmobile.api.NetworkModule
import com.example.greenguardmobile.model.Plant
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
    private lateinit var plants: List<Plant>

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_pests)

        val bottomNavMenu = findViewById<BottomNavigationView>(R.id.bottom_navigation)
        NavigationUtils.setupBottomNavigation(bottomNavMenu, this)
        bottomNavMenu.menu.findItem(R.id.pests).isChecked = true

        val toolbar = findViewById<MaterialToolbar>(R.id.toolbar)
        NavigationUtils.setupTopMenu(toolbar, this)

        recyclerView = findViewById(R.id.recyclerView)
        recyclerView.layoutManager = LinearLayoutManager(this)
        pestAdapter = PestAdapter(mutableListOf(), { pest ->
            showAddToPlantPopup(pest)
        }, { pest ->
            showRemoveFromPlantPopup(pest)
        })
        recyclerView.adapter = pestAdapter

        apiService = NetworkModule.provideApiService(this)

        fetchPests()
        fetchPlants()
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

    private fun fetchPlants() {
        apiService.getPlants().enqueue(object : Callback<List<Plant>> {
            override fun onResponse(call: Call<List<Plant>>, response: Response<List<Plant>>) {
                if (response.isSuccessful) {
                    response.body()?.let { plantsList ->
                        plants = plantsList
                    }
                }
            }

            override fun onFailure(call: Call<List<Plant>>, t: Throwable) {
                Log.d("fetchPlants", "onFailure")
                t.printStackTrace()
                // Handle error
            }
        })
    }

    private fun showAddToPlantPopup(pest: Pest) {
        val inflater = getSystemService(LAYOUT_INFLATER_SERVICE) as LayoutInflater
        val popupView = inflater.inflate(R.layout.popup_add_pest_to_plant, null)

        val width = LinearLayout.LayoutParams.WRAP_CONTENT
        val height = LinearLayout.LayoutParams.WRAP_CONTENT
        val focusable = true

        val popupWindow = PopupWindow(popupView, width, height, focusable)

        val plantSpinner = popupView.findViewById<Spinner>(R.id.plantSpinner)
        val addButtonPopup = popupView.findViewById<Button>(R.id.addButtonPopup)

        val plantNames = plants.map { "${it.plantTypeName}-${it.plantLocation}" }
        val plantIds = plants.map { it.plantId }

        val adapter = ArrayAdapter(this, android.R.layout.simple_spinner_item, plantNames)
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
        plantSpinner.adapter = adapter

        addButtonPopup.setOnClickListener {
            val selectedPosition = plantSpinner.selectedItemPosition
            val selectedPlantId = plantIds[selectedPosition]

            apiService.addPestToPlant(pest.pestId, selectedPlantId).enqueue(object : Callback<Void> {
                override fun onResponse(call: Call<Void>, response: Response<Void>) {
                    if (response.isSuccessful) {
                        popupWindow.dismiss()
                        Log.d("AddToPlant", "Pest added to plant successfully")
                        Toast.makeText(this@PestsActivity, "Pest added to plant successfully", Toast.LENGTH_SHORT).show()
                    } else {
                        Log.e("AddToPlant", "Error: ${response.code()} ${response.message()}")
                    }
                }

                override fun onFailure(call: Call<Void>, t: Throwable) {
                    Log.e("AddToPlant", "Network error")
                    t.printStackTrace()
                }
            })
        }

        popupWindow.showAtLocation(window.decorView, Gravity.CENTER, 0, 0)
    }

    private fun showRemoveFromPlantPopup(pest: Pest) {
        val inflater = getSystemService(LAYOUT_INFLATER_SERVICE) as LayoutInflater
        val popupView = inflater.inflate(R.layout.popup_remove_pest_from_plant, null)

        val width = LinearLayout.LayoutParams.WRAP_CONTENT
        val height = LinearLayout.LayoutParams.WRAP_CONTENT
        val focusable = true

        val popupWindow = PopupWindow(popupView, width, height, focusable)

        val plantSpinnerRemove = popupView.findViewById<Spinner>(R.id.plantSpinnerRemove)
        val removeButtonPopup = popupView.findViewById<Button>(R.id.removeButtonPopup)

        val plantNames = plants.map { "${it.plantTypeName}-${it.plantLocation}" }
        val plantIds = plants.map { it.plantId }

        val adapter = ArrayAdapter(this, android.R.layout.simple_spinner_item, plantNames)
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
        plantSpinnerRemove.adapter = adapter

        removeButtonPopup.setOnClickListener {
            val selectedPosition = plantSpinnerRemove.selectedItemPosition
            val selectedPlantId = plantIds[selectedPosition]

            apiService.deletePestFromPlant(pest.pestId, selectedPlantId).enqueue(object : Callback<Void> {
                override fun onResponse(call: Call<Void>, response: Response<Void>) {
                    if (response.isSuccessful) {
                        popupWindow.dismiss()
                        Log.d("RemoveFromPlant", "Pest removed from plant successfully")
                        Toast.makeText(this@PestsActivity, "Pest removed from plant successfully", Toast.LENGTH_SHORT).show()
                    } else {
                        Log.e("RemoveFromPlant", "Error: ${response.code()} ${response.message()}")
                        Toast.makeText(this@PestsActivity, "This pest with is not associated with selected plant", Toast.LENGTH_SHORT).show()

                    }
                }

                override fun onFailure(call: Call<Void>, t: Throwable) {
                    Log.e("RemoveFromPlant", "Network error")
                    t.printStackTrace()
                }
            })
        }

        popupWindow.showAtLocation(window.decorView, Gravity.CENTER, 0, 0)
    }
}
