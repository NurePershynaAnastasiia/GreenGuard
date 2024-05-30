package com.example.greenguardmobile.presentation.activities

import android.content.Intent
import android.os.Bundle
import android.widget.Button
import android.widget.EditText
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.greenguardmobile.R
import com.example.greenguardmobile.network.NetworkModule
import com.example.greenguardmobile.service.LoginService

class LoginActivity : AppCompatActivity() {

    private lateinit var emailEditText: EditText
    private lateinit var passwordEditText: EditText
    private lateinit var loginButton: Button
    private lateinit var loginService: LoginService

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_login)

        emailEditText = findViewById(R.id.emailEditText)
        passwordEditText = findViewById(R.id.passwordEditText)
        loginButton = findViewById(R.id.loginButton)

        val apiService = NetworkModule.provideApiService(this)
        loginService = LoginService(apiService, this)

        loginButton.setOnClickListener {
            val email = emailEditText.text.toString()
            val password = passwordEditText.text.toString()
            if (email.isNotBlank() && password.isNotBlank()) {
                loginService.login(email, password, { token ->
                    navigateToMainScreen()
                }, { errorMsg ->
                    Toast.makeText(this, errorMsg, Toast.LENGTH_SHORT).show()
                })
            } else {
                Toast.makeText(this, "Please enter email and password", Toast.LENGTH_SHORT).show()
            }
        }
    }

    private fun navigateToMainScreen() {
        val intent = Intent(this, ProfileActivity::class.java)
        startActivity(intent)
        finish()
    }
}
