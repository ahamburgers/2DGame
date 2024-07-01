import os
import random
import shutil

def shuffle_files(source_folder, destination_folder):
    # 创建目标文件夹（如果不存在）
    if not os.path.exists(destination_folder):
        os.makedirs(destination_folder)

    # 获取源文件夹中的所有文件
    file_list = os.listdir(source_folder)

    # 打乱文件列表顺序
    random.shuffle(file_list)

    # 将打乱后的文件复制到目标文件夹
    for index, file_name in enumerate(file_list):
        source_folder = os.path.abspath(os.path.join(script_folder, "..", "..", "all"))

        destination_path = os.path.join(destination_folder, file_name)
        shutil.copy2(source_folder, destination_path)
        print(f"复制文件: {source_folder} 到 {destination_path}")

# 获取当前脚本所在的文件夹路径
script_folder = os.path.dirname(os.path.realpath(__file__))

# 指定源文件夹和目标文件夹路径
source_folder = os.path.join(script_folder, "..", "..", "all")
destination_folder = os.path.join(source_folder, "new")

# 调用函数进行文件打乱和复制
shuffle_files(source_folder, destination_folder)
