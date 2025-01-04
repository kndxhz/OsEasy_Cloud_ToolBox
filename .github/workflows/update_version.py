import xml.etree.ElementTree as ET

# 更新版本号函数
def update_version(file_path, version_increment):
    # 解析C#项目的.csproj文件
    tree = ET.parse(file_path)
    root = tree.getroot()

    # 获取并更新版本号
    for elem in root.iter():
        if 'Version' in elem.tag:  # 假设版本号在Version标签下
            current_version = elem.text.strip()
            version_parts = current_version.split('.')
            
            # 假设版本格式为x.x.x.x，增加最后一段
            version_parts[3] = str(int(version_parts[3]) + version_increment)
            new_version = '.'.join(version_parts)
            elem.text = new_version

    # 保存更新后的文件
    tree.write(file_path)

# 使用输入参数
if __name__ == "__main__":
    import sys
    csproj_path = sys.argv[1]
    update_version(csproj_path, 1)  # 增加版本号修订部分
