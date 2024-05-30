package com.example.greenguardmobile.service

import android.util.Log
import android.widget.Toast
import com.example.greenguardmobile.activities.FertilizersActivity
import com.example.greenguardmobile.models.fertilizer.AddFertilizer
import com.example.greenguardmobile.models.fertilizer.Fertilizer
import com.example.greenguardmobile.models.fertilizer.UpdateFertilizerQuantity
import com.example.greenguardmobile.network.ApiService
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class FertilizersService(private val apiService: ApiService, private val activity: FertilizersActivity) {

    fun fetchFertilizers(onSuccess: (List<Fertilizer>) -> Unit) {
        apiService.getFertilizers().enqueue(object : Callback<List<Fertilizer>> {
            override fun onResponse(call: Call<List<Fertilizer>>, response: Response<List<Fertilizer>>) {
                if (response.isSuccessful) {
                    response.body()?.let { fertilizers ->
                        onSuccess(fertilizers)
                    }
                }
            }

            override fun onFailure(call: Call<List<Fertilizer>>, t: Throwable) {
                t.printStackTrace()
            }
        })
    }

    fun addFertilizer(fertilizer: AddFertilizer) {
        apiService.addFertilizer(fertilizer).enqueue(object : Callback<Void> {
            override fun onResponse(call: Call<Void>, response: Response<Void>) {
                if (response.isSuccessful) {
                    fetchFertilizers { fertilizers ->
                        activity.updateFertilizerList(fertilizers)
                    }
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

    fun deleteFertilizer(fertilizer: Fertilizer) {
        apiService.deleteFertilizer(fertilizer.fertilizerId).enqueue(object : Callback<Void> {
            override fun onResponse(call: Call<Void>, response: Response<Void>) {
                if (response.isSuccessful) {
                    fetchFertilizers { fertilizers ->
                        activity.updateFertilizerList(fertilizers)
                    }
                    Log.d("DeleteFertilizer", "Fertilizer deleted successfully")
                } else {
                    Log.e("DeleteFertilizer", "Error: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                Log.e("DeleteFertilizer", "Network error")
                t.printStackTrace()
            }
        })
    }

    fun updateFertilizerQuantity(fertilizer: Fertilizer, newQuantity: Int) {
        apiService.updateFertilizerQuantity(fertilizer.fertilizerId, UpdateFertilizerQuantity(newQuantity)).enqueue(object : Callback<Void> {
            override fun onResponse(call: Call<Void>, response: Response<Void>) {
                if (response.isSuccessful) {
                    fetchFertilizers { fertilizers ->
                        activity.updateFertilizerList(fertilizers)
                    }
                    Log.d("UpdateFertilizer", "Fertilizer quantity updated successfully")
                } else {
                    Log.e("UpdateFertilizer", "Error: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                Log.e("UpdateFertilizer", "Network error")
                t.printStackTrace()
            }
        })
    }
}
