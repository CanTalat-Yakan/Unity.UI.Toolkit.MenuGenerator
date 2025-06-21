using System;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuEditor
    {
        public EditorWindowDrawer Window;
        public Action Repaint;
        public Action Close;

        [MenuItem("Tools/ UI Menu Builder %g", false, priority = 1003)]
        private static void ShowWindow(MenuCommand menuCommand)
        {
            var editor = new UIMenuEditor();

            editor.Window = new EditorWindowDrawer()
                .AddUpdate(editor.Update)
                .SetPreProcess(editor.PreProcess)
                .SetHeader(editor.Header)
                .SetBody(editor.Body)
                .GetRepaintEvent(out editor.Repaint)
                .GetCloseEvent(out editor.Close)
                .SetDrawBorder()
                .ShowWindow();
        }

        private void Update()
        {
        }

        private void PreProcess()
        {
        }

        private void Header()
        {
        }

        private void Body()
        {
        }
    }
}
