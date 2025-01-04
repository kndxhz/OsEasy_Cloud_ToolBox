import re
import os

def increment_version(file_path, version_regex):
    with open(file_path, 'r') as f:
        content = f.read()

    match = re.search(version_regex, content)
    if match:
        current_version = match.group(1)
        parts = current_version.split('.')
        parts[-1] = str(int(parts[-1]) + 1)
        new_version = '.'.join(parts)

        new_content = re.sub(version_regex, f'assembly: AssemblyVersion("{new_version}")\nassembly: AssemblyFileVersion("{new_version}")', content)

        with open(file_path, 'w') as f:
            f.write(new_content)
        return new_version

# Example usage:
file_path = "OsEasy_Cloud_ToolBox/Properties/AssemblyInfo.cs"
version_regex = r"assembly: AssemblyVersion\(\"(.*?)\"\)"  # Add double quotes around the pattern
new_version = increment_version(file_path, version_regex)
print(new_version)
