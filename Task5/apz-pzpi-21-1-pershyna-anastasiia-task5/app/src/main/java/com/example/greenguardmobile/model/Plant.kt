package com.example.greenguardmobile.model

data class Plant (
    val PlantId : Int,
    val PlantTypeId : Int,
    val PlantLocation: String?,
    val Light : Float,
    val Humidity : Float,
    val Temp : Float,
    val AdditionalInfo: String?,
    val PlantState: String?,
)