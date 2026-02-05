import win32api
import win32con
import time

def move_mouse_relative(dx, dy):
    """Simulate relative mouse movement (works in Unity)."""
    win32api.mouse_event(win32con.MOUSEEVENTF_MOVE, dx, dy, 0, 0)

def move_mouse_horizontal(speed=150):
    """Move the mouse horizontally while holding J."""
    print("Hold 'J' to move mouse right. Press Ctrl+C to stop.")

    while True:
        if win32api.GetAsyncKeyState(ord('J')) & 0x8000:
            move_mouse_relative(speed, 0)  # move right
        time.sleep(0.01)

try:
    move_mouse_horizontal()
except KeyboardInterrupt:
    print("Stopped.")
