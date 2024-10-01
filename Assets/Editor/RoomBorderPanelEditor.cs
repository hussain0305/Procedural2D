// using UnityEditor;
// using UnityEngine;
//
// [CustomEditor(typeof(RoomBorderPanel))]
// public class RoomBorderPanelEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         RoomBorderPanel roomBorderPanel = (RoomBorderPanel)target;
//
//         // Draw the 'edge' field
//         roomBorderPanel.edge = (RoomEdge)EditorGUILayout.EnumPopup("Edge", roomBorderPanel.edge);
//
//         // Conditionally display fields based on the value of 'edge'
//         if (roomBorderPanel.edge == RoomEdge.Ceiling || roomBorderPanel.edge == RoomEdge.Floor)
//         {
//             roomBorderPanel.horizontalPanel = (RoomBorderHorizontal)EditorGUILayout.ObjectField(
//                 "Horizontal Panel", roomBorderPanel.horizontalPanel, typeof(RoomBorderHorizontal), true);
//         }
//         else if (roomBorderPanel.edge == RoomEdge.Left || roomBorderPanel.edge == RoomEdge.Right)
//         {
//             roomBorderPanel.verticalPanel = (RoomBorderVertical)EditorGUILayout.ObjectField(
//                 "Vertical Panel", roomBorderPanel.verticalPanel, typeof(RoomBorderVertical), true);
//         }
//
//         // Apply changes made in the inspector
//         if (GUI.changed)
//         {
//             EditorUtility.SetDirty(roomBorderPanel);
//         }
//     }
// }
