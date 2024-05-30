package com.example.greenguardmobile.service

import android.util.Log
import com.example.greenguardmobile.activities.PestsActivity
import com.example.greenguardmobile.models.plant.Plant
import com.example.greenguardmobile.models.pest.Pest
import com.example.greenguardmobile.network.ApiService
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class PestsService(private val apiService: ApiService, private val activity: PestsActivity) {

    fun fetchPests(onSuccess: (List<Pest>) -> Unit) {
        apiService.getPests().enqueue(object : Callback<List<Pest>> {
            override fun onResponse(call: Call<List<Pest>>, response: Response<List<Pest>>) {
                if (response.isSuccessful) {
                    response.body()?.let { pests ->
                        onSuccess(pests)
                    }
                }
            }

            override fun onFailure(call: Call<List<Pest>>, t: Throwable) {
                Log.d("fetchPests", "onFailure")
                t.printStackTrace()
            }
        })
    }

    fun fetchPlants(onSuccess: (List<Plant>) -> Unit) {
        apiService.getPlants().enqueue(object : Callback<List<Plant>> {
            override fun onResponse(call: Call<List<Plant>>, response: Response<List<Plant>>) {
                if (response.isSuccessful) {
                    response.body()?.let { plants ->
                        onSuccess(plants)
                    }
                }
            }

            override fun onFailure(call: Call<List<Plant>>, t: Throwable) {
                Log.d("fetchPlants", "onFailure")
                t.printStackTrace()
            }
        })
    }

    fun addPestToPlant(pestId: Int, plantId: Int, onSuccess: () -> Unit, onError: (String) -> Unit) {
        apiService.addPestToPlant(pestId, plantId).enqueue(object : Callback<Void> {
            override fun onResponse(call: Call<Void>, response: Response<Void>) {
                if (response.isSuccessful) {
                    onSuccess()
                } else {
                    val errorMsg = "Error: ${response.code()} ${response.message()}"
                    Log.e("AddToPlant", errorMsg)
                    onError(errorMsg)
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                val errorMsg = "Network error"
                Log.e("AddToPlant", errorMsg)
                t.printStackTrace()
                onError(errorMsg)
            }
        })
    }

    fun deletePestFromPlant(pestId: Int, plantId: Int, onSuccess: () -> Unit, onError: (String) -> Unit) {
        apiService.deletePestFromPlant(pestId, plantId).enqueue(object : Callback<Void> {
            override fun onResponse(call: Call<Void>, response: Response<Void>) {
                if (response.isSuccessful) {
                    onSuccess()
                } else {
                    val errorMsg = "Error: ${response.code()} ${response.message()}"
                    Log.e("RemoveFromPlant", errorMsg)
                    onError(errorMsg)
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                val errorMsg = "Network error"
                Log.e("RemoveFromPlant", errorMsg)
                t.printStackTrace()
                onError(errorMsg)
            }
        })
    }
}
