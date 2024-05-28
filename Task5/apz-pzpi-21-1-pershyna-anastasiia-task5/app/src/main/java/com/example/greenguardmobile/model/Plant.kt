package com.example.greenguardmobile.model

data class Plant (
    val plantId : Int,
    val plantTypeName : String,
    val plantLocation: String?,
    val light : Float,
    val humidity : Float,
    val temp : Float,
    val additionalInfo: String?,
    val plantState: String?,
    val pests: List<String>
)