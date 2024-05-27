package com.example.greenguardmobile.adapter

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.greenguardmobile.R
import com.example.greenguardmobile.model.Task
import java.text.SimpleDateFormat
import java.util.Locale

class TaskAdapter(private val tasks: MutableList<Task>) : RecyclerView.Adapter<TaskAdapter.TaskViewHolder>() {

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
        holder.bind(tasks[position])
    }

    override fun getItemCount(): Int = tasks.size

    class TaskViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        private val dateFormat = SimpleDateFormat("dd/MM/yyyy", Locale.getDefault())
        private val tvTaskDate: TextView = itemView.findViewById(R.id.tv_task_date)
        private val tvTaskType: TextView = itemView.findViewById(R.id.tv_task_type)
        private val tvTaskDetails: TextView = itemView.findViewById(R.id.tv_task_details)
        private val tvTaskState: TextView = itemView.findViewById(R.id.tv_task_state)

        fun bind(task: Task) {
            tvTaskDate.text = dateFormat.format(task.taskDate)
            tvTaskType.text = task.taskType
            tvTaskDetails.text = task.taskDetails
            tvTaskState.text = task.taskState
        }
    }
}
