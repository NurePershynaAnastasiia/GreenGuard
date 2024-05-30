package com.example.greenguardmobile.models.task

import java.util.Date

data class Task (
    val taskId: Int,
    val taskDate: Date,
    val taskType: String?,
    val fertilizerId: Int?,
    val taskDetails: String?,
    var taskStatus: String?
)