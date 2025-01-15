using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;
public class StateManager : NetworkBehaviour
{
    public static GameState CurrentState;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        if (IsServer)
        {
            // Set the initial game state to MainMenu when the game starts
            CurrentState = GameState.Start;
        }
    }

    private void Update()
    {
            // Update logic for different states can go here, for example:
            UpdateGameStateClientRpc(CurrentState);
            HandleStateUpdate(CurrentState);
    }

    // ServerRpc to change the game state from client-side (host changes the state)
    [ServerRpc(RequireOwnership = false)]
    public void ChangeGameStateServerRpc(GameState newState)
    {
        CurrentState = newState;
    }

    // ClientRpc to update the game state on all clients
    [ClientRpc]
    private void UpdateGameStateClientRpc(GameState newState)
    {
        if (!IsOwner)
        {
            CurrentState = newState;
        }
    }
    PlacingFinalPoint Point;
    Vector3 Origo = new Vector3(0, 0, 0);
    [SerializeField] Vector3 StartPosition = new Vector3(0,0,0);
    private List<NetworkClient> ClientList = new List<NetworkClient>();
    private List<PlayerMovementScript> player_M = new List<PlayerMovementScript>();
    GameObject DungeonGen;
    Dungeongenerator Gen;
    RoomBehaviour Room_Behaviour;
    List<GameObject> room = new List<GameObject>();
    private List<PlayerShoot>player_S = new List<PlayerShoot>();
    [SerializeField] GameObject Lance;
    private void HandleStateUpdate(GameState state)
    {
        switch (state)
        {
            case GameState.Start:
                // Handle Start state logic
                if (IsHost)
                {
                    if (Input.GetKey(KeyCode.Space))
                    {
                        DungeonGen = GameObject.Find("DungeonGenerator");
                        Gen = DungeonGen.GetComponent<Dungeongenerator>();
                        Gen.DungeonStart();
                        //room = GameObject.Find("Room");
                        //Room_Behaviour = room.GetComponent<RoomBehaviour>();
                        Point = DungeonGen.GetComponent<PlacingFinalPoint>();
                        Point.FindAllRooms();
                        StartPosition = Point.PlacingFinalTile(Origo);
                        foreach(NetworkClient player in ClientList)
                        {
                            player.playerObject.transform.position = StartPosition;
                        }
                        ChangeGameStateServerRpc(GameState.Gameplay);
                    }
                }
                if (IsClient)
                {

                }
                break;
            case GameState.Gameplay:
                // Handle Gameplay state logic
                foreach (PlayerMovementScript Player in player_M)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    Player.Rotation();
                    Player.Movement();
                    Player.ChechAnimation();
                }
                foreach(PlayerShoot shoot in player_S)
                {
                    shoot.ShootUpdate();
                }
                break;
            case GameState.GameOver:
                // Handle Game Over state logic
                break;
        }
    }
    private void OnClientConnected(ulong clientid)
    {
        var playerobject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientid);
        ClientList.Add(new NetworkClient { ClientId = clientid, playerObject = playerobject });
        player_M.Add(playerobject.GetComponent<PlayerMovementScript>());
        player_S.Add(playerobject.GetComponent<PlayerShoot>());
        //if (StartPosition == new Vector3(0,0,0) && !IsHost)
        //{
        //    Point = DungeonGen.GetComponent<PlacingFinalPoint>();
        //    Point.FindAllRooms();
        //    StartPosition = Point.PlacingFinalTile(Origo);
        //    playerobject.transform.position = StartPosition;
        //}
    }
    private void OnClientDisconnected(ulong clientId)
    {
        // Find the player object in the ClientList and remove it
        var networkClient = ClientList.FirstOrDefault(player => player.ClientId == clientId);

        if (networkClient != null)
        {
            // Remove from the ClientList
            ClientList.Remove(networkClient);

            // Get the PlayerMovementScript from the playerObject and remove it from player_M
            PlayerMovementScript playerMovementScript = networkClient.playerObject.GetComponent<PlayerMovementScript>();
            player_M.Remove(playerMovementScript);
        }
    }
    public class NetworkClient
    {
        public ulong ClientId;
        public NetworkObject playerObject;
    }
}
