## si on a "point depart ligne / point arrivée ligne" -> creer mesh
import open3d as o3d
import numpy as np
from sqlalchemy import all_

array_in = np.array([[[0,0],
                      [1,1]],
                     [[2,4],
                      [1,1]],
                    [[2,2],
                     [0,1]]])

array_2 = np.insert(array_in,1,0,axis=1)
#print(array_2)

ddarr = np.reshape(array_in.flatten(),(-1,2))
array_coords = np.insert(ddarr,1,0,axis=1)
#print("array_coords : ")
#print(array_coords)
#print(array_coords.shape)
new_array_coords = np.copy(array_coords)
new_array_coords[:,1] = 2
all_points = np.concatenate((array_coords,new_array_coords),axis=0) # liste complete des points dans l'ordre bas/haut
#print(all_points)

coords_start_end = np.reshape(array_coords, (-1,2,3)) # array_in mais avec un y
#print("coords_start_end  : ")
#print(coords_start_end)
#print(coords_start_end.shape)

ddarr = np.reshape(coords_start_end.flatten(),(-1,3))

uniq, uniq_cnt = np.unique(ddarr, axis=0, return_counts=True)
uniq_arr = uniq[uniq_cnt==1]
cnt_mask = uniq_cnt > 1
dup_arr = uniq[cnt_mask]
vertexlist = np.concatenate((uniq_arr,dup_arr)) # liste des vertex dans ce qui est donné, mode [x,z]
vertexlist2 = np.copy(vertexlist)
vertexlist2[:,1] = 2

vertex_count = len(vertexlist)*2 # nb de vertex 
all_vertex = np.concatenate((vertexlist,vertexlist2),axis=0) # liste complete des vertex
#print(all_vertex)



liste_mesh = [] # stocker les mesh

for i in range(len(coords_start_end)):
    mesh = o3d.geometry.TriangleMesh()
    
    np_triangles = [[0]*3 for i in range(2)]
    
    np_triangles[0] = [i, i + len(array_in)*2, i + len(array_in)*2 + 1]
    np_triangles[1] = [i, i + len(array_in)*2 + 1, i + 1]
    
    mesh.vertices = o3d.utility.Vector3dVector(all_points)
    mesh.triangles = o3d.utility.Vector3iVector(np_triangles)
    liste_mesh.append(mesh)

#print(np_triangles)
#print(all_points[2])
#print(all_points[8])
#print(all_points[9])
#print(all_points[2])
#print(all_points[9])
#print(all_points[3])
    

o3d.visualization.draw_geometries(liste_mesh)
    