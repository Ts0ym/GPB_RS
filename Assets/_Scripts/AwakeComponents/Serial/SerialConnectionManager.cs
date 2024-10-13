using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;
using AwakeComponents.DebugUI;
using UnityEngine;
using UnityEngine.Events;

namespace AwakeComponents.Serial
{
    /// <summary>
    /// <c>SerialConnectionManager</c> is a component that implements serial communication with Arduino or other devices.
    /// </summary>
    [ComponentInfo("1.0", "05.04.2024")]
    public class SerialConnectionManager : MonoBehaviour, IDebuggableComponent
    {
        /// <summary>
        /// Event that is triggered when a message is received from the serial port.
        /// </summary>
        public UnityEvent<string> onMessageGot = new();

        public int baudrate = 9600;

        /// <summary>
        /// Stores the serial ports that are currently open.
        /// </summary>
        List<SerialPort> serialPorts = new();
        
        string _testMessage = "Hello, World!";
        
        void Start()
        {
            OpenSavedPorts();
        }

        private void OpenSavedPorts()
        {
            List<string> savedPortsList = new();

            string savedPortsString = PlayerPrefs.GetString("serialPorts");

            if (savedPortsString != "")
                savedPortsList = new List<string>(savedPortsString.Split(','));

            // Iterate through all available serial ports and open the saved ones
            foreach (string port in SerialPort.GetPortNames())
                if (savedPortsList.Contains(port))
                    OpenSerialByName(port);
        }

        void Update()
        {
            // Iterate through all opened serial ports and read messages from them
            foreach (SerialPort serialPort in serialPorts)
                if (serialPort.IsOpen)
                    ReadSerialPort(serialPort);
        }

        private async void ReadSerialPort(SerialPort serialPort)
        {
            try
            {
                string value = null;

                await Task.Run(() =>
                {
                    value = serialPort.ReadLine();
                });

                if (value == null)
                    return;

                OnMessageGot(value.Trim());
            }
            catch (TimeoutException e)
            {
                Debug.LogError("[AwakeSerialCom] Error: " + e.Message);
            }
        }

        void OnMessageGot(string message)
        {
            onMessageGot?.Invoke(message);

            Debug.Log("[AwakeSerialCom] Serial port message received: " + message);
        }

        /// <summary>
        /// Sends a message to all opened serial ports.
        /// </summary>
        /// <param name="message">Message to send.</param>
        public void Send(string message)
        {
            foreach (SerialPort serialPort in serialPorts)
                if (serialPort.IsOpen)
                    serialPort.WriteLine(message);

            Debug.Log("[AwakeSerialCom] Serial port message sent: " + message);
        }
        
        public void RenderDebugUI()
        {
            GUI.backgroundColor = Color.red;

            if (GUILayout.Button("Clear all saved ports"))
            {
                PlayerPrefs.DeleteKey("serialPorts");

                foreach (SerialPort serialPort in serialPorts)
                    StartCoroutine(ClearPortDelayed(serialPort));
            }

            GUI.backgroundColor = Color.white;
            
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            
            _testMessage = GUILayout.TextField(_testMessage);
            
            if (GUILayout.Button("Send test message"))
                Send(_testMessage);
            
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);

            // Show all opened serial ports
            GUILayout.Label("Opened serial ports:");

            try
            {
                foreach (SerialPort serialPort in serialPorts)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Box(serialPort.PortName + " " + (serialPort.IsOpen ? "Opened" : "Closed"));

                    GUI.backgroundColor = Color.red;

                    GUI.skin.button.stretchWidth = false;

                    if (GUILayout.Button("✖"))
                    {
                        serialPort.Close();
                        serialPorts.Remove(serialPort);

                        RemoveSavedPort(serialPort);
                    }

                    GUI.skin.button.stretchWidth = true;

                    GUI.backgroundColor = Color.white;

                    GUILayout.EndHorizontal();
                }
            } catch (Exception e)
            {
                Debug.LogError("[AwakeSerialCom] Error: " + e.Message);
            }

            GUILayout.Space(10);

            // Show all available serial ports
            GUILayout.Label("Available serial ports:");

            try
            {
                foreach (string port in SerialPort.GetPortNames())
                {
                    // If port is already opened, skip it
                    bool isPortOpened = false;

                    foreach (SerialPort serialPort in serialPorts)
                        if (serialPort.PortName == port)
                            isPortOpened = true;

                    if (isPortOpened)
                        continue;

                    GUILayout.BeginHorizontal();

                    GUI.backgroundColor = Color.white;

                    GUILayout.Box(port);

                    GUI.backgroundColor = Color.green;

                    GUI.skin.button.stretchWidth = false;

                    if (GUILayout.Button("+"))
                    {
                        SerialPort _port = OpenSerialByName(port);
                    }

                    GUI.skin.button.stretchWidth = true;

                    GUI.backgroundColor = Color.white;

                    GUILayout.EndHorizontal();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[AwakeSerialCom] Error: " + e.Message);
            }
        }

        private IEnumerator ClearPortDelayed(SerialPort serialPort)
        {
            yield return new WaitForEndOfFrame();
            
            serialPort.Close();
            serialPorts.Remove(serialPort);
        }
        
        private SerialPort OpenSerialByName(string port)
        {
            SerialPort _port;
            
            try
            {
                _port = new SerialPort(port, baudrate);
                _port.ReadTimeout = 0;
                _port.Open();
            }
            catch (Exception e)
            {
                Debug.LogError("[AwakeSerialCom] Error opening port: " + e.Message);
                return null;
            }

            serialPorts.Add(_port);
            SavePort(_port);

            return _port;
        }

        private void SavePort(SerialPort serialPort)
        {
            CleanupSavedPorts();

            string savedSerialPorts = PlayerPrefs.GetString("serialPorts");
            
            // If port already saved, skip it
            if (savedSerialPorts.Contains(serialPort.PortName))
                return;
            
            PlayerPrefs.SetString("serialPorts", savedSerialPorts + "," + serialPort.PortName);
            
            CleanupSavedPorts();
        }

        private void RemoveSavedPort(SerialPort serialPort)
        {
            CleanupSavedPorts();

            string savedSerialPorts = PlayerPrefs.GetString("serialPorts");
            PlayerPrefs.SetString("serialPorts", savedSerialPorts.Replace(serialPort.PortName, ""));
            
            CleanupSavedPorts();
        }

        private void CleanupSavedPorts()
        {
            // Remove commas at the beginning and at the end
            string savedSerialPorts = PlayerPrefs.GetString("serialPorts");
            savedSerialPorts = savedSerialPorts.Trim(',');

            // Remove duplicated commas
            savedSerialPorts = PlayerPrefs.GetString("serialPorts");
            savedSerialPorts = savedSerialPorts.Replace(",,", ",");

            // Remove duplicated ports
            List<string> savedPortsNames = new List<string>(savedSerialPorts.Split(','));
            savedPortsNames.Remove("");
            savedSerialPorts = string.Join(",", savedPortsNames);
            
            PlayerPrefs.SetString("serialPorts", savedSerialPorts);
        }

        private void OnDestroy()
        {
            foreach (SerialPort serialPort in serialPorts)
                if (serialPort.IsOpen)
                    serialPort.Close();
        }
    }
}