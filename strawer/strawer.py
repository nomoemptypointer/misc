import tkinter as tk
from pynput import mouse
import threading
import time
from PIL import Image
import win32api
import win32con

start_pos = None
current_pos = None
selecting = False

# You might want to adjust time.sleep values depending on how potato is your PC or if your browser is shitty/slow

IMAGE_PATH = "image5.png"  # <-- Replace with your image file path (place it next to the script)
image = Image.open(IMAGE_PATH).convert("L")  # grayscale this shit

def on_click(x, y, button, pressed):
    global start_pos, current_pos, selecting
    if button == mouse.Button.left:
        if pressed:
            start_pos = (x, y)
            current_pos = (x, y)
            selecting = True
            print(f"Selection started at {start_pos}")
        else:
            current_pos = (x, y)
            selecting = False
            print(f"Selection ended at {current_pos}")
            print(f"Selected rectangle: {start_pos} to {current_pos}")
            # Stop listener after selection
            return False

def on_move(x, y):
    global current_pos, selecting
    if selecting:
        current_pos = (x, y)

def listener_thread():
    with mouse.Listener(on_click=on_click, on_move=on_move) as listener:
        listener.join()
            # After selection ends, close tkinter window to stop the draw loop
    root.after(0, root.destroy)

def draw_loop():
    global start_pos, current_pos, selecting
    if start_pos and current_pos and selecting:
        canvas.delete("rect")
        x1, y1 = start_pos
        x2, y2 = current_pos
        canvas.create_rectangle(x1, y1, x2, y2, outline="red", width=2, tag="rect")
    else:
        canvas.delete("rect")
    root.after(20, draw_loop)  # roughly 50 fps

def move_mouse(x, y):
    win32api.SetCursorPos((x, y))

def left_click():
    win32api.mouse_event(win32con.MOUSEEVENTF_LEFTDOWN, 0, 0)
    time.sleep(0.05)
    win32api.mouse_event(win32con.MOUSEEVENTF_LEFTUP, 0, 0)

def drag_line(x_start, y, x_end):
    move_mouse(x_start, y)
    win32api.mouse_event(win32con.MOUSEEVENTF_LEFTDOWN, 0, 0)
    # Move cursor from x_start to x_end in small steps for smooth drag
    steps = max(abs(x_end - x_start), 1)
    for i in range(steps + 1):
        x = x_start + (x_end - x_start) * i // steps
        move_mouse(x, y)
        time.sleep(0.005)  # small delay for smoothness, adjust if needed
    win32api.mouse_event(win32con.MOUSEEVENTF_LEFTUP, 0, 0)
    time.sleep(0.005)  # small pause between lines

def draw_image_in_rect(start, end, img):
    left = min(start[0], end[0])
    top = min(start[1], end[1])
    right = max(start[0], end[0])
    bottom = max(start[1], end[1])

    rect_width = right - left
    rect_height = bottom - top

    # Resize image to fit rectangle
    img_resized = img.resize((rect_width, rect_height))
    pixels = img_resized.load()

    print("Starting drawing...")

    for y in range(rect_height):
        x = 0
        while x < rect_width:
            # Skip light pixels
            while x < rect_width and pixels[x, y] >= 128:
                x += 1
            if x == rect_width:
                break
            # Found start of dark run
            start_x = x
            while x < rect_width and pixels[x, y] < 128:
                x += 1
            end_x = x - 1
            # Drag line from start_x to end_x at row y
            screen_x_start = left + start_x
            screen_y = top + y
            screen_x_end = left + end_x
            drag_line(screen_x_start, screen_y, screen_x_end)

    print("Drawing complete.")

if __name__ == "__main__":
    # Setup tkinter fullscreen transparent window
    root = tk.Tk()
    root.attributes("-fullscreen", True)
    root.attributes("-topmost", True)
    root.attributes("-transparentcolor", "white")
    root.config(bg="white")

    canvas = tk.Canvas(root, bg="white", highlightthickness=0)
    canvas.pack(fill=tk.BOTH, expand=True)

    # Run mouse listener in separate thread
    threading.Thread(target=listener_thread, daemon=True).start()

    # Start drawing loop
    draw_loop()

    root.mainloop()

    # After selection, draw the image inside selected rectangle
    if start_pos and current_pos:
        draw_image_in_rect(start_pos, current_pos, image)
