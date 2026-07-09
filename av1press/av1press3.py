import os
import shutil
import subprocess
from concurrent.futures import ThreadPoolExecutor

THREADS = 2

def unique_path(path):
    if not os.path.exists(path):
        return path

    folder, filename = os.path.split(path)
    base, ext = os.path.splitext(filename)

    counter = 1
    while True:
        candidate = os.path.join(folder, f"{base}_{counter}{ext}")
        if not os.path.exists(candidate):
            return candidate
        counter += 1

def convert_one(mp4_path, output_path, crf, delete_original, failed_folder):
    try:
        cmd = [
            "ffmpeg",
            "-y",
            "-i", mp4_path,
            "-c:v", "libsvtav1",
            "-crf", str(crf),
            "-preset", "6",
            "-c:a", "copy",
            output_path
        ]

        subprocess.run(cmd, check=True, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)

        print(f"Converted: {mp4_path} → {output_path}")

        if delete_original:
            os.remove(mp4_path)
            print(f"Deleted: {mp4_path}")

    except Exception as e:
        print(f"Failed: {mp4_path} | {e}")

        os.makedirs(failed_folder, exist_ok=True)

        failed_path = unique_path(os.path.join(failed_folder, os.path.basename(mp4_path)))
        shutil.move(mp4_path, failed_path)

        print(f"Moved failed file to: {failed_path}")

def collect_jobs(folder):
    jobs = []
    failed_folder = os.path.join(folder, "_failed")

    for root, dirs, files in os.walk(folder):
        dirs[:] = [d for d in dirs if os.path.join(root, d) != failed_folder]

        for file in files:
            if file.lower().endswith(".mp4"):
                if file.lower().endswith("_av1.mp4"):
                    continue

                mp4_path = os.path.join(root, file)

                base = os.path.splitext(file)[0]
                output_path = os.path.join(root, base + "_AV1.mp4")

                jobs.append((mp4_path, output_path))

    return jobs

def convert_mp4_to_av1(folder, crf=32, delete_original=True):
    failed_folder = os.path.join(folder, "_failed")
    jobs = collect_jobs(folder)

    print(f"Found {len(jobs)} MP4 files")

    with ThreadPoolExecutor(max_workers=THREADS) as executor:
        for mp4_path, output_path in jobs:
            executor.submit(
                convert_one,
                mp4_path,
                output_path,
                crf,
                delete_original,
                failed_folder
            )

if __name__ == "__main__":
    convert_mp4_to_av1(
        os.getcwd(),
        crf=30,
        delete_original=True
    )