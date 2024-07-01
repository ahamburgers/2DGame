import numpy as np

# 读取之前保存的.npy文件
loaded_color_labels = np.load('instance_labels/instance_label_79.npy')
loaded_color_labels2 = np.load('language_labels/language_labels_20.npy')
unique_numbers = set(np.unique(loaded_color_labels))
unique_numbers2 = set(np.unique(loaded_color_labels2))
# 打印不同数字的数量和它们本身
print("实例这些数字包括：", unique_numbers)
print("语义这些数字包括：", unique_numbers2)
# 现在，loaded_color_labels是一个字典，可以像普通的Python字典那样使用它
# 例如，打印字典
print(loaded_color_labels)
print(loaded_color_labels2)
