import os
import cv2
import glob
import subprocess
from PIL import Image
from pysstv.color import Robot36
from decode import SSTVDecoder
from concurrent.futures import ThreadPoolExecutor, ProcessPoolExecutor
from concurrent.futures import ThreadPoolExecutor, as_completed
from functools import partial
import tempfile
import shutil

VIDEO_INPUT = "r6.mp4"
FRAMES_DIR = "frames"
AUDIO_DIR = "sstv_audio"
DECODED_DIR = "decoded_images"
OUTPUT_VIDEO = "reconstructed.mp4"
SAMPLE_RATE = 48000
FPS = 25

def extract_and_resize_frames(video_path, output_dir):
    os.makedirs(output_dir, exist_ok=True)
    cap = cv2.VideoCapture(video_path)
    i = 0
    while True:
        ret, frame = cap.read()
        if not ret:
            break
        resized = cv2.resize(frame, (320, 240))
        frame_path = os.path.join(output_dir, f"frame_{i:04d}.jpg")
        cv2.imwrite(frame_path, resized)
        i += 1
    cap.release()
    print(f"[+] Extracted and resized {i} frames")

def _encode_single_image(file, input_dir, output_dir, sample_rate):
    img_path = os.path.join(input_dir, file)
    clean_wav = os.path.join(output_dir, file.replace(".jpg", ".wav"))
    try:
        img = Image.open(img_path)
        sstv = Robot36(img, sample_rate, 16)
        sstv.write_wav(clean_wav)

        # Add pink noise with ffmpeg
        with tempfile.NamedTemporaryFile(suffix=".wav", delete=False) as tmp:
            noisy_path = tmp.name

        ffmpeg_cmd = [
            "ffmpeg",
            "-y",
            "-i", clean_wav,
            "-filter_complex",
            "[0:a]aresample=async=1[first];"
            "anoisesrc=color=pink:duration=60 [raw_noise];"
            "[raw_noise]volume=2.5[noise];"
            "[first][noise]amix=inputs=2:duration=first:dropout_transition=2",
            noisy_path
        ]

        subprocess.run(ffmpeg_cmd, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
        shutil.move(noisy_path, clean_wav)

        return f"✓ Encoded + noised {file}"
    except Exception as e:
        return f"✗ Failed to encode {file}: {e}"

def encode_frames_to_sstv_audio(input_dir, output_dir, sample_rate=48000):
    os.makedirs(output_dir, exist_ok=True)
    files = sorted(f for f in os.listdir(input_dir) if f.endswith(".jpg"))

    with ProcessPoolExecutor() as executor:
        job = partial(_encode_single_image, input_dir=input_dir, output_dir=output_dir, sample_rate=sample_rate)
        for i, result in enumerate(executor.map(job, files), 1):
            print(f"[{i}/{len(files)}] {result}")

def decode_sstv(input_wav):
    with SSTVDecoder(input_wav) as decoder:
        img = decoder.decode()
        if img is None:
            raise ValueError("No SSTV signal found")
        return img

def decode_file(input_wav, output_img):
    try:
        img = decode_sstv(input_wav)
        img.save(output_img)
        return (input_wav, None)  # Success
    except Exception as e:
        return (input_wav, e)  # Error

def decode_sstv_audio_multithread(input_dir, output_dir, max_workers=4):
    os.makedirs(output_dir, exist_ok=True)
    files = sorted(f for f in os.listdir(input_dir) if f.endswith(".wav"))

    with ProcessPoolExecutor(max_workers=max_workers) as executor:
        futures = []
        for file in files:
            input_wav = os.path.join(input_dir, file)
            output_img = os.path.join(output_dir, file.replace(".wav", ".png"))
            futures.append(executor.submit(decode_file, input_wav, output_img))

        for i, future in enumerate(as_completed(futures), 1):
            input_wav, error = future.result()
            file = os.path.basename(input_wav)
            if error is None:
                print(f"[{i}/{len(files)}] Decoded {file}")
            else:
                print(f"[!] Failed to decode {file}: {error}")

def create_video_from_frames(input_dir, output_path, fps=1):
    image_files = sorted(glob.glob(os.path.join(input_dir, "*.png")))
    if not image_files:
        print("[!] No decoded images found")
        return

    frame = cv2.imread(image_files[0])
    height, width, _ = frame.shape
    out = cv2.VideoWriter(output_path, cv2.VideoWriter_fourcc(*'mp4v'), fps, (width, height))

    for img_file in image_files:
        frame = cv2.imread(img_file)
        out.write(frame)
    out.release()
    print(f"[+] Final video saved to {output_path}")


if __name__ == "__main__":
    print("[1/4] Extracting frames...")
    extract_and_resize_frames(VIDEO_INPUT, FRAMES_DIR)

    print("[2/4] Encoding SSTV audio...")
    encode_frames_to_sstv_audio(FRAMES_DIR, AUDIO_DIR, SAMPLE_RATE)

    print("[3/4] Decoding SSTV audio...")
    decode_sstv_audio_multithread(AUDIO_DIR, DECODED_DIR, max_workers=32)

    print("[4/4] Creating final video...")
    create_video_from_frames(DECODED_DIR, OUTPUT_VIDEO, FPS)

    print("[✓] Done!")
