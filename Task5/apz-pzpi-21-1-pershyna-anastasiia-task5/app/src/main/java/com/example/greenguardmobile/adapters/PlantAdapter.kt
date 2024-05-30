package com.example.greenguardmobile.adapters

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.greenguardmobile.R
import com.example.greenguardmobile.models.plant.Plant

class PlantAdapter(
    private val plants: List<Plant>,
    private val onEditClick: (Plant) -> Unit,
    private val onDeleteClick: (Plant) -> Unit
) : RecyclerView.Adapter<PlantAdapter.PlantViewHolder>() {

    class PlantViewHolder(view: View) : RecyclerView.ViewHolder(view) {
        val plantType: TextView = view.findViewById(R.id.plant_type)
        val plantLocation: TextView = view.findViewById(R.id.plant_location)
        val plantStatus: TextView = view.findViewById(R.id.plant_status)
        val plantTemperature: TextView = view.findViewById(R.id.plant_temperature)
        val plantHumidity: TextView = view.findViewById(R.id.plant_humidity)
        val plantLight: TextView = view.findViewById(R.id.plant_light)
        val plantAdditionalInfo: TextView = view.findViewById(R.id.plant_additional_info)
        val plantPests: TextView = view.findViewById(R.id.plant_pests)
        val btnEdit: Button = view.findViewById(R.id.btn_edit)
        val btnDelete: Button = view.findViewById(R.id.btn_delete)
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): PlantViewHolder {
        val view = LayoutInflater.from(parent.context).inflate(R.layout.item_plant, parent, false)
        return PlantViewHolder(view)
    }

    override fun onBindViewHolder(holder: PlantViewHolder, position: Int) {
        val plant = plants[position]
        holder.plantType.text = "Тип рослини: ${plant.plantTypeName}"
        holder.plantLocation.text = "Локація: ${plant.plantLocation ?: "Не вказано"}"
        holder.plantStatus.text = "Стан: ${plant.plantState ?: "Не вказано"}"
        holder.plantTemperature.text = "Температура: ${plant.temp}"
        holder.plantHumidity.text = "Вологість: ${plant.humidity}"
        holder.plantLight.text = "Освітлення: ${plant.light}"
        holder.plantAdditionalInfo.text = "Додаткова інформація: ${plant.additionalInfo ?: "Немає"}"
        holder.plantPests.text = "Шкідники: ${plant.pests.joinToString(", ")}"

        holder.btnEdit.setOnClickListener {
            onEditClick(plant)
        }

        holder.btnDelete.setOnClickListener {
            onDeleteClick(plant)
        }
    }

    override fun getItemCount(): Int = plants.size
}
