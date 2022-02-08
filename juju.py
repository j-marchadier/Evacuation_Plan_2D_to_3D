import numpy as np
import cv2
import matplotlib.pyplot as plt


def importImage(path):
    # open image
    img = cv2.imread(path)

    #img = cv2.imread('roi.png')
    # Image in gray scale : RGB --> [0:1]
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

    # Binary level : if >50 --> 0 Black  else 255 White
    ret, thresh = cv2.threshold(gray, 50, 255, cv2.THRESH_BINARY)

    # Create mask
    image = cv2.bitwise_and(gray, gray, mask=thresh)
    ret, new_img = cv2.threshold(image, 100, 255, cv2.THRESH_BINARY_INV)  # for black text , cv.THRESH_BINARY_INV

    kernel = cv2.getStructuringElement(cv2.MORPH_CROSS, (3,3))  # to manipulate the orientation of dilution , large x means horizonatally dilating  more, large y means vertically dilating more
    dilated = cv2.dilate(new_img, kernel, iterations=3)  # dilate , more the iteration more the dilation

    return img, thresh, dilated

def split_image(contours, hierarchy, img_input):
    coord = []
    hist = []
    for contour, hierarchy in zip(contours, hierarchy[0]):
        # get rectangle bounding contour
        [x, y, w, h] = cv2.boundingRect(contour)


        # Don't plot small false positives that aren't text
        if w < img.shape[1] // 4 or h < img.shape[0] // 4:
            continue
        #print(w,h)
        coord.append([x, y, w, h])
        hist.append(hierarchy[3])

        # draw rectangle around contour on original image
        cv2.rectangle(img_input, (x, y), (x + w, y + h), (255, 0, 255), 3)
    return coord, hist, img_input

def getPlan(contours, hierarchy, img_input, img_out):
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
        #cv2.rectangle(img_input, (x, y), (x + w, y + h), (255, 0, 255), 3)

    # Get the shape in the center of
    plan = img_input
    for i in range(len(coord_plan)):
        r = coord_plan[i]

        if (r[1] + r[3] > img_input.shape[0] / 2 > r[1]) and (r[0] + r[2] > img_input.shape[1] / 2 > r[0]):
            plan = img_out[r[1]:r[1] + r[3], r[0]:r[0] + r[2]]

    return plan


def create_img_from_split_img(contours, hierarchy, img_input):
    coord, hist, img_out = split_image(contours, hierarchy,img_input)
    for i in range(len(coord)):
        r = coord[i]
        cv2.imwrite('data/divided/zone' + str(i) + '.png', img_out[r[1]:r[1] + r[3], r[0]:r[0] + r[2]])

# cv2.rectangle(img, (coord[2][0], coord[2][1]), (coord[2][0] + coord[2][2], coord[2][1] + coord[2][3]), (255, 0, 255),3)
# cv2.imwrite('roi.png', thresh[r[1]:r[1]+r[3],r[0]:r[0]+r[2]])

# cv2.imshow('image', moment)

# cv2.imshow('dilate', image)



if __name__ == "__main__":
    img, thresh, dilated = importImage('./data/plan.jpg')
    print(img.shape)
    # find all contours in the image
    # hierarchy : [Next, Previous, First_Child, Parent]
    contours, hierarchy = cv2.findContours(dilated, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
    plan = getPlan(contours, hierarchy, img, thresh)
    cv2.imwrite('roi.png', plan)

    planBis = plan
    for i in range(1):
        #print(abs(np.mean([plan.shape[0]/img.shape[0], plan.shape[1]/img.shape[1]])))
        imgBis, threshBis, dilatedBis = importImage('roi.png')
        contoursBis, hierarchyBis = cv2.findContours(dilatedBis, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
        planBis = getPlan(contoursBis, hierarchyBis, imgBis, threshBis)
        cv2.imwrite('roi.png', planBis)

    create_img_from_split_img(contours, hierarchy, img)

    #cv2.imshow('thresh', img)
    #cv2.waitKey()
