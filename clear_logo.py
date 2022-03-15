from xml.etree.ElementTree import tostring
import cv2
import numpy as np
import tkinter as tk
from tkinter import filedialog
import pandas as pd
from matplotlib import pyplot as plt

def SelectFile(title = "Select"):
    #Select File
    root = tk.Tk()
    root.withdraw()
    path = filedialog.askopenfilename(title = title)
    return path

def ClearLogo():
    # Make an example image
    image = cv2.imread(SelectFile(title = "Sélectionnez une map pour clear"), cv2.IMREAD_UNCHANGED)

    # Define the color you're looing for
    pattern = np.array([80, 80, 80])  #Dark colors

    # Make a mask to use with where
    mask = (image <= pattern).all(axis=2)
    newshape = mask.shape + (1,)
    mask = mask.reshape(newshape)

    # Finish it off
    image = np.where(mask, [0, 0, 0], [255, 255, 255])
    cv2.imwrite('Result_without_logos.png',image) 

    #Save in 0/1 form
    image_01 = image[:,:,0]
    image_01 = np.where(image_01 == 255, 1, 0)
    image_01 = image_01.astype('int')
    df = pd.DataFrame(image_01)
    df.to_csv('Result_in_01.csv')

def DetectLogo():
    img_rgb = cv2.imread(SelectFile(title = "Sélectionnez une map pour detecter les logs"))
    img_gray = cv2.cvtColor(img_rgb, cv2.COLOR_BGR2GRAY)
    template = cv2.imread(SelectFile(title = "Sélectionnez un logo"),0)
    w, h = template.shape[::-1]
    res = cv2.matchTemplate(img_gray,template,cv2.TM_CCOEFF_NORMED)
    threshold = 0.8
    loc = np.where( res >= threshold)
    for pt in zip(*loc[::-1]):
        cv2.rectangle(img_rgb, pt, (pt[0] + w, pt[1] + h), (0,255,0), 2)
    cv2.imwrite('data/resultat_detection_logo.png', img_rgb)
    
def LineDetection():
    img = cv2.imread(cv2.samples.findFile(SelectFile(title = "Sélectionnez une map pour detecter les lignes")))
    gray = cv2.cvtColor(img,cv2.COLOR_BGR2GRAY)
    edges = cv2.Canny(gray,50,200,apertureSize = 7)
    cdst = cv2.cvtColor(edges, cv2.COLOR_GRAY2BGR)
    cdstP = np.copy(cdst)
    lines = cv2.HoughLines(edges,1,np.pi/180,200)
    for line in lines:
        rho,theta = line[0]
        a = np.cos(theta)
        b = np.sin(theta)
        x0 = a*rho
        y0 = b*rho
        x1 = int(x0 + 1000*(-b))
        y1 = int(y0 + 1000*(a))
        x2 = int(x0 - 1000*(-b))
        y2 = int(y0 - 1000*(a))
        cv2.line(cdst,(x1,y1),(x2,y2),(0,0,255),1)
    linesP = cv2.HoughLinesP(edges, 1, np.pi / 180, 50, None, 50, 10)
    
    if linesP is not None:
        for i in range(0, len(linesP)):
            l = linesP[i][0]
            cv2.line(cdstP, (l[0], l[1]), (l[2], l[3]), (0,0,255), 1, cv2.LINE_AA)
    
    #cv2.imshow("Detected Lines (in red) - Standard Hough Line Transform"+str(aperture), cdst)
    cv2.imshow("Detected Lines (in red) - Probabilistic Line Transform", cdstP)
    cv2.imwrite('houghlines.jpg',cdstP)
    cv2.waitKey()
    return 0

LineDetection()