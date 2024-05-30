package com.example.greenguardmobile.activities

import android.os.Bundle
import android.util.Log
import android.view.Gravity
import android.view.LayoutInflater
import android.widget.*
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.greenguardmobile.R
import com.example.greenguardmobile.adapters.PlantAdapter
import com.example.greenguardmobile.network.NetworkModule
import com.example.greenguardmobile.models.plant.AddPlant
import com.example.greenguardmobile.models.plant.Plant
import com.example.greenguardmobile.models.plant.PlantType
import com.example.greenguardmobile.models.plant.UpdatePlant
import com.example.greenguardmobile.service.PlantsService
import com.example.greenguardmobile.util.NavigationUtils
import com.google.android.material.appbar.MaterialToolbar
import com.google.android.material.bottomnavigation.BottomNavigationView

class PlantsActivity : AppCompatActivity() {

    private lateinit var plantsService: PlantsService
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

        val apiService = NetworkModule.provideApiService(this)
        plantsService = PlantsService(apiService, this)

        fetchPlants()
        fetchPlantTypes()

        findViewById<Button>(R.id.addButton).setOnClickListener {
            showAddPlantPopup()
        }
    }

    private fun fetchPlants() {
        plantsService.fetchPlants({ plants ->
            plantsRecyclerView.adapter = PlantAdapter(plants, ::showEditPlantPopup, ::deletePlant)
        }, { errorMsg ->
            Log.e("PlantsActivity", errorMsg)
        })
    }

    private fun fetchPlantTypes() {
        plantsService.fetchPlantTypes({ types ->
            plantTypes = types
        }, { errorMsg ->
            Log.e("PlantsActivity", errorMsg)
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
                plantsService.addPlant(newPlant, {
                    fetchPlants()
                    Log.d("AddPlant", "Plant added successfully")
                }, { errorMsg ->
                    Log.e("AddPlant", errorMsg)
                })
                popupWindow.dismiss()
            } else {
                Log.d("AddPlantPopup", "Invalid input")
            }
        }

        popupWindow.showAtLocation(window.decorView, Gravity.CENTER, 0, 0)
    }

    private fun showEditPlantPopup(plant: Plant) {
        val inflater = getSystemService(LAYOUT_INFLATER_SERVICE) as LayoutInflater
        val popupView = inflater.inflate(R.layout.popup_update_plant, null)

        val width = LinearLayout.LayoutParams.WRAP_CONTENT
        val height = LinearLayout.LayoutParams.WRAP_CONTENT
        val focusable = true

        val popupWindow = PopupWindow(popupView, width, height, focusable)

        val locationEditText = popupView.findViewById<EditText>(R.id.et_plant_location)
        val lightEditText = popupView.findViewById<EditText>(R.id.et_plant_light)
        val humidityEditText = popupView.findViewById<EditText>(R.id.et_plant_humidity)
        val tempEditText = popupView.findViewById<EditText>(R.id.et_plant_temp)
        val additionalInfoEditText = popupView.findViewById<EditText>(R.id.et_additional_info)

        locationEditText.setText(plant.plantLocation)
        lightEditText.setText(plant.light.toString())
        humidityEditText.setText(plant.humidity.toString())
        tempEditText.setText(plant.temp.toString())
        additionalInfoEditText.setText(plant.additionalInfo)

        val updateButton = popupView.findViewById<Button>(R.id.btn_update)
        updateButton.setOnClickListener {
            val location = locationEditText.text.toString()
            val light = lightEditText.text.toString().toFloatOrNull()
            val humidity = humidityEditText.text.toString().toFloatOrNull()
            val temp = tempEditText.text.toString().toFloatOrNull()
            val additionalInfo = additionalInfoEditText.text.toString()

            if (light != null && humidity != null && temp != null) {
                val updatedPlant = UpdatePlant(location, light, humidity, temp, additionalInfo)
                plantsService.updatePlant(plant.plantId, updatedPlant, {
                    fetchPlants()
                    Log.d("UpdatePlant", "Plant updated successfully")
                }, { errorMsg ->
                    Log.e("UpdatePlant", errorMsg)
                })
                popupWindow.dismiss()
            } else {
                Log.d("UpdatePlantPopup", "Invalid input")
            }
        }

        popupWindow.showAtLocation(window.decorView, Gravity.CENTER, 0, 0)
    }

    private fun deletePlant(plant: Plant) {
        plantsService.deletePlant(plant.plantId, {
            fetchPlants()
            Log.d("DeletePlant", "Plant deleted successfully")
        }, { errorMsg ->
            Log.e("DeletePlant", errorMsg)
        })
    }
}
