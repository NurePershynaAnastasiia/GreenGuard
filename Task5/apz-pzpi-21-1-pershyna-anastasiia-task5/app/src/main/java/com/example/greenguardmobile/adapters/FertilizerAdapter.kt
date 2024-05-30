package com.example.greenguardmobile.adapters

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.greenguardmobile.R
import com.example.greenguardmobile.models.fertilizer.Fertilizer

class FertilizerAdapter(
    private val fertilizers: MutableList<Fertilizer>,
    private val onDeleteClick: (Fertilizer) -> Unit,
    private val onUpdateQuantityClick: (Fertilizer) -> Unit
) : RecyclerView.Adapter<FertilizerAdapter.FertilizerViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): FertilizerViewHolder {
        val view = LayoutInflater.from(parent.context).inflate(R.layout.item_fertilizer, parent, false)
        return FertilizerViewHolder(view)
    }

    override fun onBindViewHolder(holder: FertilizerViewHolder, position: Int) {
        val fertilizer = fertilizers[position]
        holder.bind(fertilizer, onDeleteClick, onUpdateQuantityClick)
    }

    override fun getItemCount() = fertilizers.size

    fun setFertilizers(newFertilizers: List<Fertilizer>) {
        fertilizers.clear()
        fertilizers.addAll(newFertilizers)
        notifyDataSetChanged()
    }

    class FertilizerViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        private val nameTextView: TextView = itemView.findViewById(R.id.fertilizerName)
        private val quantityTextView: TextView = itemView.findViewById(R.id.fertilizerQuantity)
        private val deleteButton: Button = itemView.findViewById(R.id.deleteButton)
        private val updateQuantityButton: Button = itemView.findViewById(R.id.updateQuantityButton)

        fun bind(
            fertilizer: Fertilizer,
            onDeleteClick: (Fertilizer) -> Unit,
            onUpdateQuantityClick: (Fertilizer) -> Unit
        ) {
            nameTextView.text = fertilizer.fertilizerName
            quantityTextView.text = fertilizer.fertilizerQuantity.toString()

            deleteButton.setOnClickListener {
                onDeleteClick(fertilizer)
            }

            updateQuantityButton.setOnClickListener {
                onUpdateQuantityClick(fertilizer)
            }
        }
    }
}

