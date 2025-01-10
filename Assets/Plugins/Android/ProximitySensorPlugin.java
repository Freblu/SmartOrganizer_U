package com.example.proximitysensor;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;

public class ProximitySensorPlugin {

    private SensorManager sensorManager;
    private Sensor proximitySensor;
    private float proximityValue = -1;

    public ProximitySensorPlugin(Context context) {
        sensorManager = (SensorManager) context.getSystemService(Context.SENSOR_SERVICE);
        if (sensorManager != null) {
            proximitySensor = sensorManager.getDefaultSensor(Sensor.TYPE_PROXIMITY);
        }
    }

    public void start() {
        if (proximitySensor != null) {
            sensorManager.registerListener(proximitySensorListener, proximitySensor, SensorManager.SENSOR_DELAY_NORMAL);
        }
    }

    public void stop() {
        if (proximitySensor != null) {
            sensorManager.unregisterListener(proximitySensorListener);
        }
    }

    public float getProximityValue() {
        return proximityValue;
    }

    private final SensorEventListener proximitySensorListener = new SensorEventListener() {
        @Override
        public void onSensorChanged(SensorEvent event) {
            if (event.sensor.getType() == Sensor.TYPE_PROXIMITY) {
                proximityValue = event.values[0];
            }
        }

        @Override
        public void onAccuracyChanged(Sensor sensor, int accuracy) {
            // Nie jest używane, ale wymagane przez interfejs
        }
    };
}
