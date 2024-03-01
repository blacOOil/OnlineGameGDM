using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using QFSW.QC;
using TMPro;

public class LoginManagerScript : MonoBehaviour
{
    public int characterPrefabIndex,clientCharaterPrefabIndex;
    public string characterSelectionIndex;
    
    [Header("Online-Sections")]
    public List<uint> AlternativePlayerPrefabs;
    public TMP_InputField userNameInputField;
    public TMP_InputField passCodeInputField;
    public GameObject[] spawnPoint;

    [Header("UI-Prefabs")]
    public GameObject loginPanel;
    public GameObject scorePanel;
    public GameObject leaveButton;
    public GameObject mainmenuPanel;
    public GameObject playButton;
    public GameObject settingsButton;
    public GameObject quitButton;
    public GameObject settingsPanel;
    public GameObject backButton;

    public TMP_Dropdown ColorSelector;
   
    private bool isApproveConnection = false;
    [Command("set-approve")]

    public void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandhelClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        SetUIMainMenuVisible(false);
    }
    public void CharacterSelect(int val)
    {
        int selectedValue = ColorSelector.GetComponent<TMP_Dropdown>().value;
        switch (selectedValue)
        {
            case 0:
                characterPrefabIndex =  0;
                break;
            case 1: Debug.Log("Blue");
                characterPrefabIndex = 1;
                break;
            case 2: Debug.Log("Green");
                characterPrefabIndex =  2;
                break;
        }
    }
    public void ClientCharacterSelect(int val)
    {
        val = ColorSelector.GetComponent<TMP_Dropdown>().value;
        int selectedValue = val;
        switch (selectedValue)
        {
            case 0:
                clientCharaterPrefabIndex = 0;
                break;
            case 1:
                Debug.Log("Blue");
                clientCharaterPrefabIndex = 1;
                break;
            case 2:
                Debug.Log("Green");
                clientCharaterPrefabIndex = 2;
                break;
        }
    }

    private void HandleServerStarted()
    {
        Debug.Log("");
    }
    public void Leave()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }

        SetUILoginVisible(false);
    }

    // public void UseEscToDisconnectedServer() {
    //     if (Input.GetKeyDown(KeyCode.Escape)) {
    //         leaveButton.SetActive(false);
    //     }
    // }

    public void SetUILoginVisible(bool isUserLogin) {
        if (isUserLogin) {
            loginPanel.SetActive(false);
            leaveButton.SetActive(true);
            scorePanel.SetActive(true);
        } else {
            loginPanel.SetActive(true);
            leaveButton.SetActive(false);
            scorePanel.SetActive(false);
        }
    }

    public void SetUIMainMenuVisible(bool isUserLogin) {
        if (isUserLogin) {
            mainmenuPanel.SetActive(false);
            playButton.SetActive(false);
            settingsButton.SetActive(false);
            quitButton.SetActive(false);
        } else {
            mainmenuPanel.SetActive(true);
            playButton.SetActive(true);
            settingsButton.SetActive(true);
            quitButton.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if(NetworkManager.Singleton == null) { return; }
        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandhelClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

    private void HandhelClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
        SetUILoginVisible(true);
        }
        
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        if (NetworkManager.Singleton.IsHost)
        {
           
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            Leave();
        }
    }

    public bool SetIsApproveConnection()
    {
        isApproveConnection = !isApproveConnection;
        return isApproveConnection;
    }

    public void Host()
    {
        string username = userNameInputField.GetComponent<TMP_InputField>().text;
        string Passcode = passCodeInputField.GetComponent<TMP_InputField>().text;
        string connectionData = username + "|" + Passcode;
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(connectionData);
        
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        NetworkManager.Singleton.StartHost();
        Debug.Log(username + " : Start host");
        Debug.Log("Set Passcode is : " + Passcode);
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // The client identifier to be authenticated
        var clientId = request.ClientNetworkId;

        // Additional connection data defined by user code
        var connectionData = request.Payload;
        int clientSkin;
        int byteLength = connectionData.Length;
        bool isApprove = false;

        if (byteLength > 0)
        {
           
            string[] data = System.Text.Encoding.ASCII.GetString(connectionData, 0, byteLength).Split('|');
            string hostPassword = passCodeInputField.GetComponent<TMP_InputField>().text;
            string hostData = userNameInputField.text;
            isApprove = ApproveConnection(data, hostData, hostPassword);
            ClientCharacterSelect(0);
            characterPrefabIndex = clientCharaterPrefabIndex;
            response.PlayerPrefabHash = AlternativePlayerPrefabs[characterPrefabIndex];
            Debug.Log("Joint as: " + clientCharaterPrefabIndex);

        }
        else
        {
            if (NetworkManager.Singleton.IsHost)
            {
                CharacterSelect(0);
                response.PlayerPrefabHash = AlternativePlayerPrefabs[characterPrefabIndex];
                Debug.Log("Host as: " + characterPrefabIndex);
            
            } 
        }
        //  else
        //  {
        //  if (NetworkManager.Singleton.IsHost)
        // {

        
        //     Debug.Log("Host as :" + characterPrefabIndex);
        // }
        //  }

        // Your approval logic determines the following values
        response.Approved = isApprove;
        response.CreatePlayerObject = true;

        // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
        //response.PlayerPrefabHash = AlternativePlayerPrefabs[characterPrefabIndex];
      

        // Position to spawn the player object (if null it uses default of Vector3.zero)
        response.Position = Vector3.zero;

        // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
        response.Rotation = Quaternion.identity;
        setSpawnLocation(clientId, response);

        // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.Reason
        // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
        response.Reason = "Some reason for not approving the client";

        // If additional approval steps are needed, set this to true until the additional steps are complete
        // once it transitions from true to false the connection approval response will be processed.
        response.Pending = false;
    }

    public void Client()
    {
        
        string username = userNameInputField.GetComponent<TMP_InputField>().text;
        string Passcode = passCodeInputField.GetComponent<TMP_InputField>().text;
        
        string connectionData = username + "|" + Passcode;
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(connectionData);
        
        NetworkManager.Singleton.StartClient();
        Debug.Log(username + " : Start client");
    }

    public bool ApproveConnection(string[] data, string hostData,string hostPasscode)
    {
        string clientData = data[0];
        string clientPassword = data[1];
        bool isApprove = !System.String.Equals(clientData.Trim(), hostData.Trim());
        if (isApprove && (System.String.Equals(hostPasscode.Trim(), clientPassword.Trim()))){
            return true;
        }
        else 
        {
            return false;
        }
       

    }
    private void setSpawnLocation(ulong clientId, NetworkManager.ConnectionApprovalResponse response)
    {
        Vector3 spawnpos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        int SpawnerRandom = Random.Range(0,spawnPoint.Length);

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            spawnpos = spawnPoint[SpawnerRandom].transform.position; 
            spawnRot = spawnPoint[SpawnerRandom].transform.rotation;
        }
        else
        {
            switch (NetworkManager.Singleton.ConnectedClients.Count)
            {
                case 1:
                    spawnpos = new Vector3(0f, 0f, 0f); spawnRot = Quaternion.Euler(0f, 180f, 0f);
                    break;
                case 2:
                    spawnpos = new Vector3(2f, 0f, 0f); spawnRot = Quaternion.Euler(0f, 225f, 0f);
                    break;

            }
        }
        response.Position = spawnpos;
        response.Rotation = spawnRot;
    }
    
    public void Play() {
        SetUILoginVisible(false);
        SetUIMainMenuVisible(true);
    }

    public void Settings() {
        settingsPanel.SetActive(true);
        SetUIMainMenuVisible(true);
    }

    public void Quit() {
        Application.Quit();
    }

    public void Back() {
        settingsPanel.SetActive(false);
        SetUIMainMenuVisible(false);

        loginPanel.SetActive(false);
        leaveButton.SetActive(false);
        scorePanel.SetActive(false);
    }
}