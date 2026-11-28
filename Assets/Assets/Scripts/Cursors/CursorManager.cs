using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Cursors
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance { get; private set; }

        [SerializeField] private CustomCursor[] cursors;
        
        private CustomCursorID _currentCursor = CustomCursorID.Default;
        private bool _cursorHidden;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

        public void SetCursor(CustomCursorID id)
        {
            if (_currentCursor == id) return;

            switch (id)
            {
                case CustomCursorID.Hidden:
                    HideCursor();
                    return;
                default:
                    var cursor = cursors.FirstOrDefault(c => c.id == id);
                    if (cursor == null) return;

                    _currentCursor = id;
                    Cursor.SetCursor(cursor.texture, cursor.hotspot, CursorMode.Auto);
                    ShowCursor();
                    return;
            }
        }
        
        private void HideCursor()
        {
            if (_cursorHidden) return;
            
            _currentCursor = CustomCursorID.Hidden;
            _cursorHidden = true;
            Cursor.visible = false;
        }

        private void ShowCursor()
        {
            if (!_cursorHidden) return;
            
            _cursorHidden = false;
            Cursor.visible = true;
        }
    }
}