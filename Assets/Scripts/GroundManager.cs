using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour {
    public enum CubeState {
        Blocked,
        Path
    }

    public int width = 20; // Ground width
    public int height = 30; // Ground height
    public float cellSize = 1f; // Size of each cell in the grid
    public GameObject player; // Reference to the player GameObject

    // Dictionary to hold the state of each ground cell
    private Dictionary<Vector2Int, CubeState> groundCells;

    void Start() {
        // Initialize the dictionary
        groundCells = new Dictionary<Vector2Int, CubeState>();

        // Determine the state of each ground cell
        DetermineCellStates();

        // Print the states of all ground cells
        PrintCellStates();
    }

    void Update() {
        // Print the player position and the tag of the cell below the player
        PrintPlayerPositionAndCellTag();
    }

    void DetermineCellStates() {
        // Iterate over each cell in the grid
        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                Vector2Int cellPos = new Vector2Int(x, z);
                Vector3 worldPos = new Vector3(x * cellSize, 0, z * cellSize) + transform.position;
                bool isBlocked = IsObstacleOnCell(worldPos);
                CubeState state = isBlocked ? CubeState.Blocked : CubeState.Path;

                // Add the cell and its state to the dictionary
                groundCells.Add(cellPos, state);

                // Optionally, change the color of the cell to visualize its state
                Debug.DrawRay(worldPos, Vector3.up, isBlocked ? Color.red : Color.green, 10f);
            }
        }
    }

    bool IsObstacleOnCell(Vector3 cellPosition) {
        // Perform a box overlap to check for obstacles on top of the cell
        Collider[] hitColliders = Physics.OverlapBox(cellPosition, new Vector3(cellSize / 2, 0.5f, cellSize / 2), Quaternion.identity);

        foreach (Collider collider in hitColliders) {
            if (collider.CompareTag("Obstacle")) {
                return true;
            }
        }

        return false;
    }

    // Optional: method to get the state of a specific cell
    public CubeState GetCellState(Vector2Int cellPos) {
        if (groundCells.ContainsKey(cellPos)) {
            return groundCells[cellPos];
        } else {
            Debug.LogWarning("Cell not found in the dictionary.");
            return CubeState.Path; // Default value
        }
    }

    // Method to print the states of all ground cells
    void PrintCellStates() {
        foreach (var cellEntry in groundCells) {
            Vector2Int cellPos = cellEntry.Key;
            CubeState state = cellEntry.Value;

            string tag = state == CubeState.Blocked ? "Blocked" : "Path";
            Debug.Log($"X: {cellPos.x}, Z: {cellPos.y}, Tag: {tag}");
        }
    }

    // Method to print the player position and the tag of the cell below the player
    void PrintPlayerPositionAndCellTag() {
        if (player == null) {
            Debug.LogWarning("Player GameObject is not assigned.");
            return;
        }

        Vector3 playerPosition = player.transform.position;
        Vector2Int cellPos = new Vector2Int(
            Mathf.FloorToInt((playerPosition.x - transform.position.x) / cellSize),
            Mathf.FloorToInt((playerPosition.z - transform.position.z) / cellSize)
        );

        if (groundCells.ContainsKey(cellPos)) {
            CubeState state = groundCells[cellPos];
            string tag = state == CubeState.Blocked ? "Blocked" : "Path";
            Debug.Log($"Player Position: X: {playerPosition.x}, Z: {playerPosition.z}, Cell Tag: {tag}");
        } else {
            Debug.Log($"Player Position: X: {playerPosition.x}, Z: {playerPosition.z}, Cell Tag: Not Found");
        }
    }
}
