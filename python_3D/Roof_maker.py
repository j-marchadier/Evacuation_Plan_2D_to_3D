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
    return sqrt(((p1[0] - p2[0]) ^ 2) + ((p1[1] - p2[1]) ^ 2))


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


def printall(d):
    for i in d:
        print(i, d[i])


def main():
    wall_size = 50

    filename = sys.argv[1]
    filename = arrange_name(filename, ".txt")

    array_in = np.genfromtxt(filename, delimiter=';')
    temp = np.reshape(array_in.flatten(), (-1, 2))
    x = [temp[i][0] for i in range(temp.shape[0])]
    z = [temp[i][1] for i in range(temp.shape[0])]

    xz = zip(x, z)
    all_points = [(a, b) for a, b in temp if abs(a-b) >= 10]
    x, z = zip(*all_points)

    # make_adjacency_matrix(array_in)
    # printall(list_of_points)
    # print(all_points)
    matrix = [[0 for i in range(int(max(z)+1))]
              for j in range(int(max(x)+1))]

    for i in range(len(all_points)):
        matrix[int(all_points[i][0])][int(all_points[i][1])] = 255

    for i in range(len(matrix)):
        str = ""
        for j in range(len(matrix[0])):
            str = str + "{} ".format(matrix[i][j])
        # print(str)

    fig, ax = plt.subplots()
    ax.matshow(matrix, cmap=plt.cm.Blues)
    plt.show()

    with open('exit.txt', 'w') as f:
        f.write('Create a new text file!')


if __name__ == "__main__":
    main()
