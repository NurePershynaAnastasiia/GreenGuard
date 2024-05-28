package com.example.greenguardmobile.model

data class AddPlant (
    val plantTypeId : Int,
    val plantLocation: String?,
    val light : Float,
    val humidity : Float,
    val temp : Float,
    val additionalInfo: String?
)