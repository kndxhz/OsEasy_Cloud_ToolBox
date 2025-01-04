import re

def update_version(assembly_info_path):
    with open(assembly_info_path, 'r') as file:
        content = file.read()

    # 正则匹配版本号和修订号
    version_pattern = r'(\[assembly: AssemblyVersion\(")(\d+\.\d+\.\d+\.\d+)("\)\])'
    file_version_pattern = r'(\[assembly: AssemblyFileVersion\(")(\d+\.\d+\.\d+\.\d+)("\)\])'

    # 提取当前版本号
    version_match = re.search(version_pattern, content)
    file_version_match = re.search(file_version_pattern, content)

    if version_match and file_version_match:
        version = version_match.group(2)
        file_version = file_version_match.group(2)

        # 增加版本号和修订号
        version_parts = version.split('.')
        version_parts[-1] = str(int(version_parts[-1]) + 1)  # 更新修订号
        new_version = '.'.join(version_parts)

        file_version_parts = file_version.split('.')
        file_version_parts[-1] = str(int(file_version_parts[-1]) + 1)  # 更新修订号
        new_file_version = '.'.join(file_version_parts)

        # 替换文件中的版本号和修订号
        updated_content = content.replace(version, new_version).replace(file_version, new_file_version)

        with open(assembly_info_path, 'w') as file:
            file.write(updated_content)

        return new_version  # 返回新的版本号
    else:
        raise ValueError("Unable to find version in the AssemblyInfo.cs file.")

# 修改版本号的路径
update_version(r'D:\a\OsEasy_Cloud_ToolBox\OsEasy_Cloud_ToolBox\OsEasy_Cloud_ToolBox\OsEasy_Cloud_ToolBox\Properties\AssemblyInfo.cs')
