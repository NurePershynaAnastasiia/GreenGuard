package com.example.greenguardmobile.model

import java.sql.Time
import java.time.LocalTime

data class Worker(
    val workerId: Int,
    val workerName: String,
    val phoneNumber: String,
    val email: String,
    val startWorkTime: String,
    val endWorkTime: String,
    val isAdmin: Boolean
)