import cv2
import numpy as np
import CreateXML
import Interface
import glob
from os import system


# Detection du plan et de la legend
class detectPlanLegend:

    def __init__(self, input_path):

        self.path = input_path
        # Basic image
        self.img = cv2.imread(input_path)

        # Image precessed
        self.precessedImg = 0

    def imageProcess(self, img=None,threshLvl=30):
        if img is None: img = self.img
        # Load image, grayscale, Gaussian blur, adaptive threshold
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

        # blur = cv2.GaussianBlur(gray, (9, 9), 0)
        # thresh = cv2.adaptiveThreshold(blur, 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY_INV, 11, 2)
        ret, thresh = cv2.threshold(gray, threshLvl, 255, cv2.THRESH_BINARY)
        thresh = cv2.bitwise_not(thresh)

        # Closing is reverse of Opening, Dilation followed by Erosion. It is useful in closing small holes inside the foreground objects, or small black points on the object.
        kernel = np.ones((2, 2), np.uint8)
        opening = cv2.morphologyEx(thresh, cv2.MORPH_OPEN, kernel)  # enleve les impuretés

        # Dilate to combine adjacent text contours
        # kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (1,1))
        kernel = np.ones((5, 5), np.uint8)
        dilated = cv2.dilate(opening, kernel, iterations=4)

        self.precessedImg = dilated
        return dilated

    def lookForContours(self, img=None):
        if img is None: img = self.imageProcess()

        # Find all contour in img
        contours, _ = cv2.findContours(img, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
        return contours

    def findLegendAndPlan(self, contours=None,id=1.5):
        if contours is None: contours = self.lookForContours()

        legend = self.img
        coord_legend = []

        for contour in contours:
            # get rectangle bounding contour
            [x, y, w, h] = cv2.boundingRect(contour)

            if w+h > (self.img.shape[0] + self.img.shape[1])/id:
                #cv2.rectangle(legend, (x, y), (x + w, y + h), (255, 0, 255), 3)
                coord_legend.append([x, y, w, h])

        if len(coord_legend)<2:
            print("NO LEGEND FIND, RETRY ...")
            self.findLegendAndPlan(contours, id=id+0.5)

        else :
            ### Create a XLM file for all possible legend
            CreateXML.createXML(self.path, self.img.shape, coord_legend)

            # Start Inteface to change an perform Plan and legend
            Interface.OpenLabelImg(self.path)
            p,c = CreateXML.readXML(self.path)

            legend = self.img[c[1]: c[3], c[0]: c[2]]
            print(p)
            plan = self.img[p[1]: p[3], p[0]: p[2]]

            cv2.imwrite(self.path.replace(self.path.split("/")[-1],"legend.jpg"), legend)
            cv2.imwrite(self.path.replace(self.path.split("/")[-1],"plan.jpg"), plan)

        return

    def findLogos(self, filepath=None):
        if filepath is None : filepath = self.path.replace(self.path.split("/")[-1],"legend.jpg")
        print(filepath)
        legend = cv2.imread(filepath)
        legend_processed = self.imageProcess(legend,threshLvl=70)

        contours = self.lookForContours(legend_processed)
        #cv2.imwrite('roi.jpg', legend)

        coord_legend = []

        for contour in contours:
            [x, y, w, h] = cv2.boundingRect(contour)
            if w+h < (legend.shape[0] + legend.shape[1])/3:
                #cv2.rectangle(legend, (x, y), (x + w, y + h), (255, 0, 255), 3)
                coord_legend.append([x, y, w, h])

        if not coord_legend:
            print("NO LOGOS FIND")
            #return

        ### Create a XLM file for all possible legend
        CreateXML.createXML(filepath, legend, coord_legend)

        # Start Inteface to change an perform Plan and legend
        Interface.OpenLabelImg(filepath)
        coord_logos = CreateXML.readLogosXML(filepath)

        system("rm  data/logos/*")
        for l,i in zip(coord_logos,range(len(coord_logos))):
            cv2.imwrite('data/logos/logo_'+str(i)+'.jpg', legend[l[1]:l[3],l[0]:l[2]])

        return

    # Clean logos variations
    def clear_coord_logos(self,coord):
        coord_traiten = []
        for type in coord:
            weight = []
            for c in range(len(type) - 1):
                weightInter = 0
                for i in range(4):
                    weightInter = weightInter + int(abs(type[c][i] - type[c + 1][i]))
                weight.append(weightInter)

            if len(weight) > 0:
                weight.append(0)
                weight = np.array(weight)
                type2 = np.array(type)
                coord_traiten.append(type2[weight > 6].tolist())

        return coord_traiten

    def DetectLogo(self):
        img_rgb = cv2.imread(self.path.replace(self.path.split("/")[-1],"plan.jpg"))
        img_gray = cv2.cvtColor(img_rgb, cv2.COLOR_BGR2GRAY)
        coord_global = []

        allFiles = glob.glob("data/logos/*.jpg")
        n = len(allFiles)
        print("########## Start process  ###########")
        i = 1
        for file in allFiles:
            template = cv2.imread(file, 0)
            template = cv2.resize(template, (35, 35))

            w, h = template.shape[::-1]
            coord = []

            # loop over the scales of the image
            for scale in np.linspace(0.5, 3, 100)[::-1]:
                coord2 = []
                x = int(w * scale)
                y = int(h * scale)
                tpl = cv2.resize(template, (x, y), interpolation=cv2.INTER_AREA)

                res = cv2.matchTemplate(img_gray, tpl, cv2.TM_CCOEFF_NORMED)
                threshold = 0.7
                loc = np.where(res >= threshold)
                for pt in zip(*loc[::-1]):
                    # print(pt)
                    coord2.append((pt[0], pt[1], pt[0] + x, pt[1] + y))

                if len(coord) < len(coord2):  # selection le plus grand nombre de reperage
                    coord = coord2

            print(i, "/", n, " : ", len(coord), " logo(s) find")
            i = i + 1

            coord_global.append(coord)

            print("End process ")

        #coord_global = self.clear_coord_logos(coord_global)


       # with open('data/logos.txt', 'w') as f:
            #for i in coord_global:
                #for c in i:
                    #f.write("{}\n".format(c))
                    #cv2.rectangle(img_rgb, (int(c[0]), int(c[1])), (int(c[2]), int(c[3])), (255, 255, 255), -1)

        cv2.imwrite('data/resultat_detection_logo.png', img_rgb)
        return img_rgb
