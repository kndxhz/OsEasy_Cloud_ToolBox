import json

# 读取当前版本号
with open('version.json', 'r') as file:
    version_data = json.load(file)

# 获取当前版本号和修订号
major, minor, patch, revision = map(int, version_data['version'].split('.'))

# 更新修订号
revision += 1
new_version = f"{major}.{minor}.{patch}.{revision}"

# 保存更新后的版本号
version_data['version'] = new_version
with open('version.json', 'w') as file:
    json.dump(version_data, file, indent=4)

print(f"Updated version to {new_version}")
