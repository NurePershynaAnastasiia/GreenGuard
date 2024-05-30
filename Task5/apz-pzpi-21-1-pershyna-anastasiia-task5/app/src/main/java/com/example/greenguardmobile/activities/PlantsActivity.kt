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

    private var addPlantPopupState: Bundle? = null
    private var updatePlantPopupState: Bundle? = null
    private var currentPlantId: Int? = null
    private var currentPlantLocation: String? = null
    private var currentPlantLight: Float? = null
    private var currentPlantHumidity: Float? = null
    private var currentPlantTemp: Float? = null
    private var currentPlantAdditionalInfo: String? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_plants)
    }

    override fun onStart() {
        super.onStart()
        setupNavigation()
        initializeServices()
        setupRecyclerView()
        setupAddButton()

        fetchPlants()
        fetchPlantTypes()
    }

    override fun onPause() {
        super.onPause()
        Log.d("PlantsActivity", "onPause called")
        onSaveInstanceState(Bundle())
    }

    override fun onSaveInstanceState(outState: Bundle) {
        super.onSaveInstanceState(outState)

        addPlantPopupState?.let {
            outState.putBundle("addPlantPopupState", it)
        }
        updatePlantPopupState?.let {
            outState.putBundle("updatePlantPopupState", it)
        }
        currentPlantId?.let {
            outState.putInt("currentPlantId", it)
        }
        currentPlantLocation?.let {
            outState.putString("currentPlantLocation", it)
        }
        currentPlantLight?.let {
            outState.putFloat("currentPlantLight", it)
        }
        currentPlantHumidity?.let {
            outState.putFloat("currentPlantHumidity", it)
        }
        currentPlantTemp?.let {
            outState.putFloat("currentPlantTemp", it)
        }
        currentPlantAdditionalInfo?.let {
            outState.putString("currentPlantAdditionalInfo", it)
        }
    }

    override fun onRestoreInstanceState(savedInstanceState: Bundle) {
        super.onRestoreInstanceState(savedInstanceState)

        addPlantPopupState = savedInstanceState.getBundle("addPlantPopupState")
        updatePlantPopupState = savedInstanceState.getBundle("updatePlantPopupState")
        currentPlantId = savedInstanceState.getInt("currentPlantId")
        currentPlantLocation = savedInstanceState.getString("currentPlantLocation")
        currentPlantLight = savedInstanceState.getFloat("currentPlantLight")
        currentPlantHumidity = savedInstanceState.getFloat("currentPlantHumidity")
        currentPlantTemp = savedInstanceState.getFloat("currentPlantTemp")
        currentPlantAdditionalInfo = savedInstanceState.getString("currentPlantAdditionalInfo")
    }

    private fun setupNavigation() {
        val bottomNavMenu = findViewById<BottomNavigationView>(R.id.bottom_navigation)
        NavigationUtils.setupBottomNavigation(bottomNavMenu, this)
        bottomNavMenu.menu.findItem(R.id.plants).isChecked = true

        val toolbar = findViewById<MaterialToolbar>(R.id.toolbar)
        NavigationUtils.setupTopMenu(toolbar, this)
    }

    private fun initializeServices() {
        val apiService = NetworkModule.provideApiService(this)
        plantsService = PlantsService(apiService, this)
    }

    private fun setupRecyclerView() {
        plantsRecyclerView = findViewById(R.id.plants_recycler_view)
        plantsRecyclerView.layoutManager = LinearLayoutManager(this)
    }

    private fun setupAddButton() {
        findViewById<Button>(R.id.addButton).setOnClickListener {
            showAddPlantPopup(addPlantPopupState)
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

    private fun showAddPlantPopup(savedState: Bundle? = null) {
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

        savedState?.let {
            spinnerPlantType.setSelection(it.getInt("plantTypePosition", 0))
            locationEditText.setText(it.getString("location"))
            lightEditText.setText(it.getString("light"))
            humidityEditText.setText(it.getString("humidity"))
            tempEditText.setText(it.getString("temp"))
            additionalInfoEditText.setText(it.getString("additionalInfo"))
        }

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

        popupWindow.setOnDismissListener {
            addPlantPopupState = Bundle().apply {
                putInt("plantTypePosition", spinnerPlantType.selectedItemPosition)
                putString("location", locationEditText.text.toString())
                putString("light", lightEditText.text.toString())
                putString("humidity", humidityEditText.text.toString())
                putString("temp", tempEditText.text.toString())
                putString("additionalInfo", additionalInfoEditText.text.toString())
            }
        }

        popupWindow.showAtLocation(window.decorView, Gravity.CENTER, 0, 0)
    }

    private fun showEditPlantPopup(plant: Plant, savedState: Bundle? = null) {
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

        savedState?.let {
            locationEditText.setText(it.getString("location", plant.plantLocation))
            lightEditText.setText(it.getString("light", plant.light.toString()))
            humidityEditText.setText(it.getString("humidity", plant.humidity.toString()))
            tempEditText.setText(it.getString("temp", plant.temp.toString()))
            additionalInfoEditText.setText(it.getString("additionalInfo", plant.additionalInfo))
        }

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

        popupWindow.setOnDismissListener {
            updatePlantPopupState = Bundle().apply {
                putString("location", locationEditText.text.toString())
                putString("light", lightEditText.text.toString())
                putString("humidity", humidityEditText.text.toString())
                putString("temp", tempEditText.text.toString())
                putString("additionalInfo", additionalInfoEditText.text.toString())
            }
            currentPlantId = plant.plantId
            currentPlantLocation = locationEditText.text.toString()
            currentPlantLight = lightEditText.text.toString().toFloatOrNull()
            currentPlantHumidity = humidityEditText.text.toString().toFloatOrNull()
            currentPlantTemp = tempEditText.text.toString().toFloatOrNull()
            currentPlantAdditionalInfo = additionalInfoEditText.text.toString()
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
