package com.example.greenguardmobile.api

import com.example.greenguardmobile.model.Fertilizer
import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST

interface ApiService {

    @GET("api/Fertilizers/fertilizers")
    fun getFertilizers(): Call<List<Fertilizer>>

    //@GET("/fertilizers")
    //fun GetFertilizers(@Body map: HashMap<String, String>): Call<FertilizersInfo>

}