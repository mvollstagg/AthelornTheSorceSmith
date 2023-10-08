using UnityEngine;
using AthelornTheSorceSmith.Assets.Scripts.Core.SaveSystem.Encoders;
using AthelornTheSorceSmith.Assets.Scripts.Core.SaveSystem.Utilities;
using System;
using System.Text;

public class EncryptionDemo : MonoBehaviour
{
    private void Start()
    {
        // ! This is not working right now. 31.08.2023
        // Sample data to be encoded
        string originalData = "Hello, this is a test message.";

        // Convert the string data to bytes
        byte[] dataBytes = Encoding.UTF8.GetBytes(originalData);

        // Create an instance of Aes256Encoder (replace with your actual key)
        byte[] encryptionKey = new byte[32] { 0x41, 0x74, 0x68, 0x65, 0x6C, 0x6F, 0x72, 0x6E, 0x54, 0x68, 0x65, 0x53, 0x6F, 0x72, 0x63, 0x65,
                                              0x53, 0x6D, 0x69, 0x74, 0x68, 0x00, 0x00, 0x00, 0x00, 0x41, 0x74, 0x68, 0x65, 0x6C, 0x6F, 0x72 };
        Aes256Encoder encoder = new Aes256Encoder();

        // Encode the data
        byte[] encodedData = encoder.Encode(dataBytes);

        // Decode the data
        byte[] decodedData = encoder.Decode(encodedData);

        // Convert decoded bytes back to a string
        string decodedString = Encoding.UTF8.GetString(decodedData);

        // Display results
        Debug.Log("Original Data: " + originalData);
        Debug.Log("Encoded Data: " + BitConverter.ToString(encodedData).Replace("-", ""));
        Debug.Log("Decoded Data: " + decodedString);
    }
}
