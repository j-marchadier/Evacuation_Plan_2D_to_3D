#!/usr/bin/python2.7
# -*- coding: utf-8 -*-

import cv2
import numpy as np

class WallDetection():
    ''' Class de manipulation image utilisant la bibliothèque opencv
    Paramètres :
        - input_path : chemin de l'image en entrée
        - output_path : chemin de l'image en sortie
    '''
    def __init__(self, input_path):
        ''' Constructeur de la classe FloorPlan qui effectue divers traitements
        sur l'image
        '''
        # ouverture de l'image
        #self.img = cv.LoadImageM(input_path)
        self.img = cv2.imread(input_path)
        # liste des contours interessants
        self.list = []

    def script(self, threshold_value, min_contour_area):
        self.grayscale()
        print("1")
        # seuillage de self.img
        thresh = cv2.threshold(self.img, threshold_value, 255, cv2.THRESH_BINARY)
        print("2")
        kernel = cv2.getStructuringElement(cv2.MORPH_CROSS, (3,3))
        # dillatation de self.img
        dilate = cv2.dilate(self.img, kernel, iterations=1)
        print("3")

        # détermination des contours de l'image
        seq = cv2.findContours(self.img, cv2.RETR_CCOMP, cv2.CHAIN_APPROX_SIMPLE)

        # remise à zéro de l'image i.e. tous les pixels noirs
        #cv2.set(self.img, cv2.Scalar(0));
        # tracage des contours
        self.interesting_contours(seq, min_contour_area)
        cv2.fillPoly(self.img, self.list, color=(255, 255, 255))
        # suppression mémoire des contours
        del seq
        # sauvegarde de self.img
        cv2.SaveImage("results/output.pgm", self.img)

    def threshold(self, threshold_value):
        self.grayscale()

        # seuillage de self.img
        cv2.Threshold(self.img, self.img,threshold_value,255,cv2.CV_THRESH_BINARY)
        cv2.imwrite("results/tmp.png", self.img)

    def grayscale(self):
        # creation d'une image de la taille de la source, en 8 bits car
        # nécessaire pour convertir l'image
        grayscale = np.zeros((self.img.shape[0], self.img.shape[1], 3))

        # conversion de self.img en niveau de gris et sauvegarde dans grayscale
        grayscale = cv2.cvtColor(self.img, cv2.COLOR_BGR2GRAY)
        # on remet le résultat obtenu dans la variable self.img
        self.img = grayscale

    def interesting_contours(self, seq, min_contour_area):
        '''Parcours tous les contours contenus dans seq
        et rempli self.list des contours dont l'aire est superieure
        a MIN_CONTOUR_AREA pixels carres
        '''
        while seq:
            self.interesting_contours(seq.v_next(), min_contour_area)    # Recurse on children
            seq = seq.h_next()        # Next sibling
            pt0 = None
            pt1 = None
            pt2 = None
            try :
                if cv2.contourArea(seq) > min_contour_area:
                    tmp_list = []
                    for (x,y) in seq:
                        tmp_list.append((x,y))
                    self.list.append(tmp_list)
            except :
                pass

a = WallDetection('./data/plan.jpg').script(30,30)