package com.example.greenguardmobile.adapters

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.greenguardmobile.R
import com.example.greenguardmobile.models.pest.Pest

class PestAdapter(
    private val pests: MutableList<Pest>,
    private val onAddToPlantClick: (Pest) -> Unit,
    private val onRemoveFromPlantClick: (Pest) -> Unit
) : RecyclerView.Adapter<PestAdapter.PestViewHolder>() {

    inner class PestViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        private val pestName: TextView = itemView.findViewById(R.id.pestName)
        private val pestDescription: TextView = itemView.findViewById(R.id.pestDescription)
        private val addToPlantButton: Button = itemView.findViewById(R.id.addToPlantButton)
        private val removeFromPlantButton: Button = itemView.findViewById(R.id.removeFromPlantButton)

        fun bind(pest: Pest) {
            pestName.text = pest.pestName
            pestDescription.text = pest.pestDescription

            addToPlantButton.setOnClickListener { onAddToPlantClick(pest) }
            removeFromPlantButton.setOnClickListener { onRemoveFromPlantClick(pest) }
        }
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): PestViewHolder {
        val view = LayoutInflater.from(parent.context).inflate(R.layout.item_pest, parent, false)
        return PestViewHolder(view)
    }

    override fun onBindViewHolder(holder: PestViewHolder, position: Int) {
        val pest = pests[position]
        holder.bind(pest)
    }

    override fun getItemCount(): Int {
        return pests.size
    }

    fun setPests(pests: List<Pest>) {
        this.pests.clear()
        this.pests.addAll(pests)
        notifyDataSetChanged()
    }

    fun removePest(position: Int) {
        pests.removeAt(position)
        notifyItemRemoved(position)
    }
}
