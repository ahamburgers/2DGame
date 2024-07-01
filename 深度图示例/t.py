import numpy as np

# 读取之前保存的.npy文件
loaded_color_labels = np.load('color_labels.npy')
unique_numbers = set(np.unique(loaded_color_labels))
xyz=np.load('point_cloud_coordinates.npy')
# 打印不同数字的数量和它们本身
#print(f"该文件中总共有 {len(unique_numbers)} 种不同的数字。")
#print("这些数字包括：", unique_numbers)
# 现在，loaded_color_labels是一个字典，可以像普通的Python字典那样使用它
# 例如，打印字典
#print(loaded_color_labels)
print(xyz)
