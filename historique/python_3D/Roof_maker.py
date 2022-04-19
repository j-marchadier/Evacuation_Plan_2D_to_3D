import math
import sys
from cv2 import sqrt
import numpy as np
import matplotlib.pyplot as plt


def arrange_name(filename, extension):
    idx = filename.rfind('.')
    if idx <= 0:
        return filename+extension

    else:
        return filename


def point_distance(p1, p2):
    calc = math.sqrt(math.pow((p1[0] - p2[0]), 2) +
                     math.pow((p1[1] - p2[1]), 2))
    return calc


list_of_points = {}


def make_adjacency_matrix(mat):
    key_counter = 0
    for line in mat:

        if not (line[0], line[1]) in list_of_points.values():
            list_of_points[key_counter] = (line[0], line[1])
            key_counter = key_counter + 1

        if not (line[2], line[3]) in list_of_points.values():
            list_of_points[key_counter] = (line[2], line[3])
            key_counter = key_counter + 1

    n = max(list_of_points.keys())
    mat_adj = [[0]*n] * n

    list_p_x = [i[0] for i in list(list_of_points.values())]
    list_p_z = [i[1] for i in list(list_of_points.values())]

    for i in range(n):
        point_actuel = list_of_points[i]
        for j in range(n):
            point_regarde = list_of_points[j]
            if point_actuel[0] == point_regarde[0] or point_actuel[0] == point_regarde[1] or point_actuel[1] == point_regarde[0] or point_actuel[1] == point_regarde[1]:
                # s'il y a des connexions entre ces points.
                mat_adj[i][j] = mat_adj[i][j] + 1

    return mat_adj


def printall(d):
    for i in d:
        print(i, d[i])


def make_triangles(mat_adj):
    triangles = []
    for i in range(len(mat_adj)):
        for j in range(i+1, len(mat_adj)):
            if (mat_adj[i][j] == 1):
                for k in range(j+1, len(mat_adj)):
                    if mat_adj[i][k] == 1 and mat_adj[j][k] == 1:

                        ptlist = list(
                            (list_of_points[i], list_of_points[j], list_of_points[k]))
                        mini = min(ptlist)
                        ptlist.remove(mini)
                        maxi = max(ptlist)
                        ptlist.remove(maxi)
                        midi = ptlist[0]
                        tup = (mini, midi, maxi)
                        if tup not in triangles:
                            triangles.append(tup)
                        mat_adj[j][k] = 0

    return triangles


def main():
    wall_size = 50

    filename = sys.argv[1]
    filename = arrange_name(filename, ".txt")

    array_in = np.genfromtxt(filename, delimiter=';')

    temp = np.reshape(array_in.flatten(), (-1, 2))
    x = [temp[i][0] for i in range(temp.shape[0])]
    z = [temp[i][1] for i in range(temp.shape[0])]

    for i in range(len(x)):
        for j in range(len(z)):
            if(point_distance((x[i], z[i]), (x[j], z[j])) <= 10):
                temp[j][0] = x[i]
                temp[j][1] = z[i]

    redone_mat = np.reshape(array_in.flatten(), (-1, 4))
    matadj = make_adjacency_matrix(redone_mat)
    triangles = make_triangles(matadj)
    print(triangles)

    with open('exit.txt', 'w') as f:
        f.write('Create a new text file!')


if __name__ == "__main__":
    main()
