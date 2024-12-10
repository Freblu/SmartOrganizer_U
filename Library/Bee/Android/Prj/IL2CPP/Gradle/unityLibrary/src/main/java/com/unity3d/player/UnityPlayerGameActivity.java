package com.unity3d.player;

import android.annotation.TargetApi;
import android.content.Context;
import android.content.Intent;
import android.content.res.Configuration;
import android.os.Build;
import android.os.Bundle;
import android.view.SurfaceView;
import android.view.ViewTreeObserver;
import android.view.inputmethod.InputMethodManager;
import android.widget.FrameLayout;
import android.view.Window;

import androidx.core.view.ViewCompat;

import com.google.androidgamesdk.GameActivity;


public class UnityPlayerGameActivity extends GameActivity implements IUnityPlayerLifecycleEvents, IUnityPermissionRequestSupport, IUnityPlayerSupport
{
    protected UnityPlayerForGameActivity mUnityPlayer;
    protected String updateUnityCommandLineArguments(String cmdLine)
    {
        return cmdLine;
    }

    static
    {
        System.loadLibrary("game");
    }

    @Override
    protected void onCreate(Bundle savedInstanceState){
        super.onCreate(savedInstanceState);
        // On devices with API Level >= 30 system bars are no longer accounted for and because of that window/views don't resize (see https://jira.unity3d.com/browse/UUM-18618)
        // This is most likely due to deprecation of setSystemUiVisibility and changes to insets used in SystemUI.cpp
        // This fix forces views to shrink to account for system bars
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) {
            getWindow().setDecorFitsSystemWindows(true);
        }
    }

    @Override
    public UnityPlayerForGameActivity getUnityPlayerConnection() {
        return mUnityPlayer;
    }

    // Soft keyboard relies on inset listener for listening to various events - keyboard opened/closed/text entered.
    private void applyInsetListener(SurfaceView surfaceView)
    {
        surfaceView.getViewTreeObserver().addOnGlobalLayoutListener(
                () -> onApplyWindowInsets(surfaceView, ViewCompat.getRootWindowInsets(getWindow().getDecorView())));
    }

    @Override protected void onCreateSurfaceView() {
        super.onCreateSurfaceView();
        FrameLayout frameLayout = findViewById(contentViewId);

        applyInsetListener(mSurfaceView);

        mSurfaceView.setId(UnityPlayerForGameActivity.getUnityViewIdentifier(this));

        String cmdLine = updateUnityCommandLineArguments(getIntent().getStringExtra("unity"));
        getIntent().putExtra("unity", cmdLine);
        // Unity requires access to frame layout for setting the static splash screen.
        // Note: we cannot initialize in onCreate (after super.onCreate), because game activity native thread would be already started and unity runtime initialized
        //       we also cannot initialize before super.onCreate since frameLayout is not yet available.
        mUnityPlayer = new UnityPlayerForGameActivity(this, frameLayout, mSurfaceView, this);
    }

    @Override
    public void onUnityPlayerUnloaded() {

    }

    @Override
    public void onUnityPlayerQuitted() {

    }

    // Quit Unity
    @Override protected void onDestroy ()
    {
        mUnityPlayer.destroy();
        super.onDestroy();
    }

    @Override protected void onStop()
    {
        // Note: we want Java onStop callbacks to be processed before the native part processes the onStop callback
        mUnityPlayer.onStop();
        super.onStop();
    }

    @Override protected void onStart()
    {
        // Note: we want Java onStart callbacks to be processed before the native part processes the onStart callback
        mUnityPlayer.onStart();
        super.onStart();
    }

    // Pause Unity
    @Override protected void onPause()
    {
        // Note: we want Java onPause callbacks to be processed before the native part processes the onPause callback
        mUnityPlayer.onPause();
        super.onPause();
    }

    // Resume Unity
    @Override protected void onResume()
    {
        // Note: we want Java onResume callbacks to be processed before the native part processes the onResume callback
        mUnityPlayer.onResume();
        super.onResume();
    }

    // Configuration changes are used by Video playback logic in Unity
    @Override public void onConfigurationChanged(Configuration newConfig)
    {
        mUnityPlayer.configurationChanged(newConfig);
        super.onConfigurationChanged(newConfig);
    }

    // Notify Unity of the focus change.
    @Override public void onWindowFocusChanged(boolean hasFocus)
    {
        mUnityPlayer.windowFocusChanged(hasFocus);
        super.onWindowFocusChanged(hasFocus);
    }

    @Override protected void onNewIntent(Intent intent)
    {
        super.onNewIntent(intent);
        // To support deep linking, we need to make sure that the client can get access to
        // the last sent intent. The clients access this through a JNI api that allows them
        // to get the intent set on launch. To update that after launch we have to manually
        // replace the intent with the one caught here.
        setIntent(intent);
        mUnityPlayer.newIntent(intent);
    }

    @Override
    @TargetApi(Build.VERSION_CODES.M)
    public void requestPermissions(PermissionRequest request)
    {
        mUnityPlayer.addPermissionRequest(request);
    }

    @Override public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults)
    {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        mUnityPlayer.permissionResponse(this, requestCode, permissions, grantResults);
    }
}
