package com.example.greenguardmobile.adapters

import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ArrayAdapter
import android.widget.Button
import android.widget.PopupWindow
import android.widget.Spinner
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.greenguardmobile.R
import com.example.greenguardmobile.network.ApiService
import com.example.greenguardmobile.models.task.Task
import com.example.greenguardmobile.models.task.TaskStatus
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import java.text.SimpleDateFormat
import java.util.Locale

class TaskAdapter(
    private val tasks: MutableList<Task>,
    private val apiService: ApiService,
    private val workerId: Int
) : RecyclerView.Adapter<TaskAdapter.TaskViewHolder>() {

    fun setTasks(newTasks: List<Task>) {
        tasks.clear()
        tasks.addAll(newTasks)
        notifyDataSetChanged()
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): TaskViewHolder {
        val view = LayoutInflater.from(parent.context).inflate(R.layout.item_task, parent, false)
        return TaskViewHolder(view)
    }

    override fun onBindViewHolder(holder: TaskViewHolder, position: Int) {
        holder.bind(tasks[position], apiService, workerId)
    }

    override fun getItemCount(): Int = tasks.size

    class TaskViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        private val dateFormat = SimpleDateFormat("dd/MM/yyyy", Locale.getDefault())
        private val tvTaskDate: TextView = itemView.findViewById(R.id.tv_task_date)
        private val tvTaskType: TextView = itemView.findViewById(R.id.tv_task_type)
        private val tvTaskDetails: TextView = itemView.findViewById(R.id.tv_task_details)
        private val tvTaskState: TextView = itemView.findViewById(R.id.tv_task_state)
        private val btnChangeStatus: Button = itemView.findViewById(R.id.btn_change_status)

        fun bind(task: Task, apiService: ApiService, workerId: Int) {
            tvTaskDate.text = dateFormat.format(task.taskDate)
            tvTaskType.text = task.taskType
            tvTaskDetails.text = task.taskDetails

            // Запит для отримання статусу завдання
            apiService.getTaskStatus(task.taskId, workerId).enqueue(object : Callback<TaskStatus> {
                override fun onResponse(call: Call<TaskStatus>, response: Response<TaskStatus>) {
                    if (response.isSuccessful) {
                        task.taskStatus = response.body()?.workerTaskStatus;
                        tvTaskState.text = task.taskStatus
                    } else {
                        Log.e("TaskAdapter get", "Error: ${response.code()} ${response.message()}")
                        tvTaskState.text = "Error: ${response.code()}"
                    }
                }

                override fun onFailure(call: Call<TaskStatus>, t: Throwable) {
                    Log.e("TaskAdapter get", "Network error", t)
                    tvTaskState.text = "Network error"
                }

            })

            // Обробка натискання кнопки для зміни статусу
            btnChangeStatus.setOnClickListener {
                showChangeStatusPopup(apiService, task, workerId)
            }
        }

        private fun showChangeStatusPopup(apiService: ApiService, task: Task, workerId: Int) {
            val context = itemView.context
            val inflater = LayoutInflater.from(context)
            val popupView = inflater.inflate(R.layout.popup_update_task_status, null)

            val popupWindow = PopupWindow(
                popupView,
                ViewGroup.LayoutParams.MATCH_PARENT,
                ViewGroup.LayoutParams.WRAP_CONTENT,
                true
            )

            val spinner = popupView.findViewById<Spinner>(R.id.spinner_status)
            val btnOk = popupView.findViewById<Button>(R.id.btn_ok)

            val statusOptions = arrayOf("не розпочато", "в процесі", "закінчено", "відмінено")
            val statusValues = arrayOf("pending", "in process", "finished", "cancelled")

            val adapter = ArrayAdapter(context, android.R.layout.simple_spinner_item, statusOptions)
            adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
            spinner.adapter = adapter

            btnOk.setOnClickListener {
                val selectedStatus = spinner.selectedItemPosition
                val newStatus = statusValues[selectedStatus]

                // Відправка нового статусу на сервер
                apiService.updateTaskStatus(task.taskId, workerId, newStatus).enqueue(object : Callback<Void> {
                    override fun onResponse(call: Call<Void>, response: Response<Void>) {
                        if (response.isSuccessful) {
                            task.taskStatus = newStatus
                            tvTaskState.text = task.taskStatus
                            popupWindow.dismiss()
                        } else {
                            Log.e("TaskAdapter update", "Error: ${response.code()} ${response.message()}")
                        }
                    }

                    override fun onFailure(call: Call<Void>, t: Throwable) {
                        Log.e("TaskAdapter update", "Network error", t)
                    }
                })
            }

            popupWindow.showAtLocation(itemView, android.view.Gravity.CENTER, 0, 0)
        }
    }
}