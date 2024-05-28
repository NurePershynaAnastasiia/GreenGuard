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
import com.example.greenguardmobile.adapter.FertilizerAdapter
import com.example.greenguardmobile.api.ApiService
import com.example.greenguardmobile.api.NetworkModule
import com.example.greenguardmobile.model.Fertilizer
import com.example.greenguardmobile.model.AddFertilizer
import com.example.greenguardmobile.util.NavigationUtils
import com.google.android.material.appbar.MaterialToolbar
import com.google.android.material.bottomnavigation.BottomNavigationView
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class FertilizersActivity : AppCompatActivity() {

    private lateinit var apiService: ApiService
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
        fertilizerAdapter = FertilizerAdapter(mutableListOf())
        recyclerView.adapter = fertilizerAdapter

        apiService = NetworkModule.provideApiService(this)

        fetchFertilizers()

        findViewById<Button>(R.id.addButton).setOnClickListener {
            showAddFertilizerPopup()
        }
    }

    private fun fetchFertilizers() {
        apiService.getFertilizers().enqueue(object : Callback<List<Fertilizer>> {
            override fun onResponse(call: Call<List<Fertilizer>>, response: Response<List<Fertilizer>>) {
                if (response.isSuccessful) {
                    response.body()?.let { fertilizers ->
                        fertilizerAdapter.setFertilizers(fertilizers)
                    }
                }
            }

            override fun onFailure(call: Call<List<Fertilizer>>, t: Throwable) {
                t.printStackTrace()
            }
        })
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
                addFertilizer(newFertilizer)
                popupWindow.dismiss()
            } else {
                Log.d("AddFertilizerPopup", "Invalid input")
            }
        }

        popupWindow.showAtLocation(window.decorView, Gravity.CENTER, 0, 0)
    }

    private fun addFertilizer(fertilizer: AddFertilizer) {
        apiService.addFertilizer(fertilizer).enqueue(object : Callback<Void> {
            override fun onResponse(call: Call<Void>, response: Response<Void>) {
                if (response.isSuccessful) {
                    fetchFertilizers()
                    Log.d("AddFertilizer", "Fertilizer added successfully")
                } else {
                    Log.e("AddFertilizer", "Error: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                Log.e("AddFertilizer", "Network error")
                t.printStackTrace()
            }
        })
    }
}
