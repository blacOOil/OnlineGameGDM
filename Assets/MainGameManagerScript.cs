using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MainGameManagerScript : MonoBehaviour
{
    public void ServerStart()
    {
    NetworkManager.Singleton.StartServer();      // Starts the NetworkManager as just a server (that is, no local client).
    }
    public void Hosting()
    {
        
        NetworkManager.Singleton.StartHost();        // Starts the NetworkManager as both a server and a client (that is, has local client)
    }
    public void Clienting()
    {
      NetworkManager.Singleton.StartClient();      // Starts the NetworkManager as just a client.
        
    }
    
   
}
