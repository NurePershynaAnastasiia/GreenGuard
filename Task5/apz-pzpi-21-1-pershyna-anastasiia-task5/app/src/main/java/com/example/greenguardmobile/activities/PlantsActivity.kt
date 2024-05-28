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
import com.example.greenguardmobile.adapter.PlantAdapter
import com.example.greenguardmobile.api.ApiService
import com.example.greenguardmobile.api.NetworkModule
import com.example.greenguardmobile.model.AddPlant
import com.example.greenguardmobile.model.Plant
import com.example.greenguardmobile.model.PlantType
import com.example.greenguardmobile.util.NavigationUtils
import com.google.android.material.appbar.MaterialToolbar
import com.google.android.material.bottomnavigation.BottomNavigationView
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class PlantsActivity : AppCompatActivity() {

    private lateinit var apiService: ApiService
    private lateinit var plantsRecyclerView: RecyclerView
    private lateinit var plantTypes: List<PlantType>

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_plants)

        val bottomNavMenu = findViewById<BottomNavigationView>(R.id.bottom_navigation)
        NavigationUtils.setupBottomNavigation(bottomNavMenu, this)
        bottomNavMenu.menu.findItem(R.id.plants).isChecked = true

        val toolbar = findViewById<MaterialToolbar>(R.id.toolbar)
        NavigationUtils.setupTopMenu(toolbar, this)

        plantsRecyclerView = findViewById(R.id.plants_recycler_view)
        plantsRecyclerView.layoutManager = LinearLayoutManager(this)

        apiService = NetworkModule.provideApiService(this)

        fetchPlants()
        fetchPlantTypes()

        findViewById<Button>(R.id.addButton).setOnClickListener {
            showAddPlantPopup()
        }
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

    private fun fetchPlantTypes() {
        apiService.getPlantTypes().enqueue(object : Callback<List<PlantType>> {
            override fun onResponse(call: Call<List<PlantType>>, response: Response<List<PlantType>>) {
                if (response.isSuccessful) {
                    response.body()?.let { types ->
                        plantTypes = types
                    }
                } else {
                    Log.e("PlantsActivity", "Error: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<List<PlantType>>, t: Throwable) {
                Log.e("PlantsActivity", "Network error")
                t.printStackTrace()
            }
        })
    }

    private fun showAddPlantPopup() {
        val inflater = getSystemService(LAYOUT_INFLATER_SERVICE) as LayoutInflater
        val popupView = inflater.inflate(R.layout.popup_add_plant, null)

        val width = LinearLayout.LayoutParams.WRAP_CONTENT
        val height = LinearLayout.LayoutParams.WRAP_CONTENT
        val focusable = true

        val popupWindow = PopupWindow(popupView, width, height, focusable)

        val spinnerPlantType = popupView.findViewById<Spinner>(R.id.spinner_plant_type)
        val locationEditText = popupView.findViewById<EditText>(R.id.et_plant_location)
        val lightEditText = popupView.findViewById<EditText>(R.id.et_plant_light)
        val humidityEditText = popupView.findViewById<EditText>(R.id.et_plant_humidity)
        val tempEditText = popupView.findViewById<EditText>(R.id.et_plant_temp)
        val additionalInfoEditText = popupView.findViewById<EditText>(R.id.et_additional_info)

        val plantTypeNames = plantTypes.map { it.plantTypeName }
        val adapter = ArrayAdapter(this, android.R.layout.simple_spinner_item, plantTypeNames)
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
        spinnerPlantType.adapter = adapter

        val addButtonPopup = popupView.findViewById<Button>(R.id.addButtonPopup)
        addButtonPopup.setOnClickListener {
            val plantTypeId = plantTypes[spinnerPlantType.selectedItemPosition].plantTypeId
            val location = locationEditText.text.toString()
            val light = lightEditText.text.toString().toFloatOrNull()
            val humidity = humidityEditText.text.toString().toFloatOrNull()
            val temp = tempEditText.text.toString().toFloatOrNull()
            val additionalInfo = additionalInfoEditText.text.toString()

            if (light != null && humidity != null && temp != null) {
                val newPlant = AddPlant(plantTypeId, location, light, humidity, temp, additionalInfo)
                addPlant(newPlant)
                popupWindow.dismiss()
            } else {
                Log.d("AddPlantPopup", "Invalid input")
            }
        }

        popupWindow.showAtLocation(window.decorView, Gravity.CENTER, 0, 0)
    }

    private fun addPlant(plant: AddPlant) {
        apiService.addPlant(plant).enqueue(object : Callback<Void> {
            override fun onResponse(call: Call<Void>, response: Response<Void>) {
                if (response.isSuccessful) {
                    fetchPlants()
                    Log.d("AddPlant", "Plant added successfully")
                } else {
                    Log.e("AddPlant", "Error: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                Log.e("AddPlant", "Network error")
                t.printStackTrace()
            }
        })
    }
}
