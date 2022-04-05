from os import system
import sys
import os
from tkinter import filedialog
import xml.etree.ElementTree as ET
import cv2
import numpy as np

def SelectImage():
    img_path = filedialog.askopenfilename(initialdir=os.getcwd())
    return img_path

def DetectPlan():
    return

def CropImage(path,object):
    image = cv2.imread(path)
    path = path[:-4]
    tree = ET.parse(path+'.xml')
    root = tree.getroot()
    for child in root:
        if child.tag == "object":
            if child[0].text == object:
                xminLegend = int(child[4][0].text)
                yminLegend = int(child[4][1].text)
                xmaxLegend = int(child[4][2].text)
                ymaxLegend = int(child[4][3].text)
    imageLegend =image[yminLegend:ymaxLegend,xminLegend:xmaxLegend]
    cv2.imwrite('data/plans/'+path.split('plans/',1)[1]+object+'.jpg', imageLegend)

def LSD():
    lsd = cv2.createLineSegmentDetector(0)
    result = ""
    return

def OpenLabelImg(path):
    system("python ./labelImgmaster/labelImg.py "+'"'+path+'"')
    
if __name__ == "__main__":
    path = SelectImage()
    #path = "C:/Users/mathi/Documents/Projet E4/Evacuation_Plan_2D_to_3D-main/Evacuation_Plan_2D_to_3D-main/data/plans/esiee2.jpg"
    #DetectPlan(path)
    OpenLabelImg(path)
    CropImage(path,"Legend")
    CropImage(path,"Map")
    OpenLabelImg(path[:-4]+'Legend.jpg')