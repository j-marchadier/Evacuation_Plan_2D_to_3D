
import numpy as np
import cv2

image = cv2.imread("roi.png")

# Load image, grayscale, Gaussian blur, adaptive threshold
gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

# Closing is reverse of Opening, Dilation followed by Erosion. It is useful in closing small holes inside the foreground objects, or small black points on the object.
kernel = np.ones((2, 2), np.uint8)
a = cv2.getStructuringElement(cv2.MORPH_CROSS,(5,5))
opening = cv2.morphologyEx(gray, cv2.MORPH_CROSS, a)  # enleve les impuretés


cv2.imshow('thresh', opening)
cv2.waitKey()