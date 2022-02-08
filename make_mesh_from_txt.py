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


    mesh = o3d.geometry.TriangleMesh()
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
    liste_dist_points = [] #liste des distances à (0,0,0)
    for p in array_coords:
        liste_dist_points.append(dist_a_p_zero(p))

    min_index = liste_dist_points.index(min(liste_dist_points)) # index of smallest value
    max_index = liste_dist_points.index(max(liste_dist_points)) # index of highest value
    
    print(array_coords)
    liste_floor = []
    p_start = np.array(array_coords[min_index])
    p_end = np.array(array_coords[max_index])
    
    p_max_x = np.array([p_end[0],0,p_start[2]])
    p_max_y = np.array([p_start[0],0,p_end[2]])
    
    
    print(p_start)
    print(p_end)
    
    p_start_up = np.copy(p_start)
    p_start_up[1] = wall_size
    p_end_up = np.copy(p_end)
    p_end_up[1] = wall_size
    p_max_x_up = np.copy(p_max_x)
    p_max_x_up[1] = wall_size
    p_max_y_up = np.copy(p_max_y)
    p_max_y_up[1] = wall_size # releve / cree les points d'interet
    
    liste_floor.append(p_start)
    liste_floor.append(p_end)
    liste_floor.append(p_max_x)
    liste_floor.append(p_max_y)
    liste_floor.append(p_start_up)
    liste_floor.append(p_end_up)
    liste_floor.append(p_max_x_up)
    liste_floor.append(p_max_y_up)
    
    for p in liste_floor:
        p = np.reshape(p,(1,3))
        all_points=np.concatenate((all_points,p),axis=0) # ajoute les points a la liste des points


    for i in range(2):
        dec = 2-i # donc 2 puis 1
        start_p = len(all_points) - dec * 4
        end_p = start_p + 1
        max_x_p = end_p + 1
        max_y_p = max_x_p + 1 #index des points dans l'ordre
        print(all_points[start_p])
        print(all_points[end_p])
        print(all_points[max_x_p])
        print(all_points[max_y_p])
        
        triangle0 = [start_p, max_x_p, end_p]
        triangle1 = [end_p, max_x_p, start_p]
        triangle2 = [end_p, max_y_p, start_p]
        triangle3 = [start_p, max_y_p, end_p] # tracer les 4 triangles
        
        id_start = len(liste_triangles) - dec*4
        liste_triangles[0 + id_start] = triangle0
        liste_triangles[1 + id_start] = triangle1
        liste_triangles[2 + id_start] = triangle2
        liste_triangles[3 + id_start] = triangle3 # stoker les 4 triangles
    
    liste_triangles = np.array(liste_triangles) # mettre les triangles en np array

    m = pymeshlab.Mesh(all_points, liste_triangles) # cree la mesh
    ms = pymeshlab.MeshSet() # cree une meshliste pour contenir le mesh
    ms.add_mesh(m, mesh_name = '', set_as_current = True) # ajoute mesh a la liste
    
    point = np.array([0, 0, 0])
    ms.colorize_by_geodesic_distance_from_a_given_point(startpoint=point)
    
    outname = "mesh_with_colorv2.obj"
    if len(sys.argv) > 2:
        outname = sys.argv[2]
        
    outname = arrange_name(outname,".obj")    
    ms.save_current_mesh(outname) # enregistre
    
main()