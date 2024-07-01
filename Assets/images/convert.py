import OpenEXR
import Imath
import numpy as np
import imageio
import matplotlib.pyplot as plt
import open3d as o3d
from collections import defaultdict
def load_exr_depth(path):
    # Load the EXR file
    exr_file = OpenEXR.InputFile(path)

    # Get the size of the image
    dw = exr_file.header()['dataWindow']
    width, height = dw.max.x - dw.min.x + 1, dw.max.y - dw.min.y + 1

    # Read the depth channel
    FLOAT = Imath.PixelType(Imath.PixelType.FLOAT)
    depthstr = exr_file.channel('R', FLOAT)

    # Convert the string to a numpy array
    depth = np.frombuffer(depthstr, dtype=np.float32)

    # Reshape the array
    depth.shape = (height, width)
    return depth

def load_png_image(path):
    return imageio.imread(path).astype(np.float32) / 255.0

def pseudocolor(val, minval, maxval):
    """ Convert val in range minval..maxval to the range 0..120 degrees which
    correspond to red..blue in the HSV colorspace. """
    h = (float(val-minval) / (maxval-minval)) * 120
    return plt.cm.jet(h / 120)[:-1]

def create_colormap_image(depth):
    min_depth = np.min(depth)
    max_depth = np.max(depth)

    colormap_img = np.zeros((depth.shape[0], depth.shape[1], 3), dtype=np.float32)
    for i in range(depth.shape[0]):
        for j in range(depth.shape[1]):
            colormap_img[i, j] = pseudocolor(depth[i, j], min_depth, max_depth)
    return colormap_img

def save_png(img, path, mode="normal"):
    if mode == "colormap":
        # Convert depth data to colormap image
        img = create_colormap_image(img)
    img_8bit = (img * 255).astype(np.uint8)
    imageio.imsave(path, img_8bit)
def classify_colors(colors):
    color_classes = {}
    for idx, color in enumerate(colors):
        color_tuple = tuple(color)
        if color_tuple in color_classes:
            color_classes[color_tuple].append(idx)
        else:
            color_classes[color_tuple] = [idx]
    return color_classes
def depth_to_point_cloud(rgb, depth):
    points = []
    colors = []

    for i in range(depth.shape[0]):
        for j in range(depth.shape[1]):
            z = depth[i, j] * 255
            points.append([j, i, z])
            colors.append(rgb[i, j])

    pc = o3d.geometry.PointCloud()
    pc.points = o3d.utility.Vector3dVector(np.array(points))
    pc.colors = o3d.utility.Vector3dVector(np.array(colors))

    return pc,points


#intance

def are_colors_close(color1, color2, tolerance=1e-6):
    """检查两个颜色是否足够接近"""
    return all(abs(a - b) < tolerance for a, b in zip(color1, color2))

def instance_point_cloud(instance,depth):
    points = []
    colors = []
    for i in range(depth.shape[0]):
        for j in range(depth.shape[1]):
            z = depth[i, j] * 255
            points.append([j, i, z])
            colors.append(instance[i, j])

    color_categories = defaultdict(list)
    for idx, color in enumerate(colors):
        color_tuple = tuple(color)
        color_categories[color_tuple].append(idx)

    # 特定颜色，如果匹配此颜色，将其标记为-1
    special_color = (0.40784314, 0.38039216, 0.35686275)

    # 对颜色类别进行编号
    color_labels = {}
    label_counter = 1
    for color in color_categories.keys():
        if are_colors_close(color, special_color):
            color_labels[color] = -1
        else:
            color_labels[color] = label_counter
            label_counter += 1

    # 为每个点分配颜色标签
    color_indices = [color_labels[tuple(color)] for color in colors]

    language_pc = o3d.geometry.PointCloud()
    language_pc.points = o3d.utility.Vector3dVector(np.array(points))
    language_pc.colors = o3d.utility.Vector3dVector(np.array(colors))
    return language_pc, color_indices


def language_point_cloud(language,depth):
    points = []
    colors = []
    for i in range(depth.shape[0]):
        for j in range(depth.shape[1]):
            z = depth[i, j] * 255
            points.append([j, i, z])
            colors.append(language[i, j])

    color_categories = defaultdict(list)
    for idx, color in enumerate(colors):
        color_tuple = tuple(color)
        color_categories[color_tuple].append(idx)

    # 特定颜色，如果匹配此颜色，将其标记为-1
    special_color = (0.40784314, 0.38039216, 0.35686275)

    # 对颜色类别进行编号
    color_labels = {}
    label_counter = 10
    for color in color_categories.keys():
        if are_colors_close(color, special_color):
            color_labels[color] = 0
        else:
            color_labels[color] = label_counter
            label_counter += 1

    # 为每个点分配颜色标签
    color_indices = [color_labels[tuple(color)] for color in colors]

    language_pc = o3d.geometry.PointCloud()
    language_pc.points = o3d.utility.Vector3dVector(np.array(points))
    language_pc.colors = o3d.utility.Vector3dVector(np.array(colors))

    return language_pc, color_indices



# 假设你想处理的文件数量
number_of_files = 100  # 你可以根据需要调整这个数字

for i in range(number_of_files):
    depth_file = f'depth/depth_{i}.exr'
    output_file = f'model/color_map_{i}.png'
    rgb = load_png_image(f'image/image_{i}.png')
    language_image = load_png_image(f'language/language_label_{i}.png')
    instance_image = load_png_image(f"instance/instance_label_{i}.png")
    # 加载深度信息
    depth = load_exr_depth(depth_file)

    # 生成并保存彩色深度图
    save_png(depth, output_file, mode="colormap")
    language_label={}
    instance_label = {}
    # 如果需要，也可以为每个文件生成点云
    points = []
    pc, points = depth_to_point_cloud(rgb, depth)  # 确保rgb是正确的
    instance_pc,instance_label =instance_point_cloud(instance_image, depth)
    language_pc,language_label=language_point_cloud(language_image,depth)

    np.save(f'xyz/point_cloud_coordinates_{i}.npy', np.array(points))
    np.save(f'instance_labels/instance_label_{i}.npy', instance_label)
    np.save(f'language_labels/language_labels_{i}.npy', language_label)
    o3d.io.write_point_cloud(f'model/output_point_cloud_{i}.ply', pc)
    o3d.io.write_point_cloud(f'instance_model/instance_point_cloud_{i}.ply', instance_pc)
    o3d.io.write_point_cloud(f'language_model/language_point_cloud_{i}.ply', language_pc)




# Use the functions
#depth = load_exr_depth('depth/depth_0.exr')
#rgb = load_png_image('image/image_0.png')
#language_label=load_png_image('language/language_label_0.png')
#instance_label=load_png_image("instance/instance_label_0.png")
#save_png(depth, 'model/output_depth_colormap.png', mode="colormap")
#pc = depth_to_point_cloud(rgb, depth)
#instance_pc=instance_pc(instance_label,depth)
#language_pc=language_pc(language_label,depth)
#o3d.io.write_point_cloud("model/output_point_cloud.ply", pc)
#o3d.io.write_point_cloud("model/instance_point_cloud.ply", pc)
#o3d.io.write_point_cloud("model/language_point_cloud.ply", pc)
