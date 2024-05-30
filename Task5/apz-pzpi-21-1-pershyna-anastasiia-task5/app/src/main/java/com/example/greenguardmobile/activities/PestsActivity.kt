package com.example.greenguardmobile.presentation.activities

import android.os.Bundle
import android.util.Log
import android.view.Gravity
import android.view.LayoutInflater
import android.widget.*
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.greenguardmobile.R
import com.example.greenguardmobile.presentation.adapters.PestAdapter
import com.example.greenguardmobile.network.ApiService
import com.example.greenguardmobile.network.NetworkModule
import com.example.greenguardmobile.domain.models.plant.Plant
import com.example.greenguardmobile.domain.models.pest.Pest
import com.example.greenguardmobile.service.PestsService
import com.example.greenguardmobile.presentation.util.NavigationUtils
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
        pestsService = PestsService(apiService, this)

        fetchPests()
        fetchPlants()
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
                Log.d("AddToPlant", "Pest added to plant successfully")
                Toast.makeText(this@PestsActivity, "Pest added to plant successfully", Toast.LENGTH_SHORT).show()
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
                Toast.makeText(this@PestsActivity, "Pest removed from plant successfully", Toast.LENGTH_SHORT).show()
            }, { errorMsg ->
                Log.e("RemoveFromPlant", errorMsg)
                Toast.makeText(this@PestsActivity, "Pest is not associated with selected plant", Toast.LENGTH_SHORT).show()
            })
        }

        popupWindow.showAtLocation(window.decorView, Gravity.CENTER, 0, 0)
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
}
