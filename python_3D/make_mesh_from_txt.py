## si on a "point depart ligne / point arrivée ligne" -> creer mesh
import open3d as o3d
import numpy as np
import pymeshlab
import sys

def arrange_name(filename,extension):
    idx = filename.rfind('.')
    if idx <= 0:
        return filename+extension

    else:
        return filename

def dist_calc(x,y):
    return np.sqrt(np.power(x,2) + np.power(y,2))

def dist_a_zero(x):
    return dist_calc(x,0)

def dist_p_calc(p1,p2):
    return np.sqrt(np.power(p1[0] - p2[0],2) + np.power(p1[1] - p2[1],2))

def dist_a_p_zero(p1):
    return dist_p_calc(p1,[0,0,0])


def main():
    wall_size = 50

    filename = sys.argv[1]
    filename = arrange_name(filename,".txt")

    array_in = np.genfromtxt(filename, delimiter=';')
    #array_in = np.reshape(array_in.flatten(),(len(array_in),2,2))


    ddarr = np.reshape(array_in.flatten(),(-1,2))
    array_coords = np.insert(ddarr,1,0,axis=1) # mettre la valeur y au centre, à 0


    new_array_coords = np.copy(array_coords)
    new_array_coords[:,1] = wall_size # liste des points copiée pour le haut du mur, la valeur y au centre à 2
    all_points = np.concatenate((array_coords,new_array_coords),axis=0) # liste complete des points dans l'ordre bas->haut des murs
    #print(all_points)


    nb_lignes = len(array_in) # nb de lignes a tracer
    nb_tri = 4 #nb de triangles a faire pour chaque ligne, soit 4.


    #mesh = o3d.geometry.TriangleMesh()
    liste_triangles = [[0]*3 for i in range(nb_lignes*nb_tri + 8)] # le +8 pour sol et plafond
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



    #le sol et plafond
    liste_all_x = [p[0] for p in array_coords] #liste des x
    liste_all_z = [p[2] for p in array_coords] #liste des z


    min_index_x = liste_all_x.index(min(liste_all_x)) # index of smallest x value
    max_index_x = liste_all_x.index(max(liste_all_x)) # index of highest x value
    min_index_z = liste_all_z.index(min(liste_all_z)) # index of smallest z value
    max_index_z = liste_all_z.index(max(liste_all_z)) # index of highest z value

    min_x = array_coords[min_index_x][0]
    max_x = array_coords[max_index_x][0]
    min_z = array_coords[min_index_z][2]
    max_z = array_coords[max_index_z][2] # corresponding values


    liste_floor = []
    p_min_x_min_z = np.array([min_x, 0, min_z])
    p_min_x_max_z = np.array([min_x, 0, max_z])
    p_max_x_min_z = np.array([max_x, 0, min_z])
    p_max_x_max_z = np.array([max_x, 0, max_z])

    p_min_x_min_z_up = np.copy(p_min_x_min_z)
    p_min_x_min_z_up[1] = wall_size
    p_min_x_max_z_up = np.copy(p_min_x_max_z)
    p_min_x_max_z_up[1] = wall_size
    p_max_x_min_z_up = np.copy(p_max_x_min_z)
    p_max_x_min_z_up[1] = wall_size
    p_max_x_max_z_up = np.copy(p_max_x_max_z)
    p_max_x_max_z_up[1] = wall_size # crée les points d'interet

    liste_floor.append(p_min_x_min_z)
    liste_floor.append(p_min_x_max_z)
    liste_floor.append(p_max_x_min_z)
    liste_floor.append(p_max_x_max_z)
    liste_floor.append(p_min_x_min_z_up)
    liste_floor.append(p_min_x_max_z_up)
    liste_floor.append(p_max_x_min_z_up)
    liste_floor.append(p_max_x_max_z_up)

    for p in liste_floor:
        p = np.reshape(p,(1,3))
        all_points=np.concatenate((all_points,p),axis=0) # ajoute les points a la liste des points


    for i in range(2):
        dec = 2-i # donc 2 puis 1
        mxmz = len(all_points) - dec * 4
        mxMz = mxmz + 1
        Mxmz = mxMz + 1
        MxMz = Mxmz + 1 #index des points dans l'ordre

        triangle0 = [mxmz, Mxmz, MxMz]
        triangle1 = [MxMz, Mxmz, mxmz]
        triangle2 = [MxMz, mxMz, mxmz]
        triangle3 = [mxmz, mxMz, MxMz] # tracer les 4 triangles

        id_start = len(liste_triangles) - dec*4
        liste_triangles[0 + id_start] = triangle0
        liste_triangles[1 + id_start] = triangle1
        liste_triangles[2 + id_start] = triangle2
        liste_triangles[3 + id_start] = triangle3 # stoker les 4 triangles

    liste_triangles = np.array(liste_triangles) # mettre les triangles en np array

    m = pymeshlab.Mesh(all_points, liste_triangles) # cree la mesh
    ms = pymeshlab.MeshSet() # cree une meshliste pour contenir le mesh
    ms.add_mesh(m, mesh_name = 'mesh1', set_as_current = True) # ajoute mesh a la liste




    outname = "mesh_with_colorv2.obj"
    if len(sys.argv) > 2:
        outname = sys.argv[2]

    outname = arrange_name(outname,".obj")
    ms.save_current_mesh(outname) # enregistre


main()