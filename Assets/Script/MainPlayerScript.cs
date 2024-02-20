using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class MainPlayerScript : NetworkBehaviour
{
    public string OwnerNameDisplay;
    public float speed = 5.0f;
    public float rotationSpeed = 10.0f;
    Rigidbody rb;
    public TMP_Text namePrefab;
    private TMP_InputField NameInput;
    private TMP_Text nameLabel;
  


    private NetworkVariable<int> posX = new NetworkVariable<int>(
        0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner
        );
    public struct NetworkString : INetworkSerializable
    {
        public FixedString32Bytes info;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref info);
        }

        public static implicit operator NetworkString(string v) =>
            new NetworkString() { info = new FixedString32Bytes(v) };

        public override string ToString()
        {
            return info.ToString();
        }
    }
    private NetworkVariable<NetworkString> playernameA = new NetworkVariable<NetworkString>(
        new NetworkString { info = "Player" },
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<NetworkString> playernameB = new NetworkVariable<NetworkString>(
        new NetworkString { info = "Player" },
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private LoginManagerScript loginManagerScripts;
    public override void OnNetworkSpawn()
    {
        GameObject canvas = GameObject.FindWithTag("MainCanvas");
        nameLabel = Instantiate(namePrefab, Vector3.zero, Quaternion.identity) as TMP_Text;
        nameLabel.transform.SetParent(canvas.transform);

        posX.OnValueChanged += (int previousValue, int newValue) =>
        {
            //Debug.Log("Owner ID = " + OwnerClientId + ": PosX = " + posX.Value);
        };
        //  if (IsServer)
        //{
        //  playernameA.Value = new NetworkString() { info = new FixedString32Bytes(OwnerNameDisplay) };
        //playernameB.Value = new NetworkString() { info = new FixedString32Bytes(OwnerNameDisplay) };
        //}
        if (IsOwner)
        {
            loginManagerScripts = GameObject.FindObjectOfType<LoginManagerScript>();
            if (loginManagerScripts != null)
            {
                string name = loginManagerScripts.userNameInputField.text;
                if (IsServer) { playernameA.Value = name; }
                else { playernameB.Value = name; }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 nameLabelPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 2.5f, 0));
        nameLabel.text = gameObject.name;
        nameLabel.transform.position = nameLabelPos;
        if (IsOwner)
        {
            posX.Value = (int)System.Math.Ceiling(transform.position.x);
        }
       
        UpdatePlayerInfo();
    }
    private void UpdatePlayerInfo()
    {
        if (IsServer) { nameLabel.text = playernameA.Value.ToString(); }
        else { nameLabel.text = playernameB.Value.ToString(); }
    }
    public override void OnDestroy()
    {
        if (nameLabel != null) Destroy(nameLabel.gameObject);
        base.OnDestroy();
    }
    void Start()
    {
       
    }
    void NamingPlayer()
    {
        string username = NameInput.GetComponent<TMP_InputField>().text;
        OwnerNameDisplay = username;
    }
}
