package com.example.greenguardmobile.adapter

import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.greenguardmobile.R
import com.example.greenguardmobile.model.Fertilizer

class FertilizerAdapter(private val fertilizers: MutableList<Fertilizer>) :
    RecyclerView.Adapter<FertilizerAdapter.FertilizerViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int) : FertilizerViewHolder {
        val view = LayoutInflater.from(parent.context).inflate(R.layout.item_fertilizer, parent, false)
        return FertilizerViewHolder(view)
    }

    override fun onBindViewHolder(holder: FertilizerViewHolder, position: Int) {
        val fertilizer = fertilizers[position]
        holder.nameTextView.text = fertilizer.FertilizerName.toString()
        holder.quantityTextView.text = fertilizer.FertilizerQuantity.toString()

        Log.d("NetworkModule", fertilizer.FertilizerName.toString())
    }

    override fun getItemCount(): Int {
        return fertilizers.size
    }

    fun setFertilizers(fertilizers: List<Fertilizer>) {
        this.fertilizers.clear()
        this.fertilizers.addAll(fertilizers)
        notifyDataSetChanged()
    }

    inner class FertilizerViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        val nameTextView: TextView = itemView.findViewById(R.id.fertilizerName)
        val quantityTextView: TextView = itemView.findViewById(R.id.fertilizerQuantity)
    }
}
