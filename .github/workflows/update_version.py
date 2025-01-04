import re
import sys

def update_version(file_path, version_increment="0.0.0.1"):
    with open(file_path, 'r') as file:
        content = file.read()
    
    # Regex to find version number (x.x.x.x)
    version_pattern = re.compile(r'\[assembly: AssemblyVersion\("(\d+\.\d+\.\d+\.\d+)"\)\]')
    match = version_pattern.search(content)

    if match:
        current_version = match.group(1)
        major, minor, patch, revision = map(int, current_version.split('.'))
        
        # Add the increment to the version (x.x.x.x + 0.0.0.1)
        revision += 1
        new_version = f"{major}.{minor}.{patch}.{revision}"
        
        # Update the version in the file content
        updated_content = content.replace(current_version, new_version)

        # Write the updated content back to the file
        with open(file_path, 'w') as file:
            file.write(updated_content)

        return new_version
    else:
        print("Version not found in AssemblyInfo.cs")
        sys.exit(1)

if __name__ == "__main__":
    version = update_version("Properties\\AssemblyInfo.cs")
    print(version)
