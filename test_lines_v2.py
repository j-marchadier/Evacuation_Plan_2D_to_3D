## si on a "point depart ligne / point arrivée ligne" -> creer mesh
import open3d as o3d
import numpy as np
from sqlalchemy import all_

array_in = np.array([[[0,0],
                      [0,1]],
                     [[0,1],
                      [1,1]],
                     [[0,0],
                      [1,0]],])

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


nb_lignes = len(coords_start_end)
nb_tri = 4

#liste_mesh = [] # stocker les mesh
mesh = o3d.geometry.TriangleMesh()
liste_triangles = [[0]*3 for i in range(nb_lignes*nb_tri)]
print(liste_triangles)

for i in range(nb_lignes):
    #mesh = o3d.geometry.TriangleMesh()
    
    #np_triangles = [[0]*3 for j in range(2)]
    
    #triangles[0] = [i, i + len(array_in)*2, i + len(array_in)*2 + 1]
    #np_triangles[1] = [i + len(array_in)*2 + 1, i + len(array_in)*2, i]
    #np_triangles[2] = [i, i + len(array_in)*2 + 1, i + 1]
    #np_triangles[3] = [i + 1, i + len(array_in)*2 + 1, i]
    
    triangle0 = [i, i + nb_lignes + 1, i + nb_lignes + 2]
    triangle1 = [i + nb_lignes + 2, i + nb_lignes + 1, i]
    triangle2 = [i, i + nb_lignes + 2, i + 1]
    triangle3 = [i + 1, i + nb_lignes + 2, i]
    
    liste_triangles[0 + i*nb_tri] = triangle0
    liste_triangles[1 + i*nb_tri] = triangle1
    liste_triangles[2 + i*nb_tri] = triangle2
    liste_triangles[3 + i*nb_tri] = triangle3
    
    #liste_mesh.append(mesh)

#print(np_triangles)
#print(all_points[2])
#print(all_points[8])
#print(all_points[9])
#print(all_points[2])
#print(all_points[9])
#print(all_points[3])
liste_triangles = np.array(liste_triangles)

print(all_points)
print(liste_triangles)
print(nb_lignes)
mesh.vertices = o3d.utility.Vector3dVector(all_points)
mesh.triangles = o3d.utility.Vector3iVector(liste_triangles)
o3d.visualization.draw_geometries([mesh])