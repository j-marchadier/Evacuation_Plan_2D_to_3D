import cv2
import numpy as np

def line(imgPath=None):
    if imgPath is None: imgPath="data/result.jpg"

    img = cv2.imread(imgPath,0)

    _, thresh = cv2.threshold(img, 30, 255, cv2.THRESH_BINARY)

    kernel = np.ones((2, 2), np.uint8)
    opening = cv2.morphologyEx(thresh, cv2.MORPH_CLOSE, kernel)

    cv2.imwrite('roi.jpg', opening)
    # Create default parametrization LSD
    lsd = cv2.createLineSegmentDetector(0)
    result = ""
    # Detect lines in the image
    lines = lsd.detect(opening)[0]  # Position 0 of the returned tuple are the detected lines
    for line in lines:
        for x1, y1, x2, y2 in line:
            result = result + str(int(x1)) + ';' + str(int(y1)) + ';' + str(int(x2)) + ';' + str(int(y2)) + '\n'
    result = result[:-1]
    f = open("data/1_mur.txt", "w")
    f.write(result)
    f.close()

    # Draw detected lines in the image
    white = np.ones((img.shape[0], img.shape[1]), np.uint8) * 255
    drawn_img = lsd.drawSegments(white, lines)
    # print(len(lines))
    # Show image
    cv2.imwrite('data/lines.jpg', drawn_img)
    # cv2.waitKey(0)