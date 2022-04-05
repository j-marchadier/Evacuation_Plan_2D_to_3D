import cv2
import numpy as np
import CreateXML
import Interface


# Detection du plan et de la legend
class detectPlanLegend:

    def __init__(self, input_path):

        self.path = input_path
        # Basic image
        self.img = cv2.imread(input_path)

        # Image precessed
        self.precessedImg = 0

    def printImg(self):
        cv2.imwrite('data/plans/image.jpg', self.img)

    def printImgProcess(self):
        cv2.imwrite('data/plans/imageProcess.jpg', self.imageProcess())

    def getImgProcess(self):
        return self.imageProcess()

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

    def findLegendAndPlan(self, contours=None,id=5):
        if contours is None: contours = self.lookForContours()

        legend = self.img
        coord_legend = []
        size_legend = []

        for contour in contours:
            area = cv2.contourArea(contour)
            if area > self.img.shape[0] * id + self.img.shape[1] * id:
                # get rectangle bounding contour
                [x, y, w, h] = cv2.boundingRect(contour)
                size_legend.append((x + w) * 2 + (y + h) * 2)
                #cv2.rectangle(legend, (x, y), (x + w, y + h), (255, 0, 255), 3)
                coord_legend.append([x, y, w, h])

        if len(coord_legend)<2:
            print("NO LEGEND FIND, RETRY ...")
            self.findLegendAndPlan(contours, id=id-0.5)

        ### Create a XLM file for all possible legend
        CreateXML.createXML(self.path, self.img.shape, coord_legend)

        # Start Inteface to change an perform Plan and legend
        Interface.OpenLabelImg(self.path)
        p,c = CreateXML.readXML(self.path)

        legend = self.img[c[1]: c[3], c[0]: c[2]]
        print(p)
        plan = self.img[p[1]: p[3], p[0]: p[2]]

        cv2.imwrite('data/plans/legend.jpg', legend)
        cv2.imwrite('data/plans/plan.jpg', plan)

        return

    # Look at all contour and only select best for Plan type
    def findPlan(self, contours=None):
        if contours is None: contours = self.lookForContours()
        coord_plan = []
        size_plan = []

        for contour in contours:
            area = cv2.contourArea(contour)
            if area > self.img.shape[0] + self.img.shape[1]:
                # get rectangle bounding contour
                [x, y, w, h] = cv2.boundingRect(contour)
                size_plan.append((x + w) * 2 + (y + h) * 2)
                coord_plan.append([x, y, w, h])

        if not coord_plan:
            print("NO PLAN FIND")
            return

        c = coord_plan[0] if len(coord_plan) == 1 else coord_plan
        c = c[size_plan.index(max(size_plan))]
        plan = self.img[c[1]:c[1] + c[3], c[0]:c[0] + c[2]]
        cv2.imwrite('data/plans/plan.jpg', plan)
        return

    def findLogos(self, filepath=None):
        if filepath is None : filepath = "data/plans/legend.jpg"
        legend = cv2.imread(filepath)
        legend_processed = self.imageProcess(legend,threshLvl=70)

        contours = self.lookForContours(legend_processed)
        cv2.imwrite('roi.jpg', legend)

        coord_legend = []
        size_legend = []

        for contour in contours:
            area = cv2.contourArea(contour)
            #if area > legend.shape[0]/5 and area < legend.shape[0] *legend.shape[1]:
                # get rectangle bounding contour
            [x, y, w, h] = cv2.boundingRect(contour)
            size_legend.append((x + w) * 2 + (y + h) * 2)
            cv2.rectangle(legend, (x, y), (x + w, y + h), (255, 0, 255), 3)
            coord_legend.append([x, y, w, h])

        if not coord_legend:
            print("NO LOGOS FIND")
            #return

        ### Create a XLM file for all possible legend
        CreateXML.createXML(filepath, legend, coord_legend)

        # Start Inteface to change an perform Plan and legend
        Interface.OpenLabelImg(filepath)
        coord_logos = CreateXML.readLogosXML(filepath)
        print(coord_logos)

        return
