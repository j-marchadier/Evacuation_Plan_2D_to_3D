## si on a "point depart ligne / point arrivée ligne" -> creer mesh
import open3d as o3d
import numpy as np

array_in = np.array([[[0,0],[1,1]],
                     [[2,4],[1,1]],
                    [[0,0],[0,7]]])

ddarr = np.reshape(array_in.flatten(),(-1,2))

uniq, uniq_cnt = np.unique(ddarr, axis=0, return_counts=True)
uniq_arr = uniq[uniq_cnt==1]
cnt_mask = uniq_cnt > 1
dup_arr = uniq[cnt_mask]
vertexlist = np.concatenate((uniq_arr,dup_arr)) # liste des vertex
vertex_count = len(vertexlist) # nb de vertex
print(vertexlist)