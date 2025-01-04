import re
import chardet

def increment_version(file_path, version_regex):
    """
    Increments the version number in the specified file.

    Args:
        file_path: The path to the file containing the version number.
        version_regex: The regular expression pattern to match the version number.

    Returns:
        The new version number.
    """

    # Open the file in binary mode to handle various encodings
    with open(file_path, 'rb') as f:
        rawdata = f.read()

    # Detect the encoding using chardet
    result = chardet.detect(rawdata)
    encoding = result['encoding']

    # Decode the content using the detected encoding
    content = rawdata.decode(encoding, errors='replace')  # Handle decoding errors gracefully

    # Find and replace the version number
    match = re.search(version_regex, content)
    if match:
        current_version = match.group(1)
        parts = current_version.split('.')
        parts[-1] = str(int(parts[-1]) + 1)
        new_version = '.'.join(parts)

        new_content = re.sub(version_regex, f'assembly: AssemblyVersion("{new_version}")\nassembly: AssemblyFileVersion("{new_version}")', content)

        # Write the modified content back to the file
        with open(file_path, 'w', encoding=encoding) as f:
            f.write(new_content)
        return new_version
    else:
        print(f"Version number not found in {file_path}")
        return None

# Example usage:
file_path = "OsEasy_Cloud_ToolBox/Properties/AssemblyInfo.cs"
version_regex = r"assembly: AssemblyVersion\(\"(.*?)\"\)"
new_version = increment_version(file_path, version_regex)
if new_version:
    print("New version:", new_version)
