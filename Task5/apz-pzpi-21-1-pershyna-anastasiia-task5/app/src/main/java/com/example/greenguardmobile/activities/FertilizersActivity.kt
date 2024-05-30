package com.example.greenguardmobile.presentation.activities

import android.os.Bundle
import android.util.Log
import android.view.Gravity
import android.view.LayoutInflater
import android.widget.Button
import android.widget.EditText
import android.widget.LinearLayout
import android.widget.PopupWindow
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.greenguardmobile.R
import com.example.greenguardmobile.presentation.adapters.FertilizerAdapter
import com.example.greenguardmobile.network.ApiService
import com.example.greenguardmobile.network.NetworkModule
import com.example.greenguardmobile.domain.models.fertilizer.Fertilizer
import com.example.greenguardmobile.domain.models.fertilizer.AddFertilizer
import com.example.greenguardmobile.service.FertilizersService
import com.example.greenguardmobile.presentation.util.NavigationUtils
import com.google.android.material.appbar.MaterialToolbar
import com.google.android.material.bottomnavigation.BottomNavigationView

class FertilizersActivity : AppCompatActivity() {

    private lateinit var apiService: ApiService
    private lateinit var fertilizersService: FertilizersService
    private lateinit var recyclerView: RecyclerView
    private lateinit var fertilizerAdapter: FertilizerAdapter

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_fertilizers)

        val bottomNavMenu = findViewById<BottomNavigationView>(R.id.bottom_navigation)
        NavigationUtils.setupBottomNavigation(bottomNavMenu, this)
        bottomNavMenu.menu.findItem(R.id.fertilizers).setChecked(true)

        val toolbar = findViewById<MaterialToolbar>(R.id.toolbar)
        NavigationUtils.setupTopMenu(toolbar, this)

        recyclerView = findViewById(R.id.recyclerView)
        recyclerView.layoutManager = LinearLayoutManager(this)

        fertilizerAdapter = FertilizerAdapter(mutableListOf(),
            onDeleteClick = { fertilizer -> deleteFertilizer(fertilizer) },
            onUpdateQuantityClick = { fertilizer -> showUpdateFertilizerPopup(fertilizer) })

        recyclerView.adapter = fertilizerAdapter

        apiService = NetworkModule.provideApiService(this)
        fertilizersService = FertilizersService(apiService, this)

        fetchFertilizers()

        findViewById<Button>(R.id.addButton).setOnClickListener {
            showAddFertilizerPopup()
        }
    }

    private fun fetchFertilizers() {
        fertilizersService.fetchFertilizers { fertilizers ->
            updateFertilizerList(fertilizers)
        }
    }

    fun updateFertilizerList(fertilizers: List<Fertilizer>) {
        fertilizerAdapter.setFertilizers(fertilizers)
    }

    private fun showAddFertilizerPopup() {
        val inflater = getSystemService(LAYOUT_INFLATER_SERVICE) as LayoutInflater
        val popupView = inflater.inflate(R.layout.popup_add_fertilizer, null)

        val width = LinearLayout.LayoutParams.WRAP_CONTENT
        val height = LinearLayout.LayoutParams.WRAP_CONTENT
        val focusable = true

        val popupWindow = PopupWindow(popupView, width, height, focusable)

        val addButtonPopup = popupView.findViewById<Button>(R.id.addButtonPopup)
        val nameEditText = popupView.findViewById<EditText>(R.id.et_fertilizer_name)
        val quantityEditText = popupView.findViewById<EditText>(R.id.et_fertilizer_quantity)

        addButtonPopup.setOnClickListener {
            val name = nameEditText.text.toString()
            val quantity = quantityEditText.text.toString().toIntOrNull()

            if (name.isNotBlank() && quantity != null) {
                val newFertilizer = AddFertilizer(name, quantity)
                fertilizersService.addFertilizer(newFertilizer)
                popupWindow.dismiss()
            } else {
                Log.d("AddFertilizerPopup", "Invalid input")
            }
        }

        popupWindow.showAtLocation(window.decorView, Gravity.CENTER, 0, 0)
    }

    private fun deleteFertilizer(fertilizer: Fertilizer) {
        fertilizersService.deleteFertilizer(fertilizer)
    }

    private fun showUpdateFertilizerPopup(fertilizer: Fertilizer) {
        val inflater = getSystemService(LAYOUT_INFLATER_SERVICE) as LayoutInflater
        val popupView = inflater.inflate(R.layout.popup_update_fertilizer_quantity, null)

        val width = LinearLayout.LayoutParams.WRAP_CONTENT
        val height = LinearLayout.LayoutParams.WRAP_CONTENT
        val focusable = true

        val popupWindow = PopupWindow(popupView, width, height, focusable)

        val addButtonPopup = popupView.findViewById<Button>(R.id.addButtonPopup)
        val quantityEditText = popupView.findViewById<EditText>(R.id.et_fertilizer_quantity)

        addButtonPopup.setOnClickListener {
            val newQuantity = quantityEditText.text.toString().toIntOrNull()

            if (newQuantity != null) {
                fertilizersService.updateFertilizerQuantity(fertilizer, newQuantity)
                popupWindow.dismiss()
            } else {
                Log.d("UpdateFertilizer", "Invalid input")
            }
        }

        popupWindow.showAtLocation(window.decorView, Gravity.CENTER, 0, 0)
    }
}
