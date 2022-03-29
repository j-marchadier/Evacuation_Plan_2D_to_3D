import cv2
import numpy as np


# Detection du plan et de la legend
class detectPlanLegend:

    def __init__(self, input_path):
        # Basic image
        self.img = cv2.imread(input_path)

        # Image precessed
        self.precessedImg = 0

    def printImg(self):
        cv2.imwrite('data/image.jpg', self.img)

    def printImgProcess(self):
        cv2.imwrite('data/imageProcess.jpg', self.imageProcess())

    def getImgProcess(self):
        return self.imageProcess()

    def imageProcess(self, img = None):
        if img is None: img = self.img
        # Load image, grayscale, Gaussian blur, adaptive threshold
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

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

        self.precessedImg = dilated
        return dilated

    def lookForContours(self,img = None):
        if img is None: img = self.imageProcess()

        #Find all contour in img
        contours,_ = cv2.findContours(img, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
        return contours

    def findLegend(self,contours = None):
        if contours is None: contours = self.lookForContours()

        coord_legend = []
        size_legend = []

        for contour in contours:
            area = cv2.contourArea(contour)
            if area > self.img.shape[0] + self.img.shape[1]:
                # get rectangle bounding contour
                [x, y, w, h] = cv2.boundingRect(contour)
                size_legend.append((x + w) * 2 + (y + h) * 2)
                coord_legend.append([x, y, w, h])

        if not coord_legend:
            print("NO LEGEND FIND")
            return

        c = coord_plan[0] if len(coord_plan) == 1 else coord_plan
        c = c[size_plan.index(max(size_plan))]
        plan = self.img[c[1]:c[1] + c[3], c[0]:c[0] + c[2]]
        cv2.imwrite('data/roi.jpg', plan)
        return

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
    cv2.imwrite('data/roi.jpg', plan)
    return
