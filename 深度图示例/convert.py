import OpenEXR
import Imath
import numpy as np
import imageio
import matplotlib.pyplot as plt
import open3d as o3d

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

# Use the functions
depth = load_exr_depth('depth/depth_0.exr')
rgb = load_png_image('image/image_0.png')
save_png(depth, 'output_depth_colormap.png', mode="colormap")
points=[]
pc,points = depth_to_point_cloud(rgb, depth)
np.save('point_cloud_coordinates.npy', np.array(points))
o3d.io.write_point_cloud("output_point_cloud.ply", pc)
