package com.nivra.app.ui.theme

import androidx.compose.foundation.isSystemInDarkTheme
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.darkColorScheme
import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable
import androidx.compose.ui.graphics.Color

private val NivraTeal = Color(0xFF0F766E)
private val NivraTealLight = Color(0xFF2DD4BF)
private val NivraSand = Color(0xFFF5F5F0)

private val LightColors = lightColorScheme(
    primary = NivraTeal,
    onPrimary = Color.White,
    secondary = NivraTeal,
    background = NivraSand,
    surface = Color.White,
)

private val DarkColors = darkColorScheme(
    primary = NivraTealLight,
    onPrimary = Color(0xFF00201C),
    secondary = NivraTealLight,
)

@Composable
fun NivraTheme(
    darkTheme: Boolean = isSystemInDarkTheme(),
    content: @Composable () -> Unit,
) {
    MaterialTheme(
        colorScheme = if (darkTheme) DarkColors else LightColors,
        content = content,
    )
}