package com.trialation.iotremoteapp

import android.content.Context
import android.content.SharedPreferences
import android.os.Bundle
import android.os.StrictMode
import android.text.Editable
import android.text.TextWatcher
import android.widget.EditText
import android.widget.ImageButton
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import java.net.URL


class MainActivity : AppCompatActivity() {

    lateinit var txtID: EditText
    lateinit var btnShutdown: ImageButton
    lateinit var sharedPreferences: SharedPreferences

    var id: String? = ""

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        val policy =
            StrictMode.ThreadPolicy.Builder().permitAll().build()
        StrictMode.setThreadPolicy(policy)

        sharedPreferences = this.getSharedPreferences("token", Context.MODE_PRIVATE)
        id = sharedPreferences.getString("id", "")

        txtID = findViewById(R.id.txtID)
        btnShutdown = findViewById(R.id.btnShutdown)

        if(!id.equals("")){
            txtID.setText(id)
        }

        txtID.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(s: Editable?) {
                id = txtID.text.toString()
                val editor: SharedPreferences.Editor = sharedPreferences.edit()
                editor.putString("id", id)
                editor.apply()
            }

            override fun beforeTextChanged(s: CharSequence?, start: Int, count: Int, after: Int) {

            }

            override fun onTextChanged(s: CharSequence?, start: Int, before: Int, count: Int) {

            }

        })

        btnShutdown.setOnClickListener {
            sendRequest()
            Toast.makeText(this, "Your request has been sent to shutdown target!", Toast.LENGTH_LONG).show()
        }
    }

    fun sendRequest(){
        val url = URL("https://us-central1-iotremoteshutdown.cloudfunctions.net/api/shutdown?id=$id").readText()
        println(url)
    }
}
