package com.example.fingerprintauth;

import android.content.Context;
import android.hardware.fingerprint.FingerprintManager;
import android.os.CancellationSignal;

public class FingerprintAuthentication {
    private Context context;
    private CancellationSignal cancellationSignal;

    public FingerprintAuthentication(Context context) {
        this.context = context;
    }

    public boolean isFingerprintAvailable() {
        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.M) {
            FingerprintManager fingerprintManager = 
                    (FingerprintManager) context.getSystemService(Context.FINGERPRINT_SERVICE);
            return fingerprintManager != null && fingerprintManager.isHardwareDetected();
        }
        return false;
    }

    public boolean authenticateWithFingerprint() {
        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.M) {
            try {
                FingerprintManager fingerprintManager = 
                        (FingerprintManager) context.getSystemService(Context.FINGERPRINT_SERVICE);
                cancellationSignal = new CancellationSignal();

                final boolean[] isSuccess = {false};

                fingerprintManager.authenticate(null, cancellationSignal, 0,
                        new FingerprintManager.AuthenticationCallback() {
                            @Override
                            public void onAuthenticationSucceeded(FingerprintManager.AuthenticationResult result) {
                                isSuccess[0] = true; // Uwierzytelnienie zakończone sukcesem
                            }

                            @Override
                            public void onAuthenticationFailed() {
                                isSuccess[0] = false; // Uwierzytelnienie nieudane
                            }
                        }, null);

                // Zwracamy wynik uwierzytelnienia
                return isSuccess[0];
            } catch (Exception e) {
                e.printStackTrace();
                return false;
            }
        }
        return false;
    }
}
