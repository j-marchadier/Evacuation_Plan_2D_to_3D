import numpy as np
import cv2
import matplotlib.pyplot as plt

# %matplotlib inline  # if you are running this code in Jupyter notebook

# reads image 'opencv-logo.png' as grayscale
img = cv2.imread('./data/plan-evacuation.jpg')
gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

ret, thresh = cv2.threshold(gray, 50, 255, cv2.THRESH_BINARY)
image = cv2.bitwise_and(gray, gray, mask=thresh)
ret, new_img = cv2.threshold(image, 100, 255, cv2.THRESH_BINARY_INV)  # for black text , cv.THRESH_BINARY_INV

kernel = cv2.getStructuringElement(cv2.MORPH_CROSS, (3,3))  # to manipulate the orientation of dilution , large x means horizonatally dilating  more, large y means vertically dilating more
dilated = cv2.dilate(new_img, kernel, iterations=3)  # dilate , more the iteration more the dilation

contours, hierarchy = cv2.findContours(dilated,cv2.RETR_TREE,cv2.CHAIN_APPROX_SIMPLE)
#print(contours[2])

coord = []
hist = []

for contour,hierarchy in zip(contours,hierarchy[0]):
    # get rectangle bounding contour
    [x, y, w, h] = cv2.boundingRect(contour)

    # Don't plot small false positives that aren't text
    if w < img.shape[0]//2 and h < img.shape[0]//2:
        continue
    coord.append([x, y, w, h])
    hist.append(hierarchy[3])

    # draw rectangle around contour on original image
    cv2.rectangle(img, (x, y), (x + w, y + h), (255, 0, 255),3)

# hierarchy : [Next, Previous, First_Child, Parent]

r =coord[hist.index(max(hist))]
#cv2.rectangle(img, (coord[2][0], coord[2][1]), (coord[2][0] + coord[2][2], coord[2][1] + coord[2][3]), (255, 0, 255),3)
cv2.imwrite('roi.png', thresh[r[1]:r[1]+r[3],r[0]:r[0]+r[2] ])

cv2.imshow('image', img)
#cv2.imshow('thresh', img)
#cv2.imshow('dilate', image)

cv2.waitKey()

print(gray.shape)
