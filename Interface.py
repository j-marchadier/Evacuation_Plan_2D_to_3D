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

def OpenLabelImg(path):
    system("python ./labelImgmaster/labelImg.py "+'"'+path+'"')

def startApplication():
    system("./wallMake_build/content/")

def delLogos():
    system("rm -rf data/logos")

def createLogos():
    system("mkdir data/logos")
