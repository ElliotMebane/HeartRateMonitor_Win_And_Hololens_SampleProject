using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using WSAUnityHRMStub;

public class WindowsBareBonesController : MonoBehaviour
{
    private HRMPlugin _hrmPlugin;
    private HeartRateServiceDevice _selectedHRSDevice;
    public Queue<Action> callbackQueue = new Queue<Action>();
     
    private Text _hrmStatusLabel; 
    private Button _connectBtn;
    private Button _disconnectBtn;
    private Text _BPMDisplay;
     
    private string _hrmStatusMsgConnect = "Click Connect";
    private string _hrmStatusMsgConnecting = "Connecting...";
    private string _disconnectedMessage;

    void Start()
    { 
        GameObject tCanvasGO = GameObject.Find("Canvas");
       
        _hrmStatusLabel = tCanvasGO.transform.Find("HRMStatus").GetComponent<Text>();
          
        _connectBtn = tCanvasGO.transform.Find("ConnectButton").GetComponent<Button>();
        _connectBtn.onClick.AddListener(onConnectClicked); 

        _disconnectBtn = tCanvasGO.transform.Find( "DisconnectButton" ).GetComponent<Button>();
        _disconnectBtn.onClick.AddListener( onDisconnectClicked ); 

        _BPMDisplay = tCanvasGO.transform.Find("BPMDisplay").GetComponent<Text>();
         
        _disconnectedMessage = _hrmStatusMsgConnect;

        _hrmPlugin = new HRMPlugin();
    }

    private void onConnectClicked()
    {
        _hrmPlugin.disconnectService();

        callbackQueue.Clear();

        _disconnectedMessage = _hrmStatusMsgConnecting; 

        _hrmPlugin.scan(callbackQueue, onScanComplete);
    }

    private void onScanComplete()
    {
        // For this Bare Bones example we simply pick the first HRM device in the list of found devices.
        _selectedHRSDevice = _hrmPlugin.hrsDevices[ 0 ];

        _hrmPlugin.initializeService( _selectedHRSDevice, callbackQueue, processHRValues ); 
    }
      
    private void onDisconnectClicked()
    { 
        _hrmPlugin.disconnectService();

        _BPMDisplay.text = "0";
    }

    private void processHRValues()
    { 
        ushort tHRM = _hrmPlugin.getLastHRM(true);
        if (tHRM != 0 )
        {
            _BPMDisplay.text = tHRM.ToString();            
        }       
    }

    void Update()
    {
        if( _hrmPlugin.isServiceInitialized )
        {
            // reset disconnected message
            _disconnectedMessage = _hrmStatusMsgConnect;

            _hrmStatusLabel.text = String.Format( "Connected to {0}", _selectedHRSDevice.bluetoothLEDeviceDisplayName );
        }
        else
        {
            _hrmStatusLabel.text = _disconnectedMessage;
        }
         
        if ( callbackQueue.Count > 0 )
        {
            Action tAction = callbackQueue.Dequeue();
            tAction.Invoke(); 
        } 
    }
}