from minescript import *
from mido import MidiFile
import pynbs
import time

# Hypixel piano range
LOW = 43   # G2
HIGH = 89  # F6

def build_piano_coords():
    coords = {}
    x_lower, y_lower = 100, 80
    x_upper, y_upper = 101, 81
    
    # Bottom row: G2 (43) -> F4 (65)
    z_positions = [-3, -2, -1, 0, 1, 2, 3]  # 7 positions
    for i, note in enumerate(range(LOW, 66)):
        z = z_positions[i % len(z_positions)]
        coords[note] = (x_lower, y_lower, z)

    # Top row: G4 (67) -> F6 (89)
    for i, note in enumerate(range(67, HIGH + 1)):
        z = z_positions[i % len(z_positions)]
        coords[note] = (x_upper, y_upper, z)

    return coords

PIANO_COORDS = build_piano_coords()

def clamp_note(note):
    return max(LOW, min(HIGH, note))

def get_piano_coord(midi_note):
    note = clamp_note(midi_note)
    return PIANO_COORDS.get(note, None)

def click_note(note):
    coord = get_piano_coord(note)
    if coord:
        x, y, z = coord
        # Replace with your MineScript click/execute method
        player_look_at(x, y, z)
        player_press_attack(True)
        time.sleep(0.03) # we need to press longer for minecraft to register this click, this is needed with my awful 150ms ping.
        player_press_attack(False)
        echo(f"Played note {note} at {coord}")

# ---------------- MIDI PLAYER ----------------
# Note: Pitch is really wrong but works.
def play_midi(filename, note_delay=0.03):
    mid = MidiFile(filename)

    start_time = time.time()
    for msg in mid.play():
        if msg.type == 'note_on' and msg.velocity > 0:
            click_note(msg.note)
            time.sleep(note_delay)  # small delay so Minecraft can register multiple notes at the same tick, this is needed with my awful 150ms ping.
        elif msg.type == 'note_off':
            pass  # not needed

# ---------------- NBS PLAYER (broken) ----------------
def play_nbs(filename, note_delay=0.03):
    # load header (for tempo)
    song = pynbs.read(filename)
    ticks_per_second = song.header.tempo / 100.0
    seconds_per_tick = 1.0 / ticks_per_second
    start_time = time.time()

    # iterate using pynbs.read -- yields (tick, chord)
    for tick, chord in pynbs.read(filename):
        target_time = start_time + (tick * seconds_per_tick)
        while time.time() < target_time: # this while loop is probably a bad idea and is the problem
            time.sleep(0.001)

        for note in chord:
            mc_note = LOW + (note.key % (HIGH - LOW + 1))
            click_note(mc_note)
            time.sleep(note_delay)

# ---------------- MAIN ----------------
if __name__ == "__main__":
    play_midi("/home/memfrag/.local/share/PrismLauncher/instances/1.21.7 Minescript/minecraft/minescript/example.mid")
    echo("mmplay job exited")
