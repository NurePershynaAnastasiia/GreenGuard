package com.example.greenguardmobile.activities

import android.content.Intent
import android.os.Bundle
import android.util.Log
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

        savedInstanceState?.let {
            restoreSavedInstanceState(it)
        }
    }

    override fun onStart() {
        super.onStart()
        initializeViews()
        initializeServices()

        loginButton.setOnClickListener {
            handleLoginButtonClick()
        }
    }

    override fun onSaveInstanceState(outState: Bundle) {
        super.onSaveInstanceState(outState)
        saveInstanceState(outState)

        val preferences = getPreferences(MODE_PRIVATE).edit()
        preferences.putString("email", emailEditText.text.toString())
        preferences.putString("password", passwordEditText.text.toString())
        preferences.apply()
    }

    override fun onRestoreInstanceState(outState: Bundle) {
        super.onRestoreInstanceState(outState)
        saveInstanceState(outState)

        emailEditText.setText(getPreferences(MODE_PRIVATE).getString("email", ""))
        passwordEditText.setText(getPreferences(MODE_PRIVATE).getString("password", ""))
    }

    private fun initializeViews() {
        emailEditText = findViewById(R.id.emailEditText)
        passwordEditText = findViewById(R.id.passwordEditText)
        loginButton = findViewById(R.id.loginButton)
    }

    private fun initializeServices() {
        val apiService = NetworkModule.provideApiService(this)
        loginService = LoginService(apiService, this)
    }

    private fun handleLoginButtonClick() {
        val email = emailEditText.text.toString()
        val password = passwordEditText.text.toString()
        if (email.isNotBlank() && password.isNotBlank()) {
            loginService.login(email, password, { token ->
                navigateToMainScreen()
            }, { errorMsg ->
                Toast.makeText(this, getResources().getString(R.string.login_error), Toast.LENGTH_SHORT).show()
            })
        } else {
            Toast.makeText(this, getResources().getString(R.string.login_toast), Toast.LENGTH_SHORT).show()
        }
    }

    private fun saveInstanceState(outState: Bundle) {
        outState.putString("email", emailEditText.text.toString())
        outState.putString("password", passwordEditText.text.toString())
    }

    private fun restoreSavedInstanceState(savedInstanceState: Bundle) {
        emailEditText.setText(savedInstanceState.getString("email"))
        passwordEditText.setText(savedInstanceState.getString("password"))
    }

    private fun navigateToMainScreen() {
        val intent = Intent(this, ProfileActivity::class.java)
        startActivity(intent)
        finish()
    }
}
