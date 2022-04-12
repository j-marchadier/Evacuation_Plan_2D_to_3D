from os import system
import os
from tkinter import filedialog
from sys import platform

def SelectImage():
    img_path = filedialog.askopenfilename(initialdir=os.getcwd())
    return img_path

def OpenLabelImg(path):
    if platform == "linux" or platform == "linux2" or platform == "darwin":
        system("python ./labelImgmaster/labelImg.py "+'"'+path+'"')
    elif platform == "win32":
        system("python .\labelImgmaster\labelImg.py "+'"'+path+'"')

def startApplication():
    if platform == "linux" or platform == "linux2" or platform == "darwin":
        system("./wallMake_build/content/")
    elif platform == "win32":
        system(".\wallMake_build\content")

def delLogos():
    if platform == "linux" or platform == "linux2" or platform == "darwin":
        system("rm -rf data/logos")
    elif platform == "win32":
        system("rm -rf data\logos")

def createLogos():
    if platform == "linux" or platform == "linux2" or platform == "darwin":
        system("mkdir data/logos")
    elif platform == "win32":
        system("mkdir data\logos")
