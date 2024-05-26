package com.example.greenguardmobile.model

import java.util.Date

data class Task (
    val TaskId: Int,
    val TaskDate: Date,
    val TaskType: String?,
    val FertilizerId: Int?,
    val TaskDetails: String?,
    val TaskState: String?
)