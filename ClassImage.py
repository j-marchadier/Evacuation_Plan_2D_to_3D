# -*- coding: utf-8 -*-

import numpy
import os
import cv2
import sys
import networkx as nx

THRESHOLD = 200
MIN_CONTOUR_AREA = 50
MIN_SEGMENT_SIZE = 7
WALL_ELEVATION = str(0.04)

class FloorPlan():
    ''' Classe de manipulation d'image utilisant la bibliothèque opencv
    Paramètres :
        - input_path : chemin de l'image en entrée
        - output_path : chemin de l'image en sortie
    '''
    def __init__(self, input_path):
        ''' Constructeur de la classe FloorPlan qui effectue divers traitements
        sur l'image
        '''
        # ouverture de l'image
        self.img = cv2.LoadImageM(input_path)
        # liste des contours interessants
        self.list = []

    def script(self):
        self.grayscale()
        # seuillage de self.img
        cv.Threshold(self.img, self.img,THRESHOLD,255,cv.CV_THRESH_BINARY)
        # dillatation de self.img
        cv.Dilate(self.img, self.img)
        # détermination des contours de l'image
        seq = cv.FindContours(self.img, cv.CreateMemStorage(), cv.CV_RETR_CCOMP, cv.CV_CHAIN_APPROX_SIMPLE)
        # remise à zéro de l'image i.e. tous les pixels noirs
        cv.Set(self.img, cv.Scalar(0));
        # tracage des contours
        self.interesting_contours(seq)
        cv.FillPoly(self.img, self.list, cv.Scalar(255))
        # suppression mémoire des contours
        del seq
        # sauvegarde de self.img
        cv.SaveImage("results/output.pgm", self.img)

    def grayscale(self):
        # creation d'une image de la taille de la source, en 8 bits car
        # nécessaire pour convertir l'image
        grayscale = cv.CreateImage((self.img.width,self.img.height),8,1)
        # conversion de self.img en niveau de gris et sauvegarde dans grayscale
        cv.CvtColor(self.img, grayscale, cv.CV_BGR2GRAY)
        # on remet le résultat obtenu dans la variable self.img
        self.img = grayscale

    def interesting_contours(self, seq):
        '''Parcours tous les contours contenus dans seq
        et rempli self.list des contours dont l'aire est superieure
        a MIN_CONTOUR_AREA pixels carres
        '''
        while seq:
            self.interesting_contours(seq.v_next())    # Recurse on children
            seq = seq.h_next()        # Next sibling
            pt0 = None
            pt1 = None
            pt2 = None
            try :
                if cv.ContourArea(seq) > MIN_CONTOUR_AREA:
                    tmp_list = []
                    for (x,y) in seq:
                        tmp_list.append((x,y))
                    self.list.append(tmp_list)
            except :
                pass

class Skeleton():
    def __init__(self, skeleton_file):
        self.graph = nx.Graph()
        self.file = open(skeleton_file)

    def parse(self):
        [self.width,self.height] = self.file.readline().split()[5:7]
        self.width = int(self.width)
        self.height = int(self.height)
        line = self.file.readline()
        while line.split()[0] != "END":
            # boucle qui permet de venir jusqu'aux sommets finaux
            line = self.file.readline()
        line = self.file.readline()
        while line.split()[0] != "CURV":
            # boucle qui parcourt tous les sommets finaux
            nb_obj = line.split()[1]
            adj = self.file.readline().split()[2]
            pixel_img = self.file.readline().split()[2]
            self.graph.add_node(nb_obj, end=True, pixel=[pixel_img], adj=adj)
            line = self.file.readline()
        line = self.file.readline()
        while line.split()[0] != "JUNC":
            try:
                # boucle qui parcourt tous les arcs
                nb_obj = line.split()[1]
                adj = self.file.readline().split()[2:]
                pixel_img = self.file.readline().split()[2:]
                if len(pixel_img) > MIN_SEGMENT_SIZE :
                    self.graph.add_edge(adj[0],adj[1], pixel=pixel_img)
            except:
                pass
            line = self.file.readline()
        line = self.file.readline()
        while line != "":
            # boucle qui parcourt tous les points de jonctions
            nb_obj = line.split()[1]
            adj = self.file.readline().split()[2:]
            pixel_img = self.file.readline().split()[2:]
            self.graph.add_node(nb_obj, pixel=pixel_img, adj=adj)
            line = self.file.readline()
        self.file.close()
        degree = nx.degree(self.graph)
        for key in degree.keys():
            if(degree[key] == 0):
                self.graph.remove_node(key)

    def nxgraph_to_pgm(self):
        ########### NON UTILISEE #####################
        file = open('results/graph_traite.txt','w')
        #file.write('2Dskel 8 435 2603 820 842 595 1\n')
        #file.write('ISOL 0\n')
        #file.write('END ' + str(self.nb_end) + '\n')
        #for node in self.graph.nodes(data=True):
        #    if 'end' in node[1]:
        #        file.write('vertex ' + node[0] + '\n')
        #        file.write('adj 1 ' + node[1]['adj'] + '\n')
        #        file.write('pts 1 ' + node[1]['pixel'] + '\n')
        array = numpy.zeros((self.width*self.height))
        file.write('P2\n' + str(self.width) + ' ' + str(self.height) + '\n255\n')
        #line=''
        #for i in range(842):
        #    line += ' 0'
        #line += '\n'
        #for j in range(595):
        #    file.write(line)
        #for edge in self.graph.edges(data=True):
        #    for pixel in edge[2]['pixel']:
        #       array[int(pixel)] = 125
        for node in self.graph.nodes(data=True):
            if isinstance(node[1]['pixel'],str):
                #print node[1]['pixel']
                array[int(node[1]['pixel'])] = 180
            else:
                for pixel in node[1]['pixel']:
                    #print pixel
                    array[int(pixel)] = 255
        array.tofile(file," ","%i")
        self.points_to_segments()
        file.close

    def redress_lines(self):
        for component in nx.connected_components(self.graph):
            for node in component:
                if self.graph.degree(node) == 1:
                    component.remove(node)
                    self.align_points(component, node)
                    break

    def align_points(self, component, fixed_point):
        for neighbor in self.graph.neighbors(fixed_point):
            if neighbor in component:
                self.move_point(fixed_point, neighbor)
                component.remove(neighbor)
                self.align_points(component, neighbor)

    def move_point(self, fixed_point, floating_point):
        x1 = int(self.graph.node[fixed_point]['pixel'][0])%self.width
        y1 = int(self.graph.node[fixed_point]['pixel'][0])//self.width
        x2 = int(self.graph.node[floating_point]['pixel'][0])%self.width
        y2 = int(self.graph.node[floating_point]['pixel'][0])//self.width
        if(abs(x1-x2)<10):
            x2=x1
        if(abs(y1-y2)<10):
            y2=y1
        self.graph.node[floating_point]['pixel'][0] = y2 * self.width + x2

    def points_to_segments(self):
        img = cv.CreateImage((self.width,self.height),8,1)
        cv.Set(img, cv.Scalar(0));
        for edge in self.graph.edges(data=True):
            pt1 = edge[0]
            pt2 = edge[1]
            x1 = int(self.graph.node[pt1]['pixel'][0])%self.width
            y1 = int(self.graph.node[pt1]['pixel'][0])//self.width
            x2 = int(self.graph.node[pt2]['pixel'][0])%self.width
            y2 = int(self.graph.node[pt2]['pixel'][0])//self.width
            cv.Line(img,(x1,y1),(x2,y2),cv.Scalar(255))
        cv.SaveImage('results/droites.pgm', img)

    def points_to_VRML(self):
        vrml = VRML(self.graph)
        txt = ''
        txt += '\tgeometry IndexedFaceSet {\n'
        txt += '\t\tcoord Coordinate {\n'
        txt += '\t\t\tpoint [\n'
        for edge in self.graph.edges(data=True):
            pt1 = edge[0]
            pt2 = edge[1]
            x1 = str(float(self.graph.node[pt1]['pixel'][0])%self.width/self.width)
            y1 = str(float(self.graph.node[pt1]['pixel'][0])//self.width/self.height)
            x2 = str(float(self.graph.node[pt2]['pixel'][0])%self.width/self.width)
            y2 = str(float(self.graph.node[pt2]['pixel'][0])//self.width/self.height)
            txt += '\t\t\t\t' + x1 + ' ' + y1 + ' 0,\n'
            txt += '\t\t\t\t' + x2 + ' ' + y2 + ' 0,\n'
            txt += '\t\t\t\t' + x1 + ' ' + y1 + ' ' + WALL_ELEVATION +',\n'
            txt += '\t\t\t\t' + x2 + ' ' + y2 + ' ' + WALL_ELEVATION +',\n'
        txt += '\t\t\t]\n\t\t}\n'
        txt += '\t\tcoordIndex [\n'
        for i in range(len(self.graph.edges())):
            txt += '\t\t\t' + str(i*4) + ',' + str(1+i*4) + ',' + str(3+i*4) + ',' + str(2+i*4) + ',' + str(i*4) + ',-1,\n'
            txt += '\t\t\t' + str(i*4) + ',' + str(2+i*4) + ',' + str(3+i*4) + ',' + str(1+i*4) + ',' + str(i*4) + ',-1,\n'
        txt += '\t\t]\n\t}\n'
        vrml.file.write(txt)

class VRML():
    def __init__(self, graph):
        self.graph = graph
        self.file = open('results/vrml.wrl','w')
        txt = ''
        txt += '#VRML V2.0 utf8\n\n'
        txt += 'Shape {\n'
        txt += '\tappearance Appearance{\n'
        txt += '\t\tmaterial Material {\n'
        txt += '\t\t\tdiffuseColor     1 0 0 #simple red\n'
        txt += '\t\t\t}\n\t\t}\n'
        self.file.write(txt)

    def __del__(self):
        self.file.write('}')
        self.file.close()

def main(argv):
    try :
        img_path = argv[1]
        output_img = "results/output.PGM"
    except :
        print "Specify input image path (first parameter)"
        sys.exit()
    floor_plan = FloorPlan(img_path)
    floor_plan.script()
    os.system("../../../../Downloads/ProjetENSG-2D3D/ProjetRechercheMymap/script.sh")
    skeleton = Skeleton('results/squelette.txt')
    skeleton.parse()
    skeleton.redress_lines()
    #skeleton.points_to_segments()
    skeleton.points_to_VRML()

if __name__ == "__main__":
    main(sys.argv)
