using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, NetworkTransform> networkTransforms = new Dictionary<int, NetworkTransform>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    
    public GenericDictionary<int, GameObject> networkPrefabs = new GenericDictionary<int, GameObject>();


    /// <summary>Spawns a player.</summary>
    /// <param name="_id">The player's ID.</param>
    /// <param name="_name">The player's name.</param>
    /// <param name="_position">The player's starting position.</param>
    /// <param name="_rotation">The player's starting rotation.</param>
    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;
        if (_id == Client.Instance.myId)
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
        else
            _player = Instantiate(playerPrefab, _position, _rotation);

        _player.GetComponent<PlayerManager>().id = _id;
        _player.GetComponent<PlayerManager>().username = _username;
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }

    public void SpawnPrefab(int _id,int networkId, Vector3 _position, Quaternion _rotation, Vector3 _scale)
    {
        GameObject _prefab = Instantiate(networkPrefabs[_id], _position, _rotation);
        _prefab.transform.localScale = _scale;
        var networkTransform = _prefab.gameObject.GetComponent<NetworkTransform>();
        if (networkTransform != null)
        {        
            networkTransform.SetId(networkId);
            networkTransforms.Add(networkId, networkTransform);
        }
    }
}