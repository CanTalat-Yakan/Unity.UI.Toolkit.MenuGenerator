using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEssentials
{
    class SimpleTreeView : TreeView
    {
        private List<TreeViewItem> _items;
        public SimpleTreeView(TreeViewState state, List<TreeViewItem> items) : base(state)
        {
            _items = items;
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
            SetupParentsAndChildrenFromDepths(root, _items);
            return root;
        }

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return true;
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            if (hasSearch)
                return;

            DragAndDrop.PrepareStartDrag();
            var draggedRows = GetRows().Where(item => args.draggedItemIDs.Contains(item.id)).ToList();
            DragAndDrop.objectReferences = new UnityEngine.Object[] { }; // Required, but not used
            DragAndDrop.SetGenericData("TreeViewDragging", draggedRows);
            DragAndDrop.StartDrag("Dragging TreeView");
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            var draggedRows = DragAndDrop.GetGenericData("TreeViewDragging") as List<TreeViewItem>;
            if (draggedRows == null)
                return DragAndDropVisualMode.None;

            if (args.performDrop)
            {
                foreach (var draggedItem in draggedRows)
                    _items.RemoveAll(i => i.id == draggedItem.id);

                int insertIndex = args.parentItem == null || args.parentItem.id == 0
                    ? args.insertAtIndex
                    : _items.FindIndex(i => i.id == args.parentItem.id) + 1;

                if (insertIndex < 0 || insertIndex > _items.Count)
                    insertIndex = _items.Count;

                _items.InsertRange(insertIndex, draggedRows);
                Reload();
            }
            return DragAndDropVisualMode.Move;
        }
    }

    public class UIMenuEditor
    {
        public EditorWindowDrawer Window;
        public Action Repaint;
        public Action Close;

        private UIMenuData _data = new();
        private TreeViewState _treeViewState;
        private SimpleTreeView _treeView;
        public List<ScriptableObject> Root = new();

        [MenuItem("Tools/ UI Menu Builder %g", false, priority = 1003)]
        private static void ShowWindow()
        {
            var editor = new UIMenuEditor();
            editor.Window = new EditorWindowDrawer("UI Menu Builder", new(300, 400), new(600, 800))
                .SetHeader(editor.Header, EditorWindowStyle.Toolbar)
                .SetPane(editor.Pane, EditorPaneStyle.Left)
                .SetBody(editor.Body)
                .SetFooter(editor.Footer, EditorWindowStyle.HelpBox)
                .GetRepaintEvent(out editor.Repaint)
                .GetCloseEvent(out editor.Close)
                .ShowUtility();
        }

        private void Header()
        {
            if (EditorGUILayout.DropdownButton(EditorGUIUtility.IconContent("Toolbar Plus"), FocusType.Passive, EditorStyles.toolbarDropDown))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Category"), false, () => Root.Add(ScriptableObject.CreateInstance<CategoryData>()));
                menu.AddItem(new GUIContent("Header"), false, () => Debug.Log("Option 2 selected"));
                menu.AddItem(new GUIContent("Space"), false, () => Debug.Log("Option 2 selected"));
                menu.DropDown(GUILayoutUtility.GetLastRect());
            }

            GUILayout.Label("UI Menu Builder Header", EditorStyles.boldLabel);
            GUILayout.Button("Header", EditorStyles.toolbarButton);
            GUILayout.FlexibleSpace();
        }

        private List<TreeViewItem> _treeData = new()
        {
            new TreeViewItem { id = 1, depth = 0, displayName = "Category 1" },
            new TreeViewItem { id = 2, depth = 1, displayName = "Header 1" },
            new TreeViewItem { id = 3, depth = 0, displayName = "Category 2" },
        };

        private void Pane()
        {
            _treeView ??= new SimpleTreeView(_treeViewState ??= new(), _treeData);
            var rect = GUILayoutUtility.GetRect(0, float.MaxValue, 0, float.MaxValue, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            _treeView.Reload();
            _treeView.OnGUI(rect);
        }

        private void Body()
        {
            GUILayout.Button("Test2");
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
            GUILayout.Label("UI Menu Builder Body", EditorStyles.boldLabel);
        }

        private void Footer()
        {
            GUILayout.Label("UI Menu Builder Footer", EditorStyles.boldLabel);
        }
    }
}