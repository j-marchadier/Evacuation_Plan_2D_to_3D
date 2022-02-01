## si on a "point depart ligne / point arrivée ligne" -> creer mesh
import open3d as o3d
import numpy as np
import pymeshlab

array_in = np.array([[[0,0],[0,1]],
                     [[0,1],[1,1]],
                     [[0,0],[1,0]],]) # array d'entree type [[x1, z1][x2, z2]]

array_2 = np.insert(array_in,1,0,axis=1) 
ddarr = np.reshape(array_in.flatten(),(-1,2))
array_coords = np.insert(ddarr,1,0,axis=1) # mettre la valeur y au centre, à 0


new_array_coords = np.copy(array_coords)
new_array_coords[:,1] = 2 # liste des points copiée pour le haut du mur, la valeur y au centre à 2
all_points = np.concatenate((array_coords,new_array_coords),axis=0) # liste complete des points dans l'ordre bas->haut des murs
#print(all_points)


''' coords_start_end = np.reshape(array_coords, (-1,2,3)) # array_in mais avec un y, bien réorganisé
print(coords_start_end)

ddarr = np.reshape(coords_start_end.flatten(),(-1,3))
uniq, uniq_cnt = np.unique(ddarr, axis=0, return_counts=True)
uniq_arr = uniq[uniq_cnt==1]
cnt_mask = uniq_cnt > 1
dup_arr = uniq[cnt_mask]
vertexlist = np.concatenate((uniq_arr,dup_arr)) # liste des vertex dans ce qui est donné, mode [x,z] (points uniques)
vertexlist2 = np.copy(vertexlist) # pour les valeurs en haut du mur
vertexlist2[:,1] = 2 # donne à y la valeur 2

vertex_count = len(vertexlist)*2 # nb de vertex 
all_vertex = np.concatenate((vertexlist,vertexlist2),axis=0) # liste complete des vertex
#print(all_vertex) '''


nb_lignes = len(array_in) # nb de lignes a tracer
nb_tri = 4 #nb de triangles a faire pour chaque ligne, soit 4.


mesh = o3d.geometry.TriangleMesh()
liste_triangles = [[0]*3 for i in range(nb_lignes*nb_tri)]
# pour stocker tous les triangles

for i in range(0, nb_lignes*2, 2):
    
    triangle0 = [i, i + nb_lignes * 2, i + nb_lignes * 2 + 1]
    triangle1 = [i + nb_lignes * 2 + 1, i + nb_lignes * 2, i]
    triangle2 = [i, i + nb_lignes * 2 + 1, i + 1]
    triangle3 = [i + 1, i + nb_lignes * 2 + 1, i] # tracer les 4 triangles
    
    liste_triangles[0 + (i//2)*nb_tri] = triangle0
    liste_triangles[1 + (i//2)*nb_tri] = triangle1
    liste_triangles[2 + (i//2)*nb_tri] = triangle2
    liste_triangles[3 + (i//2)*nb_tri] = triangle3 # stoker les 4 triangles



liste_triangles = np.array(liste_triangles) # mettre les triangles en np array

m = pymeshlab.Mesh(all_points, liste_triangles) # cree la mesh
ms = pymeshlab.MeshSet() # cree une meshliste pour contenir le mesh
ms.add_mesh(m, mesh_name = '', set_as_current = True) # ajoute mesh a la liste

ms.save_current_mesh('mesh_without_color.obj', save_face_color=False) # enregistre