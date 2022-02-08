import numpy as np
import cv2
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

        blur = cv2.GaussianBlur(gray, (9, 9), 0)
        thresh = cv2.adaptiveThreshold(blur, 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY_INV, 11, 2)

        # Closing is reverse of Opening, Dilation followed by Erosion. It is useful in closing small holes inside the foreground objects, or small black points on the object.
        kernel = np.ones((2, 2), np.uint8)
        closing = cv2.morphologyEx(thresh, cv2.MORPH_CLOSE, kernel)  # enleve les impuretés

        # Dilate to combine adjacent text contours
        # kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (1,1))
        kernel = np.ones((4, 4), np.uint8)
        dilated = cv2.dilate(closing, kernel, iterations=4)

        contours, hierarchy = cv2.findContours(dilated, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)

        self.splitImage(contours, hierarchy, self.img)

        cv2.imwrite('roi.png', self.getPlan(contours, hierarchy, dilated, thresh))

        return

    def getPlan(self, contours, hierarchy, img_input, img_out):
        coord_plan = []
        hist_plan = []
        for contour, hierarchy in zip(contours, hierarchy[0]):
            # get rectangle bounding contour
            [x, y, w, h] = cv2.boundingRect(contour)

            if w > img_input.shape[1] // 2 or h > img_input.shape[0] // 2:
                coord_plan.append([x, y, w, h])
                hist_plan.append(hierarchy[3])

        print(len(coord_plan))
        # draw rectangle around contour on original image
        # cv2.rectangle(img_input, (x, y), (x + w, y + h), (255, 0, 255), 3)

        # Get the shape in the center of
        plan = img_input
        for i in range(len(coord_plan)):
            r = coord_plan[i]

            if (r[1] + r[3] > img_input.shape[0] / 2 > r[1]) and (r[0] + r[2] > img_input.shape[1] / 2 > r[0]):
                plan = img_out[r[1]:r[1] + r[3], r[0]:r[0] + r[2]]

        return plan

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
    # img_rgb = cv2.imread(SelectFile(title="Sélectionnez une map pour detecter les logs"))
    img_rgb = image
    img_gray = cv2.cvtColor(img_rgb, cv2.COLOR_BGR2GRAY)
    coord_global = []
    for link in ("extincteur", "incendie", "tension", "evacuation", "coupure"):
        template = cv2.imread("data/logos/" + link + ".png", 0)
        kernel = np.ones((1, 1), np.uint8)
        template = cv2.dilate(template, kernel, iterations=2)

        w, h = template.shape[::-1]
        coord = []
        # loop over the scales of the image
        for scale in np.linspace(0.2, 2.0, 10)[::-1]:
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
                coord2.append((pt[0], pt[1], pt[0] + x, pt[1] + y))

            if len(coord) < len(coord2):  # selection le plus grand nombre de reperage
                coord = coord2

        print(len(coord))
        coord_global.append(coord)

    for i in coord_global:
        for c in i:
            cv2.rectangle(img_rgb, (c[0], c[1]), (c[2], c[3]), (255, 255, 255), -1)
    cv2.imwrite('resultat_detection_logo.png', img_rgb)


if __name__ == "__main__":
    input_path = "./data/plan.jpg"
    image = cv2.imread(input_path)
    DetectLogo(image)
    # WallDetection(image).imageProcess()

    # cv2.imshow('thresh', img)
    # cv2.waitKey()
