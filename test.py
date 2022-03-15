import cv2
import numpy as np


a= []
for i in range(5):
    a.append(i)
a=np.array(a)
print(a[a>2])
# Load image, grayscale, Gaussian blur, adaptive threshold
image = cv2.imread('data/resultat_detection_logo.png')
height, width = image.shape[:2]
r = width / height
image = cv2.resize(image, (int(1080 * r), 1080))
gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

blur = cv2.GaussianBlur(gray, (9, 9), 0)
#thresh = cv2.adaptiveThreshold(blur, 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY_INV, 11, 2)
ret, thresh = cv2.threshold(gray, 30, 255, cv2.THRESH_BINARY)
thresh = cv2.bitwise_not(thresh)

# Closing is reverse of Opening, Dilation followed by Erosion. It is useful in closing small holes inside the foreground objects, or small black points on the object.
kernel = np.ones((2, 2), np.uint8)
closing = cv2.morphologyEx(thresh, cv2.MORPH_OPEN, kernel)  # enleve les impuretés
# Dilate to combine adjacent text contours
# kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (1,1))
kernel = np.ones((5, 5), np.uint8)
dilate = cv2.dilate(closing, kernel, iterations=4)

# Find contours, highlight text areas, and extract ROIs
cnts = cv2.findContours(dilate, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
cnts = cnts[0] if len(cnts) == 2 else cnts[1]

ROI_number = 0
for c in cnts:
    area = cv2.contourArea(c)
    if area > 100000:
        x, y, w, h = cv2.boundingRect(c)
        cv2.rectangle(image, (x, y), (x + w, y + h), (36, 255, 12), 5)
        # ROI = image[y:y+h, x:x+w]
        # cv2.imwrite('ROI_{}.png'.format(ROI_number), ROI)
        # ROI_number += 1

cv2.imshow('thresh', thresh)
cv2.imshow('dilate', dilate)
cv2.imshow('image', image)
cv2.waitKey()
