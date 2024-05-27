package com.example.greenguardmobile.model

data class PlantType (
    val plantTypeId : Int,
    val plantTypeName: String?,
    val waterFreq : Int,
    val optLight : Float,
    val optHumidity : Float,
    val optTemp : Float,
    val plantTypeDescription: String?
)