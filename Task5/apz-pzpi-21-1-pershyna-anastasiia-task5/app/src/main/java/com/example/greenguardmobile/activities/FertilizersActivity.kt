package com.example.greenguardmobile.activities

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
import com.example.greenguardmobile.adapters.FertilizerAdapter
import com.example.greenguardmobile.network.ApiService
import com.example.greenguardmobile.network.NetworkModule
import com.example.greenguardmobile.models.fertilizer.Fertilizer
import com.example.greenguardmobile.models.fertilizer.AddFertilizer
import com.example.greenguardmobile.service.FertilizersService
import com.example.greenguardmobile.util.NavigationUtils
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
    }

    override fun onStart() {
        super.onStart()
        initializeViews()
        initializeServices()
        setupRecyclerView()
        setupNavigation()
        fetchFertilizers()

        findViewById<Button>(R.id.addButton).setOnClickListener {
            showAddFertilizerPopup()
        }
    }

    override fun onResume() {
        super.onResume()
        Log.d("FertilizersActivity", "onResume called")
    }

    override fun onPause() {
        super.onPause()
        Log.d("FertilizersActivity", "onPause called")
    }

    override fun onStop() {
        super.onStop()
        Log.d("FertilizersActivity", "onStop called")
    }

    override fun onDestroy() {
        super.onDestroy()
        Log.d("FertilizersActivity", "onDestroy called")
    }

    private fun initializeViews() {
        recyclerView = findViewById(R.id.recyclerView)
    }

    private fun initializeServices() {
        apiService = NetworkModule.provideApiService(this)
        fertilizersService = FertilizersService(apiService, this)
    }

    private fun setupRecyclerView() {
        recyclerView.layoutManager = LinearLayoutManager(this)
        fertilizerAdapter = FertilizerAdapter(
            mutableListOf(),
            onDeleteClick = { fertilizer -> deleteFertilizer(fertilizer) },
            onUpdateQuantityClick = { fertilizer -> showUpdateFertilizerPopup(fertilizer) }
        )
        recyclerView.adapter = fertilizerAdapter
    }

    private fun setupNavigation() {
        val bottomNavMenu = findViewById<BottomNavigationView>(R.id.bottom_navigation)
        NavigationUtils.setupBottomNavigation(bottomNavMenu, this)
        bottomNavMenu.menu.findItem(R.id.fertilizers).isChecked = true

        val toolbar = findViewById<MaterialToolbar>(R.id.toolbar)
        NavigationUtils.setupTopMenu(toolbar, this)
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

        val popupWindow = PopupWindow(
            popupView,
            LinearLayout.LayoutParams.WRAP_CONTENT,
            LinearLayout.LayoutParams.WRAP_CONTENT,
            true
        )

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

        val popupWindow = PopupWindow(
            popupView,
            LinearLayout.LayoutParams.WRAP_CONTENT,
            LinearLayout.LayoutParams.WRAP_CONTENT,
            true
        )

        val updateButtonPopup = popupView.findViewById<Button>(R.id.updateButtonPopup)
        val quantityEditText = popupView.findViewById<EditText>(R.id.et_fertilizer_quantity)

        updateButtonPopup.setOnClickListener {
            val newQuantity = quantityEditText.text.toString().toIntOrNull()

            if (newQuantity != null) {
                fertilizersService.updateFertilizerQuantity(fertilizer, newQuantity)
                popupWindow.dismiss()
            } else {
                Log.d("UpdateFertilizerPopup", "Invalid input")
            }
        }

        popupWindow.showAtLocation(window.decorView, Gravity.CENTER, 0, 0)
    }
}
