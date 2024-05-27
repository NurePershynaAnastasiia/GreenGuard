package com.example.greenguardmobile.adapter

import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.greenguardmobile.R
import com.example.greenguardmobile.model.Fertilizer
import com.example.greenguardmobile.model.Pest

class PestAdapter(private val pests: MutableList<Pest>) :
    RecyclerView.Adapter<PestAdapter.PestViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int) : PestViewHolder {
        val view = LayoutInflater.from(parent.context).inflate(R.layout.item_pest, parent, false)
        return PestViewHolder(view)
    }

    override fun onBindViewHolder(holder: PestViewHolder, position: Int) {
        val pest = pests[position]
        holder.nameTextView.text = pest.pestName.toString()
        holder.descriptionTextView.text = pest.pestDescription.toString()

        //Log.d("NetworkModule", fertilizer.fertilizerName.toString())
    }

    override fun getItemCount(): Int {
        return pests.size
    }

    fun setPests(pests: List<Pest>) {
        this.pests.clear()
        this.pests.addAll(pests)
        notifyDataSetChanged()
    }

    inner class PestViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        val nameTextView: TextView = itemView.findViewById(R.id.pestName)
        val descriptionTextView: TextView = itemView.findViewById(R.id.pestDescription)
    }
}
