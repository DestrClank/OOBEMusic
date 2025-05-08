using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

public class AudioDeviceWatcher : IMMNotificationClient
{
    private readonly MMDeviceEnumerator deviceEnumerator;

    public event Action DefaultAudioDeviceChanged;

    public AudioDeviceWatcher()
    {
        deviceEnumerator = new MMDeviceEnumerator();
        deviceEnumerator.RegisterEndpointNotificationCallback(this);
    }

    public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
    {
        if (flow == DataFlow.Render && role == Role.Multimedia)
        {
            DefaultAudioDeviceChanged?.Invoke();
        }
    }

    public void OnDeviceAdded(string pwstrDeviceId) { }
    public void OnDeviceRemoved(string deviceId) { }
    public void OnDeviceStateChanged(string deviceId, DeviceState newState) { }
    public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key) { }

    public void Dispose()
    {
        deviceEnumerator.UnregisterEndpointNotificationCallback(this);
    }
}
