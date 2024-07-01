import numpy as np

loaded_color_labels2 = np.load('instance_labels/instance_label_47.npy', allow_pickle=True)
loaded_color_labels = np.load('language_labels/language_labels_99.npy', allow_pickle=True)
print(loaded_color_labels2)
print(loaded_color_labels)