package com.nivra.app.ui.home

import android.content.ActivityNotFoundException
import android.widget.Toast
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.DirectionsWalk
import androidx.compose.material.icons.filled.EmojiEvents
import androidx.compose.material.icons.filled.Logout
import androidx.compose.material.icons.filled.Refresh
import androidx.compose.material3.Button
import androidx.compose.material3.Card
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.LinearProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedButton
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.material3.TopAppBar
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.lifecycle.viewmodel.compose.viewModel
import com.nivra.app.ServiceLocator
import com.nivra.app.data.HealthConnectAvailability
import java.text.NumberFormat

private const val DAILY_STEP_GOAL = 10_000

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun HomeScreen(
    onNavigateToLeaderboard: () -> Unit,
    onLoggedOut: () -> Unit,
    viewModel: HomeViewModel = viewModel(),
) {
    val state = viewModel.uiState
    val healthManager = ServiceLocator.healthManager
    val context = LocalContext.current

    val permissionLauncher = rememberLauncherForActivityResult(
        contract = healthManager.permissionContract,
    ) { viewModel.refresh() }

    val settingsLauncher = rememberLauncherForActivityResult(
        contract = ActivityResultContracts.StartActivityForResult(),
    ) { viewModel.refresh() }

    fun openHealthConnectSettings() {
        try {
            settingsLauncher.launch(healthManager.healthConnectSettingsIntent())
        } catch (_: ActivityNotFoundException) {
            Toast.makeText(context, "Health Connect is not installed", Toast.LENGTH_LONG).show()
        }
    }

    // Ask once per screen entry; after a denial the banner offers a retry.
    LaunchedEffect(state.availability, state.hasPermission) {
        if (state.availability == HealthConnectAvailability.Available && !state.hasPermission) {
            permissionLauncher.launch(healthManager.requiredPermissions)
        }
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Nivra") },
                actions = {
                    IconButton(onClick = viewModel::refresh) {
                        Icon(Icons.Default.Refresh, contentDescription = "Refresh")
                    }
                    IconButton(onClick = onNavigateToLeaderboard) {
                        Icon(
                            Icons.Default.EmojiEvents,
                            contentDescription = "Leaderboard",
                        )
                    }
                    IconButton(onClick = { viewModel.logout(onLoggedOut) }) {
                        Icon(Icons.Default.Logout, contentDescription = "Log out")
                    }
                },
            )
        },
    ) { padding ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
                .verticalScroll(rememberScrollState())
                .padding(horizontal = 20.dp, vertical = 16.dp),
        ) {
            Text(
                text = "Sveiki ${state.username}",
                style = MaterialTheme.typography.titleMedium,
            )

            Spacer(Modifier.height(16.dp))

            when (state.availability) {
                HealthConnectAvailability.NotSupported ->
                    NoticeCard(
                        text = "This device has no Health Connect, so steps cannot be read " +
                            "automatically. Use the web app to log steps manually.",
                    )

                HealthConnectAvailability.UpdateRequired ->
                    NoticeCard(
                        text = "Health Connect needs to be updated to read your steps.",
                        actionLabel = "Open Health Connect",
                        onAction = ::openHealthConnectSettings,
                    )

                HealthConnectAvailability.Available ->
                    if (!state.hasPermission) {
                        NoticeCard(
                            text = "Nivra needs permission to read your step count.",
                            actionLabel = "Grant access",
                            onAction = {
                                permissionLauncher.launch(healthManager.requiredPermissions)
                            },
                        )
                    }
            }

            if (state.availability == HealthConnectAvailability.Available && state.hasPermission) {
                Spacer(Modifier.height(20.dp))
                StepsCard(
                    healthConnectSteps = state.healthConnectSteps,
                    serverSteps = state.serverSteps,
                    isLoading = state.isLoading,
                )

                Spacer(Modifier.height(20.dp))

                Button(
                    onClick = viewModel::sync,
                    enabled = !state.isSyncing && state.healthConnectSteps != null,
                    modifier = Modifier.fillMaxWidth(),
                ) {
                    Text(if (state.isSyncing) "Syncing…" else "Sync steps to Nivra")
                }
            }

            state.info?.let {
                Spacer(Modifier.height(12.dp))
                Text(it, style = MaterialTheme.typography.bodyMedium)
            }

            state.error?.let {
                Spacer(Modifier.height(12.dp))
                Text(
                    text = it,
                    color = MaterialTheme.colorScheme.error,
                    style = MaterialTheme.typography.bodyMedium,
                )
            }

            state.tip?.let { tip ->
                Spacer(Modifier.height(28.dp))
                Card(modifier = Modifier.fillMaxWidth()) {
                    Column(Modifier.padding(16.dp)) {
                        Text(
                            text = tip.category,
                            style = MaterialTheme.typography.labelMedium,
                            color = MaterialTheme.colorScheme.primary,
                        )
                        Spacer(Modifier.height(8.dp))
                        Text(text = tip.content, style = MaterialTheme.typography.bodyMedium)
                        tip.author?.let {
                            Spacer(Modifier.height(8.dp))
                            Text(
                                text = "— $it",
                                style = MaterialTheme.typography.labelSmall,
                            )
                        }
                    }
                }
            }

            Spacer(Modifier.height(24.dp))
        }
    }
}

@Composable
private fun StepsCard(
    healthConnectSteps: Int?,
    serverSteps: Int,
    isLoading: Boolean,
) {
    val steps = healthConnectSteps ?: 0
    val progress = (steps.toFloat() / DAILY_STEP_GOAL).coerceIn(0f, 1f)
    val formatter = NumberFormat.getIntegerInstance()

    Card(modifier = Modifier.fillMaxWidth()) {
        Column(
            modifier = Modifier
                .fillMaxWidth()
                .padding(20.dp),
            horizontalAlignment = Alignment.CenterHorizontally,
        ) {
            Box(contentAlignment = Alignment.Center) {
                CircularProgressIndicator(
                    progress = { progress },
                    modifier = Modifier.size(168.dp),
                    strokeWidth = 14.dp,
                )
                Column(horizontalAlignment = Alignment.CenterHorizontally) {
                    Icon(
                        Icons.Default.DirectionsWalk,
                        contentDescription = null,
                        tint = MaterialTheme.colorScheme.primary,
                    )
                    Text(
                        text = if (isLoading) "—" else formatter.format(steps),
                        style = MaterialTheme.typography.displaySmall,
                        fontWeight = FontWeight.Bold,
                    )
                    Text(
                        text = "of ${formatter.format(DAILY_STEP_GOAL)} steps",
                        style = MaterialTheme.typography.bodySmall,
                    )
                }
            }

            Spacer(Modifier.height(20.dp))

            LinearProgressIndicator(
                progress = {
                    (serverSteps.toFloat() / DAILY_STEP_GOAL).coerceIn(0f, 1f)
                },
                modifier = Modifier.fillMaxWidth(),
            )
            Spacer(Modifier.height(8.dp))

            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
            ) {
                Text(
                    text = "Health Connect: ${formatter.format(steps)}",
                    style = MaterialTheme.typography.bodySmall,
                )
                Text(
                    text = "Nivra: ${formatter.format(serverSteps)}",
                    style = MaterialTheme.typography.bodySmall,
                    textAlign = TextAlign.End,
                )
            }
        }
    }
}

@Composable
private fun NoticeCard(
    text: String,
    actionLabel: String? = null,
    onAction: (() -> Unit)? = null,
) {
    Card(modifier = Modifier.fillMaxWidth()) {
        Column(
            modifier = Modifier.padding(16.dp),
            horizontalAlignment = Alignment.CenterHorizontally,
        ) {
            Text(
                text = text,
                style = MaterialTheme.typography.bodyMedium,
                textAlign = TextAlign.Center,
            )
            if (actionLabel != null && onAction != null) {
                Spacer(Modifier.height(12.dp))
                OutlinedButton(onClick = onAction) { Text(actionLabel) }
            }
        }
    }
}