package com.nivra.app.ui

import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import androidx.navigation.NavHostController
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.nivra.app.ServiceLocator
import com.nivra.app.ui.auth.LoginScreen
import com.nivra.app.ui.auth.SignUpScreen
import com.nivra.app.ui.home.HomeScreen
import com.nivra.app.ui.leaderboard.LeaderboardScreen

object Routes {
    const val LOGIN = "login"
    const val SIGN_UP = "signup"
    const val HOME = "home"
    const val LEADERBOARD = "leaderboard"
}

@Composable
fun NivraApp(navController: NavHostController = rememberNavController()) {
    val session by ServiceLocator.sessionStore.session.collectAsStateWithLifecycle()
    val startDestination = if (session != null) Routes.HOME else Routes.LOGIN

    NavHost(navController = navController, startDestination = startDestination) {

        composable(Routes.LOGIN) {
            LoginScreen(
                onLoggedIn = {
                    navController.navigate(Routes.HOME) {
                        popUpTo(Routes.LOGIN) { inclusive = true }
                    }
                },
                onNavigateToSignUp = { navController.navigate(Routes.SIGN_UP) },
            )
        }

        composable(Routes.SIGN_UP) {
            SignUpScreen(onNavigateToLogin = { navController.popBackStack() })
        }

        composable(Routes.HOME) {
            HomeScreen(
                onNavigateToLeaderboard = { navController.navigate(Routes.LEADERBOARD) },
                onLoggedOut = {
                    navController.navigate(Routes.LOGIN) {
                        popUpTo(0) { inclusive = true }
                    }
                },
            )
        }

        composable(Routes.LEADERBOARD) {
            LeaderboardScreen(onNavigateBack = { navController.popBackStack() })
        }
    }
}