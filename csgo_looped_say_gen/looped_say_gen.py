import sys
from pathlib import Path

def main():
    if len(sys.argv) != 3:
        print("Usage: script.py <input.txt> <output.cfg>")
        sys.exit(1)

    input_file = Path(sys.argv[1])
    output_file = Path(sys.argv[2])

    if not input_file.exists():
        print(f"File not found: {input_file}")
        sys.exit(1)

    lines = [
        line.strip()
        for line in input_file.read_text(encoding="utf-8").splitlines()
        if line.strip()
    ]

    if not lines:
        output_file.write_text("// Input file is empty\n", encoding="utf-8")
        return

    key = "n"
    trigger_alias = "nx"
    alias_prefix = "sc"

    output = []
    output.append(f'bind "{key}" "{trigger_alias}"')
    output.append(f'alias "{trigger_alias}" "{alias_prefix}1"')

    count = len(lines)

    for i, text in enumerate(lines, start=1):
        next_alias = f"{alias_prefix}{i + 1}" if i < count else f"{alias_prefix}1"
        output.append(
            f'alias "{alias_prefix}{i}" "say {text}; alias {trigger_alias} {next_alias}"'
        )

    output_file.write_text("\n".join(output) + "\n", encoding="utf-8")
    print(f"Generated {count} aliases → {output_file}")

if __name__ == "__main__":
    main()
