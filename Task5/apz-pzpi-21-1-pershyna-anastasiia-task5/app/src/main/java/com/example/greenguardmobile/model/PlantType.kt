package com.example.greenguardmobile.model

data class PlantType (
    val PlantTypeId : Int,
    val PlantTypeName: String?,
    val WaterFreq : Int,
    val OptLight : Float,
    val OptHumidity : Float,
    val OptTemp : Float,
    val PlantTypeDescription: String?
)