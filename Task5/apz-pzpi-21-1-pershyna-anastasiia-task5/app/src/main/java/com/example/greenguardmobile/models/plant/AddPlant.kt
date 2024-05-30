package com.example.greenguardmobile.domain.models.plant

data class AddPlant (
    val plantTypeId : Int,
    val plantLocation: String?,
    val light : Float,
    val humidity : Float,
    val temp : Float,
    val additionalInfo: String?
)