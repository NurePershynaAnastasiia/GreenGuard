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

    private var addFertilizerPopupState: Bundle? = null
    private var updateFertilizerPopupState: Bundle? = null
    private var currentFertilizerId: Int? = null
    private var currentFertilizerName: String? = null
    private var currentFertilizerQuantity: Int? = null

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
            showAddFertilizerPopup(addFertilizerPopupState)
        }
    }

    override fun onSaveInstanceState(outState: Bundle) {
        super.onSaveInstanceState(outState)

        addFertilizerPopupState?.let {
            outState.putBundle("addFertilizerPopupState", it)
        }
        updateFertilizerPopupState?.let {
            outState.putBundle("updateFertilizerPopupState", it)
        }
        currentFertilizerId?.let {
            outState.putInt("currentFertilizerId", it)
        }
        currentFertilizerName?.let {
            outState.putString("currentFertilizerName", it)
        }
        currentFertilizerQuantity?.let {
            outState.putInt("currentFertilizerQuantity", it)
        }
    }

    override fun onRestoreInstanceState(savedInstanceState: Bundle) {
        super.onRestoreInstanceState(savedInstanceState)

        addFertilizerPopupState = savedInstanceState.getBundle("addFertilizerPopupState")
        updateFertilizerPopupState = savedInstanceState.getBundle("updateFertilizerPopupState")
        currentFertilizerId = savedInstanceState.getInt("currentFertilizerId")
        currentFertilizerName = savedInstanceState.getString("currentFertilizerName")
        currentFertilizerQuantity = savedInstanceState.getInt("currentFertilizerQuantity")
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
            onUpdateQuantityClick = { fertilizer -> showUpdateFertilizerPopup(fertilizer, updateFertilizerPopupState) }
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

    private fun showAddFertilizerPopup(savedState: Bundle? = null) {
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

        savedState?.let {
            nameEditText.setText(it.getString("name"))
            quantityEditText.setText(it.getString("quantity"))
        }

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

        popupWindow.setOnDismissListener {
            addFertilizerPopupState = Bundle().apply {
                putString("name", nameEditText.text.toString())
                putString("quantity", quantityEditText.text.toString())
            }
        }

        popupWindow.showAtLocation(window.decorView, Gravity.CENTER, 0, 0)
    }

    private fun showUpdateFertilizerPopup(fertilizer: Fertilizer, savedState: Bundle? = null) {
        currentFertilizerId = fertilizer.fertilizerId
        currentFertilizerName = fertilizer.fertilizerName
        currentFertilizerQuantity = fertilizer.fertilizerQuantity

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

        savedState?.let {
            quantityEditText.setText(it.getString("quantity"))
        } ?: run {
            quantityEditText.setText(fertilizer.fertilizerQuantity.toString())
        }

        updateButtonPopup.setOnClickListener {
            val newQuantity = quantityEditText.text.toString().toIntOrNull()

            if (newQuantity != null) {
                fertilizersService.updateFertilizerQuantity(fertilizer, newQuantity)
                popupWindow.dismiss()
            } else {
                Log.d("UpdateFertilizerPopup", "Invalid input")
            }
        }

        popupWindow.setOnDismissListener {
            updateFertilizerPopupState = Bundle().apply {
                putString("quantity", quantityEditText.text.toString())
            }
        }

        popupWindow.showAtLocation(window.decorView, Gravity.CENTER, 0, 0)
    }

    private fun deleteFertilizer(fertilizer: Fertilizer) {
        fertilizersService.deleteFertilizer(fertilizer)
    }
}
