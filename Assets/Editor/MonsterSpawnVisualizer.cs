using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(MonsterManager))]
public class MonsterSpawnVisualizer : Editor
{
    private void OnSceneGUI()
    {
        MonsterManager manager = (MonsterManager)target;

        if (manager.monsterPrefabs == null || manager.monsterPrefabs.Count == 0)
            return;

        DrawSpawnBounds(manager.bullseyePoints, manager.monsterPrefabs[0], Color.red);
        DrawSpawnBounds(manager.ring1Points, manager.monsterPrefabs[1], Color.green);
        DrawSpawnBounds(manager.triangle2Points, manager.monsterPrefabs[2], Color.blue);
        DrawSpawnBounds(manager.ring3Points, manager.monsterPrefabs[3], Color.cyan);
        DrawSpawnBounds(manager.triangle4Points, manager.monsterPrefabs[4], Color.magenta);
        DrawSpawnBounds(manager.ring5Points, manager.monsterPrefabs[5], Color.yellow);
    }

    private void DrawSpawnBounds(List<Transform> points, GameObject monsterPrefab, Color color)
    {
        if (points == null || monsterPrefab == null)
            return;

        SpriteRenderer sr = monsterPrefab.GetComponentInChildren<SpriteRenderer>();
        if (sr == null || sr.sprite == null)
            return;

        Vector2 spriteSize = sr.sprite.bounds.size;

        Handles.color = color;

        foreach (Transform point in points)
        {
            if (point == null) continue;

            // Account for rotation
            Quaternion rot = point.rotation;
            Vector3 center = point.position;

            // Use matrix to draw in local rotation
            using (new Handles.DrawingScope(Matrix4x4.TRS(center, rot, Vector3.one)))
            {
                Handles.DrawWireCube(Vector3.zero, spriteSize);
            }

            // Optional: Draw label for clarity
            Handles.Label(center + Vector3.up * spriteSize.y * 0.6f, monsterPrefab.name, new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = color },
                fontStyle = FontStyle.Bold
            });
        }
    }
}
