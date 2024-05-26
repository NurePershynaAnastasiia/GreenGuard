import android.content.Context
import android.util.Log
import com.example.greenguardmobile.api.ApiService
import com.example.greenguardmobile.api.TokenManager
import okhttp3.OkHttpClient
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

object NetworkModule {

    fun provideApiService(context: Context): ApiService {
        /*
        val tokenManager = TokenManager(context)

        val client = OkHttpClient.Builder()
            .addInterceptor { chain ->
                val original = chain.request()
                val requestBuilder = original.newBuilder()
                    .header("Authorization", "Bearer ${tokenManager.getJwtToken()}")
                val request = requestBuilder.build()
                chain.proceed(request)
            }
            .build()*/

        /*
        val retrofit = Retrofit.Builder()
            .baseUrl("https://localhost:7042/")
            .client(client)
            .addConverterFactory(GsonConverterFactory.create())
            .build()

         */

        val retrofit = Retrofit.Builder()
            .baseUrl("http://10.0.2.2:5159/")
            .addConverterFactory(GsonConverterFactory.create())
            .build()


        return retrofit.create(ApiService::class.java)

    }
}