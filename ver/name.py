import os

def rename_files(folder_path):
    # 获取目标文件夹中的所有文件
    file_list = os.listdir(folder_path)

    # 逐个文件进行重命名
    for index, file_name in enumerate(file_list):
        # 构建新的文件名
        new_file_name = f"cucumber{index}.pcd"
        new_path = os.path.join(folder_path, new_file_name)

        # 获取旧文件路径
        old_path = os.path.join(folder_path, file_name)

        # 重命名文件
        os.rename(old_path, new_path)
        print(f"重命名文件: {old_path} 为 {new_path}")

# 获取当前脚本所在的文件夹路径
script_folder = os.path.dirname(os.path.realpath(__file__))

# 指定目标文件夹路径
target_folder = os.path.join(script_folder, "..", "..", "all", "new")

# 调用函数进行文件重命名
rename_files(target_folder)
