import open3d as o3d
import numpy as np

def display_point_cloud(ply_path,sampling_fraction=1.0):
    # 读取点云数据
    pcd = o3d.io.read_point_cloud(ply_path)
    
    # 调整Z轴坐标
    points = np.asarray(pcd.points)
    colors = np.asarray(pcd.colors)
    
    # 如果需要进行采样
    if sampling_fraction < 1.0:
        num_points = len(points)
        sampled_indices = np.random.choice(num_points, int(num_points * sampling_fraction), replace=False)
        points = points[sampled_indices]
        colors = colors[sampled_indices]
    
    sampled_pcd = o3d.geometry.PointCloud()
    sampled_pcd.points = o3d.utility.Vector3dVector(points)
    sampled_pcd.colors = o3d.utility.Vector3dVector(colors)

    # 显示点云
    o3d.visualization.draw_geometries([sampled_pcd])

# 使用函数展示点云，Z轴坐标乘以10，并进行稀疏采样
display_point_cloud("model/output_point_cloud.ply", sampling_fraction=1/20)
