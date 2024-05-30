package com.example.greenguardmobile.domain.models.worker

data class UpdateWorker(
    val workerName: String,
    val phoneNumber: String,
    val email: String,
    val startWorkTime: String,
    val endWorkTime: String,
)