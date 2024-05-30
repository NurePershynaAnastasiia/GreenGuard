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
import com.example.greenguardmobile.adapters.PestAdapter
import com.example.greenguardmobile.network.ApiService
import com.example.greenguardmobile.network.NetworkModule
import com.example.greenguardmobile.models.plant.Plant
import com.example.greenguardmobile.models.pest.Pest
import com.example.greenguardmobile.service.PestsService
import com.example.greenguardmobile.util.NavigationUtils
import com.google.android.material.appbar.MaterialToolbar
import com.google.android.material.bottomnavigation.BottomNavigationView

class PestsActivity : AppCompatActivity() {

    private lateinit var apiService: ApiService
    private lateinit var pestsService: PestsService
    private lateinit var recyclerView: RecyclerView
    private lateinit var pestAdapter: PestAdapter
    private lateinit var plants: List<Plant>

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_pests)
    }

    override fun onStart() {
        super.onStart()
        initViews()
        setupNavigation()
        setupRecyclerView()
        initializeServices()
        fetchPests()
        fetchPlants()
    }

    private fun initViews() {
        recyclerView = findViewById(R.id.recyclerView)
    }

    private fun setupNavigation() {
        val bottomNavMenu = findViewById<BottomNavigationView>(R.id.bottom_navigation)
        NavigationUtils.setupBottomNavigation(bottomNavMenu, this)
        bottomNavMenu.menu.findItem(R.id.pests).isChecked = true

        val toolbar = findViewById<MaterialToolbar>(R.id.toolbar)
        NavigationUtils.setupTopMenu(toolbar, this)
    }

    private fun setupRecyclerView() {
        recyclerView.layoutManager = LinearLayoutManager(this)
        pestAdapter = PestAdapter(mutableListOf(), { pest ->
            showAddToPlantPopup(pest)
        }, { pest ->
            showRemoveFromPlantPopup(pest)
        })
        recyclerView.adapter = pestAdapter
    }

    private fun initializeServices() {
        apiService = NetworkModule.provideApiService(this)
        pestsService = PestsService(apiService, this)
    }

    private fun fetchPests() {
        pestsService.fetchPests { pests ->
            pestAdapter.setPests(pests)
        }
    }

    private fun fetchPlants() {
        pestsService.fetchPlants { plantsList ->
            plants = plantsList
        }
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

            pestsService.addPestToPlant(pest.pestId, selectedPlantId, {
                popupWindow.dismiss()
                Log.d("AddToPlant", getResources().getString(R.string.pest_added_success))
                Toast.makeText(this@PestsActivity, getResources().getString(R.string.pest_added_success), Toast.LENGTH_SHORT).show()
            }, { errorMsg ->
                Log.e("AddToPlant", errorMsg)
                Toast.makeText(this@PestsActivity, errorMsg, Toast.LENGTH_SHORT).show()
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

            pestsService.deletePestFromPlant(pest.pestId, selectedPlantId, {
                popupWindow.dismiss()
                Log.d("RemoveFromPlant", "Pest removed from plant successfully")
                Toast.makeText(this@PestsActivity, getResources().getString(R.string.pest_removed_success), Toast.LENGTH_SHORT).show()
            }, { errorMsg ->
                Log.e("RemoveFromPlant", errorMsg)
                Toast.makeText(this@PestsActivity, getResources().getString(R.string.no_pest_in_plant), Toast.LENGTH_SHORT).show()
            })
        }

        popupWindow.showAtLocation(window.decorView, Gravity.CENTER, 0, 0)
    }
}
