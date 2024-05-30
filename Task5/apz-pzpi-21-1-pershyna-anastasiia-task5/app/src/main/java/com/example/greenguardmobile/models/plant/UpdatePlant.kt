package com.example.greenguardmobile.models.plant

data class UpdatePlant (
    val plantLocation: String?,
    val light : Float,
    val humidity : Float,
    val temp : Float,
    val additionalInfo: String?
)