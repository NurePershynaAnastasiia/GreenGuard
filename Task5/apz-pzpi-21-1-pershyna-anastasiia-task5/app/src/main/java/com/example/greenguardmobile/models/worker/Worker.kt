package com.example.greenguardmobile.models.worker

data class Worker(
    val workerId: Int,
    val workerName: String,
    val phoneNumber: String,
    val email: String,
    val startWorkTime: String,
    val endWorkTime: String,
    val isAdmin: Boolean
)