import numpy as np
import cv2
import glob
import pandas as pd
import matplotlib.pyplot as plt


class WallDetection():
    ''' Constructeur de la classe FloorPlan qui effectue divers traitements sur l'image'''

    def __init__(self, input_path):

        # ouverture de l'image
        self.img = input_path

        # liste des contours interessants
        self.list = []

    def imageProcess(self):
        # Load image, grayscale, Gaussian blur, adaptive threshold
        gray = cv2.cvtColor(self.img, cv2.COLOR_BGR2GRAY)

        # blur = cv2.GaussianBlur(gray, (9, 9), 0)
        # thresh = cv2.adaptiveThreshold(blur, 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY_INV, 11, 2)
        ret, thresh = cv2.threshold(gray, 30, 255, cv2.THRESH_BINARY)
        thresh = cv2.bitwise_not(thresh)

        # Closing is reverse of Opening, Dilation followed by Erosion. It is useful in closing small holes inside the foreground objects, or small black points on the object.
        kernel = np.ones((2, 2), np.uint8)
        opening = cv2.morphologyEx(thresh, cv2.MORPH_OPEN, kernel)  # enleve les impuretés

        # Dilate to combine adjacent text contours
        # kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (1,1))
        kernel = np.ones((5, 5), np.uint8)
        dilated = cv2.dilate(opening, kernel, iterations=4)

        contours, hierarchy = cv2.findContours(dilated, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
        # contours = contours[0] if (len(contours) == 2 or len(contours) == 1) else contours
        c = self.getPlan(contours)[0]

        cv2.imwrite('roi.png', thresh[c[1]:c[1] + c[3], c[0]:c[0] + c[2]])

        self.splitImage(contours, hierarchy, self.img)

        return

    def getPlan(self, contours):
        coord_plan = []
        hist_plan = []
        for contour in contours:
            area = cv2.contourArea(contour)
            if area > 100000:
                # get rectangle bounding contour
                [x, y, w, h] = cv2.boundingRect(contour)
                coord_plan.append([x, y, w, h])
                return coord_plan

        # cv2.rectangle(img_input, (x, y), (x + w, y + h), (255, 0, 255), 3)
        return coord_plan

    def splitImage(self, contours, hierarchy, img_input):
        coord = []
        hist = []
        for contour, hierarchy in zip(contours, hierarchy[0]):
            # get rectangle bounding contour
            [x, y, w, h] = cv2.boundingRect(contour)

            # Don't plot small false positives that aren't text
            if w < self.img.shape[1] // 4 or h < self.img.shape[0] // 4:
                continue
            # print(w,h)
            coord.append([x, y, w, h])
            hist.append(hierarchy[3])

            # draw rectangle around contour on original image
            cv2.rectangle(img_input, (x, y), (x + w, y + h), (255, 0, 255), 3)

        for i in range(len(coord)):
            r = coord[i]
            cv2.imwrite('data/divided/zone' + str(i) + '.png', img_input[r[1]:r[1] + r[3], r[0]:r[0] + r[2]])


def DetectLogo(image):
    img_rgb = image
    img_gray = cv2.cvtColor(img_rgb, cv2.COLOR_BGR2GRAY)
    coord_global = []
    height, width = img_rgb.shape[:2]
    r = width / height
    img_gray = cv2.resize(img_gray, (int(1080 * r), 1080))
    img_rgb = cv2.resize(img_rgb, (int(1080 * r), 1080))

    for logosDir in ("exit", "extincteur", "incendie", "porte", "BT"):
        allFiles = glob.glob("data/logos/" + logosDir + "/*.png")
        n = len(allFiles)
        print("########## Start process " + logosDir + " ###########")
        i = 1
        for file in allFiles:

            # for link in ("extincteur", "incendie", "tension", "evacuation", "coupure"):
            template = cv2.imread(file, 0)
            if logosDir != "BT":
                template = cv2.resize(template, (35, 35))
            kernel = np.ones((1, 1), np.uint8)
            template = cv2.dilate(template, kernel, iterations=2)

            w, h = template.shape[::-1]
            coord = []

            # loop over the scales of the image
            for scale in np.linspace(0.5, 1.5, 50)[::-1]:
                coord2 = []
                x = int(w * scale)
                y = int(h * scale)
                tpl = cv2.resize(template, (x, y), interpolation=cv2.INTER_AREA)
                if x > img_rgb.shape[0] or y > img_rgb.shape[1]: continue

                res = cv2.matchTemplate(img_gray, tpl, cv2.TM_CCOEFF_NORMED)
                threshold = 0.8
                loc = np.where(res >= threshold)
                for pt in zip(*loc[::-1]):
                    # print(pt)
                    coord2.append((pt[0], pt[1], pt[0] + x, pt[1] + y, logosDir))

                if len(coord) < len(coord2):  # selection le plus grand nombre de reperage
                    coord = coord2

            print(i, "/", n, " : ", len(coord), " logo(s) find")
            i = i + 1

            # print(len(coord))
            coord_global.append(coord)

        print("End process " + logosDir)

    with open('logos.txt', 'w') as f:
        for i in coord_global:
            for c in i:
                f.write("{}\n".format(c))
                cv2.rectangle(img_rgb, (c[0], c[1]), (c[2], c[3]), (255, 255, 255), -1)
    cv2.imwrite('resultat_detection_logo.png', img_rgb)

def line():
    #Read gray image
    img = cv2.imread("roi.png",0)

    #Create default parametrization LSD
    lsd = cv2.createLineSegmentDetector(0)
    result = ""
    #Detect lines in the image
    lines = lsd.detect(img)[0] #Position 0 of the returned tuple are the detected lines
    for line in lines:
        for x1,y1,x2,y2 in line:
            result = result+str(int(x1))+';'+str(int(y1))+';'+str(int(x2))+';'+str(int(y2))+'\n'
    result = result[:-1]
    f = open("1_mur.txt","w")
    f.write(result)
    f.close()
    #Draw detected lines in the image
    drawn_img = lsd.drawSegments(img,lines)
    print(len(lines))
    #Show image
    cv2.imwrite('LSD.jpg',drawn_img)
    cv2.imshow('LSD.jpg',drawn_img)
    cv2.waitKey(0)

if __name__ == "__main__":

    input_path = "./data/esiee.jpg"
    #image = cv2.imread(input_path)
    #DetectLogo(image)

    #image = cv2.imread("resultat_detection_logo.png")
    #WallDetection(image).imageProcess()

    line()
    # cv2.imshow('thresh', img)
    # cv2.waitKey()
