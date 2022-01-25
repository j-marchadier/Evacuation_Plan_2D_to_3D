## si on a matrice 0 et 1 (0 mur) -> creer mesh
from stringprep import in_table_a1
import open3d as o3d
import numpy as np

print("Testing IO for meshes ...")
in_array = np.array([[0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                    [0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0],
                    [0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0],
                    [0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0],
                    [0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0],
                    [0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0],
                    [0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0],
                    [0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0],
                    [0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0],
                    [0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                    [0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0],
                    [0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0],
                    [0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0],
                    [0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 1, 0],
                    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]])
in_array = np.array(~in_array)

in_array = np.genfromtxt('myfile.csv',delimiter=',').astype(int)

coordlist = np.where(in_array < 1)
xlist = np.array(coordlist[0]).astype(int)
zlist = np.array(coordlist[1]).astype(int)


vertexlist = np.zeros(shape=(len(xlist)*2,3))
for pos in range(0,len(xlist)*2,2):
    coordsdown = np.array([xlist[pos//2],0,zlist[pos//2]]).astype(int)
    coordsup = np.array([xlist[pos//2],2,zlist[pos//2]]).astype(int)
    vertexlist[pos] = coordsdown
    vertexlist[pos+1] = coordsup
print(vertexlist)
# toutes les vertex

