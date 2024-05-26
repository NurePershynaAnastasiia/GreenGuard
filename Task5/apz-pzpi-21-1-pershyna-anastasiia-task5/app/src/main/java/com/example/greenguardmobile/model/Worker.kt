package com.example.greenguardmobile.model

import java.sql.Time
import java.time.LocalTime

data class Worker (
    val WorkerId: Int,
    val WorkerName: String?,
    val PhoneNumber: String?,
    val Email: String?,
    val StartWorkTime: LocalTime?,
    val EndWorkTime: LocalTime?,
    val PasswordHash: String?,
    val IsAdmin: Boolean?
)