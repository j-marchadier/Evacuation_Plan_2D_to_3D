from os import system
import os
from tkinter import filedialog
from sys import platform

def init():
    system("pyrcc5 -o labelImgmaster/libs/resources.py labelImgmaster/resources.qrc")

def SelectImage():
    img_path = filedialog.askopenfilename(initialdir=os.getcwd())
    return img_path

def OpenLabelImg(path):
    system("python ./labelImgmaster/labelImg.py "+'"'+path+'"')
    #if platform == "linux" or platform == "linux2" or platform == "darwin":
       # print("A")
       # system("python ./labelImgmaster/labelImg.py "+'"'+path+'"')
        #print("B")
    #elif platform == "win32":
       # system("python .\labelImgmaster\labelImg.py "+'"'+path+'"')

def startApplication():
    system("chmod 777 wallmaker_macos.app/Contents/MacOS/wallmake")
    system("./wallmaker_macos.app/Contents/MacOS/wallmake")

def delLogos():

    system("rm -rf data/logos")


def createLogos():
    system("mkdir data/logos")

