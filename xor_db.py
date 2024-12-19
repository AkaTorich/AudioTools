import sys

def xor_file(input_path, output_path, key=0xAA):
    with open(input_path, 'rb') as f_in:
        data = f_in.read()

    encrypted = bytes([b ^ key for b in data])

    with open(output_path, 'wb') as f_out:
        f_out.write(encrypted)

if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("Usage: python xor_encrypt.py <input_file> <output_file> [key]")
        sys.exit(1)

    input_file = sys.argv[1]
    output_file = sys.argv[2]
    key = 0xAA
    if len(sys.argv) > 3:
        key = int(sys.argv[3], 0)

    xor_file(input_file, output_file, key)
