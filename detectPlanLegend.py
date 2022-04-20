import cv2
import numpy as np
import CreateXML
import Interface
import glob
import scipy
import scipy.misc
import scipy.cluster
import easyocr



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
        cv2.imwrite('imageProcess.jpg', opening)
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
            coord_legend = coord_legend[0:2]
            #cv2.imwrite('lookForContour.jpg', legend)
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

    def findLogos(self, filepath=None, id = 1.5):

        Interface.delLogos()
        if filepath is None : filepath = self.path.replace(self.path.split("/")[-1],"legend.jpg")

        legend = cv2.imread(filepath)
        legend_processed = self.imageProcess(legend,threshLvl=70)

        contours = self.lookForContours(legend_processed)
        #cv2.imwrite('roi.jpg', legend)

        coord_legend = []

        for contour in contours:
            [x, y, w, h] = cv2.boundingRect(contour)
            if w+h < (legend.shape[0] + legend.shape[1])/id:
                #cv2.rectangle(legend, (x, y), (x + w, y + h), (255, 0, 255), 3)
                coord_legend.append([x, y, w, h])

        if not coord_legend:
            print("NO LOGOS FIND, RETRY...")
            self.findLogos(filepath,id=id+0.5)

            #return

        ### Create a XLM file for all possible legend
        CreateXML.createXML(filepath, legend, coord_legend)

        # Start Inteface to change an perform Plan and legend
        Interface.OpenLabelImg(filepath)
        coord_logos = CreateXML.readLogosXML(filepath)

        Interface.createLogos()

        for l,i in zip(coord_logos,range(len(coord_logos))):
            cv2.imwrite('data/logos/logo_'+str(i)+'.jpg', legend[l[1]:l[3],l[0]:l[2]])

        return

    def dominant_color(self, img):
  
        NUM_CLUSTERS = 5

        ar = np.asarray(img)
        shape = ar.shape
        ar = ar.reshape(scipy.product(shape[:2]), shape[2]).astype(float)

        codes, dist = scipy.cluster.vq.kmeans(ar, NUM_CLUSTERS)

        vecs, dist = scipy.cluster.vq.vq(ar, codes)         # assign codes
        counts, bins = scipy.histogram(vecs, len(codes))    # count occurrences

        index_max = scipy.argmax(counts)                    # find most frequent
        peak = codes[index_max]
          
        return peak

    def only_picto(self, img_path=None):

        Interface.delLogos()
        if img_path is None : img_path = self.path.replace(self.path.split("/")[-1],"legend.jpg")

        ###### On supprime le texte de l'image ######

        img = cv2.imread(img_path)

        reader = easyocr.Reader(['en','fr'])
        result = reader.readtext(img_path)

        if result == []:
            print('No text detected')
            return img

        top_left = tuple(result[0][0][0])
        bottom_right = tuple(result[0][0][2])

        main_color = self.dominant_color(img)

        for detection in result: 
            top_left = tuple(detection[0][0])
            bottom_right = tuple(detection[0][2])

            img = cv2.rectangle(img,top_left,bottom_right,main_color,-1)

        return img

    def findLogos_bis(self, img_path=None, inf=.7, sup=.9):

        Interface.delLogos()
        if img_path is None : img_path = self.path.replace(self.path.split("/")[-1],"legend.jpg")

        # img = self.only_picto(img_path)
        img = cv2.imread(img_path)

        ###### On récupère les coordonnées des pictogrammes ######

        edges = cv2.Canny(img,100,200)
        coord_picto = []
        list_rad = []

        contours,_ = cv2.findContours(edges, 1, 2)

        for cnt in contours:
            (x,y),radius = cv2.minEnclosingCircle(cnt)
            radius = int(radius)
            list_rad.append(radius)

        # Création de quantiles pour réduire le nombre de tâches détectées qui ne correspondent pas à des logos
        Qinf = int(np.quantile(list_rad, inf))
        Qsup = int(np.quantile(list_rad, sup))

        for cnt in contours:
            (x_center,y_center),radius = cv2.minEnclosingCircle(cnt)
            radius = int(radius)

            x = int(x_center) - radius
            y = int(y_center) - radius
            w = 2*radius
            h = w

            if (Qinf < radius < Qsup+1):
                coord_picto.append([x, y, w, h])

        ### Create a XLM file for all possible legends

        CreateXML.createXML(img_path, img, coord_picto)

        # Start Inteface to change an perform Plan and legend
        Interface.OpenLabelImg(img_path)
        coord_logos = CreateXML.readLogosXML(img_path)

        Interface.createLogos()

        for l,i in zip(coord_logos,range(len(coord_logos))):
            cv2.imwrite('data/logos/logo_'+str(i)+'.jpg', img[l[1]:l[3],l[0]:l[2]])

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
