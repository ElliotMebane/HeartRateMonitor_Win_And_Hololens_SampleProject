using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using WSAUnityHRMStub;

// heart image source: https://commons.wikimedia.org/wiki/File:Heart_icon_red_hollow.svg

public class HRMController : MonoBehaviour
{
    private const float HEART_SCALE_MIN = 0.25f;
    private const float HEART_SCALE_MAX = 0.26f;
    private const float HEART_SCALE_RANGE = HEART_SCALE_MAX - HEART_SCALE_MIN;
    
    public Queue<Action> callbackQueue = new Queue<Action>();
    
    private HRMPlugin _hrmPlugin;
    private HeartRateServiceDevice _selectedDevice;

    private int _lastHRValue;
    private float _beatStartTime;
    private float _beatCompleteTime;
    private float _rrDuration; // time in seconds for one heart beat

    // optional polling instead of Queue/Action technique for connecting to HRM device
    private int _scanPoll = 0;
   
    private GameObject _heartGO;
    private Text _hrmStatusLabel;
    private Button _scanBtn;
    private GameObject _hrmDropDownGO;
    private Dropdown _hrmDropdown;
    private Button _connectBtn;
    private Button _disconnectBtn;
    private Text _BPMDisplay;

    private string _hrmStatusMsgStart = "Scan for Paired HRM Devices";
    private string _hrmStatusMsgScanning = "Scanning...";
    private string _hrmStatusMsgScanCompleteSuccess = "Select an HRM Device";
    private string _hrmStatusMsgScanNoDevices = "Pair your HRM in Settings then Try Again";
    private string _hrmStatusMsgDeviceSelected = "Click Connect";
    private string _hrmStatusMsgConnecting = "Connecting...";
    private string _hrmStatusMsgConnected = "Connected";
    private string _dropdownBlankEntryLabel = "Devices Loaded";
    private string _dropdownCaptionStart = "HRM Device List";
    private string _dropdownCaptionSelect = "Select an HRM Device";

    void Start()
    {
        Application.targetFrameRate = 60;

        resetValues();

        _heartGO = GameObject.Find("Heart").gameObject;

        GameObject tCanvasGO = GameObject.Find("Canvas");
       
        _hrmStatusLabel = tCanvasGO.transform.Find("HRMStatus").GetComponent<Text>();
        _hrmStatusLabel.text = _hrmStatusMsgStart;

        _scanBtn = tCanvasGO.transform.Find("ScanButton").GetComponent<Button>();
        _scanBtn.onClick.AddListener(onScanClicked);
        _scanBtn.enabled = true;

        _hrmDropDownGO = tCanvasGO.transform.Find("Dropdown").gameObject;
        _hrmDropdown = _hrmDropDownGO.GetComponent<Dropdown>();
        _hrmDropdown.captionText.text = _dropdownCaptionStart;
        _hrmDropdown.onValueChanged.AddListener(onDropDownSelected);
        _hrmDropdown.enabled = false;

        _connectBtn = tCanvasGO.transform.Find("ConnectButton").GetComponent<Button>();
        _connectBtn.onClick.AddListener(onConnectClicked);
        _connectBtn.enabled = false;

        _disconnectBtn = tCanvasGO.transform.Find("DisconnectButton").GetComponent<Button>();
        _disconnectBtn.onClick.AddListener(onDisconnectClicked);
        _disconnectBtn.enabled = false;

        _BPMDisplay = tCanvasGO.transform.Find("BPMDisplay").GetComponent<Text>();

        _hrmPlugin = new HRMPlugin();
        _hrmStatusLabel.text = "HRMPlugin v." + HRMPlugin.VERSION;

        // optional
        // Enable or Disable Advertisement watching for receiving RSSI updates
        // _hrmPlugin.useWatchAdvertisements = false;

        // optional 
        // _hrmPlugin.gattProtectionLevel = HRMPlugin.PluginGattProtectionLevel.EncryptionRequired;
    }

    private void resetAll()
    {
        resetValues();
        
        callbackQueue.Clear();
    }

    private void resetValues()
    {
        _lastHRValue = 0;
        _beatStartTime = 0;
        _beatCompleteTime = 0;
        _rrDuration = 0;
    }

    private void onScanClicked()
    {
        _hrmPlugin.disconnectService();
        resetAll();

        _hrmStatusLabel.text = _hrmStatusMsgScanning;

       // scan with Queue/Action callback technique
        _hrmPlugin.scan(callbackQueue, onScanComplete);
    }

    public void onScanComplete()
    {
        if (_hrmPlugin.hrsDevices.Count == 0)
        {
            _hrmStatusLabel.text = _hrmStatusMsgScanNoDevices;
        }
        else if (_hrmPlugin.hrsDevices.Count > 0)
        {
            _hrmStatusLabel.text = _hrmStatusMsgScanCompleteSuccess;

            addDropDownEntries();
        }
    }

    private void addDropDownEntries()
    {
        _hrmDropdown.ClearOptions();
        List<string> tDeviceNames = new List<string>();
         
        foreach ( HeartRateServiceDevice tDevice in _hrmPlugin.hrsDevices)
        {
            // Old
            //  tDeviceNames.Add( tDevice.deviceId );

            // New
            try
            {
                tDeviceNames.Add( tDevice.bluetoothLEDeviceDisplayName );
            }
            catch ( Exception ex )
            {
                Debug.Log( "OOPS, exception when adding bluetoothLEDeviceDisplayName" );
            }

            Debug.LogFormat( "device bluetoothLEDeviceDisplayName: {0}", tDevice.bluetoothLEDeviceDisplayName );
            Debug.LogFormat( "device name: {0}", tDevice.name );
            Debug.LogFormat( "device id: {0}", tDevice.deviceId );
            Debug.LogFormat( "device isEnabled: {0}", tDevice.isEnabled.ToString() ); 
            Debug.LogFormat( "device properties count: {0}", tDevice.properties.Count );
            foreach ( KeyValuePair<string, string> entry in tDevice.properties )
            {
                Debug.LogFormat( "device property, key: {0}, value: {1}", entry.Key, entry.Value );
            }
        }

        _hrmDropdown.AddOptions(tDeviceNames);

        _hrmDropdown.value = 0;
         
        _hrmDropdown.enabled = true;

        _connectBtn.enabled = true; 
    }

    private void onDropDownSelected(int pSelectedIndex)
    {
        _connectBtn.enabled = true;

        _hrmStatusLabel.text = _hrmStatusMsgDeviceSelected; 
    }

    private void onConnectClicked()
    {
        int tSelectedIndex;
         
        tSelectedIndex = _hrmDropDownGO.GetComponent<Dropdown>().value;

        _selectedDevice = _hrmPlugin.hrsDevices[tSelectedIndex];
       
        resetValues();

        _hrmStatusLabel.text = _hrmStatusMsgConnecting;
        _BPMDisplay.text = "0";

        _connectBtn.enabled = false;
        _disconnectBtn.enabled = true;

        // connect with Queue/Action callback technique 
        _hrmPlugin.initializeService(_selectedDevice, callbackQueue, processHRValues );
    }

    private void onDisconnectClicked()
    {
        resetAll();
        
        _connectBtn.enabled = true;
        _disconnectBtn.enabled = false;

        _hrmPlugin.disconnectService();        
    }

    private void processHRValues()
    {
        if (_hrmPlugin.hrms.Count <= 0)
            return;

        // returns ushort if there's a new entry, otherwise returns 0
        // pass true (default) to reset status of *new* available data in plugin
        ushort tHRM = _hrmPlugin.getLastHRM(true);
        if (tHRM != 0 )
        {
            _lastHRValue = tHRM;
            
            // _hrmStatusLabel.text = _hrmStatusMsgConnected;
            _BPMDisplay.text = _lastHRValue.ToString();            
        }       
    }

    void Update()
    {
        if ( _hrmPlugin.receivedMeasurementDataLimited != null && _hrmPlugin.receivedMeasurementDataLimited.Count >= 1 )
        {
            byte[] tBytes = _hrmPlugin.receivedMeasurementDataLimited[ _hrmPlugin.receivedMeasurementDataLimited.Count - 1 ];

            String tByteString = "";
            foreach( byte tByte in tBytes )
            {
                tByteString += tByte.ToString() + " ";
            }

            _hrmStatusLabel.text = String.Format( "{0}", tByteString );
        }

        if ( callbackQueue.Count > 0 )
        {
            Action tAction = callbackQueue.Dequeue();
            tAction.Invoke(); 
        }

        // On some interval we could check the latest RSSI values (signal strength) for known devices. 
        // Here's a quick and dirty test for debugging only. Write your own if 
        // you want to use RSSI. 
        // Useful for sorting by distance
        if ( Time.time % 1 <= 0.1f && !_hrmPlugin.isServiceInitialized )
        {
            foreach ( HeartRateServiceDevice tHRSDevice in _hrmPlugin.hrsDevices )
            {
                Debug.LogFormat( "Name: {0}, BLEAddress: {1}, RSSI: {2}", tHRSDevice.bluetoothLEDeviceDisplayName, tHRSDevice.bluetoothAddress, tHRSDevice.properties[ "RSSI" ] );
                //Debug.LogFormat( "Name: {0} ", tHRSDevice.bluetoothLEDeviceDisplayName );
                //Debug.LogFormat( "BLEAddress: {0}", tHRSDevice.bluetoothAddress );
                //Debug.LogFormat( "RSSI: {0}", tHRSDevice.properties[ "RSSI" ] );
            }
        }

        // animate heart
        if ( _lastHRValue != 0)
        {
            float tBeatPhase = 0;
            if (Time.time >= _beatCompleteTime)
            {
                _rrDuration = 60f / _lastHRValue;
                _beatStartTime = Time.time;
                _beatCompleteTime = Time.time + _rrDuration;
                tBeatPhase = 0;
            }
            else
            {
                tBeatPhase = (Time.time - _beatStartTime) / _rrDuration;
            }

            float tBeatPhaseSmoothed = Mathf.SmoothStep(0, 1, tBeatPhase);
            float tScale = HEART_SCALE_MAX - (HEART_SCALE_RANGE * tBeatPhaseSmoothed);

            Mathf.Clamp(tScale, HEART_SCALE_MIN, HEART_SCALE_MAX);
            _heartGO.transform.localScale = new Vector3(tScale, tScale, tScale);
        }
    }
}