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
    if platform == "linux" or platform == "linux2" or platform == "darwin":
        system("python ./labelImgmaster/labelImg.py "+'"'+path+'"')
    else :
        system("python labelImgmaster\labelImg.py "+path+'"')

def startApplication():
    if platform == "linux" or platform == "linux2" or platform == "darwin":
        system("chmod 777 wallmaker_macos.app/Contents/MacOS/wallmake")
        system("./wallmaker_macos.app/Contents/MacOS/wallmake")
    else:
        system("start wallmaker_windows\wallmake.exe")


def delLogos():
    if platform == "linux" or platform == "linux2" or platform == "darwin":
        system("rm -rf data/logos")
    else :
        system("rd /s /q data\logos")


def createLogos():
    if platform == "linux" or platform == "linux2" or platform == "darwin":
        system("mkdir data/logos")
    else:
        system("mkdir data\logos")

