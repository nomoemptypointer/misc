#!/bin/bash

# Backs up raw Android partitions over adb while in TWRP/OFOX.
# Can optionally compress output with xz.
# I made this script because my Mi 9T Pro has a lot of data and I can't backup anything in OFOX recovery because it's too large and the Mi 9T Pro doesn't have any MicroSD card slot.

# ----------------------------
# Config
# ----------------------------

# List of partitions to back up (mount points OR block names)
PARTITIONS=(
    "/data"
    "/system"
    "/boot"
    "/vendor"
)

BLOCKSIZE=4 # This refers to the amount of data read and written at a time, it must be the same between dumping and restoring.

COMPRESS=1 # 0 = no compression, 1 = compress with xz
XZ_LEVEL=6 # compression level (0–9, default 6)

# ----------------------------
# Globals
# ----------------------------

DATETIME=$(date +"%Y-%m-%d_%H-%M-%S")
COMPRESS_CMD="xz -T0 -z -${XZ_LEVEL}"

# ----------------------------
# Functions
# ----------------------------

get_block_device() {
    local mount_point="$1"
    # Try to detect block device from /proc/mounts
    local dev
    dev=$(adb shell "grep ' $mount_point ' /proc/mounts | awk '{print \$1}'" | tr -d '\r')

    # If nothing found, maybe user passed a block name (e.g. /dev/block/by-name/boot)
    if [ -z "$dev" ]; then
        if [[ "$mount_point" == /dev/* ]]; then
            dev="$mount_point"
        else
            dev="/dev/block/bootdevice/by-name/${mount_point#/}" # strip leading /
        fi
    fi
    echo "$dev"
}

backup_partition() {
    local part="$1"
    local dev
    dev=$(get_block_device "$part")

    if [ -z "$dev" ]; then
        echo "[e] Could not determine block device for $part"
        return 1
    fi

    local out="backup_${DATETIME}_${part#/}_bs${BLOCKSIZE}M.img"
    out=${out//\//_} # replace slashes with underscores

    if [ "$COMPRESS" -eq 1 ]; then
        # use COMPRESS_EXT (e.g. ".xz") or a literal ".xz"
        out="${out}${COMPRESS_EXT}"
        echo "[i] Dumping $part ($dev) -> $out (compressed)"
        adb exec-out "dd if=$dev bs=${BLOCKSIZE}M" | $COMPRESS_CMD > "$out"
    else
        echo "[i] Dumping $part ($dev) -> $out"
        adb exec-out "dd if=$dev bs=${BLOCKSIZE}M" > "$out"
    fi

    echo " [i] Finished $part"
}


# ----------------------------
# Main
# ----------------------------

echo "================== recbb =================="
echo "Partitions to back up: ${PARTITIONS[*]}"
[ "$COMPRESS" -eq 1 ] && echo "Compression: enabled" || echo "Compression: disabled"
echo "==========================================="

for p in "${PARTITIONS[@]}"; do
    backup_partition "$p"
done

echo "🎉 All done!"
