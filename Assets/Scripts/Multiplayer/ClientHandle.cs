using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        var _msg = _packet.ReadString();
        var _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.Instance.myId = _myId;
        ClientSend.WelcomeReceived();

        // Now that we have the client's id, connect UDP
        Client.Instance.udp.Connect(((IPEndPoint) Client.Instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet _packet)
    {
        var _id = _packet.ReadInt();
        var _username = _packet.ReadString();
        var _position = _packet.ReadVector3();
        var _rotation = _packet.ReadQuaternion();

        GameManager.Instance.SpawnPlayer(_id, _username, _position, _rotation);
    }

    public static void PlayerPosition(Packet _packet)
    {
        var _id = _packet.ReadInt();
        var _position = _packet.ReadVector3();

        GameManager.players[_id].transform.position = _position;
    }

    public static void PlayerRotation(Packet _packet)
    {
        var _id = _packet.ReadInt();
        var _rotation = _packet.ReadQuaternion();

        GameManager.players[_id].transform.rotation = _rotation;
    }

    public static void ObjectTransform(Packet _packet)
    {
        var _id = _packet.ReadInt();
        var _position = _packet.ReadVector3();
        var _rotation = _packet.ReadQuaternion();
        var _scale = _packet.ReadVector3();
        
        GameManager.networkTransforms[_id].ReciveTransform(_position, _rotation, _scale);
    }

    public static void SpawnPrefab(Packet _packet)
    {
        var _prefabId = _packet.ReadInt();
        var _networkId = _packet.ReadInt();
        var _position = _packet.ReadVector3();
        var _rotation = _packet.ReadQuaternion();
        var _scale = _packet.ReadVector3();
        
        GameManager.Instance.SpawnPrefab(_prefabId, _networkId , _position, _rotation, _scale);
    }
    
}