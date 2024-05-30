package com.example.greenguardmobile.service

import android.content.Context
import com.example.greenguardmobile.models.plant.AddPlant
import com.example.greenguardmobile.models.plant.Plant
import com.example.greenguardmobile.models.plant.PlantType
import com.example.greenguardmobile.models.plant.UpdatePlant
import com.example.greenguardmobile.network.ApiService
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class PlantsService(private val apiService: ApiService, private val context: Context) {

    fun fetchPlants(onSuccess: (List<Plant>) -> Unit, onError: (String) -> Unit) {
        apiService.getPlants().enqueue(object : Callback<List<Plant>> {
            override fun onResponse(call: Call<List<Plant>>, response: Response<List<Plant>>) {
                if (response.isSuccessful) {
                    response.body()?.let { plants ->
                        onSuccess(plants)
                    } ?: onError("Failed to parse plants")
                } else {
                    onError("Error: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<List<Plant>>, t: Throwable) {
                onError("Network error")
                t.printStackTrace()
            }
        })
    }

    fun fetchPlantTypes(onSuccess: (List<PlantType>) -> Unit, onError: (String) -> Unit) {
        apiService.getPlantTypes().enqueue(object : Callback<List<PlantType>> {
            override fun onResponse(call: Call<List<PlantType>>, response: Response<List<PlantType>>) {
                if (response.isSuccessful) {
                    response.body()?.let { types ->
                        onSuccess(types)
                    } ?: onError("Failed to parse plant types")
                } else {
                    onError("Error: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<List<PlantType>>, t: Throwable) {
                onError("Network error")
                t.printStackTrace()
            }
        })
    }

    fun addPlant(plant: AddPlant, onSuccess: () -> Unit, onError: (String) -> Unit) {
        apiService.addPlant(plant).enqueue(object : Callback<Void> {
            override fun onResponse(call: Call<Void>, response: Response<Void>) {
                if (response.isSuccessful) {
                    onSuccess()
                } else {
                    onError("Error: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                onError("Network error")
                t.printStackTrace()
            }
        })
    }

    fun updatePlant(plantId: Int, updatedPlant: UpdatePlant, onSuccess: () -> Unit, onError: (String) -> Unit) {
        apiService.updatePlant(plantId, updatedPlant).enqueue(object : Callback<Void> {
            override fun onResponse(call: Call<Void>, response: Response<Void>) {
                if (response.isSuccessful) {
                    onSuccess()
                } else {
                    onError("Error: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                onError("Network error")
                t.printStackTrace()
            }
        })
    }

    fun deletePlant(plantId: Int, onSuccess: () -> Unit, onError: (String) -> Unit) {
        apiService.deletePlant(plantId).enqueue(object : Callback<Void> {
            override fun onResponse(call: Call<Void>, response: Response<Void>) {
                if (response.isSuccessful) {
                    onSuccess()
                } else {
                    onError("Error: ${response.code()} ${response.message()}")
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                onError("Network error")
                t.printStackTrace()
            }
        })
    }
}
